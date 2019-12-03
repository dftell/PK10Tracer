using WolfInv.com.WXMessageLib;

namespace WolfInv.com.ShareLotteryLib
{
    public class ResponseAction_CancelCurr : ResponseAction_ModifyStatus
    {
        public ResponseAction_CancelCurr(WXMsgProcess wxprs, wxMessageClass msg) : base(wxprs, msg)
        {
            ActionName = "响应取消合买信息";
            changeToStatus = SharePlanStatus.Completed;
        }

        

     
    }



    public class ResponseAction_ClosePlan : ResponseAction_CancelCurr
    {
        public ResponseAction_ClosePlan(WXMsgProcess wxprs, wxMessageClass msg) : base(wxprs, msg)
        {
            ActionName = "响应结束合买信息";
        }
    }


}
