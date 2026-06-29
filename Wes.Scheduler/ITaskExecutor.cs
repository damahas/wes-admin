namespace Wes.Scheduler
{
    /// <summary>
    /// 任务执行器接口，外部项目实现并注册到DI即可被定时任务调度
    /// </summary>
    public interface ITaskExecutor
    {
        /// <summary>
        /// 任务唯一标识，对应 sys_job.invoke_target 中括号前的部分
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="parameters">invoke_target 括号内的参数</param>
        /// <returns>执行结果信息</returns>
        Task<string> ExecuteAsync(string? parameters);
    }
}
