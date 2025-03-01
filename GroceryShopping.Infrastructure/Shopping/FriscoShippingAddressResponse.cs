using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.Shopping;

public class FriscoShippingAddressResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("userId")]
    public string UserId { get; set; } = null!;

    [JsonPropertyName("shippingAddress")]
    public FriscoShippingAddress ShippingAddress { get; set; } = null!;

    [JsonPropertyName("deliveryMethod")]
    public string DeliveryMethod { get; set; } = null!;
}