namespace GroceryShopping.Infrastructure.Shopping;

using System.Text.Json.Serialization;

public class FriscoText
{
    [JsonPropertyName("pl")]
    public string Pl { get; set; } = null!;

    [JsonPropertyName("en")]
    public string En { get; set; } = null!;
}