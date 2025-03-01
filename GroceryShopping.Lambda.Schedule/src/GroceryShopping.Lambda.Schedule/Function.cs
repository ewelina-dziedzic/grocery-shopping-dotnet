using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;

using GroceryShopping.Application;

using Microsoft.Extensions.DependencyInjection;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace GroceryShopping.Lambda.Schedule;

public class Function : BaseFunction
{
    private readonly IStoreService _storeService;

    public Function()
    {
        _storeService = ServiceProvider.GetRequiredService<IStoreService>();
    }

    public async Task<ScheduleResult> Handler(ScheduleInput input)
    {
        return await ExecuteWithExceptionsHandling(
            async () =>
            {
                var result = await _storeService.ScheduleAsync(input.PreferredStartTimes);
                return result == null
                    ? ScheduleResult.CreateUnscheduledResult()
                    : ScheduleResult.CreateScheduledResult(result.StartTime, result.EndTime);
            });
    }
}