namespace GroceryShopping.Infrastructure.Shopping;

using System.Text.Json.Serialization;

public class FriscoFeedProduct
{
    [JsonPropertyName("productId")]
    public string ProductId { get; set; } = string.Empty;

    [JsonPropertyName("ean")]
    public string Ean { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public FriscoText Name { get; set; } = null!;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("producer")]
    public string Producer { get; set; } = string.Empty;

    [JsonPropertyName("brand")]
    public string Brand { get; set; } = string.Empty;

    [JsonPropertyName("subbrand")]
    public string Subbrand { get; set; } = string.Empty;

    [JsonPropertyName("supplier")]
    public string Supplier { get; set; } = string.Empty;

    [JsonPropertyName("packSize")]
    public int PackSize { get; set; }

    [JsonPropertyName("unitOfMeasure")]
    public string UnitOfMeasure { get; set; } = string.Empty;

    [JsonPropertyName("grammage")]
    public float Grammage { get; set; }

    [JsonPropertyName("countryOfOrigin")]
    public string CountryOfOrigin { get; set; } = string.Empty;

    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("tags")]
    public string[] Tags { get; set; } = [];

    [JsonPropertyName("categories")]
    public FriscoCategory[] Categories { get; set; } = null!;
}