namespace GroceryShopping.Application;

public interface IStoreService
{
    Task<DeliveryWindow?> ScheduleAsync(string[] preferredStartTimes);

    Task<ShoppingSummary> ShopAsync();
}