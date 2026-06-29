using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Wes.Business;
using Wes.Utils;
using Wes.Utils.Model;

namespace Wes.WebApi.Extensions;

/// <summary>
/// JWT 认证配置
/// </summary>
public static class JwtSetup
{
    public static IServiceCollection AddJwtSetup(this IServiceCollection services)
    {
        var jwt = GlobalContext.JwtSettings;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer("Bearer", o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwt.SecretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                };

                o.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        string token = JWTUtils.GetToken(context.Request.Headers["authorization"]);
                        var sysUserBiz = GlobalContext.ServiceProvider.GetService<ISysUserBiz>();
                        if (sysUserBiz != null && sysUserBiz.TryGetUser(token, out var userInfo))
                        {
                            if (userInfo.Status == "1")
                            {
                                context.HttpContext.Items["AuthError"] = "AccountDisabled";
                                context.Fail("Account disabled");
                            }
                            return Task.CompletedTask;
                        }

                        context.HttpContext.Items["AuthError"] = "UserNotFound";
                        context.Fail("User not found");
                        return Task.CompletedTask;
                    },

                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            if (!context.Response.HasStarted)
                                context.Response.Headers.Add("Token-Expired", "true");
                            context.HttpContext.Items["AuthError"] = "TokenExpired";
                        }
                        return Task.CompletedTask;
                    },

                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        var httpContext = context.HttpContext;
                        if (httpContext.Response.HasStarted)
                            return Task.CompletedTask;

                        httpContext.Response.StatusCode = StatusCodes.Status200OK;
                        httpContext.Response.ContentType = "application/json";

                        string codeMsgJson;
                        if (httpContext.Items.TryGetValue("AuthError", out var errObj) && errObj is string err)
                        {
                            codeMsgJson = err switch
                            {
                                "AccountDisabled" => "{\"code\":403,\"msg\":\"账户已停用\"}",
                                "UserNotFound"   => "{\"code\":403,\"msg\":\"找不到用户\"}",
                                "TokenExpired"    => "{\"code\":401,\"msg\":\"Token已过期\"}",
                                _                 => $"{{\"code\":401,\"msg\":\"未登录\",\"path\":\"{httpContext.Request.Path}\"}}",
                            };
                        }
                        else
                        {
                            codeMsgJson = $"{{\"code\":401,\"msg\":\"未登录\",\"path\":\"{httpContext.Request.Path}\"}}";
                        }

                        return httpContext.Response.WriteAsync(codeMsgJson);
                    },
                };
            });

        return services;
    }
}
