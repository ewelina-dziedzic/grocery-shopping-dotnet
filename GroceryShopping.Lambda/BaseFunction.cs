using System;
using System.Threading.Tasks;

using GroceryShopping.Application;
using GroceryShopping.Core;
using GroceryShopping.Infrastructure;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GroceryShopping.Lambda;

public abstract class BaseFunction
{
    private readonly INotifier _notifier;

    protected BaseFunction()
    {
        _notifier = ServiceProvider.GetRequiredService<INotifier>();
    }

    protected ServiceProvider ServiceProvider { get; } = ConfigureServices();

    private static ServiceProvider ConfigureServices()
    {
        var configuration = new ConfigurationBuilder().AddSystemsManager("/GroceryShopping").Build();

        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);
        services.AddApplication();
        return services.BuildServiceProvider();
    }

    protected async Task<TOutput> ExecuteWithExceptionsHandling<TOutput>(Func<Task<TOutput>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception exception)
        {
            await _notifier.SendAsync($"\ud83d\udca5 error: {exception.Message}");
            throw;
        }
    }
}