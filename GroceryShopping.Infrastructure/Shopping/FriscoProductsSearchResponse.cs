using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.Shopping;

public class FriscoProductsSearchResponse
{
    [JsonPropertyName("products")]
    public FriscoSearchProduct[] Products { get; set; } = null!;
}