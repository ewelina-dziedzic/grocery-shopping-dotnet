using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.Shopping;

public class FriscoProductPrice
{
    [JsonPropertyName("price")]
    public float Price { get; set; }

    [JsonPropertyName("priceAfterPromotion")]
    public float PriceAfterPromotion { get; set; }
}