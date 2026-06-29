using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Wes.Utils;
using Wes.Utils.Converter;
using Wes.Utils.Hepler;
using Wes.Utils.Model;
using Wes.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ==================== 全局配置 ====================
GlobalContext.AppSettings = builder.Configuration.GetSection("SystemConfig").Get<AppSettings>();
GlobalContext.JwtSettings  = builder.Configuration.GetSection("JwtConfig").Get<JwtSettings>();

// ==================== 服务注册 ====================

// MVC + JSON + Swagger 分组
builder.Services.AddControllers(opt =>
{
    opt.Conventions.Add(new SwaggerGroup());
})
.AddApplicationPart(Assembly.Load("Wes.System"))
.AddApplicationPart(Assembly.Load("Wes.Scheduler"))
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
})
.AddMvcOptions(options => options.Filters.Add(new AuthorizeFilter()));

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// 基础设施
builder.Services.AddSqlsugarSetup(builder.Configuration);
builder.Services.AddCors(options =>
    options.AddPolicy("Cors", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// Swagger
builder.Services.AddSwaggerSetup();

// Autofac DI 扫描
builder.Host.UseAutofacSetup();

// 分布式定时任务
builder.Services.AddQuartzSetup(builder.Configuration);

// JWT 认证
builder.Services.AddJwtSetup();

// ==================== 管道 ====================

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseSwaggerSetup();

app.UseCors("Cors");

// 所有 API 统一 /api 前缀，中间件仅作用于 API 路由
app.Map("/api", apiApp =>
{
    apiApp.UseGlobalExceptionHandler();
    apiApp.UseAuthentication();
    apiApp.UseAuthorization();
    apiApp.UseForbiddenHandler();
    apiApp.UseTokenExtraction();
    apiApp.UseRouting();
    apiApp.UseEndpoints(endpoints => endpoints.MapControllers());
});

// ==================== 全局初始化 ====================

GlobalContext.ServiceProvider = app.Services;
NetHepler.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

// IP 归属地初始化（xdb 文件缺失时静默跳过）
try
{
    var xdbPath = Path.Combine(app.Environment.ContentRootPath, "UploadFile", "IpRegion", "ip2region.xdb");
    IpLocationHelper.Initialize(xdbPath);
}
catch (Exception) { /* 数据库文件未下载，IP 归属地为空 */ }

app.Run();
