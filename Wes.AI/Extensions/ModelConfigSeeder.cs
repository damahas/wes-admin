using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlSugar;
using Wes.AI.Models.Entity;
using Wes.AI.Models.ViewModel;

namespace Wes.AI.Extensions;

/// <summary>
/// 启动种子：若 ai_model 表为空，插入默认模型数据（apikey 为空）
/// </summary>
public class ModelConfigSeeder : IHostedService
{
    private readonly ISqlSugarClient _db;
    private readonly ILogger<ModelConfigSeeder> _logger;

    public ModelConfigSeeder(ISqlSugarClient db, ILogger<ModelConfigSeeder> logger)
    {
        _db = db;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            SeedIfEmpty();
        }
        catch (Exception ex)
        {
            // 数据库未就绪（如连接串未配置）时不影响启动，稍后可手动补充
            _logger.LogWarning(ex, "AI 模型种子数据初始化失败（可忽略，稍后通过数据库管理）");
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private void SeedIfEmpty()
    {
        if (_db.Queryable<AiModelEntity>().Any()) return;

        var now = DateTime.Now;
        var list = new List<AiModelEntity>
        {
            // 全局默认提供商：DeepSeek
            new() { Id = SnowFlakeSingle.Instance.NextId(), Provider = "deepseek", ProviderName = "DeepSeek", ModelId = "deepseek-chat", DisplayName = "DeepSeek Chat", ApiKey = "", BaseUrl = "https://api.deepseek.com/v1", IsDefault = true, IsDefaultProvider = true, Sort = 1, CreatedAt = now, UpdatedAt = now },
            new() { Id = SnowFlakeSingle.Instance.NextId(), Provider = "deepseek", ProviderName = "DeepSeek", ModelId = "deepseek-reasoner", DisplayName = "DeepSeek Reasoner", ApiKey = "", BaseUrl = "https://api.deepseek.com/v1", IsDefault = false, IsDefaultProvider = false, Sort = 2, CreatedAt = now, UpdatedAt = now },

            new() { Id = SnowFlakeSingle.Instance.NextId(), Provider = "qwen", ProviderName = "通义千问", ModelId = "qwen-plus", DisplayName = "通义千问 Plus", ApiKey = "", BaseUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1", IsDefault = true, IsDefaultProvider = false, Sort = 3, CreatedAt = now, UpdatedAt = now },
            new() { Id = SnowFlakeSingle.Instance.NextId(), Provider = "qwen", ProviderName = "通义千问", ModelId = "qwen-max", DisplayName = "通义千问 Max", ApiKey = "", BaseUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1", IsDefault = false, IsDefaultProvider = false, Sort = 4, CreatedAt = now, UpdatedAt = now },

            new() { Id = SnowFlakeSingle.Instance.NextId(), Provider = "glm", ProviderName = "智谱 GLM", ModelId = "glm-4-flash", DisplayName = "GLM-4-Flash", ApiKey = "", BaseUrl = "https://open.bigmodel.cn/api/paas/v4", IsDefault = true, IsDefaultProvider = false, Sort = 5, CreatedAt = now, UpdatedAt = now },

            new() { Id = SnowFlakeSingle.Instance.NextId(), Provider = "openai", ProviderName = "OpenAI", ModelId = "gpt-4o-mini", DisplayName = "GPT-4o Mini", ApiKey = "", BaseUrl = "https://api.openai.com/v1", IsDefault = true, IsDefaultProvider = false, Sort = 6, CreatedAt = now, UpdatedAt = now },
            new() { Id = SnowFlakeSingle.Instance.NextId(), Provider = "openai", ProviderName = "OpenAI", ModelId = "gpt-4o", DisplayName = "GPT-4o", ApiKey = "", BaseUrl = "https://api.openai.com/v1", IsDefault = false, IsDefaultProvider = false, Sort = 7, CreatedAt = now, UpdatedAt = now }
        };

        _db.Insertable(list).ExecuteCommand();
        _logger.LogInformation("已插入 {Count} 条默认 AI 模型数据", list.Count);
    }
}
