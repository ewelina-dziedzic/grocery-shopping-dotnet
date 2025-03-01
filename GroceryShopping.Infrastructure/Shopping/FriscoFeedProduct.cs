using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.Shopping;

public class FriscoFeedProduct
{
    [JsonPropertyName("productId")]
    public string ProductId { get; set; } = null!;

    [JsonPropertyName("contentData")]
    public FriscoFeedProductContent ContentData { get; set; } = null!;
}