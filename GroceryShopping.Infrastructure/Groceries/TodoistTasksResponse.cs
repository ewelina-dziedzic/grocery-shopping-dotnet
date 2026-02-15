using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.Groceries;

public class TodoistTasksResponse
{
    [JsonPropertyName("results")]
    public TodoistReadTask[] Results { get; set; } = null!;

    [JsonPropertyName("next_cursor")]
    public string NextCursor { get; set; } = null!;
}