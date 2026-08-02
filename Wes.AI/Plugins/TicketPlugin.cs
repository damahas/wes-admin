using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Wes.AI.Plugins;

/// <summary>
/// 工单操作插件
/// </summary>
public class TicketPlugin
{
    private readonly ILogger<TicketPlugin> _logger;

    public TicketPlugin(ILogger<TicketPlugin> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 创建工单
    /// </summary>
    [KernelFunction("create_ticket")]
    [Description("创建一个新的工单")]
    public async Task<string> CreateTicketAsync(
        [Description("工单标题")] string title,
        [Description("工单描述/内容")] string description,
        [Description("优先级：low/medium/high/urgent")] string priority = "medium",
        [Description("分配给的用户ID")] string? assigneeId = null)
    {
        _logger.LogInformation("TicketPlugin.CreateTicket called: title={Title}, priority={Priority}",
            title, priority);

        // TODO: 对接真实的工单系统
        return await Task.FromResult(
            $"已创建工单：标题={title}, 优先级={priority}。后续将对接真实工单系统。");
    }

    /// <summary>
    /// 查询工单列表
    /// </summary>
    [KernelFunction("query_tickets")]
    [Description("根据条件查询工单列表")]
    public async Task<string> QueryTicketsAsync(
        [Description("查询条件：如状态、优先级、创建人等")] string filter,
        [Description("返回最大条数，默认20")] int limit = 20)
    {
        _logger.LogInformation("TicketPlugin.QueryTickets called: filter={Filter}, limit={Limit}",
            filter, limit);

        // TODO: 对接真实工单系统
        return await Task.FromResult(
            $"查询工单条件：{filter}，限制：{limit} 条。后续将对接真实工单系统。");
    }

    /// <summary>
    /// 更新工单状态
    /// </summary>
    [KernelFunction("update_ticket_status")]
    [Description("更新指定工单的状态")]
    public async Task<string> UpdateTicketStatusAsync(
        [Description("工单ID")] string ticketId,
        [Description("新状态：pending/in_progress/resolved/closed")] string status,
        [Description("备注说明")] string? comment = null)
    {
        _logger.LogInformation("TicketPlugin.UpdateTicketStatus called: id={Id}, status={Status}",
            ticketId, status);

        return await Task.FromResult(
            $"工单 {ticketId} 状态已更新为 {status}。备注：{comment ?? "无"}");
    }
}
