namespace GroceryShopping.Core.Model;

public class DeliveryWindow(DateTime startTime, DateTime endTime)
{
    public DateTime StartTime { get; } = startTime;

    public DateTime EndTime { get; } = endTime;
}