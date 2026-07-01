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
        var ingredientsDataSourceId = options.Value.IngredientsDataSourceId;

        var query = new QueryDataSourceRequest
        {
            DataSourceId = ingredientsDataSourceId,
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

        var pages = await client.DataSources.QueryAsync(query);
        if (pages.HasMore)
        {
            throw new NotImplementedException("Pagination is not implemented");
        }

        foreach (var page in pages.Results)
        {
            if (page is not Page notionPage)
            {
                continue;
            }

            var ingredientProperty = notionPage.Properties["Ingredient"] as TitlePropertyValue;
            var quantityProperty = notionPage.Properties["Quantity"] as NumberPropertyValue;
            var neededForDateProperty = notionPage.Properties["Needed for date"] as FormulaPropertyValue;
            var storeLinkProperty = notionPage.Properties["Frisco"] as FormulaPropertyValue;

            var productName = ingredientProperty?.Title.FirstOrDefault()?.PlainText;
            result.Add(
                new ShoppingListItem(
                    productName ?? throw new InvalidOperationException("Product name can't be null"),
                    Convert.ToInt32(quantityProperty?.Number ?? 1),
                    neededForDateProperty?.Formula.Date?.Start?.DateTime,
                    storeLinkProperty?.Formula?.String));
        }

        return result;
    }
}