namespace GroceryShopping.Lambda.Schedule.IntegrationTests;

public class FunctionTests
{
    [Test]
    public async Task Handler_VariousStartTimes_ShouldSchedule()
    {
        var function = new Function();
        var input = new ScheduleInput
        {
            PreferredStartTimes = ["08:00", "07:30", "15:30", "22:00"],
        };

        var result = await function.Handler(input);

        Assert.That(result.Scheduled, Is.True);
    }
}