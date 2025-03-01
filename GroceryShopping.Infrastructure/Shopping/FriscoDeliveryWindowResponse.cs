using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.Shopping;

public class FriscoDeliveryWindowResponse
{
    [JsonPropertyName("canReserve")]
    public bool CanReserve { get; set; }

    [JsonPropertyName("deliveryWindow")]
    public FriscoDeliveryWindow DeliveryWindow { get; set; } = null!;
}