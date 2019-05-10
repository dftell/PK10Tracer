using AspectCore.DynamicProxy;
using AspectCore.Injector;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebMsg.Aop
{
    /// <summary>
    /// 使用属性进行依赖注入
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public class DependencyAttribute : AbstractInterceptorAttribute
    {
        public override Task Invoke(AspectContext context, AspectDelegate next)
        {
            return null;
        }
    }
}
