using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.Shopping;

public class FriscoDeliveryWindowRequest
{
    public FriscoDeliveryWindowRequest(FriscoShippingAddress shippingAddress)
    {
        ShippingAddress = shippingAddress;
    }

    [JsonPropertyName("shippingAddress")]
    public FriscoShippingAddress ShippingAddress { get; }
}