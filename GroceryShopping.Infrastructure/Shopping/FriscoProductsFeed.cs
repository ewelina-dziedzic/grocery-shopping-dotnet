using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.Shopping;

public class FriscoProductsFeed
{
    [JsonPropertyName("products")]
    public FriscoFeedProduct[] Products { get; set; } = null!;
}