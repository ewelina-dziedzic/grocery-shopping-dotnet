using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;

using GroceryShopping.Application;

using Microsoft.Extensions.DependencyInjection;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace GroceryShopping.Lambda.Shop;

public class Function : BaseFunction
{
    private readonly IStoreService _storeService;

    public Function()
    {
        _storeService = ServiceProvider.GetRequiredService<IStoreService>();
    }

    public async Task<ShopResult> Handler()
    {
        return await ExecuteWithExceptionsHandling(
            async () =>
            {
                var result = await _storeService.ShopAsync();
                return new ShopResult(result.BoughtProductsCount, result.NotFoundProductsCount);
            });
    }
}