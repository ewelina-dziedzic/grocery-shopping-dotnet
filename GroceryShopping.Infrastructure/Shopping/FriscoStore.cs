using GroceryShopping.Core;
using GroceryShopping.Core.Model;
using GroceryShopping.Infrastructure.Network;

namespace GroceryShopping.Infrastructure.Shopping;

public class FriscoStore(IHttpNamedClient httpClient, ILlm llm, ILogger logger) : IStore
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
            var foundProducts = await httpClient.GetAsync<FriscoProductsSearchResponse>(
                HttpClientName.FriscoUser,
                $"/app/commerce/api/v1/users/{FriscoAccessTokenHandler.UserIdPlaceholder}/offer/products/query?purpose=Listing&pageIndex=1&search={groceryItem.Name}&includeFacets=true&deliveryMethod=Van&pageSize=50&language=pl&disableAutocorrect=false");

            if (foundProducts == null)
            {
                throw new InvalidOperationException("Found products collection must not be null");
            }

            var availableProducts = foundProducts.Products.Select(product => product.Product)
                .Where(product => product.IsAvailable).Select(
                    friscoProduct =>
                    {
                        return new Product(
                            friscoProduct.Id,
                            friscoProduct.Name.Pl,
                            friscoProduct.PackSize,
                            friscoProduct.UnitOfMeasure,
                            friscoProduct.Grammage,
                            friscoProduct.Price.Price,
                            friscoProduct.Price.PriceAfterPromotion,
                            friscoProduct.Tags.Where(tag => !TagsToIgnore.Contains(tag)).ToList(),
                            []);
                    }).ToList();

            var choice = llm.Ask(groceryItem.Name, availableProducts);
            await logger.LogChoice(groceryShoppingId, groceryItem, choice);

            if (!choice.IsProductChosen)
            {
                continue;
            }

            if (choice.Product == null)
            {
                throw new InvalidOperationException("Product must not be null when a product is chosen.");
            }

            await httpClient.PutAsync(
                HttpClientName.FriscoUser,
                $"/app/commerce/api/v1/users/{FriscoAccessTokenHandler.UserIdPlaceholder}/cart",
                new FriscoShoppingCartUpdateRequest
                {
                    Products =
                    [
                        new FriscoShoppingCartProduct
                        {
                            ProductId = choice.Product.Id, Quantity = groceryItem.Quantity,
                        },
                    ],
                });
            boughtGroceryItems.Add(groceryItem);
        }

        await logger.LogShoppingEnd(groceryShoppingId);
        return boughtGroceryItems;
    }
}