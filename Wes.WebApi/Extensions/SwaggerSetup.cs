using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.OpenApi;

namespace Wes.WebApi.Extensions;

/// <summary>
/// Swagger 文档配置（按模块分组）
/// </summary>
public static class SwaggerSetup
{
    public static IServiceCollection AddSwaggerSetup(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("SystemManage", new OpenApiInfo { Title = "系统管理", Version = "v1" });
            c.SwaggerDoc("FlowManage",   new OpenApiInfo { Title = "流程管理", Version = "v1" });
            c.SwaggerDoc("Scheduler",    new OpenApiInfo { Title = "定时任务", Version = "v1" });
        });
        return services;
    }

    public static IApplicationBuilder UseSwaggerSetup(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/SystemManage/swagger.json", "系统管理");
            c.SwaggerEndpoint("/swagger/FlowManage/swagger.json",   "流程管理");
            c.SwaggerEndpoint("/swagger/Scheduler/swagger.json",    "定时任务");
        });
        return app;
    }
}

/// <summary>
/// 根据 Controller 的 namespace 自动分配 Swagger 分组
/// </summary>
public class SwaggerGroup : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        var ns = controller.ControllerType.Namespace;
        if (string.IsNullOrEmpty(ns)) return;

        // "Wes.WebApi.Areas.SystemManage" → "SystemManage"
        // "Wes.WebApi.Areas.MonitorManage" → "SystemManage"
        // "Wes.WebApi.Controllers"         → "SystemManage"
        // "Wes.WebApi.Areas.FlowManage"    → "FlowManage"
        var parts = ns.Split('.');
        var group = parts[^1];

        if (group == "MonitorManage" || group == "Controllers")
            group = "SystemManage";

        // 定时任务控制器 → "Scheduler"
        if (controller.Attributes.OfType<RouteAttribute>().Any(r =>
            r.Template?.StartsWith("monitor/job") == true))
        {
            group = "Scheduler";
        }

        foreach (var action in controller.Actions)
        {
            action.ApiExplorer.GroupName = group;
        }
    }
}
