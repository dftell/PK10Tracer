using System.Text.RegularExpressions;
using WolfInv.com.WXMessageLib;
using System.Linq;
namespace WolfInv.com.ShareLotteryLib
{
    public class ResponseAction_SubcribeShares : ResponseActionClass
    {
        public ResponseAction_SubcribeShares(WXMsgProcess wxprs, wxMessageClass msg) : base(wxprs, msg)
        {
            ActionName = "响应认购份额";
        }

        protected override string getMsg()
        {
            string ret = "确认";
            return ret;
        }

        public override bool Response(ref ShareLotteryPlanClass optPlan)
        {
            if (currPlan.sharePlanStatus != SharePlanStatus.Subscribing)
            {
                answerMsg(string.Format("合买处于非{0}状态，认购份数无效！",ShareLotteryPlanClass.StatusDic[SharePlanStatus.Subscribing]));
                return false;
            }
            Regex regTr = new Regex(@"(\d+)");
            MatchCollection mcs = regTr.Matches(pureMsg);
            int shares = 0;
            if (mcs.Count > 0)
            {
                int.TryParse(mcs[0].Value, out shares);
            }
            var version = Regex.Replace(pureMsg, @"(.*\[)(.*)(\].*)", "$2"); //小括号()
            Regex rgx = new Regex(@"(?i)(?<=\[)(.*)(?=\])");//中括号[]
            string tmp = rgx.Match(pureMsg).Value;//中括号[]
            if (shares == 0||mcs.Count == 0)
            {
                string ret = "你耍我，提供真正的份数！";
                

                answerMsg(ret);
                return false;
            }
            TheAskWaitingUserAnswer ask = new TheAskWaitingUserAnswer(this);
            this.Buffs.Add(shares);
            ask.askData = MutliLevelData.createAValidateSubmitData("1", "确定", "0", "取消");
            ask.askMsg = string.Format(@"您的认购分数为:{0}份，合计金额:{1}元;
请确定您提供的信息无误？
{2}",
shares,
shares*currPlan.shareAmount,
ask.AskText);
            wxprocess.InjectAsk(ask);
            answerMsg(ask.askMsg);
            return false;
    
        }

        public override bool ResponseAsk(TheAskWaitingUserAnswer ask)
        {
            if (ask.AnswerResult.key == "0")//结束流程
            {

                answerMsg("欢迎下次使用！");
                return false;
            }
            else
            {
                
                PlanShareBetInfo pbi = new PlanShareBetInfo(ask.LastRequestWaitResponse.currPlan.shareAmount.Value);
                pbi.betWxName = requestUser;
                pbi.betNikeName = requestNike;
                pbi.subscribeShares = int.Parse(Buffs[0].ToString());
                pbi.needPayAmount = pbi.subscribeShares * ask.LastRequestWaitResponse.currPlan.shareAmount.Value;

                currPlan.subscribeList.Add(pbi);
                answerMsg(pbi.toSubscribeString());
                ResponseAction_ShowPlan sp = new ResponseAction_ShowPlan(wxprocess, wxmsg);
                sp.currPlan = currPlan;

                sp.answerMsg();
                int realcnt = sp.currPlan.subscribeList.Sum(a => a.subscribeShares);
                if (realcnt >=sp.currPlan.planShares)
                {
                    ResponseAction_ModifyStatus msp = new ResponseAction_ModifyStatus(wxprocess, wxmsg,SharePlanStatus.Paying,realcnt.ToString());
                    msp.requestUser = currPlan.creator;
                    msp.requestNike = currPlan.creatorNike;
                    string m = "计划募集份数已经达到，请确定是否继续超额募集！选否将停止接受认购！";
                    TheAskWaitingUserAnswer newask = new TheAskWaitingUserAnswer(msp,roomId,currPlan.creator,currPlan.creatorNike);
                    newask.askData = MutliLevelData.createAValidateSubmitData("1", "确定", "0", "取消");
                    newask.askMsg = string.Format(@"计划募集份数已经达到，请确定是否继续超额募集！选否将停止接受认购！
{0}",newask.AskText);
                    wxprocess.InjectAsk(newask);
                    answerMsg(newask.askMsg,null,currPlan.creatorNike);

                }
                return true;
            }
        }
    }


}
