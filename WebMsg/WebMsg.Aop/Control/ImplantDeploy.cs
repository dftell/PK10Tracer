using Microsoft.Extensions.DependencyInjection;
using AspectCore.Extensions.DependencyInjection;
using AspectCore.Injector;

namespace WebMsg.Aop.Control
{
    /// <summary>
    /// 注入Aop
    /// </summary>
    public static class ImplantDeploy
    {
        /// <summary>
        /// 注入到DI
        /// </summary>
        /// <param name="services"></param>
        public static IServiceResolver ImplantAop(this IServiceCollection services)
        {
            //注入AOP服务（没有使用属性注入）
            services.ConfigureDynamicProxy();
            return services.ToServiceContainer().Build();
        }
    }
}
