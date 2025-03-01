namespace GroceryShopping.Application;

public interface IGroceryListService
{
    Task<int> ListifyAsync();
}