using GroceryShopping.Core.Entities;

namespace GroceryShopping.Core;

public interface IStore
{
    Task<DeliveryWindow?> ScheduleAsync(string[] preferredStartTime);

    Task<IReadOnlyCollection<GroceryItem>> ShopAsync(IEnumerable<GroceryItem> groceryItems);
}