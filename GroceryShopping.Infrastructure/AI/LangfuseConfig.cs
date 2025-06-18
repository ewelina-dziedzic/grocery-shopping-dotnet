using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.AI;

public class LangfuseConfig
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;
}