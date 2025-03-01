using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.Shopping;

public class FriscoShippingAddress
{
    [JsonPropertyName("recipient")]
    public string Recipient { get; set; } = null!;

    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = null!;

    [JsonPropertyName("country")]
    public string Country { get; set; } = null!;

    [JsonPropertyName("postcode")]
    public string PostCode { get; set; } = null!;

    [JsonPropertyName("city")]
    public string City { get; set; } = null!;

    [JsonPropertyName("street")]
    public string Street { get; set; } = null!;

    [JsonPropertyName("buildingNumber")]
    public string BuildingNumber { get; set; } = null!;

    [JsonPropertyName("apartmentNumber")]
    public string ApartmentNumber { get; set; } = null!;

    [JsonPropertyName("stairwayNumber")]
    public string StairwayNumber { get; set; } = null!;

    [JsonPropertyName("floorNumber")]
    public string FloorNumber { get; set; } = null!;
}