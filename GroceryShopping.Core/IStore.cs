using GroceryShopping.Core.Model;

namespace GroceryShopping.Core;

public interface IStore
{
    Task<IReadOnlyCollection<FeedProduct>> GetAllProductsAsync();

    Task<DeliveryWindow?> ScheduleAsync(string[] preferredStartTime);

    Task<IReadOnlyCollection<GroceryItem>> ShopAsync(IEnumerable<GroceryItem> groceryItems);
}