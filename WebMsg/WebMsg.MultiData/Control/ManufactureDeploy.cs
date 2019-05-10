using System;
using System.Collections.Generic;
using System.Text;

namespace WebMsg.MultiData.Control
{
    /// <summary>
    /// 多数据库配置的链接字符串
    /// </summary>
    public class ManufactureDeploy
    {
        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
        public int ServiceType { get; set; }
        /// <summary>
        /// 数据库服务器类型（只读，读写）
        /// </summary>
        internal ServiceType ServiceTypeEnum
        {
            get
            {
                return (ServiceType)ServiceType;
            }
        }
        /// <summary>
        /// 链接字符串
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
