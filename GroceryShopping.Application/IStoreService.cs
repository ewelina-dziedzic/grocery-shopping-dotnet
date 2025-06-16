namespace GroceryShopping.Application;

public interface IStoreService
{
    Task SyncProductsAsync();

    Task<DeliveryWindow?> ScheduleAsync(string[] preferredStartTimes);

    Task<ShoppingSummary> ShopAsync();
}