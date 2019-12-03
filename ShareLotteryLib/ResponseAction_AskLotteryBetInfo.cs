using System.Linq;
using System;
using WolfInv.com.WXMessageLib;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WolfInv.com.ShareLotteryLib
{
    public class ResponseAction_AskLotteryBetInfo : ResponseActionClass
    {
        public ResponseAction_AskLotteryBetInfo(WXMsgProcess wxprs, wxMessageClass msg):base(wxprs,msg)
        {
            ActionName = "响应买啥";
        }

        protected override string getMsg()
        {
            string ret = "请";
            return ret;
        }

        KeyText getLottery()
        {
            KeyText ret = null;
            
            if (ret != null)//如果既往计划中找不到，到所有彩票清单中找，如果找到就返回
                return ret;
            ShareLotteryPlanClass.AllLotteryKeyNames.Keys.ToList().ForEach(
                a =>
                {
                    string txt = ShareLotteryPlanClass.AllLotteryKeyNames[a];
                    if (Msg.Contains(txt))
                    {
                        ret = new KeyText(a, txt);
                        return;
                    }
                    if(txt == "北京赛车" && Msg.ToLower().Contains("pk10"))
                    {
                        ret = new KeyText(a, txt);
                        return;
                    }
                }
                );
            return ret;
        }


        public override bool Response(ref ShareLotteryPlanClass optPlan)
        {

            KeyText hasLottery = getLottery();
            if (hasLottery == null)//未找到彩种，先确认彩种
            {

                TheAskWaitingUserAnswer ask = new TheAskWaitingUserAnswer(this);
                this.lastAsk = ask;
                ask.LastRequestWaitResponse = this;
                string sendmsg = @"请根据我提供的问题选择您需要推荐的彩种：" +
                    "{0}";
                
                ask.askData = ShareLotteryPlanClass.AllLottery;
                
                
                string fullmsg = string.Format(sendmsg, ask.AskText);
                ask.askMsg = fullmsg;
                wxprocess.InjectAsk(ask);
                this.answerMsg(fullmsg);
                return true;
            }
            else//如果提供了彩票品种，直接反馈信息
            {
                
               
            }
            
            return true;
            //return base.Response();
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
