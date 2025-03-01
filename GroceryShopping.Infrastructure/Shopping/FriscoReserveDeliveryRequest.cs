using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.Shopping;

public class FriscoReserveDeliveryRequest
{
    [JsonPropertyName("deliveryWindow")]
    public FriscoDeliveryWindow DeliveryWindow { get; set; } = null!;

    [JsonPropertyName("shippingAddress")]
    public FriscoShippingAddress ShippingAddress { get; set; } = null!;
}