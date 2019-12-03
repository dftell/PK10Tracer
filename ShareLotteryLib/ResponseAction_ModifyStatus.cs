using WolfInv.com.WXMessageLib;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace WolfInv.com.ShareLotteryLib
{
    public class ResponseAction_ModifyStatus : ResponseActionClass
    {
        protected SharePlanStatus changeToStatus;
        protected string updateVals = null;
        SharePlanStatus currstatus ;
        string newStatusName
        {
            get
            {
                return ShareLotteryPlanClass.StatusDic[changeToStatus]; 
            }
        }
        public ResponseAction_ModifyStatus(WXMsgProcess wxprs, wxMessageClass msg) : base(wxprs, msg)
        {
            ActionName = "响应取消合买信息";
        }

        public ResponseAction_ModifyStatus(WXMsgProcess wxprs, wxMessageClass msg,SharePlanStatus newStatus,string vals = null) : base(wxprs, msg)
        {
            ActionName = "响应取消合买信息";
            changeToStatus = newStatus;
            updateVals = vals;
        }

        protected override string getMsg()
        {
            return base.getMsg();
        }

        public override bool Response(ref ShareLotteryPlanClass optPlan)
        {

            optPlan = null;

            if (currPlan != null)
                optPlan = currPlan;
            if (currPlan == null)
            {
                answerMsg(optPlan == null ? "本群目前没有合买" : null);
                return false;
            }
            if(currPlan.creator != requestUser)
            {
                answerMsg(string.Format("本次合买由{0}创建，您没有修改权限！",currPlan.creatorNike));
                return false;
            }
            currstatus = currPlan.sharePlanStatus;
            if(changeToStatus == currstatus)
            {
                answerMsg(string.Format("状态{0}！", newStatusName));
                return false;
            }
            if(((int)changeToStatus - (int)currstatus)>1 && changeToStatus!= SharePlanStatus.Completed)
            {
                answerMsg(string.Format("除非关闭合买，否则合买状态不能越级操作！"));
                return false;
            }
            if (updateVals!=null)
            {
                Buffs.Add(updateVals);
            }
            TheAskWaitingUserAnswer ask = new TheAskWaitingUserAnswer(this);
            ask.askData = MutliLevelData.createAValidateSubmitData("1", "确定", "0", "取消");
            ask.askMsg = string.Format(@"请确定将合买状态修改为{0}！
{1}",
newStatusName,
ask.AskText);
            wxprocess.InjectAsk(ask);
            answerMsg(ask.askMsg);
            return false;
        }

        public override bool ResponseAsk(TheAskWaitingUserAnswer ask)
        {
            string currmsg = ask.LastRequestWaitResponse.pureMsg;
            if (ask.AnswerResult.key == "0")
            {
                answerMsg("取消当前操作！",null,ask.userNike);
                return false;
            }
            else
            {

                if (Buffs.Count > 0)
                {
                    updateVals = Buffs[0].ToString();
                }
                Regex regTr = null;
                switch (changeToStatus)
                {
                    case SharePlanStatus.Paying:
                        {
                            currPlan.subscribeShares = currPlan.subscribeList.Sum(a => a.subscribeShares);
                            break;
                        }
                    case SharePlanStatus.Opened://开奖
                        {

                            regTr = new Regex(string.Format(@"\d+", currmsg));
                            MatchCollection mcs = regTr.Matches(currmsg);
                            if (mcs.Count > 1)
                            {
                                List<string> listres = new List<string>();
                                for (int i = 0; i < mcs.Count; i++)
                                {
                                    listres.Add(mcs[i].Value);
                                }
                                int profit = int.Parse(listres.Last());
                                string opencode = string.Join(",", listres.Take(listres.Count - 1));
                                if (profit > 0)
                                    currPlan.matched = true;
                                currPlan.openInfo = opencode;
                                currPlan.profitAmout = profit;
                                decimal rs = (decimal)(profit * 1.0 / (currPlan.subscribeShares * 1.0));
                                currPlan.shareProfit = System.Math.Floor(10*rs)/10;
                            }
                            break;
                        }
                }
                    

                
                currPlan.sharePlanStatus = changeToStatus;
                string AddTxt = null;
                string m = @"已将合买状态修改为{0}！
{1}";
                if (changeToStatus == SharePlanStatus.Completed && currstatus < SharePlanStatus.Opened)
                {
                    AddTxt = "所有已认购份额全部无效，如果您已付款，请找管理员联系！";
                }
                if(changeToStatus == SharePlanStatus.Opened )
                {
                    string mm = @"本次合买总共中奖{0}元，共计份{1},每份返奖{2:N1}元;
{3}";
                    string ml = "";
                    if (currPlan.matched)
                        ml = currPlan.ToGroupedProfitInfo();
                    AddTxt = string.Format(mm, currPlan.profitAmout, currPlan.subscribeShares.Value, currPlan.shareProfit, ml);
                }
                answerMsg(string.Format(m, newStatusName, AddTxt));
                return true;
            }
        }
    }


}
