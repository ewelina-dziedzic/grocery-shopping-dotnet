namespace GroceryShopping.Lambda.Schedule;

public class ScheduleResult
{
    private ScheduleResult()
    {
    }

    private ScheduleResult(bool scheduled)
    {
        Scheduled = scheduled;
    }

    private ScheduleResult(bool scheduled, DateTime startTime, DateTime endTime)
    {
        Scheduled = scheduled;
        StartTime = startTime;
        EndTime = endTime;
    }

    public bool Scheduled { get; }

    public DateTime? StartTime { get; }

    public DateTime? EndTime { get; }

    public static ScheduleResult CreateUnscheduledResult()
    {
        return new ScheduleResult(false);
    }

    public static ScheduleResult CreateScheduledResult(DateTime startTime, DateTime endTime)
    {
        return new ScheduleResult(true, startTime, endTime);
    }
}