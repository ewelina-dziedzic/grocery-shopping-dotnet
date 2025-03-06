using System.ClientModel;
using System.Text.Json;

using GroceryShopping.Core;
using GroceryShopping.Core.Entities;

using Microsoft.Extensions.Options;

using OpenAI.Assistants;

namespace GroceryShopping.Infrastructure.AI;

#pragma warning disable OPENAI001
public class OpenAI : ILlm
{
    private readonly AssistantClient _assistantClient;
    private readonly OpenAIOptions _options;

    public OpenAI(IOptions<OpenAIOptions> options)
    {
        _options = options.Value;
        _assistantClient = new AssistantClient(new ApiKeyCredential(_options.ApiKey));
    }

    public Choice Ask(string productName, IEnumerable<Product> options)
    {
        var optionsCollection = options.ToList();
        var threadOptions = new ThreadCreationOptions
        {
            InitialMessages =
            {
                $"Chcę kupić produkt o nazwie {productName}. Który produkt z listy powinnam kupić? {JsonSerializer.Serialize(optionsCollection)}",
            },
        };
        ThreadRun threadRun = _assistantClient.CreateThreadAndRun(_options.GroceryShoppingAssistantId, threadOptions);

        do
        {
            Thread.Sleep(TimeSpan.FromSeconds(2));
            threadRun = _assistantClient.GetRun(threadRun.ThreadId, threadRun.Id);
        }
        while (!threadRun.Status.IsTerminal);

        if (threadRun.Status != RunStatus.Completed)
        {
            throw new OpenAIThreadRunFailedException(threadRun.Status);
        }

        var messages = _assistantClient.GetMessages(
            threadRun.ThreadId,
            new MessageCollectionOptions { Order = MessageCollectionOrder.Ascending });

        var message = messages.Last().Content.Last().Text;
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
}
#pragma warning restore OPENAI001