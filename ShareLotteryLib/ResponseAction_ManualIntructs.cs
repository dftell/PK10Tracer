using System.Collections.Generic;
using System.Text.RegularExpressions;
using WolfInv.com.WXMessageLib;
using System.Net;
using System.Net.Http;
using WolfInv.Com.JsLib;
using System;
using System.Threading.Tasks;
using System.Web;

namespace WolfInv.com.ShareLotteryLib
{
    /// <summary>
    /// 充值类型，目前不属于彩票流程
    /// </summary>
    public class ResponseAction_ManualIntructs : ResponseActionClass
    {
        //public ResponseAction_Charge()
        //{

        //}
        //DbClass db;

        public ResponseAction_ManualIntructs(WXMsgProcess wxprs, wxMessageClass msg) : base(wxprs, msg)
        {
            ActionName = "手动下注";
            //db = new DbClass(SystemSetting.DbHost, SystemSetting.DbUser, SystemSetting.DbPwd, SystemSetting.DbName);
        }

        protected override string getMsg()
        {
            string ret = "请确认手动指令，决定是否继续！";
            return ret;
        }

        public override bool Response(ref ShareLotteryPlanClass optPlan)
        {//(.*?)手动下注(|:)((\d+|.*)/(.*)\d+((/\d+)?))((\+|\s)((\d+|.*)/\d+((/\d+)?)))*
            Regex regTr = new Regex(@"(.*?)手动下注(|:)((\d+|.*)/(.*)\d+((/\d+)?))((\+|\s)((\d+|.*)/\d+((/\d+)?)))*");
            MatchCollection mcs = regTr.Matches(pureMsg);
            string lotteryName = null;
            string content = null;
            if (mcs.Count == 1)
            {
                lotteryName = mcs[0].Groups[1].Value;
                content = mcs[0].Groups[3].Value;
            }
            else
            {
                answerMsg("抱歉，本汪无法理解您提交的内容！");
                return false;
            }
            TheAskWaitingUserAnswer ask = new TheAskWaitingUserAnswer(this);

            this.Buffs.Clear();
            this.Buffs.Add(lotteryName);
            this.Buffs.Add(content);
            ask.askData = new MutliLevelData();
            ask.askData.AddSub("1", "确定", null);
            MutliLevelData noselect = ask.askData.AddSub("0", "否", new MutliLevelData());
            noselect.AddSub("0", "停止手动增加指令", null);
            noselect.AddSub("1", "重新提交其他指令", null);
            ask.askMsg = string.Format(@"确定为彩种{0}手动增加指令{1}?
{2}", lotteryName,content , ask.AskText);
            wxprocess.InjectAsk(ask);
            answerMsg(ask.askMsg);
            return false;


            optPlan = null;
            return false;
            //return base.Response();
        }
       

        public override bool ResponseAsk(TheAskWaitingUserAnswer ask)
        {
            //wxprocess.CopyToHistoryAsks(ask);
            // wxprocess.CloseCurrAsk(ask);
            if (ask.AnswerResult.key == "0")//结束流程
            {

                answerMsg("欢迎下次使用！");
                return false;
            }
            else
            {
                ShareLotteryPlanClass plan = null;
                if (ask.UserResponseAnswer.Count == 1) //确定上次的金额
                {
                    if (Buffs.Count != 2)
                    {
                        answerMsg("上次存储的指令信息丢失！请重新申请");
                        return false;
                    }
                    string signatrue="";
                
                    //if (signatrue.ToString().ToUpper().Equals(sha1result))
                    //{

                    //}
                    try
                    {
                        
                        
                            Task.Run(() => {
                                submitData(Buffs[0] as string, Buffs[1] as string);
                            });
                       

                    }
                    catch (Exception ce)
                    {
                        answerMsg(string.Format("{0}:{1}", ce.Message, ce.StackTrace));
                        //throw ce;
                        return false;
                    }

                }
                else
                {
                    answerMsg("请重新提交金额！");
                    return false;
                }

                return false;
            }
            return false;
        }

        void submitData(string lname,string content)
        {
            string urlM = "http://www.wolfinv.com/pk10/app/submitInstructs.asp?";
            string req = "lottery={0}&reqInsts={1}";
            string urlReq = string.Format(req,
                lname,
                HttpUtility.UrlEncode(content)
                );
            string url = string.Format("{0}{1}", urlM, urlReq);
            string ret = new WebClient().DownloadString(url);
            answerMsg(ret);
            
        }
    }

   
}
