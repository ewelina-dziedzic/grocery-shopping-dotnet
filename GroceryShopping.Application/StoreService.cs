using GroceryShopping.Core;
using GroceryShopping.Core.Model;

namespace GroceryShopping.Application;

public class StoreService(IStore store, IRepository<FeedProduct> productsRepository, IGroceryList list, INotifier notifier) : IStoreService
{
    public async Task SyncProductsAsync()
    {
        var products = await store.GetAllProductsAsync();
        foreach (var product in products)
        {
            await productsRepository.AddAsync(product);
        }
    }

    public async Task<DeliveryWindow?> ScheduleAsync(string[] preferredStartTimes)
    {
        var deliveryWindow = await store.ScheduleAsync(preferredStartTimes);
        return deliveryWindow == null ? null : new DeliveryWindow(deliveryWindow.StartTime, deliveryWindow.EndTime);
    }

    public async Task<ShoppingSummary> ShopAsync()
    {
        var groceryList = await list.GetGroceryListAsync();
        var boughtGroceryItems = await store.ShopAsync(groceryList);
        await list.CompleteAsync(boughtGroceryItems);
        await notifier.SendAsync($"\u2705 {boughtGroceryItems.Count} items successfully added to your cart");
        return new ShoppingSummary(boughtGroceryItems.Count, groceryList.Count - boughtGroceryItems.Count);
    }
}