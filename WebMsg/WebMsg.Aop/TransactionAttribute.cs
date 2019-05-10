using AspectCore.DynamicProxy;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace WebMsg.Aop
{
    /*该拦截器中支持FromContainer特性注入*/
    /// <summary>
    /// 用于标记该方法体内的所有数据库执行操作为分布式事务操作
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TransactionAttribute : AbstractInterceptorAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    await next(context);//执行被拦截的方法
                    //提交事务
                    transactionScope.Complete();
                }
                catch (Exception ex)
                {
                    //出现异常
                    throw ex;
                }
            }
        }
    }
}
