using GroceryShopping.Core.Model;

namespace GroceryShopping.Infrastructure.ProductSelection;

public interface IPackagingParser
{
    Task<string> ParseAsync(FeedProduct product);
}