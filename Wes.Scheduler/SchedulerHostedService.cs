using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using SqlSugar;
using Wes.DbModel;
using Wes.Scheduler;

namespace Wes.Scheduler
{
    /// <summary>
    /// 应用启动时启动 Quartz 调度器，加载所有启用的定时任务。
    /// 分布式模式：所有节点共享 QRTZ_* 数据库表，Quartz 集群锁保证同一任务只在一个节点执行。
    /// </summary>
    public class SchedulerHostedService : IHostedService
    {
        private readonly JobSchedulerService _schedulerService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISchedulerFactory _schedulerFactory;

        public SchedulerHostedService(
            JobSchedulerService schedulerService,
            IServiceProvider serviceProvider,
            ISchedulerFactory schedulerFactory)
        {
            _schedulerService = schedulerService;
            _serviceProvider = serviceProvider;
            _schedulerFactory = schedulerFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // 1. 注册所有 ITaskExecutor 实现
            var executors = _serviceProvider.GetServices<ITaskExecutor>().ToList();
            foreach (var executor in executors)
            {
                _schedulerService.RegisterExecutor(executor);
            }

            // 2. 启动 Quartz
            await _schedulerService.StartAsync();
            var scheduler = await _schedulerFactory.GetScheduler();
            Console.WriteLine($"[Scheduler] 节点启动，InstanceId = {scheduler.SchedulerInstanceId}");

            // 3. 自动发现：扫描 ITaskExecutor，未在 sys_job 表中注册的自动入库（默认暂停）
            if (executors.Count > 0)
            {
                await AutoRegisterTasks(executors);
            }

            // 4. 加载所有启用状态的任务
            //    分布式模式：Quartz Job/Trigger 状态存储在 QRTZ_ 表中，多节点共享。
            //    此处从 sys_job 读取后逐个 CheckExists 再 Add/Reschedule，保证与业务表一致。
            //    过滤掉 cron 为空或格式无效的任务，避免加载异常。
            var db = _serviceProvider.GetRequiredService<ISqlSugarClient>();
            var jobs = await db.Queryable<SysJobModel>()
                .Where(p => p.Status == "0")
                .ToListAsync(cancellationToken);

            int added = 0, updated = 0, skipped = 0, badCron = 0;
            foreach (var job in jobs)
            {
                // cron 校验：为空或表达式无效则跳过
                if (string.IsNullOrWhiteSpace(job.CronExpression))
                {
                    badCron++;
                    Console.WriteLine($"[Scheduler] 跳过任务 [{job.JobName}]：Cron 表达式为空");
                    continue;
                }
                if (!CronExpression.IsValidExpression(job.CronExpression))
                {
                    badCron++;
                    Console.WriteLine($"[Scheduler] 跳过任务 [{job.JobName}]：Cron 表达式无效 [{job.CronExpression}]");
                    continue;
                }

                try
                {
                    if (await _schedulerService.ExistsAsync(job))
                    {
                        await _schedulerService.AddJobAsync(job);
                        updated++;
                    }
                    else
                    {
                        await _schedulerService.AddJobAsync(job);
                        added++;
                    }
                }
                catch (Exception ex)
                {
                    skipped++;
                    Console.WriteLine($"[Scheduler] 加载任务失败 [{job.JobName}]: {ex.Message}");
                }
            }

            Console.WriteLine($"[Scheduler] 已就绪，新增 {added}、更新 {updated}、跳过 {skipped} 个任务，{badCron} 个 Cron 无效（共 {jobs.Count} 个）");
        }

        /// <summary>
        /// 扫描所有 ITaskExecutor，将未注册的自动插入 sys_job 表，默认状态为暂停（1）
        /// </summary>
        private async Task AutoRegisterTasks(List<ITaskExecutor> executors)
        {
            var db = _serviceProvider.GetRequiredService<ISqlSugarClient>();

            // 查出现有 invoke_target 集合
            var existingTargets = await db.Queryable<SysJobModel>()
                .Select(p => p.InvokeTarget)
                .ToListAsync();

            // 仅匹配括号前的主任务名
            var existingNames = new HashSet<string>(
                existingTargets.Select(t =>
                {
                    var idx = t.IndexOf('(');
                    return idx > 0 ? t[..idx] : t;
                }),
                StringComparer.OrdinalIgnoreCase
            );

            int registered = 0;
            foreach (var executor in executors)
            {
                if (existingNames.Contains(executor.Name))
                    continue;

                var model = new SysJobModel
                {
                    JobId = 0,
                    JobName = executor.Name,
                    JobGroup = "SYSTEM",
                    InvokeTarget = executor.Name,
                    CronExpression = "0 0 0 * * ?",  // 默认每天午夜，管理员按需修改
                    MisfirePolicy = "3",
                    Concurrent = "1",
                    Status = "1",                   // 默认暂停，需手动启用
                    CreateBy = "SYSTEM",
                    CreateTime = DateTime.Now,
                    Remark = "系统自动发现",
                };

                try
                {
                    model.JobId = db.Insertable(model).ExecuteReturnSnowflakeId();
                    registered++;
                    Console.WriteLine($"[Scheduler] 自动发现任务 [{executor.Name}] 已入库（暂停状态，JobId={model.JobId}）");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Scheduler] 自动注册任务失败 [{executor.Name}]: {ex.Message}");
                }
            }

            if (registered > 0)
            {
                Console.WriteLine($"[Scheduler] 自动发现 {registered} 个新任务，已入库且默认为暂停状态");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _schedulerService.StopAsync();
            Console.WriteLine("[Scheduler] 节点已停止");
        }
    }
}
