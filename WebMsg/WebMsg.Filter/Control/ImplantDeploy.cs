using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace WebMsg.Filter.Control
{
    public static class ImplantDeploy
    {
        /// <summary>
        /// 注入过滤器
        /// </summary>
        /// <param name="services"></param>
        public static void ImplantFilter(this IServiceCollection services)
        {
            services.AddScoped<LeachAction>();
            services.AddScoped<LeachAuthorization>();
            services.AddScoped<LeachException>();
        }
    }
}
