using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Wes.Utils;

namespace Wes.Scheduler
{
    /// <summary>
    /// Quartz IJob 实现，负责调度到 JobSchedulerService 执行
    /// </summary>
    [DisallowConcurrentExecution]
    public class QuartzJobRunner : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            var schedulerService = GlobalContext.ServiceProvider!.GetRequiredService<JobSchedulerService>();
            await schedulerService.ExecuteJob(
                dataMap.GetLong("JobId"),
                dataMap.GetString("JobName") ?? "",
                dataMap.GetString("JobGroup") ?? "",
                dataMap.GetString("InvokeTarget") ?? ""
            );
        }
    }
}
