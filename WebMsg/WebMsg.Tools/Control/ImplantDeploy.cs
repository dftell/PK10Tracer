using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebMsg.Tools.Control
{
    /// <summary>
    /// 注入Tool
    /// </summary>
    public static class ImplantDeploy
    {
        /// <summary>
        /// 注入到DI
        /// </summary>
        /// <param name="services"></param>
        public static void ImplantTool(this IServiceCollection services)
        {
            services.AddTransient<ITool, Tool>();
        }
    }
}
