using GroceryShopping.Core.Model;

namespace GroceryShopping.Core;

public interface IProductSelector
{
    Task<Choice> ChooseAsync(string productName, IEnumerable<Product> options);
}