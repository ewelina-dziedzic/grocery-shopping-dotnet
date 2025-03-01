using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.Groceries;

public class TodoistReadTask
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("content")]
    public string Content { get; set; } = null!;
}