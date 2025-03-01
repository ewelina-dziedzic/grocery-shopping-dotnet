using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.Shopping;

public class FriscoShoppingCartUpdateRequest
{
    [JsonPropertyName("products")]
    public FriscoShoppingCartProduct[] Products { get; set; } = null!;
}