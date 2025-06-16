using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;

using GroceryShopping.Application;

using Microsoft.Extensions.DependencyInjection;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace GroceryShopping.Lambda.SyncProducts;

public class Function : BaseFunction
{
    private readonly IStoreService _storeService;

    public Function()
    {
        _storeService = ServiceProvider.GetRequiredService<IStoreService>();
    }

    public async Task Handler()
    {
        await ExecuteWithExceptionsHandling(
            async () =>
            {
                await _storeService.SyncProductsAsync();
                return "ALL DONE";
            });
    }
}