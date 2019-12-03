using WolfInv.com.WXMessageLib;
using System.Linq;
namespace WolfInv.com.ShareLotteryLib
{
    public class ResponseAction_ShowPlan : ResponseActionClass
    {
        public ResponseAction_ShowPlan(WXMsgProcess wxprs, wxMessageClass msg) : base(wxprs, msg)
        {
            ActionName = "响应显示合买信息";
        }

        protected override string getMsg()
        {
            string m = 
@"当前合买信息如下:
{0}
目前认购信,共计:{1}人{2}份
{3}
";
            string ret = string.Format(m, 
                currPlan.ToBasePlanInfo(),
                currPlan.subscribeList.GroupBy(a=>a.betWxName).Count(),
                currPlan.subscribeList.Sum(a=>a.subscribeShares), 
                currPlan.ToGroupedSubscribeInfo());
            return ret;
        }

        public override bool Response(ref ShareLotteryPlanClass optPlan)
        {
            
            optPlan = null;
            if (currPlan != null)
                optPlan = currPlan;
            answerMsg(optPlan==null?"本群目前没有合买":null);
            return true;
        }
    }


}
