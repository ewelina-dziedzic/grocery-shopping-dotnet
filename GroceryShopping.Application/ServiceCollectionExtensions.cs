using Microsoft.Extensions.DependencyInjection;

namespace GroceryShopping.Application;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IGroceryListService, GroceryListService>();
        services.AddTransient<IStoreService, StoreService>();
    }
}