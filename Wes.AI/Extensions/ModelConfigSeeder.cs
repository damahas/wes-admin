using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlSugar;
using Wes.AI.Models.Entity;
using Wes.AI.Models.Enum;
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
            BackfillCapabilities();
        }
        catch (Exception ex)
        {
            // 数据库未就绪（如连接串未配置）时不影响启动，稍后可手动补充
            _logger.LogWarning(ex, "AI 模型种子数据初始化/能力回填失败（可忽略，稍后通过数据库管理）");
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private void SeedIfEmpty()
    {
        if (_db.Queryable<AiModelEntity>().Any()) return;

        var now = DateTime.Now;

        // 能力位（见 AiModelCapability）：文本对话 + 工具 + 流式，视觉模型再加图片输入
        const int chat = (int)AiModelCapabilities.AgentChat;
        const int vision = (int)(AiModelCapabilities.AgentChat | AiModelCapability.InputImage);
        const int reason = (int)(AiModelCapabilities.AgentChat | AiModelCapability.Reasoning);

        var list = new List<AiModelEntity>
        {
            // 全局默认模型：DeepSeek Chat（上下文 64K），其 Provider 同时作为默认提供商
            new() { Id = SnowFlakeSingle.Instance.NextId(), Provider = "DeepSeek", ModelId = "deepseek-chat", DisplayName = "DeepSeek Chat", ApiKey = "", BaseUrl = "https://api.deepseek.com/v1", IsDefault = true, Sort = 1, MaxContext = 65536, Capabilities = chat, CreatedAt = now, UpdatedAt = now },
            new() { Id = SnowFlakeSingle.Instance.NextId(), Provider = "DeepSeek", ModelId = "deepseek-reasoner", DisplayName = "DeepSeek Reasoner", ApiKey = "", BaseUrl = "https://api.deepseek.com/v1", IsDefault = false, Sort = 2, MaxContext = 65536, Capabilities = reason, CreatedAt = now, UpdatedAt = now },

            new() { Id = SnowFlakeSingle.Instance.NextId(), Provider = "通义千问", ModelId = "qwen-plus", DisplayName = "通义千问 Plus", ApiKey = "", BaseUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1", IsDefault = false, Sort = 3, MaxContext = 131072, Capabilities = chat, CreatedAt = now, UpdatedAt = now },
            new() { Id = SnowFlakeSingle.Instance.NextId(), Provider = "通义千问", ModelId = "qwen-max", DisplayName = "通义千问 Max", ApiKey = "", BaseUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1", IsDefault = false, Sort = 4, MaxContext = 32768, Capabilities = chat, CreatedAt = now, UpdatedAt = now },

            new() { Id = SnowFlakeSingle.Instance.NextId(), Provider = "智谱 GLM", ModelId = "glm-4-flash", DisplayName = "GLM-4-Flash", ApiKey = "", BaseUrl = "https://open.bigmodel.cn/api/paas/v4", IsDefault = false, Sort = 5, MaxContext = 131072, Capabilities = chat, CreatedAt = now, UpdatedAt = now },

            new() { Id = SnowFlakeSingle.Instance.NextId(), Provider = "OpenAI", ModelId = "gpt-4o-mini", DisplayName = "GPT-4o Mini", ApiKey = "", BaseUrl = "https://api.openai.com/v1", IsDefault = false, Sort = 6, MaxContext = 128000, Capabilities = vision, CreatedAt = now, UpdatedAt = now },
            new() { Id = SnowFlakeSingle.Instance.NextId(), Provider = "OpenAI", ModelId = "gpt-4o", DisplayName = "GPT-4o", ApiKey = "", BaseUrl = "https://api.openai.com/v1", IsDefault = false, Sort = 7, MaxContext = 128000, Capabilities = vision, CreatedAt = now, UpdatedAt = now },

            // 视觉模型示例（其余条目未显式指定 ModelType，取实体默认值 llm）
            new() { Id = SnowFlakeSingle.Instance.NextId(), Provider = "通义千问", ModelId = "qwen-vl-max", DisplayName = "通义千问 VL Max", ApiKey = "", BaseUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1", IsDefault = false, Sort = 9, MaxContext = 32768, Capabilities = vision, ModelType = AiModelType.Vision, CreatedAt = now, UpdatedAt = now }
        };

        _db.Insertable(list).ExecuteCommand();
        _logger.LogInformation("已插入 {Count} 条默认 AI 模型数据", list.Count);
    }

    /// <summary>
    /// 存量数据迁移：capabilities 为 0 的历史行，按 model_type 回填能力位，
    /// 避免老库升级后所有模型「没有能力」而被能力筛选过滤掉。
    /// </summary>
    private void BackfillCapabilities()
    {
        var chat = (int)AiModelCapabilities.AgentChat;
        var vision = (int)(AiModelCapabilities.AgentChat | AiModelCapability.InputImage);

        _db.Updateable<AiModelEntity>()
            .SetColumns(m => m.Capabilities == chat)
            .Where(m => m.Capabilities <= 0 && m.ModelType != AiModelType.Vision)
            .ExecuteCommand();

        _db.Updateable<AiModelEntity>()
            .SetColumns(m => m.Capabilities == vision)
            .Where(m => m.Capabilities <= 0 && m.ModelType == AiModelType.Vision)
            .ExecuteCommand();
    }
}
