namespace GroceryShopping.Core;

public interface INotifier
{
    Task SendAsync(string message);
}