using GroceryShopping.Core.Model;

namespace GroceryShopping.Core;

public interface ILlm
{
    Task<Choice> AskAsync(string productName, IEnumerable<Product> options);
}