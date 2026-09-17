using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wes.AI.Models.Enum;
using Wes.AI.Models.ViewModel;
using Wes.AI.Services;
using Wes.Utils.Model;

namespace Wes.AI.Controllers;

/// <summary>
/// AI 模型配置接口（提供商 / 模型 / 能力 / 密钥维护）
/// </summary>
[ApiController]
[Authorize]
[Route("ai/model")]
public class AiModelController : ControllerBase
{
    private readonly IAiModelService _service;

    public AiModelController(IAiModelService service)
    {
        _service = service;
    }

    /// <summary>
    /// 模型配置列表（可按关键字、提供商过滤）
    /// </summary>
    [HttpGet]
    public async Task<ResultData<List<AiModelDto>>> List([FromQuery] string? keyword, [FromQuery] string? provider)
    {
        var list = await _service.ListAsync(keyword, provider);
        return new ResultData<List<AiModelDto>>(list);
    }

    /// <summary>
    /// 能力字典：配置页按分组渲染「输入 / 输出 / 附加」勾选框
    /// </summary>
    [HttpGet]
    [Route("capabilities")]
    public ResultData<List<CapabilityOption>> Capabilities()
    {
        return new ResultData<List<CapabilityOption>>(_service.Capabilities());
    }

    /// <summary>
    /// 模型详情（ApiKey 只回显掩码）
    /// </summary>
    [HttpGet]
    [Route("{id}")]
    public async Task<ResultData<AiModelDto?>> Get(long id)
    {
        var dto = await _service.GetAsync(id);
        return new ResultData<AiModelDto?>(dto);
    }

    /// <summary>
    /// 新增模型配置
    /// </summary>
    [HttpPost]
    public async Task<ResultData<long>> Create([FromBody] SaveAiModelRequest req)
    {
        req.Id = 0;
        var id = await _service.SaveAsync(req);
        return new ResultData<long>(id);
    }

    /// <summary>
    /// 修改模型配置
    /// </summary>
    [HttpPut]
    [Route("{id}")]
    public async Task<ResultData<bool>> Update(long id, [FromBody] SaveAiModelRequest req)
    {
        req.Id = id;
        var newId = await _service.SaveAsync(req);
        return new ResultData<bool>(newId > 0);
    }

    /// <summary>
    /// 删除模型配置（批量，逗号分隔）
    /// </summary>
    [HttpDelete]
    [Route("{ids}")]
    public async Task<ResultData<bool>> Delete(string ids)
    {
        var idList = (ids ?? "")
            .Split(',', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries)
            .Select(s => long.TryParse(s, out var v) ? v : 0)
            .Where(v => v > 0);

        var rows = await _service.DeleteAsync(idList);
        return new ResultData<bool>(rows > 0);
    }
}
