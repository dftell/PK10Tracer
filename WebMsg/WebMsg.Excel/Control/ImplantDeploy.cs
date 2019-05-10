using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebMsg.Excel.Control
{
    /// <summary>
    /// 注入帮助
    /// </summary>
    public static class ImplantDeploy
    {
        /// <summary>
        /// 注入到DI
        /// </summary>
        /// <param name="services"></param>
        public static void ImplantExcel(this IServiceCollection services)
        {
            services.AddTransient<IExcel, Excel>();
        }
    }
}
