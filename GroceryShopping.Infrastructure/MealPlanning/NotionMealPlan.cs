using GroceryShopping.Core;
using GroceryShopping.Core.Model;

using Microsoft.Extensions.Options;

using Notion.Client;

namespace GroceryShopping.Infrastructure.MealPlanning;

public class NotionMealPlan(IOptions<NotionOptions> options) : IMealPlan
{
    public async Task<IReadOnlyCollection<ShoppingListItem>> GetShoppingListAsync()
    {
        var result = new List<ShoppingListItem>();
        var client = NotionClientFactory.Create(
            new ClientOptions
            {
                AuthToken = options.Value.ApiKey,
            });

        var query = new DatabasesQueryParameters
        {
            Filter = new CompoundFilter
            {
                And = new List<Filter>
                {
                    new CheckboxFilter("Got it", false),
                    new CheckboxFilter("To buy", true),
                },
            },
            PageSize = 100,
        };

        var pages = await client.Databases.QueryAsync(options.Value.IngredientsDatabaseId, query);
        if (pages.HasMore)
        {
            throw new NotImplementedException("Pagination is not implemented");
        }

        foreach (var page in pages.Results)
        {
            var ingredientProperty = page.Properties["Ingredient"] as TitlePropertyValue;
            var quantityProperty = page.Properties["Quantity"] as NumberPropertyValue;
            var neededForDateProperty = page.Properties["Needed for date"] as FormulaPropertyValue;
            var storeLinkProperty = page.Properties["Frisco"] as FormulaPropertyValue;

            var productName = ingredientProperty?.Title.FirstOrDefault()?.PlainText;
            result.Add(
                new ShoppingListItem(
                    productName ?? throw new InvalidOperationException("Product name can't be null"),
                    Convert.ToInt32(quantityProperty?.Number ?? 1),
                    neededForDateProperty?.Formula.Date?.Start,
                    storeLinkProperty?.Formula?.String));
        }

        return result;
    }
}