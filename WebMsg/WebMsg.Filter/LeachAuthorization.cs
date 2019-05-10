using WebMsg.IApp;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebMsg.Filter
{
    /// <summary>
    /// 授权过滤，如果需要使用注入的服务必须这样使用[ServiceFilter(typeof(WebMsg.Filter.LeachAuthorization))]
    /// </summary>
    public class LeachAuthorization : Attribute, IAuthorizationFilter
    {
        IAuthCode _ac;
        public LeachAuthorization(IAuthCode _act)
        {
            _ac = _act;
        }
        /// <summary>
        /// 授权功能
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var a = _ac;
        }
    }
}
