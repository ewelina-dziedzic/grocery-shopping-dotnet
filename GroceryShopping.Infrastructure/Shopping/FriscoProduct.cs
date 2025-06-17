namespace GroceryShopping.Infrastructure.Shopping;

using System.Text.Json.Serialization;

public class FriscoProduct
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("ean")]
    public string Ean { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public FriscoProductName Name { get; set; } = null!;

    [JsonPropertyName("producer")]
    public string Producer { get; set; } = string.Empty;

    [JsonPropertyName("countryOfOrigin")]
    public string CountryOfOrigin { get; set; } = string.Empty;

    [JsonPropertyName("packSize")]
    public int PackSize { get; set; }

    [JsonPropertyName("unitOfMeasure")]
    public string UnitOfMeasure { get; set; } = null!;

    [JsonPropertyName("grammage")]
    public float Grammage { get; set; }

    [JsonPropertyName("price")]
    public FriscoProductPrice Price { get; set; } = null!;

    [JsonPropertyName("tags")]
    public string[] Tags { get; set; } = [];

    [JsonPropertyName("categories")]
    public FriscoCategory[] Categories { get; set; } = [];

    [JsonPropertyName("isAvailable")]
    public bool IsAvailable { get; set; }
}