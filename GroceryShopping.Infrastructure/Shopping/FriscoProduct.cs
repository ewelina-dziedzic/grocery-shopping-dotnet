using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.Shopping;

public class FriscoProduct
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("name")]
    public FriscoProductName Name { get; set; } = null!;

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

    [JsonPropertyName("isAvailable")]
    public bool IsAvailable { get; set; }
}