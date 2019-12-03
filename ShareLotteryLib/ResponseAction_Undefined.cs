using WolfInv.com.WXMessageLib;

namespace WolfInv.com.ShareLotteryLib
{
    public class ResponseAction_Undefined : ResponseActionClass
    {
        public ResponseAction_Undefined(WXMsgProcess wxprs, wxMessageClass msg) : base(wxprs, msg)
        {

        }

        public override bool Response(ref ShareLotteryPlanClass optPlan)
        {
            optPlan = null;
            return base.Response(ref optPlan);
        }
    }


}
