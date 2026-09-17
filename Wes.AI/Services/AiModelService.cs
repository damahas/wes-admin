using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlSugar;
using Wes.AI.Kernel;
using Wes.AI.Models.Entity;
using Wes.AI.Models.Enum;
using Wes.AI.Models.ViewModel;
using Wes.Utils.Extension;

namespace Wes.AI.Services;

/// <summary>
/// AI 模型配置维护（能力、密钥、默认项）
/// </summary>
public interface IAiModelService
{
    /// <summary>模型列表（可按关键字、提供商过滤）</summary>
    Task<List<AiModelDto>> ListAsync(string? keyword, string? provider);

    /// <summary>模型详情</summary>
    Task<AiModelDto?> GetAsync(long id);

    /// <summary>新增或更新，返回主键</summary>
    Task<long> SaveAsync(SaveAiModelRequest req);

    /// <summary>批量删除</summary>
    Task<int> DeleteAsync(IEnumerable<long> ids);

    /// <summary>能力字典（供前端配置页渲染）</summary>
    List<CapabilityOption> Capabilities();
}

public class AiModelService : IAiModelService
{
    private readonly ISqlSugarClient _db;
    private readonly KernelProvider _kernelProvider;

    public AiModelService(ISqlSugarClient db, KernelProvider kernelProvider)
    {
        _db = db;
        _kernelProvider = kernelProvider;
    }

    public async Task<List<AiModelDto>> ListAsync(string? keyword, string? provider)
    {
        var query = _db.Queryable<AiModelEntity>();

        if (!string.IsNullOrWhiteSpace(provider))
            query = query.Where(m => m.Provider == provider);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim();
            query = query.Where(m =>
                m.Provider.Contains(kw) ||
                m.ModelId.Contains(kw) || m.DisplayName.Contains(kw));
        }

        var rows = await query
            .OrderBy(m => m.Provider)
            .OrderBy(m => m.Sort)
            .ToListAsync();

        return rows.Select(ToDto).ToList();
    }

    public async Task<AiModelDto?> GetAsync(long id)
    {
        var row = await _db.Queryable<AiModelEntity>().FirstAsync(m => m.Id == id);
        return row == null ? null : ToDto(row);
    }

    public async Task<long> SaveAsync(SaveAiModelRequest req)
    {
        // 未勾选能力时给一套合理的默认能力（文本对话 + 工具 + 流式）
        var capabilities = req.CapabilityKeys is { Count: > 0 }
            ? AiModelCapabilities.FromKeys(req.CapabilityKeys)
            : AiModelCapabilities.Default;

        var now = DateTime.Now;

        if (req.Id > 0)
        {
            var entity = await _db.Queryable<AiModelEntity>().FirstAsync(m => m.Id == req.Id);
            if (entity == null) return 0;

            entity.Provider = req.Provider.Trim();
            entity.ModelId = req.ModelId.Trim();
            entity.DisplayName = req.DisplayName.Trim();
            entity.BaseUrl = req.BaseUrl.Trim();
            entity.MaxContext = req.MaxContext;
            entity.Capabilities = (int)capabilities;
            // 模型类型跟随能力派生，保持与旧字段一致（仅用于兼容展示）
            entity.ModelType = AiModelCapabilities.IsMultimodal(capabilities)
                ? AiModelType.Vision
                : AiModelType.Llm;
            entity.IsDefault = req.IsDefault;
            entity.Sort = req.Sort;
            entity.UpdatedAt = now;
            // 密钥留空表示不修改，避免前端回显掩码后又把掩码写回库里
            if (!string.IsNullOrWhiteSpace(req.ApiKey))
                entity.ApiKey = req.ApiKey.Trim();

            await _db.Updateable(entity).ExecuteCommandAsync();
            await EnsureUniqueDefaults(entity);

            // 配置变更必须同步清 Kernel 缓存：Kernel 构建时即固化了 endpoint 与默认 modelId，
            // 不清会导致「新模型名 + 旧地址」，调用时报 404 model_not_found
            _kernelProvider.Invalidate();

            return entity.Id;
        }

        var add = new AiModelEntity
        {
            Id = SnowFlakeSingle.Instance.NextId(),
            Provider = req.Provider.Trim(),
            ModelId = req.ModelId.Trim(),
            DisplayName = req.DisplayName.Trim(),
            BaseUrl = req.BaseUrl.Trim(),
            ApiKey = (req.ApiKey ?? "").Trim(),
            MaxContext = req.MaxContext,
            Capabilities = (int)capabilities,
            ModelType = AiModelCapabilities.IsMultimodal(capabilities) ? AiModelType.Vision : AiModelType.Llm,
            IsDefault = req.IsDefault,
            Sort = req.Sort,
            CreatedAt = now,
            UpdatedAt = now
        };
        await _db.Insertable(add).ExecuteCommandAsync();
        await EnsureUniqueDefaults(add);

        _kernelProvider.Invalidate();

        return add.Id;
    }

    public async Task<int> DeleteAsync(IEnumerable<long> ids)
    {
        var list = ids?.Where(i => i > 0).Distinct().ToList() ?? new List<long>();
        if (list.Count == 0) return 0;

        var rows = await _db.Deleteable<AiModelEntity>().In(list).ExecuteCommandAsync();

        // 删除可能影响默认项与提供商分组，全量清缓存
        _kernelProvider.Invalidate();

        return rows;
    }

    public List<CapabilityOption> Capabilities() => AiModelCapabilities.Options();

    /// <summary>
    /// 保证「全局默认模型唯一」：开启后其余行的 IsDefault 自动置 false
    /// </summary>
    private async Task EnsureUniqueDefaults(AiModelEntity entity)
    {
        if (!entity.IsDefault) return;

        await _db.Updateable<AiModelEntity>()
            .SetColumns(m => m.IsDefault == false)
            .Where(m => m.Id != entity.Id)
            .ExecuteCommandAsync();
    }

    private static AiModelDto ToDto(AiModelEntity m)
    {
        var cap = AiModelCapabilities.Normalize(m.Capabilities, m.ModelType);
        return new AiModelDto
        {
            Id = m.Id,
            Provider = m.Provider,
            ModelId = m.ModelId,
            DisplayName = m.DisplayName,
            BaseUrl = m.BaseUrl,
            MaxContext = m.MaxContext,
            Capabilities = (int)cap,
            CapabilityKeys = AiModelCapabilities.ToKeys(cap),
            CapabilitySummary = AiModelCapabilities.Summary(cap),
            ModelTypeName = AiModelCapabilities.TypeName(cap),
            IsDefault = m.IsDefault,
            Sort = m.Sort,
            HasApiKey = !string.IsNullOrWhiteSpace(m.ApiKey),
            ApiKeyMasked = Mask(m.ApiKey)
        };
    }

    /// <summary>密钥掩码：sk-****1a2b</summary>
    private static string Mask(string? key)
    {
        if (string.IsNullOrEmpty(key)) return "";
        if (key.Length <= 8) return new string('*', key.Length);
        return key[..4] + "****" + key[^4..];
    }
}
