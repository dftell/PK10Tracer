using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMsg.Enum.Overall
{
    /// <summary>
    /// 模板消息
    /// </summary>
    public enum MessageTemplate
    {
        /// <summary>
        /// 提交问卷使用【表单反馈提醒】TM00406
        /// </summary>
        新的评价通知 = 1000,
        /// <summary>
        /// 追评使用【新的评价通知】OPENTM207498036
        /// </summary>
        追评通知 = 1001,
        /// <summary>
        /// 问卷提交成功给用户的通知【表单已提交通知】OPENTM417523869
        /// </summary>
        表单提交成功通知 = 1002,
        /// <summary>
        /// 订单提交成功给负责人的通知【新订单通知】OPENTM203353600
        /// </summary>
        商城新订单通知 = 2001,
        /// <summary>
        /// 订单进度通知【订单进度提醒】OPENTM402104418
        /// </summary>
        商城订单进度通知 = 2002
    }
    /// <summary>
    /// 异常错误代码
    /// </summary>
    public enum ErrCode
    {
        没有权限 = 403,
        系统异常 = 404,
        操作成功 = 200,
        操作失败 = 201
    }
    /// <summary>
    /// 是否删除
    /// </summary>
    public enum IsRemove
    {
        否 = 0,
        是 = 1
    }

}
