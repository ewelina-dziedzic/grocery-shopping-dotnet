using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.Groceries;

public class TodoistCreateTask(string content, string? dueString, string? description)
{
    [JsonPropertyName("content")]
    public string Content { get; } = content;

    [JsonPropertyName("due_string")]
    public string? DueString { get; } = dueString;

    [JsonPropertyName("description")]
    public string? Description { get; } = description;
}