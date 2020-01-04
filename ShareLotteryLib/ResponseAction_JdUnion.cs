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
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;

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
            string ret = "很抱歉，无法找到您要查找的券！请尝试修改下其他条件查找！";
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
            ask.askMsg = string.Format(@"确定查找{0}的券?{1}", lotteryName, ask.AskText);
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

        public static string getShortLink(string url)
        {
            string createUrl = "https://dwz.cn/admin/v2/create";
            String SIGN = "7d2772ad2792943aa67ae1213f9288d9";
            string content_type = "application/json";
            string strUrl = "{\"Url\":\"{0}\"}";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(createUrl);
            string ret = null;
            try
            {
                req.Method = "Post";
                //req.Headers.Add("Content-Type", content_type);
                req.ContentType = content_type;
                req.Headers.Add("Token", SIGN);


                string Data = strUrl.Replace("{0}", url);
                byte[] byteArray = Encoding.UTF8.GetBytes(Data);
                Stream newStream = req.GetRequestStream();//创建一个Stream,赋值是写入HttpWebRequest对象提供的一个stream里面
                newStream.Write(byteArray, 0, byteArray.Length);
                newStream.Close();
                using (WebResponse wr = req.GetResponse())
                {
                    wr.GetResponseStream();
                    ret = new StreamReader(wr.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                    wr.Close();
                }
                if (!string.IsNullOrEmpty(ret))
                {
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    shortLinkReturnResult returnResult = javaScriptSerializer.Deserialize<shortLinkReturnResult>(ret);
                    if (returnResult.Code != 0)
                    {
                        return null;
                    }
                    return returnResult.ShortUrl;
                }
                return null;
            }
            catch (Exception ce)
            {

            }
            return "";
        }
        class shortLinkReturnResult
        {
            public int Code;
            public string ShortUrl;
            public string LongUrl;
            public string ErrMsg;
        }

        void submitData(string lname,string content)
        {
            //jd_union_goods_jingfen_query_response 
            ////if(JdGoodsQueryClass.Inited == false)//一定要检查是否完全初始化，才能查询
            ////{
            ////    answerMsg("很抱歉，尚未完成初始化！请稍候再提交请求！");
            ////    return;
            ////}
            Dictionary<string, JdGoodSummayInfoItemClass> ret = JdGoodsQueryClass.QueryWeb(lname,5);
            //string strRet = string.Join("\r\n", ret.Select(a => a.Value.getFullContent()));
            if(ret == null || ret.Count==0)
            {
                answerMsg("很抱歉，无法找到您要查找的券！请尝试修改下其他条件查找！");
                return;
            }
            foreach(JdGoodSummayInfoItemClass ji in ret.Values)
            {
                ji.shortLinkFunc = getShortLink;
                ji.commissionUrl = ji.getShortLink();
                answerMsg(ji.imgageUrl, null, null, true, true);
                answerMsg(ji.getFullContent(ji.discount!="0")+ji.commissionUrl);
            }
            answerMsg(string.Format(@"{0}

更多优惠请{2}或移步到武府乐购网站{1}获取！","", JdGoodsQueryClass.NavigateUrl,JdGoodsQueryClass.MyPublic));
        }
    }

   
}
