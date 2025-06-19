using GroceryShopping.Core.Model;
using GroceryShopping.Infrastructure.AI;
using GroceryShopping.Infrastructure.Network;

namespace GroceryShopping.Infrastructure.Observability;

public class LangfuseProductSelectionTracing(IHttpNamedClient httpClient) : IProductSelectionTracing
{
    private readonly string _sessionId = $"grocery-shopping-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}";

    public async Task<(string TraceId, string SpanId)> StartTraceAsync(
        string productName,
        IReadOnlyCollection<Product> options)
    {
        var traceId = Guid.NewGuid().ToString();
        var spanId = Guid.NewGuid().ToString();
        var payload = new
        {
            batch = new object[]
            {
                new
                {
                    id = Guid.NewGuid().ToString(),
                    type = "trace-create",
                    timestamp = DateTime.UtcNow.ToString("o"),
                    body = new
                    {
                        id = traceId,
                        sessionId = _sessionId,
                        name = "add-to-cart",
                        input = new { productName, },
                    },
                },
                new
                {
                    id = Guid.NewGuid().ToString(),
                    type = "span-create",
                    timestamp = DateTime.UtcNow.ToString("o"),
                    body = new
                    {
                        id = spanId,
                        traceId,
                        sessionId = _sessionId,
                        name = "product-selection",
                        startTime = DateTime.UtcNow.ToString("o"),
                        input = new { productName, options, },
                    },
                },
            },
        };

        await httpClient.PostAsync(HttpClientName.Langfuse, "/api/public/ingestion", payload);
        return (traceId, spanId);
    }

    public async Task AddChatCompletion(
        string traceId,
        string parentObservationId,
        IEnumerable<ChatMessage> prompt,
        OpenAIResponse completion,
        string model,
        string promptName,
        int promptVersion)
    {
        var payload = new
        {
            batch = new object[]
            {
                new
                {
                    id = Guid.NewGuid().ToString(),
                    type = "generation-create",
                    timestamp = DateTime.UtcNow.ToString("o"),
                    body = new
                    {
                        id = Guid.NewGuid().ToString(),
                        traceId,
                        parentObservationId,
                        sessionId = _sessionId,
                        name = "chat-completion",
                        input = new { prompt },
                        output = new { completion, },
                        model,
                        promptName,
                        promptVersion,
                    },
                },
            },
        };

        await httpClient.PostAsync(HttpClientName.Langfuse, "/api/public/ingestion", payload);
    }

    public async Task EndTraceAsync(string spanId, Choice choice)
    {
        var payload = new
        {
            batch = new object[]
            {
                new
                {
                    id = Guid.NewGuid().ToString(),
                    type = "span-update",
                    timestamp = DateTime.UtcNow.ToString("o"),
                    body = new { id = spanId, endTime = DateTime.UtcNow.ToString("o"), output = choice, },
                },
            },
        };

        await httpClient.PostAsync(HttpClientName.Langfuse, "/api/public/ingestion", payload);
    }
}