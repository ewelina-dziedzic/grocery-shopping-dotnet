using GroceryShopping.Core;
using GroceryShopping.Infrastructure.Network;

using Microsoft.Extensions.Options;

namespace GroceryShopping.Infrastructure.Notifications;

public class MakeNotifier(IHttpNamedClient httpClient, IOptions<MakeOptions> options) : INotifier
{
    public async Task SendAsync(string message)
    {
        await httpClient.PostAsync(HttpClientName.Plain, options.Value.StatusUpdateWebhook, message);
    }
}