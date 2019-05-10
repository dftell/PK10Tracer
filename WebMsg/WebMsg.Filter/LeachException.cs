using Microsoft.AspNetCore.Mvc.Filters;

namespace WebMsg.Filter
{
    /// <summary>
    /// 异常过滤
    /// </summary>
    public class LeachException : ExceptionFilterAttribute
    {
        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
        }
    }
}
