using Quartz;

namespace AdvancedFullProject.Infrastructure.Background;

public sealed class EmployeeSyncJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine("Quartz job executed at: " + DateTime.UtcNow);
        return Task.CompletedTask;
    }
}
