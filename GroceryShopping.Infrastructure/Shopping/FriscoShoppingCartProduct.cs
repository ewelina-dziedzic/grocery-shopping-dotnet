using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.Shopping;

public class FriscoShoppingCartProduct
{
    [JsonPropertyName("productId")]
    public string ProductId { get; set; } = null!;

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}