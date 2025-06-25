using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.ProductSelection;

public class LangfuseChatMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}