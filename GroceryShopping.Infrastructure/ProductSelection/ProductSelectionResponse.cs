using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.ProductSelection;

public record struct ProductSelectionResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("reason")]
    public string Reason { get; set; }
}