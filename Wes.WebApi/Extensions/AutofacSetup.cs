using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace Wes.WebApi.Extensions;

/// <summary>
/// Autofac DI 容器配置：自动扫描 Wes.*.dll 并注册服务
/// </summary>
public static class AutofacSetup
{
    public static IHostBuilder UseAutofacSetup(this IHostBuilder host)
    {
        host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        host.ConfigureContainer<ContainerBuilder>(container =>
        {
            var assemblys = Directory.GetFiles(AppContext.BaseDirectory, "Wes.*.dll")
                .Select(dll => Assembly.Load(Path.GetFileNameWithoutExtension(dll)))
                .ToArray();

            container.RegisterAssemblyTypes(assemblys)
                .PublicOnly()
                .Where(c => c.IsClass && !c.IsAbstract && c.GetInterfaces().Length > 0)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        });
        return host;
    }
}
