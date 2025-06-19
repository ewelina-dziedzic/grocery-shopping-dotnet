namespace GroceryShopping.Infrastructure.Observability;

public class ChatMessage(string role, string content)
{
    public string Role { get; } = role;

    public string Content { get; } = content;
}