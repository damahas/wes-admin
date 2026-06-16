using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using System.IO;
using Wes.Business;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.WebApi.Extensions;
using Wes.Utils.Converter;
using Wes.Utils.Hepler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(opt =>
{
    opt.UseCentralRoutePrefix(new RouteAttribute("api"));
}).AddJsonOptions(options =>
{
    //options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
}).AddMvcOptions(options => options.Filters.Add(new AuthorizeFilter()));
builder.Services.AddMemoryCache();
GlobalContext.AppSettings = builder.Configuration.GetSection("SystemConfig").Get<AppSettings>();
GlobalContext.JwtSettings = builder.Configuration.GetSection("JwtConfig").Get<JwtSettings>();
// sqlsugar
builder.Services.AddSqlsugarSetup(builder.Configuration);
// 跨域
builder.Services.AddCors(
 options => options.AddPolicy("Cors", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod())
);
// 注入HttpContext，获取网络信息
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Register Swagger (basic setup)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 注入dll
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(container =>
{
    Assembly[] assemblys = new Assembly[] {
 Assembly.Load("Wes.Business"),
 Assembly.Load("Wes.Service"),
 Assembly.Load("Wes.WebApi")
 };
    container.RegisterAssemblyTypes(assemblys).PublicOnly()
    .Where(c => c.IsClass)
    .AsImplementedInterfaces()
    .InstancePerLifetimeScope();
});

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer("Bearer", o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,//是否验证签名,不验证的话可以篡改数据，不安全
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(GlobalContext.JwtSettings.SecretKey)),//解密的密钥
        ValidateIssuer = true,//是否验证发行人，就是验证载荷中的Iss是否对应ValidIssuer参数
        ValidIssuer = GlobalContext.JwtSettings.Issuer,//发行人,
        ValidateAudience = true,//是否验证订阅人，就是验证载荷中的Aud是否对应ValidAudience参数
        ValidAudience = GlobalContext.JwtSettings.Audience//订阅人 
    };
    o.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            // 不要直接写入响应，标记失败并把原因放入 HttpContext.Items，由 OnChallenge统一处理
            string token = JWTUtils.GetToken(context.Request.Headers["authorization"]);
            var sysUserBiz = GlobalContext.ServiceProvider.GetService<ISysUserBiz>();
            UserInfo userInfo;
            if (sysUserBiz.TryGetUser(token, out userInfo))
            {
                if (userInfo.Status == "1")
                {
                    context.HttpContext.Items["AuthError"] = "AccountDisabled";
                    context.Fail("Account disabled");
                    return Task.CompletedTask;
                }
                return Task.CompletedTask;
            }

            context.HttpContext.Items["AuthError"] = "UserNotFound";
            context.Fail("User not found");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            // 如果过期，则把<是否过期>添加到返回头信息中，并记录原因，不直接写响应体
            if (context.Exception is SecurityTokenExpiredException)
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                context.HttpContext.Items["AuthError"] = "TokenExpired";
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            //统一处理认证失败或缺失的响应，避免在其他事件中直接写响应导致冲突
            context.HandleResponse();

            var httpContext = context.HttpContext;
            if (!httpContext.Response.HasStarted)
            {
                httpContext.Response.StatusCode = StatusCodes.Status200OK; // 保持原有行为，实际状态通过 body.code 返回
                httpContext.Response.ContentType = "application/json";

                string codeMsgJson;
                if (httpContext.Items.TryGetValue("AuthError", out var errObj) && errObj is string err)
                {
                    switch (err)
                    {
                        case "AccountDisabled":
                            codeMsgJson = "{\"code\":403,\"msg\":\"账户已停用\"}";
                            break;
                        case "UserNotFound":
                            codeMsgJson = "{\"code\":403,\"msg\":\"找不到用户\"}";
                            break;
                        case "TokenExpired":
                            codeMsgJson = "{\"code\":401,\"msg\":\"Token已过期\"}";
                            break;
                        default:
                            codeMsgJson = $"{{\"code\":401,\"msg\":\"未登录\",\"path\":\"{httpContext.Request.Path}\"}}";
                            break;
                    }
                }
                else
                {
                    codeMsgJson = $"{{\"code\":401,\"msg\":\"未登录\",\"path\":\"{httpContext.Request.Path}\"}}";
                }

                return httpContext.Response.WriteAsync(codeMsgJson);
            }

            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Cors");

// 全局异常处理中间件
app.Use(async (context, next) =>
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
            context.Response.ContentType = "application/json";
            var msg = System.Text.Json.JsonSerializer.Serialize(ex.Message);
            await context.Response.WriteAsync($"{{\"code\":501,\"msg\":{msg}}}");
        }
    }
});

app.UseAuthentication();
app.UseAuthorization();

// 拦截403 响应并返回统一 JSON 格式
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == StatusCodes.Status403Forbidden && !context.Response.HasStarted)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status200OK; // 保持与项目中其他错误响应一致
        await context.Response.WriteAsync("{\"code\":403,\"msg\":\"无权限\"}");
    }
});

app.Use(next =>
{
    return context =>
    {
        GlobalContext.Token.Value = JWTUtils.GetToken(context.Request.Headers["authorization"]);
        return next(context);
    };
});

app.MapControllers();

GlobalContext.ServiceProvider = app.Services;

NetHepler.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

// IP 归属地初始化（xdb 文件未下载时静默跳过，不影响启动）
try
{
    var xdbPath = Path.Combine(app.Environment.ContentRootPath, "UploadFile", "IpRegion", "ip2region.xdb");
    IpLocationHelper.Initialize(xdbPath);
}
catch (Exception) { /* 数据库文件未下载，IP 归属地为空 */ }

app.Run();
