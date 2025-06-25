using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.Observability;

public class ChatMessage(string role, string content)
{
    [JsonPropertyName("role")]
    public string Role { get; } = role;

    [JsonPropertyName("content")]
    public string Content { get; } = content;
}