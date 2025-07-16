using GroceryShopping.Core.Model;
using GroceryShopping.Infrastructure.Network;
using GroceryShopping.Infrastructure.ProductSelection;
using GroceryShopping.Infrastructure.Shopping;

namespace GroceryShopping.Infrastructure.Observability;

public class LangfuseAddToCartTracing(IHttpNamedClient httpClient) : IAddToCartTracing
{
    private readonly string _sessionId = $"grocery-shopping-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}";

    private string? _traceId;
    private string? _productSelectionSpanId;
    private string? _apiRequestSpanId;

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
        string chatCompletionName,
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
                        name = chatCompletionName,
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

        _productSelectionSpanId = null;
    }

    public async Task StartApiRequest(string requestType, string requestUri, object? requestBody)
    {
        _apiRequestSpanId = Guid.NewGuid().ToString();

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
                        id = _apiRequestSpanId,
                        traceId = _traceId,
                        sessionId = _sessionId,
                        name = requestType,
                        startTime = DateTime.UtcNow.ToString("o"),
                        input = new { requestUri, requestBody, },
                    },
                },
            },
        };

        await httpClient.PostAsync(HttpClientName.Langfuse, "/api/public/ingestion", payload);
    }

    public async Task EndApiRequestAsync(object? responseBody)
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
                        id = _apiRequestSpanId,
                        endTime = DateTime.UtcNow.ToString("o"),
                        output = responseBody,
                    },
                },
            },
        };

        await httpClient.PostAsync(HttpClientName.Langfuse, "/api/public/ingestion", payload);

        _apiRequestSpanId = null;
    }
}