namespace GroceryShopping.Lambda.Listify;

public class ListifyResult(int addedShoppingListItemsCount)
{
    public int AddedShoppingListItemsCount { get; } = addedShoppingListItemsCount;
}