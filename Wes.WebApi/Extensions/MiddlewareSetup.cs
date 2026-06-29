using Wes.Utils;

namespace Wes.WebApi.Extensions;

/// <summary>
/// 自定义中间件：异常处理、403 拦截、Token 注入
/// </summary>
public static class MiddlewareSetup
{
    /// <summary>
    /// 全局异常处理 → 统一 JSON 响应
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    await context.Response.WriteAsJsonAsync(new { code = 501, msg = ex.Message });
                }
            }
        });
    }

    /// <summary>
    /// 拦截 403 Forbidden → 统一 JSON 响应
    /// </summary>
    public static IApplicationBuilder UseForbiddenHandler(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            await next();

            if (context.Response.StatusCode == StatusCodes.Status403Forbidden && !context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsync("{\"code\":403,\"msg\":\"无权限\"}");
            }
        });
    }

    /// <summary>
    /// 从请求头提取 Token 并注入 GlobalContext
    /// </summary>
    public static IApplicationBuilder UseTokenExtraction(this IApplicationBuilder app)
    {
        return app.Use(next => context =>
        {
            GlobalContext.Token.Value = JWTUtils.GetToken(context.Request.Headers["authorization"]);
            return next(context);
        });
    }
}
