using GroceryShopping.Core.Model;
using GroceryShopping.Infrastructure.ProductSelection;

namespace GroceryShopping.Infrastructure.Observability;

public interface IAddToCartTracing
{
    Task StartTraceAsync(string productName);

    Task StartProductSelectionAsync(string productName, IReadOnlyCollection<Product> options);

    Task AddChatCompletionAsync(string chatCompletionName, object prompt, object completion, string model, string promptName, int promptVersion);

    Task EndProductSelectionAsync(Choice choice);

    Task StartApiRequest(string requestType, string requestUri, object? requestBody = null);

    Task EndApiRequestAsync(object? responseBody = null);
}