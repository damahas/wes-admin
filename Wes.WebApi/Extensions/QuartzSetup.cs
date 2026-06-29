using Quartz;
using Quartz.Impl;
using Wes.Scheduler;

namespace Wes.WebApi.Extensions;

/// <summary>
/// Quartz 分布式定时任务配置（ADO.NET JobStore + 集群模式）
/// </summary>
public static class QuartzSetup
{
    public static IServiceCollection AddQuartzSetup(this IServiceCollection services, IConfiguration configuration)
    {
        var connStr = configuration.GetConnectionString("WesConnectionString");
        var instanceId = $"{Environment.MachineName}_{Guid.NewGuid():N}"[..100]; // 截断到 100 字符以内

        services.AddSingleton<ISchedulerFactory>(_ => new StdSchedulerFactory(
            new System.Collections.Specialized.NameValueCollection
            {
                { "quartz.scheduler.instanceName", "WesScheduler" },
                { "quartz.scheduler.instanceId", instanceId },
                { "quartz.threadPool.type", "Quartz.Simpl.SimpleThreadPool, Quartz" },
                { "quartz.threadPool.threadCount", "10" },

                // ADO.NET JobStore（MySQL）
                { "quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz" },
                { "quartz.jobStore.driverDelegateType", "Quartz.Impl.AdoJobStore.MySQLDelegate, Quartz" },
                { "quartz.jobStore.tablePrefix", "QRTZ_" },
                { "quartz.jobStore.dataSource", "default" },
                { "quartz.jobStore.clustered", "true" },
                { "quartz.jobStore.clusterCheckinInterval", "15000" },
                { "quartz.jobStore.maxMisfiresToHandleAtATime", "1" },
                { "quartz.jobStore.misfireThreshold", "60000" },
                { "quartz.jobStore.txIsolationLevelSerializable", "false" },

                // 数据源
                { "quartz.dataSource.default.connectionString", connStr },
                { "quartz.dataSource.default.provider", "MySql" },

                // 序列化
                { "quartz.serializer.type", "json" },
            }));

        services.AddSingleton<JobSchedulerService>();
        services.AddHostedService<SchedulerHostedService>();
        return services;
    }
}
