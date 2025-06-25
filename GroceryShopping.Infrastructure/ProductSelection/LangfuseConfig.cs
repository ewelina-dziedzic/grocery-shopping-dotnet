using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.ProductSelection;

public class LangfuseConfig
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;
}