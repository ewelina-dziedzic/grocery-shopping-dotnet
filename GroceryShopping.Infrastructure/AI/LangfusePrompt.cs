using System.Text.Json.Serialization;

namespace GroceryShopping.Infrastructure.AI;

public class LangfusePrompt
{
    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonPropertyName("config")]
    public LangfuseConfig Config { get; set; } = null!;

    [JsonPropertyName("prompt")]
    public LangfuseChatMessage[] Prompt { get; set; } = [];
}