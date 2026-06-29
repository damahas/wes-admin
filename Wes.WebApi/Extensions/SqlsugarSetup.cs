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
            SqlSugarScope sqlSugar = new SqlSugarScope(new ConnectionConfig()
            {
                DbType = DbType.MySql,
                ConnectionString = configuration.GetConnectionString(dbName),
                IsAutoCloseConnection = true,
            },
                db =>
                {
                    //单例参数配置，所有上下文生效
                    db.Aop.OnLogExecuting = (sql, pars) =>
                        {
                            Console.WriteLine(sql);//输出sql
                        };
                });
            // 自动扫描所有 Wes.* 程序集中带 [SugarTable] 的实体类
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetName().Name!.StartsWith("Wes."))
                .ToList();

            // 补充未被 JIT 加载的程序集
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
            sqlSugar.CodeFirst.SetStringDefaultLength(200).InitTables(types);
            //这边是SqlSugarScope用AddSingleton
            services.AddSingleton<ISqlSugarClient>(sqlSugar);
        }
    }
}
