using Microsoft.AspNetCore.Mvc;
using Wes.AI.Models.Entity;
using Wes.AI.Models.ViewModel;
using Wes.AI.Services;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.AI.Controllers;

/// <summary>
/// AI 会话持久化接口（按当前登录用户隔离）
/// </summary>
[ApiController]
[Route("ai/session")]
public class AiSessionController : ControllerBase
{
    private readonly ISessionService _service;

    public AiSessionController(ISessionService service)
    {
        _service = service;
    }

    private long UserId => GlobalContext.CurrentUser?.UserId ?? 0;

    /// <summary>
    /// 当前用户的会话列表（按更新时间倒序）
    /// </summary>
    [HttpGet]
    public async Task<ResultData<List<AiSessionDto>>> List()
    {
        var list = await _service.ListAsync(UserId);
        return new ResultData<List<AiSessionDto>>(list);
    }

    /// <summary>
    /// 创建会话
    /// </summary>
    [HttpPost]
    public async Task<ResultData<AiSessionDto>> Create([FromBody] CreateSessionRequest req)
    {
        var dto = await _service.CreateAsync(UserId, req);
        return new ResultData<AiSessionDto>(dto);
    }

    /// <summary>
    /// 获取会话详情（含消息）
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ResultData<AiSessionDto?>> Get(long id)
    {
        var dto = await _service.GetAsync(UserId, id);
        return new ResultData<AiSessionDto?>(dto);
    }

    /// <summary>
    /// 更新会话（标题 / 场景 / 模型）
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ResultData<bool>> Update(long id, [FromBody] UpdateSessionRequest req)
    {
        var rows = await _service.UpdateAsync(UserId, id, req);
        return new ResultData<bool>(rows > 0);
    }

    /// <summary>
    /// 删除会话（含消息）
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ResultData<bool>> Delete(long id)
    {
        var rows = await _service.DeleteAsync(UserId, id);
        return new ResultData<bool>(rows > 0);
    }

    /// <summary>
    /// 保存（全量覆盖）会话消息
    /// </summary>
    [HttpPut("{id}/messages")]
    public async Task<ResultData<bool>> SaveMessages(long id, [FromBody] SaveMessagesRequest req)
    {
        var rows = await _service.SaveMessagesAsync(UserId, id, req.Messages);
        return new ResultData<bool>(rows > 0);
    }
}
