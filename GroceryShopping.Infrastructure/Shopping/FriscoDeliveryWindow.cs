using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.Shopping;

public class FriscoDeliveryWindow
{
    [JsonPropertyName("startsAt")]
    public DateTime StartsAt { get; set; }

    [JsonPropertyName("endsAt")]
    public DateTime EndsAt { get; set; }

    [JsonPropertyName("finalAt")]
    public DateTime FinalAt { get; set; }

    [JsonPropertyName("closesAt")]
    public DateTime ClosesAt { get; set; }

    [JsonPropertyName("deliveryMethod")]
    public string DeliveryMethod { get; set; } = null!;

    [JsonPropertyName("warehouse")]
    public string Warehouse { get; set; } = null!;

    [JsonPropertyName("isMondayAfterNonTradeSunday")]
    public bool IsMondayAfterNonTradeSunday { get; set; }

    [JsonPropertyName("IsNonTradeSunday")]
    public bool IsNonTradeSunday { get; set; }
}