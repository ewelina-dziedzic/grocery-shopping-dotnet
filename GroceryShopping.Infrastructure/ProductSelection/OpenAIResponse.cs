using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.ProductSelection;

public record struct OpenAIResponse
{
    [JsonPropertyName("choices")]
    public OpenAIChoice[] Choices { get; set; }
}