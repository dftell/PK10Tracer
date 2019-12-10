using System.Collections.Generic;
using System.Text.RegularExpressions;
using WolfInv.com.WXMessageLib;
using System.Net;
using System.Net.Http;
using WolfInv.Com.JsLib;
using System;
using System.Threading.Tasks;
using System.Web;
using WolfInv.com.JdUnionLib;
namespace WolfInv.com.ShareLotteryLib
{
    /// <summary>
    /// 京东联盟
    /// </summary>
    public class ResponseAction_JdUnion : ResponseActionClass
    {
        //public ResponseAction_Charge()
        //{

        //}
        //DbClass db;

        public ResponseAction_JdUnion(WXMsgProcess wxprs, wxMessageClass msg) : base(wxprs, msg)
        {
            ActionName = "京东联盟";
            //db = new DbClass(SystemSetting.DbHost, SystemSetting.DbUser, SystemSetting.DbPwd, SystemSetting.DbName);
        }

        protected override string getMsg()
        {
            string ret = "请确认手动指令，决定是否继续！";
            return ret;
        }

        public override bool Response(ref ShareLotteryPlanClass optPlan)
        {//(.*?)手动下注(|:)((\d+|.*)/(.*)\d+((/\d+)?))((\+|\s)((\d+|.*)/\d+((/\d+)?)))*
            Regex regTr = new Regex(@"有(.*?)[的券|券|吗]");
            MatchCollection mcs = regTr.Matches(pureMsg);
            string lotteryName = null;
            string content = null;
            if (mcs.Count == 1)
            {
                lotteryName = mcs[0].Groups[1].Value;
                content = mcs[0].Groups[3].Value;
                lotteryName = lotteryName.Replace("没有", "").Replace("的优惠", "").Replace("优惠", "").Replace("的券", "").Replace("券","");
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
            noselect.AddSub("0", "停止查询商品", null);
            noselect.AddSub("1", "重新提交其他条件", null);
            ask.askMsg = string.Format(@"确定查找{0}的券?", lotteryName, ask.AskText);
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
                    answerMsg("请重新提交条件！");
                    return false;
                }

                return false;
            }
            return false;
        }

        void submitData(string lname,string content)
        {
            //jd_union_goods_jingfen_query_response 
            
        }
    }

   
}
