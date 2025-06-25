using GroceryShopping.Core.Model;
using GroceryShopping.Infrastructure.Network;
using GroceryShopping.Infrastructure.ProductSelection;

namespace GroceryShopping.Infrastructure.Observability;

public class LangfuseAddToCartTracing(IHttpNamedClient httpClient) : IAddToCartTracing
{
    private readonly string _sessionId = $"grocery-shopping-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}";

    private string? _traceId;
    private string? _productSelectionSpanId;

    public async Task StartTraceAsync(string productName)
    {
        _traceId = Guid.NewGuid().ToString();

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
                        id = _traceId,
                        sessionId = _sessionId,
                        name = "add-to-cart",
                        input = new { productName, },
                    },
                },
            },
        };

        await httpClient.PostAsync(HttpClientName.Langfuse, "/api/public/ingestion", payload);
    }

    public async Task StartProductSelectionAsync(string productName, IReadOnlyCollection<Product> options)
    {
        _productSelectionSpanId = Guid.NewGuid().ToString();

        var payload = new
        {
            batch = new object[]
            {
                new
                {
                    id = Guid.NewGuid().ToString(),
                    type = "span-create",
                    timestamp = DateTime.UtcNow.ToString("o"),
                    body = new
                    {
                        id = _productSelectionSpanId,
                        traceId = _traceId,
                        sessionId = _sessionId,
                        name = "product-selection",
                        startTime = DateTime.UtcNow.ToString("o"),
                        input = new { productName, options, },
                    },
                },
            },
        };

        await httpClient.PostAsync(HttpClientName.Langfuse, "/api/public/ingestion", payload);
    }

    public async Task AddChatCompletionAsync(
        object prompt,
        object completion,
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
                        traceId = _traceId,
                        parentObservationId = _productSelectionSpanId,
                        sessionId = _sessionId,
                        name = "chat-completion",
                        input = prompt,
                        output = completion,
                        model,
                        promptName,
                        promptVersion,
                    },
                },
            },
        };

        await httpClient.PostAsync(HttpClientName.Langfuse, "/api/public/ingestion", payload);
    }

    public async Task EndProductSelectionAsync(Choice choice)
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
                    body = new
                    {
                        id = _productSelectionSpanId,
                        endTime = DateTime.UtcNow.ToString("o"),
                        output = choice,
                    },
                },
            },
        };

        await httpClient.PostAsync(HttpClientName.Langfuse, "/api/public/ingestion", payload);

        _traceId = null;
        _productSelectionSpanId = null;
    }
}