using GroceryShopping.Core;

namespace GroceryShopping.Application;

public class GroceryListService(IMealPlan mealPlan, IGroceryList groceryList) : IGroceryListService
{
    public async Task<int> ListifyAsync()
    {
        var shoppingList = await mealPlan.GetShoppingListAsync();
        await groceryList.LoadAsync(shoppingList);
        return shoppingList.Count;
    }
}