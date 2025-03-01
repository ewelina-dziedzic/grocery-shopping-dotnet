using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.Shopping;

public class FriscoSearchProduct
{
    [JsonPropertyName("product")]
    public FriscoProduct Product { get; set; } = null!;
}