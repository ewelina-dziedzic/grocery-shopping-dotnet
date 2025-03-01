using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.Shopping;

public class FriscoFeedProductContent
{
    [JsonPropertyName("components")]
    public string[] Components { get; set; } = [];
}