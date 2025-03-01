using GroceryShopping.Core.Entities;

namespace GroceryShopping.Core;

public interface IMealPlan
{
    Task<IReadOnlyCollection<ShoppingListItem>> GetShoppingListAsync();
}