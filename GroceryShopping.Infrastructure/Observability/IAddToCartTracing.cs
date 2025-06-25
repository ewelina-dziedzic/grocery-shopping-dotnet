using GroceryShopping.Core.Model;
using GroceryShopping.Infrastructure.ProductSelection;

namespace GroceryShopping.Infrastructure.Observability;

public interface IAddToCartTracing
{
    Task StartTraceAsync(string productName);

    Task StartProductSelectionAsync(string productName, IReadOnlyCollection<Product> options);

    Task AddChatCompletionAsync(object prompt, object completion, string model, string promptName, int promptVersion);

    Task EndProductSelectionAsync(Choice choice);
}