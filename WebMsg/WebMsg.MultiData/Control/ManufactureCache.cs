using System;
using System.Collections.Generic;
using System.Text;

namespace WebMsg.MultiData.Control
{
    public class ManufactureCache
    {
        /// <summary>
        /// 操作的类
        /// </summary>
        public Type HandleClass { get; set; }
        /// <summary>
        /// 数据库上下文
        /// </summary>
        internal CommonContext Context { get; set; }
        /// <summary>
        /// 执行写的时间
        /// </summary>
        public DateTime? UseWriteTime { get; set; }
    }
}
