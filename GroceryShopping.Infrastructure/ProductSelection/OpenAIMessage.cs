using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.ProductSelection;

public record struct OpenAIMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }
}