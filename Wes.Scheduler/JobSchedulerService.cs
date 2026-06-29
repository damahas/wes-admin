using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl.Matchers;
using Wes.DbModel;
using Wes.Utils;

namespace Wes.Scheduler
{
    /// <summary>
    /// 定时任务调度器，封装 Quartz 的调度增删改查与任务执行日志
    /// </summary>
    public class JobSchedulerService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private IScheduler? _scheduler;
        private readonly ConcurrentDictionary<string, ITaskExecutor> _executors = new();

        public JobSchedulerService(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        public async Task StartAsync()
        {
            _scheduler = await _schedulerFactory.GetScheduler();
            await _scheduler.Start();
        }

        public async Task StopAsync()
        {
            if (_scheduler is { IsShutdown: false })
                await _scheduler.Shutdown(waitForJobsToComplete: true);
        }

        public void RegisterExecutor(ITaskExecutor executor)
        {
            _executors[executor.Name] = executor;
        }

        // ==================== 调度操作 ====================

        public async Task AddJobAsync(SysJobModel job)
        {
            if (_scheduler == null) return;

            var jobKey = JobKey(job);
            var triggerKey = TriggerKey(job);

            var detail = JobBuilder.Create<QuartzJobRunner>()
                .WithIdentity(jobKey)
                .WithDescription(job.JobName)
                .UsingJobData("JobId", job.JobId)
                .UsingJobData("JobName", job.JobName)
                .UsingJobData("JobGroup", job.JobGroup)
                .UsingJobData("InvokeTarget", job.InvokeTarget)
                .StoreDurably()
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .WithCronSchedule(job.CronExpression ?? "0/30 * * * * ?", x =>
                {
                    switch (job.MisfirePolicy)
                    {
                        case "1": x.WithMisfireHandlingInstructionFireAndProceed(); break;
                        case "2": x.WithMisfireHandlingInstructionIgnoreMisfires(); break;
                        default:  x.WithMisfireHandlingInstructionDoNothing(); break;
                    }
                })
                .WithDescription(job.JobName)
                .Build();

            if (await _scheduler.CheckExists(jobKey))
            {
                await _scheduler.RescheduleJob(triggerKey, trigger);
            }
            else
            {
                await _scheduler.ScheduleJob(detail, trigger);
            }

            if (job.Status == "1")
                await _scheduler.PauseJob(jobKey);
        }

        public async Task DeleteJobAsync(SysJobModel job)
        {
            if (_scheduler == null) return;
            var triggerKey = TriggerKey(job);
            if (await _scheduler.CheckExists(triggerKey))
            {
                await _scheduler.PauseTrigger(triggerKey);
                await _scheduler.UnscheduleJob(triggerKey);
            }
            var jobKey = JobKey(job);
            if (await _scheduler.CheckExists(jobKey))
                await _scheduler.DeleteJob(jobKey);
        }

        public async Task PauseJobAsync(SysJobModel job)
        {
            if (_scheduler != null)
                await _scheduler.PauseJob(JobKey(job));
        }

        public async Task ResumeJobAsync(SysJobModel job)
        {
            if (_scheduler != null)
                await _scheduler.ResumeJob(JobKey(job));
        }

        public async Task TriggerJobNowAsync(SysJobModel job)
        {
            if (_scheduler != null)
                await _scheduler.TriggerJob(JobKey(job));
        }

        public async Task<bool> ExistsAsync(SysJobModel job)
        {
            return _scheduler != null && await _scheduler.CheckExists(JobKey(job));
        }

        // ==================== 任务执行 & 日志 ====================

        public async Task ExecuteJob(long jobId, string jobName, string jobGroup, string invokeTarget)
        {
            var sw = Stopwatch.StartNew();
            var log = new SysJobLogModel
            {
                JobName = jobName,
                JobGroup = jobGroup,
                InvokeTarget = invokeTarget,
                CreateTime = DateTime.Now,
                Status = "0",
            };

            try
            {
                // 解析 invoke_target: "TaskName" 或 "TaskName(param1=val1,...)"
                (string taskName, string? parameters) = ParseInvokeTarget(invokeTarget);

                if (_executors.TryGetValue(taskName, out var executor))
                {
                    log.JobMessage = await executor.ExecuteAsync(parameters);
                }
                else
                {
                    log.Status = "1";
                    log.ExceptionInfo = $"找不到任务执行器: {taskName}";
                }
            }
            catch (Exception ex)
            {
                log.Status = "1";
                log.ExceptionInfo = Truncate(ex.ToString(), 2000);
            }
            finally
            {
                sw.Stop();
                log.ElapsedTime = sw.ElapsedMilliseconds;
            }

            await SaveLogAsync(log);
        }

        // ==================== private ====================

        private static JobKey JobKey(SysJobModel job) => new(job.JobId.ToString(), job.JobGroup);
        private static TriggerKey TriggerKey(SysJobModel job) => new($"t_{job.JobId}", job.JobGroup);

        private static (string name, string? parameters) ParseInvokeTarget(string raw)
        {
            raw = raw.Trim();
            var idx = raw.IndexOf('(');
            if (idx > 0 && raw.EndsWith(')'))
                return (raw[..idx].Trim(), raw[(idx + 1)..^1]);
            return (raw, null);
        }

        private static string Truncate(string text, int max)
        {
            return text.Length <= max ? text : text[..max];
        }

        private static async Task SaveLogAsync(SysJobLogModel log)
        {
            try
            {
                using var scope = GlobalContext.ServiceProvider!.CreateScope();
                var svc = scope.ServiceProvider.GetRequiredService<Wes.Service.ISysJobLogService>();
                svc.Save(log);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Scheduler] 写日志失败: {ex.Message}");
            }
        }
    }
}
