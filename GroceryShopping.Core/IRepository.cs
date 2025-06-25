using GroceryShopping.Core.Model;

namespace GroceryShopping.Core;

public interface IRepository<in T>
{
    Task AddAsync(T model);

    Task UpdateAsync(T model);

    Task<FeedProduct?> GetBySourceIdAsync(string sourceId);
}