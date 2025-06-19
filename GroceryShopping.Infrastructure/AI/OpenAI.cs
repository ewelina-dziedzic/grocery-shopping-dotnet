using System.Text.Json;

using GroceryShopping.Core;
using GroceryShopping.Core.Model;
using GroceryShopping.Infrastructure.Network;
using GroceryShopping.Infrastructure.Observability;

using Microsoft.Extensions.Options;

using OpenAI.Chat;

using ChatMessage = OpenAI.Chat.ChatMessage;
using ChatMessageForTracing = GroceryShopping.Infrastructure.Observability.ChatMessage;

namespace GroceryShopping.Infrastructure.AI;

public class OpenAI(IOptions<OpenAIOptions> options, IHttpNamedClient httpClient, IProductSelectionTracing tracing)
    : ILlm
{
    private const string PromptName = "grocery-shopping-assistant";

    private readonly OpenAIOptions _options = options.Value;

    private LangfusePrompt? _prompt = null;
    private ChatClient? _chatClient = null;

    public async Task<Choice> AskAsync(string productName, IEnumerable<Product> options)
    {
        var optionsCollection = options.ToList();
        var trace = await tracing.StartTraceAsync(productName, optionsCollection);

        if (_prompt == null)
        {
            _prompt = await httpClient.GetAsync<LangfusePrompt>(
                HttpClientName.Langfuse,
                $"/api/public/v2/prompts/{PromptName}");
            if (_prompt == null)
            {
                throw new InvalidOperationException("No response was returned from Langfuse");
            }

            _chatClient = new ChatClient(model: _prompt.Config.Model, apiKey: _options.ApiKey);
        }

        var messages = _prompt.Prompt.Select(message => BuildChatMessage(message, productName, optionsCollection))
            .ToList();
        var completion = await _chatClient!.CompleteChatAsync(messages);
        var completionContent = completion.Value.Content.Last().Text;
        var prompt = messages.Select(
            message => new ChatMessageForTracing(GetRole(message), message.Content.Single().Text));
        var answer = JsonSerializer.Deserialize<OpenAIResponse>(
            completionContent.Replace("```json", string.Empty).Replace("```", string.Empty));
        await tracing.AddChatCompletion(trace.TraceId, trace.SpanId, prompt, answer, _prompt.Config.Model, PromptName, _prompt.Version);

        if (string.IsNullOrEmpty(answer.Id))
        {
            var productNotChosen = new Choice(false, answer.Reason);
            await tracing.EndTraceAsync(trace.SpanId, productNotChosen);
            return productNotChosen;
        }

        var chosenOption = optionsCollection.First(option => option.Id == answer.Id);

        if (answer.Name == null)
        {
            throw new InvalidOperationException("Name must not be null when an option is chosen.");
        }

        var productChosen = new Choice(
            true,
            answer.Reason,
            new ChosenProduct(answer.Id, answer.Name, chosenOption.Price, chosenOption.PriceAfterPromotion));
        await tracing.EndTraceAsync(trace.SpanId, productChosen);
        return productChosen;
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

    private static string GetRole(ChatMessage message)
    {
        if (message.GetType() == typeof(SystemChatMessage))
        {
            return "system";
        }

        if (message.GetType() == typeof(UserChatMessage))
        {
            return "user";
        }

        throw new InvalidOperationException($"Unknown message type {message.GetType().Name}");
    }
}