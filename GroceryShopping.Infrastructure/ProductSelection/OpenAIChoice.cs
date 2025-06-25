using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.ProductSelection;

public record struct OpenAIChoice
{
    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("message")]
    public OpenAIMessage Message { get; set; }
}