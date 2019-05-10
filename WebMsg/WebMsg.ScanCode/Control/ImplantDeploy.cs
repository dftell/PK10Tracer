using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebMsg.ScanCode.Control
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
        public static void ImplantScanCode(this IServiceCollection services)
        {
            services.AddTransient<IQRCode, QRCode>();
        }
    }
}
