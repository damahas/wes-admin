using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wes.AI.Models.Entity;
using Wes.AI.Models.Enum;
using Wes.AI.Models.ViewModel;
using Wes.AI.Services;
using Wes.Utils.Model;

namespace Wes.AI.Controllers;

/// <summary>
/// AI Agent 接口
/// </summary>
[ApiController]
[Route("ai/agent")]
public class AiAgentController : ControllerBase
{
    private readonly IAgentService _agentService;

    // SSE 分块统一使用 camelCase 序列化，保证前端按 chunk.content 解析（与 MVC 接口保持一致）
    private static readonly System.Text.Json.JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
    };

    public AiAgentController(IAgentService agentService)
    {
        _agentService = agentService;
    }

    /// <summary>
    /// 执行 Agent 对话（非流式）
    /// </summary>
    [HttpPost]
    [Route("{type}")]
    public async Task<ResultData<AiAgentResponse>> Execute(
        string type,
        [FromBody] AiAgentRequest request)
    {
        var result = await _agentService.ExecuteAsync(type, request);
        return new ResultData<AiAgentResponse>(result);
    }

    /// <summary>
    /// 执行 Agent 对话（SSE 流式）
    /// </summary>
    [HttpPost]
    [Route("{type}/stream")]
    public async Task ExecuteStream(
        string type,
        [FromBody] AiAgentRequest request)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        await foreach (var chunk in _agentService.ExecuteStreamAsync(type, request))
        {
            var json = System.Text.Json.JsonSerializer.Serialize(chunk, JsonOptions);
            await Response.WriteAsync($"data: {json}\n\n");
            await Response.Body.FlushAsync();
        }

        await Response.WriteAsync("data: [DONE]\n\n");
        await Response.Body.FlushAsync();
    }

    /// <summary>
    /// 获取可用的 Agent 类型列表
    /// </summary>
    [HttpGet]
    [Route("types")]
    public ResultData<List<AgentTypeInfo>> GetAgentTypes()
    {
        var types = _agentService.GetAgentTypes();
        return new ResultData<List<AgentTypeInfo>>(types);
    }

    /// <summary>
    /// 获取可用的 AI 提供商列表
    /// </summary>
    [HttpGet]
    [Route("providers")]
    public ResultData<List<ProviderInfo>> GetProviders([FromServices] Kernel.KernelProvider kernelProvider)
    {
        var list = kernelProvider.GetProviderList();
        return new ResultData<List<ProviderInfo>>(list);
    }

    /// <summary>
    /// 获取指定提供商的模型列表
    /// </summary>
    /// <param name="provider">提供商，为空表示不限</param>
    /// <param name="modelType">模型类型（llm / vision），为空表示不限</param>
    /// <param name="capabilities">需要具备的能力位掩码（见 AiModelCapability），为空表示不限。
    /// 例：前端要发图片时传 input_image 对应的位值，只返回能看图的模型。</param>
    [HttpGet]
    [Route("models")]
    public ResultData<List<ModelInfo>> GetModels(
        [FromQuery] string? provider,
        [FromQuery] string? modelType,
        [FromQuery] int? capabilities,
        [FromServices] Kernel.KernelProvider kernelProvider)
    {
        var require = capabilities is > 0 ? (AiModelCapability)capabilities.Value : AiModelCapability.None;
        var list = kernelProvider.GetModelList(provider, modelType, require);
        return new ResultData<List<ModelInfo>>(list);
    }
}
