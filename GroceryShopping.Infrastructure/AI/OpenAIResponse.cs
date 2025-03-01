using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.AI;

public record struct OpenAIResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("reason")]
    public string Reason { get; set; }
}