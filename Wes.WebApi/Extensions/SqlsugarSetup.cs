using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

namespace Wes.WebApi.Extensions
{
    public static class SqlsugarSetup
    {
        public static void AddSqlsugarSetup(this IServiceCollection services, IConfiguration configuration, string dbName = "WesConnectionString")
        {
            // Scoped 确保每个请求独立 SqlSugarScope，事务线程安全
            services.AddScoped<ISqlSugarClient>(sp => new SqlSugarScope(new ConnectionConfig()
            {
                DbType = DbType.MySql,
                ConnectionString = configuration.GetConnectionString(dbName),
                IsAutoCloseConnection = true,
            },
                db =>
                {
                    db.Aop.OnLogExecuting = (sql, pars) =>
                    {
                        Console.WriteLine(sql);
                    };
                }));

            // CodeFirst 初始化（启动时执行一次）
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetName().Name!.StartsWith("Wes."))
                .ToList();

            foreach (var dll in Directory.GetFiles(AppContext.BaseDirectory, "Wes.*.dll"))
            {
                var name = Path.GetFileNameWithoutExtension(dll);
                if (!assemblies.Any(a => a.GetName().Name == name))
                    assemblies.Add(Assembly.LoadFrom(dll));
            }

            var types = assemblies
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch (ReflectionTypeLoadException) { return Type.EmptyTypes; }
                })
                .Where(p => p.IsClass && p.GetCustomAttribute<SugarTable>() != null)
                .ToArray();

            using (var initDb = new SqlSugarScope(new ConnectionConfig()
            {
                DbType = DbType.MySql,
                ConnectionString = configuration.GetConnectionString(dbName),
                IsAutoCloseConnection = true,
            }))
            {
                initDb.CodeFirst.SetStringDefaultLength(200).InitTables(types);
                // CodeFirst 只建表不补列：补齐 AI 模块后续新增的列（耗时、真实 token 标记等）
                Wes.AI.Services.AiMessageSchema.EnsureColumns(initDb);
            }
        }
    }
}
