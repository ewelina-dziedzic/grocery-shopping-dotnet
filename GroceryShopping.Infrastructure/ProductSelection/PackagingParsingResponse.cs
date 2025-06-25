using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.ProductSelection;

public record struct PackagingParsingResponse
{
    [JsonPropertyName("packaging")]
    public string Packaging { get; set; }
}