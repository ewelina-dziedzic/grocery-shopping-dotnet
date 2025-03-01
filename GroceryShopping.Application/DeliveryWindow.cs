namespace GroceryShopping.Application;

public class DeliveryWindow(DateTime startTime, DateTime endTime)
{
    public DateTime StartTime { get; } = startTime;

    public DateTime EndTime { get; } = endTime;
}