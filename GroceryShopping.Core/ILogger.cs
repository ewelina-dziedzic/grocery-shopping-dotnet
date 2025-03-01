using GroceryShopping.Core.Entities;

namespace GroceryShopping.Core;

public interface ILogger
{
    Task<string> LogShoppingStart(string storeName);

    Task LogShoppingEnd(string groceryShoppingId);

    Task LogChoice(string groceryShoppingId, GroceryItem groceryItem, Choice choice);
}