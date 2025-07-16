using System.Text.Json;

using GroceryShopping.Core.Model;
using GroceryShopping.Infrastructure.Network;
using GroceryShopping.Infrastructure.Observability;

using Microsoft.Extensions.Options;

namespace GroceryShopping.Infrastructure.ProductSelection;

public class OpenAIPackagingParser(
    IHttpNamedClient httpNamedClient,
    IOptions<OpenAIOptions> options,
    IAddToCartTracing tracing,
    IHttpClientFactory httpClientFactory) : IPackagingParser
{
    private const string PromptName = "product-packaging-parser";

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    private readonly OpenAIOptions _options = options.Value;

    private LangfusePrompt? _prompt = null;

    public async Task<string> ParseAsync(FeedProduct product)
    {
        if (_prompt == null)
        {
            _prompt = await httpNamedClient.GetAsync<LangfusePrompt>(
                HttpClientName.Langfuse,
                $"/api/public/v2/prompts/{PromptName}");
            if (_prompt == null)
            {
                throw new InvalidOperationException("No response was returned from Langfuse");
            }
        }

        var promptProduct = new PromptProduct(
            product.Ean,
            product.Name,
            product.Description,
            product.Categories.ToArray());

        var userMessageContent = new List<object>
        {
            new { type = "text", text = JsonSerializer.Serialize(promptProduct), },
        };

        if (!string.IsNullOrEmpty(product.ImageUrl))
        {
            var imageResponse = await _httpClient.GetAsync(product.ImageUrl);
            if (imageResponse.IsSuccessStatusCode)
            {
                userMessageContent.Add(new { type = "image_url", image_url = new { url = product.ImageUrl } });
            }
        }

        var messages = new[]
        {
            new
            {
                role = "system",
                content = new object[]
                {
                    new
                    {
                        type = "text",
                        text = _prompt.Prompt.Single(message => message.Role == "system").Content,
                    },
                },
            },
            new { role = "user", content = userMessageContent.ToArray(), },
        };

        var payload = new { model = _prompt.Config.Model, messages, };
        var response = await httpNamedClient.PostAsync<OpenAIResponse>(
            HttpClientName.OpenAI,
            "/v1/chat/completions",
            payload);
        var content = response.Choices.Last().Message.Content;
        var packaging = JsonSerializer.Deserialize<PackagingParsingResponse>(
            content.Replace("```json", string.Empty).Replace("```", string.Empty));
        await tracing.AddChatCompletionAsync("packaging-parsing-chat-completion", messages, packaging, _prompt.Config.Model, PromptName, _prompt.Version);
        return packaging.Packaging;
    }
}