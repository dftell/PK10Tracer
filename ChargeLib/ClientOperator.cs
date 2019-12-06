using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolfInv.com.ChargeLib
{
    /// <summary>
    /// 客户端操作类，包括设置
    /// </summary>
    public class ClientOperator
    {
        public OperateType operateType { get; set; }
        public string[] operateParams { get; set; }
        public Func<OperateType, string[], OperateResult> ExecFunc;
        public OperateResult Exec()
        {
            OperateResult ret = ExecFunc.Invoke(operateType,operateParams);
            return ret;
        }
    }

    public struct OperateResult
    {
        public bool Succ;
        public string Msg;
        public string Result;
    }

    public enum OperateType
    {
        /// <summary>
        /// 账户指向
        /// </summary>
        AccountDefine,
        /// <summary>
        /// 登录
        /// </summary>
        Login,
        /// <summary>
        /// 切换通道
        /// </summary>
        SwitchChanle,
        /// <summary>
        /// 唤醒客户端
        /// </summary>
        WakeClient,
        /// <summary>
        /// 发送指令
        /// </summary>
        SendInsts,
        /// <summary>
        /// 发送当前指令
        /// </summary>
        SendCurrInsts

    }
}
