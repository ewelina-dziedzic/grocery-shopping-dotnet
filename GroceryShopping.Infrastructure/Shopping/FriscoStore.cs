﻿using GroceryShopping.Core;
using GroceryShopping.Core.Model;
using GroceryShopping.Infrastructure.Network;
using GroceryShopping.Infrastructure.Observability;
using GroceryShopping.Infrastructure.ProductSelection;

namespace GroceryShopping.Infrastructure.Shopping;

public class FriscoStore(
    IHttpNamedClient httpClient,
    IProductSelector productSelector,
    ILogger logger,
    IRepository<FeedProduct> productRepository,
    IPackagingParser packagingParser,
    IAddToCartTracing tracing) : IStore
{
    private static readonly string[] TagsToIgnore =
    [
        "displayVariant",
        "isAvailable",
        "isStocked",
        "isNotAlcohol",
        "isSearchable",
        "isIndexable",
        "isPositioned",
        "isBargain",
    ];

    public async Task<IReadOnlyCollection<FeedProduct>> GetAllProductsAsync()
    {
        var productsFeed = await httpClient.GetAsync<FriscoProductsFeed>(
            HttpClientName.FriscoPublic,
            "/api/v1/integration/feeds/public?language=pl");

        if (productsFeed == null)
        {
            throw new InvalidOperationException("No products feed found");
        }

        return productsFeed.Products.Select(
            product => new FeedProduct(
                null,
                product.ProductId,
                product.Ean,
                product.Name.Pl,
                product.Description,
                product.Producer,
                product.Brand,
                product.Subbrand,
                product.Supplier,
                product.PackSize,
                product.UnitOfMeasure,
                product.Grammage,
                product.CountryOfOrigin,
                product.ImageUrl,
                null,
                product.Tags,
                product.Categories.Select(category => category.Name.Pl))).ToList();
    }

    public async Task<DeliveryWindow?> ScheduleAsync(string[] preferredStartTime)
    {
        var shippingAddresses = await httpClient.GetAsync<FriscoShippingAddressResponse[]>(
            HttpClientName.FriscoUser,
            $"/app/commerce/api/v1/users/{FriscoAccessTokenHandler.UserIdPlaceholder}/addresses/shipping-addresses");

        if (shippingAddresses == null)
        {
            throw new InvalidOperationException("Shipping addresses cannot be null");
        }

        var warsawTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var warsawToday = TimeZoneInfo.ConvertTime(DateTime.UtcNow, warsawTimeZone);
        var warsawTomorrow = warsawToday.AddDays(1);
        var deliveryWindows = await httpClient.PostAsync<FriscoDeliveryWindowResponse[]>(
            HttpClientName.FriscoUser,
            $"/app/commerce/api/v2/users/{FriscoAccessTokenHandler.UserIdPlaceholder}/calendar/Van/{warsawTomorrow.Year}/{warsawTomorrow.Month}/{warsawTomorrow.Day}",
            new FriscoDeliveryWindowRequest(shippingAddresses.Single().ShippingAddress));

        if (deliveryWindows == null)
        {
            throw new InvalidOperationException("Delivery windows cannot be null");
        }

        foreach (var startTime in preferredStartTime)
        {
            var time = startTime.Split(':');
            var startDateTime = warsawTomorrow.Date.AddHours(int.Parse(time[0])).AddMinutes(int.Parse(time[1]));
            var deliveryWindow = deliveryWindows.FirstOrDefault(
                deliveryWindow => deliveryWindow.CanReserve && deliveryWindow.DeliveryWindow.StartsAt == startDateTime);

            if (deliveryWindow == null)
            {
                continue;
            }

            await httpClient.PostAsync(
                HttpClientName.FriscoUser,
                $"/app/commerce/api/v2/users/{FriscoAccessTokenHandler.UserIdPlaceholder}/cart/reservation",
                new FriscoReserveDeliveryRequest
                {
                    ShippingAddress = shippingAddresses.Single().ShippingAddress,
                    DeliveryWindow = deliveryWindow.DeliveryWindow,
                });
            return new DeliveryWindow(deliveryWindow.DeliveryWindow.StartsAt, deliveryWindow.DeliveryWindow.EndsAt);
        }

        return null;
    }

    public async Task<IReadOnlyCollection<GroceryItem>> ShopAsync(IEnumerable<GroceryItem> groceryItems)
    {
        var boughtGroceryItems = new List<GroceryItem>();
        var groceryShoppingId = await logger.LogShoppingStart("Frisco");

        await httpClient.DeleteAsync(
            HttpClientName.FriscoUser,
            $"/app/commerce/api/v1/users/{FriscoAccessTokenHandler.UserIdPlaceholder}/cart");

        foreach (var groceryItem in groceryItems)
        {
            await tracing.StartTraceAsync(groceryItem.Name);

            var productsSearchRequestUri =
                $"/app/commerce/api/v1/users/{FriscoAccessTokenHandler.UserIdPlaceholder}/offer/products/query?purpose=Listing&pageIndex=1&search={groceryItem.Name}&includeFacets=true&deliveryMethod=Van&pageSize=20&language=pl&disableAutocorrect=false";
            await tracing.StartApiRequest("products-search", productsSearchRequestUri);
            var foundProducts = await httpClient.GetAsync<FriscoProductsSearchResponse>(
                HttpClientName.FriscoUser,
                productsSearchRequestUri);
            await tracing.EndApiRequestAsync(foundProducts);

            if (foundProducts == null)
            {
                throw new InvalidOperationException("Found products collection must not be null");
            }

            var availableProducts = new List<Product>();

            foreach (var product in foundProducts.Products)
            {
                var friscoProduct = product.Product;

                if (!friscoProduct.IsAvailable)
                {
                    continue;
                }

                var feedProduct = await productRepository.GetBySourceIdAsync(friscoProduct.Id);
                if (feedProduct is { Packaging: null })
                {
                    feedProduct.Packaging = await packagingParser.ParseAsync(feedProduct);
                    await productRepository.UpdateAsync(feedProduct);
                }

                var productModel = new Product(
                    friscoProduct.Id,
                    friscoProduct.Ean,
                    friscoProduct.Name.Pl,
                    friscoProduct.Producer,
                    friscoProduct.CountryOfOrigin,
                    friscoProduct.PackSize,
                    friscoProduct.UnitOfMeasure,
                    friscoProduct.Grammage,
                    friscoProduct.Price.Price,
                    friscoProduct.Price.PriceAfterPromotion,
                    friscoProduct.Tags.Where(tag => !TagsToIgnore.Contains(tag)).ToArray(),
                    friscoProduct.Categories.Select(category => category.Name.Pl).ToArray(),
                    feedProduct?.Description ?? string.Empty,
                    feedProduct?.ImageUrl ?? string.Empty,
                    feedProduct?.Packaging ?? string.Empty);

                availableProducts.Add(productModel);
            }

            var choice = await productSelector.ChooseAsync(groceryItem.Name, availableProducts);
            await logger.LogChoice(groceryShoppingId, groceryItem, choice);

            if (!choice.IsProductChosen)
            {
                continue;
            }

            if (choice.Product == null)
            {
                throw new InvalidOperationException("Product must not be null when a product is chosen.");
            }

            const string addToCartRequestUri =
                $"/app/commerce/api/v1/users/{FriscoAccessTokenHandler.UserIdPlaceholder}/cart";
            var requestBody = new FriscoShoppingCartUpdateRequest
            {
                Products =
                [
                    new FriscoShoppingCartProduct { ProductId = choice.Product.Id, Quantity = groceryItem.Quantity, },
                ],
            };
            await tracing.StartApiRequest("add-to-cart", addToCartRequestUri, requestBody);
            await httpClient.PutAsync(HttpClientName.FriscoUser, addToCartRequestUri, requestBody);
            await tracing.EndApiRequestAsync();
            boughtGroceryItems.Add(groceryItem);
        }

        await logger.LogShoppingEnd(groceryShoppingId);
        return boughtGroceryItems;
    }
}