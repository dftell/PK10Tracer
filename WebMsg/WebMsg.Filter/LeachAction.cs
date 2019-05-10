using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebMsg.Filter
{
    /// <summary>
    /// 方法过滤器,如果需要使用注入的服务必须这样使用[ServiceFilter(typeof(WebMsg.Filter.LeachAction))]
    /// </summary>
    public class LeachAction : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }
    }
}
