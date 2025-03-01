namespace GroceryShopping.Core.Entities;

public class ShoppingListItem(string name, int quantity, DateTime? neededForDate, string? storeLink)
{
    public string Name { get; } = name;

    public int Quantity { get; } = quantity;

    public DateTime? NeededForDate { get; } = neededForDate;

    public string? StoreLink { get; } = storeLink;
}