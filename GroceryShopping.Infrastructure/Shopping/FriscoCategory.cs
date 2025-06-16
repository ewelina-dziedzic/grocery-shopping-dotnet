using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.Shopping;

public class FriscoCategory
{
    [JsonPropertyName("name")]
    public FriscoText Name { get; set; } = null!;
}