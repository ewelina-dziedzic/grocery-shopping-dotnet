using GroceryShopping.Core.Model;
using GroceryShopping.Infrastructure.AI;

namespace GroceryShopping.Infrastructure.Observability;

public interface IProductSelectionTracing
{
    Task<(string TraceId, string SpanId)> StartTraceAsync(string productName, IReadOnlyCollection<Product> options);

    Task AddChatCompletion(
        string traceId,
        string parentObservationId,
        IEnumerable<ChatMessage> prompt,
        OpenAIResponse completion,
        string model,
        string promptName,
        int promptVersion);

    Task EndTraceAsync(string spanId, Choice choice);
}