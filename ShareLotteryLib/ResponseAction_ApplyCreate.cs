using System.Linq;
using System;
using WolfInv.com.WXMessageLib;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WolfInv.com.ShareLotteryLib
{
    public class ResponseAction_ApplyCreate: ResponseActionClass
    {
        public ResponseAction_ApplyCreate(WXMsgProcess wxprs, wxMessageClass msg):base(wxprs,msg)
        {
            ActionName = "响应新建合买";
        }

        protected override string getMsg()
        {
            string ret = "请";
            return ret;
        }

        public override bool Response(ref ShareLotteryPlanClass optPlan)
        {
            if (wxprocess.AllPlan != null)
            {
                var runningPlans = wxprocess.AllPlan.Where(a => {
                    if(
                    a.Value.sharePlanStatus != SharePlanStatus.Completed && a.Value.wxRootNo == roomId)
                    {
                        return true;
                    }
                    return false;
                    });
                if (runningPlans.Count() > 0)
                {
                    answerMsg("非法请求！一个群只能有一个合买在运行，请结束上一个合买再新建新的合买！");
                    return false;
                }
            }
            KeyText hasLottery = getLottery();
            ShareLotteryPlanClass slp = new ShareLotteryPlanClass();
            if (hasLottery == null)//未找到彩种，先确认彩种
            {

                TheAskWaitingUserAnswer ask = new TheAskWaitingUserAnswer(this);
                this.lastAsk = ask;
                ask.LastRequestWaitResponse = this;
                string sendmsg = @"您是我们的新用户，请根据我提供的问题选择您需要合买的彩种：" +
                    "{0}";
                
                ask.askData = ShareLotteryPlanClass.AllLottery;
                
                
                string fullmsg = string.Format(sendmsg, ask.AskText);
                ask.askMsg = fullmsg;
                wxprocess.InjectAsk(ask);
                this.answerMsg(fullmsg);
                return true;
            }
            else
            {
                Regex regTr = new Regex(string.Format("{0}(.*?)期", hasLottery.text));
                MatchCollection mcs = regTr.Matches(pureMsg);
                if(mcs.Count>0)
                {
                    slp.betExpectNo = mcs[0].Value.Trim().Replace("第","").Replace(hasLottery.text,"").Replace("期","");
                }
                slp.betLottery = hasLottery.key;
                slp.wxRootNo = roomId;
                slp.wxRootName = roomName;
                slp.createTime = DateTime.Now;
                slp.creator = requestUser;
                slp.creatorNike = requestNike;
                slp.sharePlanStatus = SharePlanStatus.Ready;
                this.answerMsg(slp.ToUserCreateModel());
                wxprocess.AllPlan.Add(slp.guid, slp);
                optPlan = slp;
            }

            
            return true;
            //return base.Response();
        }

        KeyText getLottery()
        {
            KeyText ret = null;
            if (this.wxprocess.AllPlan!=null)
            {
                Dictionary<string, KeyText> hasCreatedLotteries = this.wxprocess.AllPlan.getUserCreatedLotteries(wxmsg.FromMemberUserName);
                hasCreatedLotteries.Values.ToList().ForEach(a =>
                {

                    if (Msg.Contains(a.text))
                    {
                        ret = a;
                        return;
                    }
                });
            }
            if (ret != null)//如果既往计划中找不到，到所有彩票清单中找，如果找到就返回
                return ret;
            ShareLotteryPlanClass.AllLotteryKeyNames.Keys.ToList().ForEach(
                a=>
                {
                    string txt = ShareLotteryPlanClass.AllLotteryKeyNames[a];
                    if(Msg.Contains(txt))
                    {
                        ret = new KeyText(a, txt);
                        return;
                    }
                }
                );
            return ret;
        }

        public override bool ResponseAsk(TheAskWaitingUserAnswer ask)
        {
            ShareLotteryPlanClass slp = new ShareLotteryPlanClass();
            if(!ShareLotteryPlanClass.AllLotteryNameKeys.ContainsKey(ask.AnswerResult.text))
            {
                answerMsg(string.Format("无法找到彩种{0}对应的编号,请联系管理员修改配置！",ask.AnswerResult.text));
                return false;
            }
            slp.betLottery = ShareLotteryPlanClass.AllLotteryNameKeys[ask.AnswerResult.text];
            slp.wxRootNo = roomId;
            slp.wxRootName = roomName;
            slp.createTime = DateTime.Now;
            slp.creator = requestUser;
            slp.creatorNike = requestNike;
            slp.sharePlanStatus = SharePlanStatus.Ready;
            this.answerMsg(slp.ToUserCreateModel());
            this.currPlan = slp;
            //wxprocess.AllPlan.Add(slp.guid, slp);
            return true;
        }

        public override bool ResumeResponse()
        {
            return false;
        }

        
    }


}
