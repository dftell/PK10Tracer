using AspectCore.DynamicProxy;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace WebMsg.Aop
{
    /// <summary>
    /// 实现方法的返回值缓存
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CachingAttribute : AbstractInterceptorAttribute
    {
        /// <summary>
        /// 内存缓存服务
        /// </summary>
        private static readonly IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
        /// <summary>
        /// 缓存时间/秒  0表示永不失效
        /// </summary>
        public int CachingTime { get; set; }
        /// <summary>
        /// 动态拦截的方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var Key = new Cachekey(context.ServiceMethod, context.Parameters).GetHashCode();
            if (memoryCache.TryGetValue(Key, out object value))
            {
                context.ReturnValue = ((MemoryCacheEntity)value).ReturnValue;
                var Parameters = ((MemoryCacheEntity)value).Parameters;
                if (context.Parameters != null && Parameters != null && Parameters.Length == context.Parameters.Length)
                {
                    for (int i = 0; i < context.Parameters.Length; i++)
                    {
                        context.Parameters[i] = Parameters[i];
                    }
                }
            }
            else
            {
                await context.Invoke(next);
                if (CachingTime <= 0)
                {
                    //不过期
                    memoryCache.Set(Key, new MemoryCacheEntity(context.ReturnValue, context.Parameters));
                }
                else
                {
                    memoryCache.Set(Key, new MemoryCacheEntity(context.ReturnValue, context.Parameters), new TimeSpan(0, 0, CachingTime));
                }
            }
        }
        /// <summary>
        /// 缓存的键
        /// </summary>
        private class Cachekey
        {
            public MethodBase Method { get; }
            public object[] InputArguments { get; }

            public Cachekey(MethodBase method, object[] arguments)
            {
                Method = method;
                InputArguments = arguments;
            }

            public override bool Equals(object obj)
            {
                Cachekey another = obj as Cachekey;
                if (null == another)
                {
                    return false;
                }
                if (!Method.Equals(another.Method))
                {
                    return false;
                }
                for (int index = 0; index < InputArguments.Length; index++)
                {
                    var argument1 = InputArguments[index];
                    var argument2 = another.InputArguments[index];
                    if (argument1 == null && argument2 == null)
                    {
                        continue;
                    }

                    if (argument1 == null || argument2 == null)
                    {
                        return false;
                    }

                    if (!argument2.Equals(argument2))
                    {
                        return false;
                    }
                }
                return true;
            }

            public override int GetHashCode()
            {
                int hashCode = Method.GetHashCode();
                foreach (var argument in InputArguments)
                {
                    hashCode = hashCode ^ argument.GetHashCode();
                }
                return hashCode;
            }
        }
        /// <summary>
        /// 缓存的内容
        /// </summary>
        private class MemoryCacheEntity
        {
            public MemoryCacheEntity(object returnValue, object[] parameters)
            {
                ReturnValue = returnValue;
                Parameters = parameters;
            }
            /// <summary>
            /// 返回值
            /// </summary>
            public object ReturnValue { get; set; }
            /// <summary>
            /// 返回的参数
            /// </summary>
            public object[] Parameters { get; set; }
        }
    }
}
