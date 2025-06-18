using System.Text.Json;

using GroceryShopping.Core;
using GroceryShopping.Core.Model;
using GroceryShopping.Infrastructure.Network;

using Microsoft.Extensions.Options;

using OpenAI.Chat;

namespace GroceryShopping.Infrastructure.AI;

public class OpenAI : ILlm
{
    private readonly IHttpNamedClient _httpClient;
    private readonly OpenAIOptions _options;

    private LangfusePrompt? _prompt = null;

    public OpenAI(IOptions<OpenAIOptions> options, IHttpNamedClient httpClient)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<Choice> AskAsync(string productName, IEnumerable<Product> options)
    {
        var optionsCollection = options.ToList();

        if (_prompt == null)
        {
            _prompt = await _httpClient.GetAsync<LangfusePrompt>(
                HttpClientName.Langfuse,
                "/api/public/v2/prompts/grocery-shopping-assistant");
            if (_prompt == null)
            {
                throw new InvalidOperationException("No response was returned from Langfuse");
            }
        }

        var chatClient = new ChatClient(model: _prompt.Config.Model, apiKey: _options.ApiKey);
        var messages = _prompt.Prompt.Select(message => BuildChatMessage(message, productName, optionsCollection))
            .ToList();
        var completion = await chatClient.CompleteChatAsync(messages);
        var message = completion.Value.Content.Last().Text.Replace("```json", string.Empty)
            .Replace("```", string.Empty);
        var answer = JsonSerializer.Deserialize<OpenAIResponse>(message);

        if (string.IsNullOrEmpty(answer.Id))
        {
            return new Choice(false, answer.Reason);
        }

        var chosenOption = optionsCollection.First(option => option.Id == answer.Id);

        if (answer.Name == null)
        {
            throw new InvalidOperationException("Name must not be null when an option is chosen.");
        }

        return new Choice(
            true,
            answer.Reason,
            new ChosenProduct(answer.Id, answer.Name, chosenOption.Price, chosenOption.PriceAfterPromotion));
    }

    private static ChatMessage BuildChatMessage(
        LangfuseChatMessage message,
        string productName,
        List<Product> optionsCollection)
    {
        switch (Enum.Parse<ChatMessageRole>(message.Role, ignoreCase: true))
        {
            case ChatMessageRole.System:
                return new SystemChatMessage(message.Content);
            case ChatMessageRole.User:
                var compiledPrompt = message.Content.Replace("{{product_name}}", productName).Replace(
                    "{{options}}",
                    JsonSerializer.Serialize(optionsCollection));
                return new UserChatMessage(compiledPrompt);
            default:
                throw new InvalidOperationException($"Unknown message role: {message.Role}");
        }
    }
}