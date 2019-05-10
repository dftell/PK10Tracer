using WebMsg.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebMsg.MultiData.Control
{
    /// <summary>
    /// 
    /// </summary>
    public static class ManufactureContextExpand
    {
        /// <summary>
        /// 获取数据库上下文
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="handleType"></param>
        /// <returns></returns>
        public static CommonContext Manufacture<T>(this IBaseRepository<T> source, HandleType handleType) where T : class
        {
            return ManufactureContext.Instance.Manufacture(handleType, typeof(T));
        }
    }
}
