namespace GroceryShopping.Core.Entities;

public class GroceryItem(string name, int quantity, string taskId)
{
    public string Name { get; } = name;

    public int Quantity { get; } = quantity;

    public string TaskId { get; } = taskId;

    public override string ToString()
    {
        return $"{Quantity}x {Name}";
    }
}