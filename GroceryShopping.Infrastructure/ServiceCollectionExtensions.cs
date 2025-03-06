using System.Net.Http.Headers;

using GroceryShopping.Core;
using GroceryShopping.Infrastructure.AI;
using GroceryShopping.Infrastructure.Groceries;
using GroceryShopping.Infrastructure.Logging;
using GroceryShopping.Infrastructure.MealPlanning;
using GroceryShopping.Infrastructure.Network;
using GroceryShopping.Infrastructure.Notifications;
using GroceryShopping.Infrastructure.Shopping;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GroceryShopping.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TodoistOptions>(configuration.GetSection("Todoist"));
        services.Configure<NotionOptions>(configuration.GetSection("Notion"));
        services.Configure<FriscoOptions>(configuration.GetSection("Frisco"));
        services.Configure<OpenAIOptions>(configuration.GetSection("OpenAI"));
        services.Configure<MakeOptions>(configuration.GetSection("Make"));

        services.AddHttpClient(
            nameof(HttpClientName.Todoist),
            client =>
            {
                var token = configuration[$"Todoist:{nameof(TodoistOptions.ApiKey)}"] ??
                            throw new InvalidOperationException($"Todoist:{nameof(TodoistOptions.ApiKey)} is missing");
                client.BaseAddress = new Uri("https://api.todoist.com");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            });

        services.AddHttpClient(
            nameof(HttpClientName.FriscoAuthentication),
            client =>
            {
                client.BaseAddress = new Uri("https://www.frisco.pl");
                client.DefaultRequestHeaders.Referrer = new Uri("https://www.frisco.com");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

        services.AddHttpClient(
            nameof(HttpClientName.FriscoUser),
            client =>
            {
                client.BaseAddress = new Uri("https://www.frisco.pl");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }).AddHttpMessageHandler<FriscoAccessTokenHandler>();

        services.AddHttpClient(
            nameof(HttpClientName.FriscoPublic),
            client =>
            {
                client.BaseAddress = new Uri("https://commerce.frisco.pl");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

        services.AddHttpClient(nameof(HttpClientName.Plain));

        services.AddTransient<FriscoAccessTokenHandler>();
        services.AddTransient<IHttpNamedClient, HttpNamedClient>();
        services.AddTransient<IGroceryList, TodoistGroceryList>();
        services.AddTransient<IMealPlan, NotionMealPlan>();
        services.AddTransient<IStore, FriscoStore>();
        services.AddTransient<ILlm, AI.OpenAI>();
        services.AddTransient<INotifier, MakeNotifier>();
        services.AddTransient<ILogger, NotionLogger>();
    }
}