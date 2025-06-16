using GroceryShopping.Core.Model;

namespace GroceryShopping.Core;

public interface IMealPlan
{
    Task<IReadOnlyCollection<ShoppingListItem>> GetShoppingListAsync();
}