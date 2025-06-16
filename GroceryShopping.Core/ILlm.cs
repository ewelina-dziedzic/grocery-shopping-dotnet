using GroceryShopping.Core.Model;

namespace GroceryShopping.Core;

public interface ILlm
{
    Choice Ask(string productName, IEnumerable<Product> options);
}