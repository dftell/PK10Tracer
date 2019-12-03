using System.Collections.Generic;
using System.Text.RegularExpressions;
using WolfInv.com.WXMessageLib;
using System.Linq;
namespace WolfInv.com.ShareLotteryLib
{
    public class ResponseAction_SubmitNewInfo : ResponseActionClass
    {
        
        public ResponseAction_SubmitNewInfo(WXMsgProcess wxprs, wxMessageClass msg) : base(wxprs, msg)
        {
            ActionName = "响应提供详细合买信息";
        }

        

        public override bool Response(ref ShareLotteryPlanClass optPlan)
        {
            optPlan = currPlan;
            if (currPlan == null)
            {
                answerMsg(optPlan == null ? "本群目前没有合买" : null);
                return false;
            }
            if (currPlan.creator != requestUser)
            {
                answerMsg(string.Format("本次合买由{0}创建，您没有修改权限！", currPlan.creatorNike));
                return false;
            }
            if (currPlan.sharePlanStatus != SharePlanStatus.Ready)
            {
                answerMsg("合买处于非初始状态，提供合买信息无效！");
                return false;
            }
            List<string> errmsgs = null;
            Dictionary<string, string> sitems = null;
            bool succ = CheckInfo(ref sitems, ref errmsgs);
            string info = string.Join("\r\n" ,sitems.Select(a => string.Format("{0}=>{1}", a.Key,a.Value)));
            if(succ)
            {
                TheAskWaitingUserAnswer ask = new TheAskWaitingUserAnswer(this);
                this.Buffs.Add(sitems);
                ask.askData = MutliLevelData.createAValidateSubmitData("1","确定","0","取消");
                ask.askMsg = string.Format(@"{0}
请确定您提供的信息无误？如果确定，我们将开始进入认购环节！
{1}", 
info, 
ask.AskText);
                wxprocess.InjectAsk(ask);
                answerMsg(ask.askMsg);
                return false;
            }
            else
            {
                string m = string.Format(@"下列信息有误,请修改后重新提交正确的信息:
{0}", string.Join(";", errmsgs));
                answerMsg(m);
            }
            return true;
        }

        bool CheckInfo(ref Dictionary<string,string> items , ref List<string> retmsgs)
        {
            List < string> msgs = new List<string>();
            Dictionary<string,string> dic = new Dictionary<string,string>();
            Regex regTr = new Regex(@"\[[\s\S]*?]");
            MatchCollection mcs = regTr.Matches(pureMsg);
            bool hasErr = false;
            if (mcs.Count > 0)
            {
                string infos = mcs[0].Value.Trim();
                infos = infos.Substring(1, infos.Length - 2);
                infos = infos.Replace("\r\n", "").Replace("\r","");
                List<string> subItems =  infos.Split(';').ToList();
                subItems.ForEach(
                    a=> {
                        string[] siarr = a.Trim().Split(':');
                        if(siarr.Length<2)
                        {
                            hasErr = true;
                            msgs.Add(string.Format("{0}未填写数据！",siarr[0].Trim()));
                            return;
                        }
                        string key = siarr[0];
                        string val = a.Substring(key.Length + ":".Length);
                        if(val.Trim().Length ==0)
                        {
                            hasErr = true;
                            msgs.Add(string.Format("{0}数据长度为0！", siarr[0].Trim()));
                            return;
                        }
                        if(dic.ContainsKey(key))
                        {
                            hasErr = true;
                            msgs.Add(string.Format("{0}数据填写重复！", siarr[0].Trim()));
                            return;
                        }
                        dic.Add(key, val);
                    });
            }
            retmsgs = msgs;
            items = dic;
            return !hasErr;
            //return false;
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
                Dictionary<string, string> sitems = Buffs[0] as Dictionary<string,string>;
                /*
                 合买编号:{0};
                合买彩种:{1};
                投注期号:{2};
                投注内容:{3};
                单份金额:{4};
                认购份数:{5}]";*/
                currPlan.betExpectNo = sitems["投注期号"];
                currPlan.betInfo = sitems["投注内容"];
                currPlan.shareAmount = int.Parse(sitems["单份金额"]);
                currPlan.planShares = int.Parse(sitems["认购份数"]);
                currPlan.sharePlanStatus = SharePlanStatus.Subscribing;//可以认购
                answerMsg("合买信息已经完善，请大家踊跃认购！");
                answerMsg(currPlan.ToBasePlanInfo());
                return true;
            }
        }
    }


}
