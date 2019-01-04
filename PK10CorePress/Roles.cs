using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PK10CorePress
{
    /// <summary>
    /// 用户权限
    /// </summary>
    public class Role
    {
        /// <summary>
        /// 是否允许控制服务器
        /// </summary>
        public bool AllowControlServer;
        /// <summary>
        /// 是否允许交易
        /// </summary>
        public bool AllowExchange;
        /// <summary>
        /// 是否允许显示所有概要
        /// </summary>
        public bool AllowDisplayAllSummay;
        /// <summary>
        /// 是否允许管理用户
        /// </summary>
        public bool AllowAdminUser;
        /// <summary>
        /// 是否允许回测
        /// </summary>
        public bool AllowUseBackTest;
        /// <summary>
        /// 是否允许自定义组合
        /// </summary>
        public bool AllowSelfDefineInsts;
        public Role()
        {
        }
    }
}
