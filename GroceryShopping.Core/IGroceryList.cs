using GroceryShopping.Core.Entities;

namespace GroceryShopping.Core;

public interface IGroceryList
{
    Task LoadAsync(IEnumerable<ShoppingListItem> shoppingList);

    Task<IReadOnlyCollection<GroceryItem>> GetGroceryListAsync();

    Task CompleteAsync(IEnumerable<GroceryItem> groceryItems);
}