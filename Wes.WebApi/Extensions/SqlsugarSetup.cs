using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
            Type[] types = Assembly.LoadFrom(AppContext.BaseDirectory + "Wes.DbModel.dll")
            .GetTypes().Where(p => p.IsClass && p.FullName.EndsWith("Model")).ToArray();
            sqlSugar.CodeFirst.SetStringDefaultLength(200).InitTables(types);
            //这边是SqlSugarScope用AddSingleton
            services.AddSingleton<ISqlSugarClient>(sqlSugar);
        }
    }
}
