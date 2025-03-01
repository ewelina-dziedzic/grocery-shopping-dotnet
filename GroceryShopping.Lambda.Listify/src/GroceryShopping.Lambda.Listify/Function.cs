using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;

using GroceryShopping.Application;

using Microsoft.Extensions.DependencyInjection;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace GroceryShopping.Lambda.Listify;

public class Function : BaseFunction
{
    private readonly IGroceryListService _groceryListService;

    public Function()
    {
        _groceryListService = ServiceProvider.GetRequiredService<IGroceryListService>();
    }

    public async Task<ListifyResult> Handler()
    {
        return await ExecuteWithExceptionsHandling(
            async () =>
            {
                var addedShoppingListItems = await _groceryListService.ListifyAsync();
                return new ListifyResult(addedShoppingListItems);
            });
    }
}