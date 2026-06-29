using Wes.Business;

namespace Wes.Scheduler;

/// <summary>
/// 钉钉用户同步任务 — 从钉钉拉取全部用户和部门，同步到本地数据库
/// </summary>
/// <remarks>
/// invoke_target: DingTalkUserSync
/// 依赖 sys.integration.dingtalk 配置项（AppId / ClientId / ClientSecret 等）
/// </remarks>
public class DingTalkUserSyncJob : ITaskExecutor
{
    private readonly ISysConfigBiz _configBiz;

    public DingTalkUserSyncJob(ISysConfigBiz configBiz)
    {
        _configBiz = configBiz;
    }

    /// <summary>
    /// 任务标识，对应 sys_job.invoke_target
    /// </summary>
    public string Name => "DingTalkUserSync";

    /// <summary>
    /// 执行钉钉组织架构同步（部门 + 用户）
    /// </summary>
    public async Task<string> ExecuteAsync(string? parameters)
    {
        var result = await _configBiz.SyncThirdPartyAsync("DingTalk");

        if (result.Code == 200)
            return $"钉钉用户同步成功：{result.Msg}";

        return $"钉钉用户同步失败 (Code={result.Code})：{result.Msg}";
    }
}
