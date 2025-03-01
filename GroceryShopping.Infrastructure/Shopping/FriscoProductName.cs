using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.Shopping;

public class FriscoProductName
{
    [JsonPropertyName("pl")]
    public string Pl { get; set; } = null!;
}