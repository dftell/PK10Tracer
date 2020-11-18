using System.Collections.Generic;
using System.Text.RegularExpressions;
using WolfInv.com.WXMessageLib;
using System.Net;
using System.Net.Http;
using WolfInv.Com.JsLib;
using System;
using System.Threading.Tasks;

namespace WolfInv.com.ShareLotteryLib
{
    /// <summary>
    /// 充值类型，目前不属于彩票流程
    /// </summary>
    public class ResponseAction_Charge:ResponseActionClass
    {
        //public ResponseAction_Charge()
        //{

        //}
        //DbClass db;

        public ResponseAction_Charge(WXMsgProcess wxprs, wxMessageClass msg) : base(wxprs, msg)
        {
            ActionName = "充值";
            //db = new DbClass(SystemSetting.DbHost, SystemSetting.DbUser, SystemSetting.DbPwd, SystemSetting.DbName);
        }

        protected override string getMsg()
        {
            string ret = "请确认充值金额，决定是否继续！";
            return ret;
        }

        public override bool Response(ref ShareLotteryPlanClass optPlan)
        {
            Regex regTr = new Regex(@"\d+");
            MatchCollection mcs = regTr.Matches(pureMsg);
            int chargeAmt = 0;
            if (mcs.Count > 0)
            {
                int.TryParse(mcs[0].Value.Trim(), out chargeAmt);
            }
            TheAskWaitingUserAnswer ask = new TheAskWaitingUserAnswer(this);
            if (chargeAmt <=0 ||  (chargeAmt%100)==0 || chargeAmt > 4500 || chargeAmt <151)
            {
                
                
                string retMsg = "充值金额非法！请输入正确的充值金额数量,金额介于151~4500之间，不能为100的整数倍！";
                answerMsg(retMsg);
                return false;
            }
            else
            {
                this.Buffs.Clear();
                this.Buffs.Add(chargeAmt);
                ask.askData = new MutliLevelData();
                ask.askData.AddSub("1", "确定",null);
                MutliLevelData noselect = ask.askData.AddSub("0", "否", new MutliLevelData());
                noselect.AddSub("0", "停止充值",null);
                noselect.AddSub("1", "重新申请其他金额", null);
                ask.askMsg = string.Format(@"确定刷卡{0}元?
{1}",chargeAmt,ask.AskText);
                wxprocess.InjectAsk(ask);
                answerMsg(ask.askMsg);
                return false;
                
            }
            optPlan = null ;
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
                    if (Buffs.Count != 1)
                    {
                        answerMsg("上次存储的金额信息丢失！请重新申请");
                        return false;
                    }
                    string signatrue="";
                    //if (signatrue.ToString().ToUpper().Equals(sha1result))
                    //{

                    //}
                    try
                    {
                        answerMsg("生成和获取二维码需要一定时间，请您耐心等候！");
                        string url = string.Format("http://www.wolfinv.com/pk10/app/charge.asp?chargeAmt={0}&rnd={1}&wxId={2}&wxName={3}&provide={4}", ask.LastRequestWaitResponse.Buffs[0], new Random().Next(), wxmsg.FromMemberUserName, wxmsg.FromMemberNikeName,wxprocess.RobotNikeName);
                        WebClientTo wc = new WebClientTo(3 * 60 * 1000);
                        wc.Encoding = System.Text.Encoding.UTF8;
                        string res = wc.DownloadString(url);

                        JavaScriptClass jsc = new JavaScriptClass();
                        ChargeResult cr = new ChargeResult();

                        cr = cr.GetFromJson<ChargeResult>(res);

                        if (cr == null)
                        {
                            answerMsg("服务器异常！" + url);
                            return false;
                        }
                        if (!string.IsNullOrEmpty(cr.imgData) && cr.imgData.Trim() != "空")
                        {
                            
                            answerMsg(string.Format(@"订单号:{0};订单金额:{1}元 ;
请在三分钟内在云闪付内完成支付！", cr.orderNum, cr.chargeAmt));
                            answerMsg(cr.imgData, null, null, true);
                            //string insql = "insert into userchargetable(
                            //chargeid,wxid,wxname,chargeamt,ordernum,imgurl,ChargeAccount) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')";
                            /*if (db != null)
                            {
                                int cnt = db.ExecSql(new ConditionSql(string.Format(insql, 
                                    cr.reqId, 
                                    ask.LastRequestWaitResponse.roomName, 
                                    ask.LastRequestWaitResponse.requestNike,
                                    cr.chargeAmt,
                                    cr.orderNum, 
                                    cr.imgData, 
                                    cr.chargeAccount)));
                            }*/
                            if(!DisableMultiTaskProcess)
                                Task.Run(() => {
                                submitData(cr);
                            });
                            else
                                submitData(cr);
                        }
                        else
                        {
                            
                            answerMsg(cr.msg);
                            return false;
                        }

                        //answerMsg(cr.imgData);
                        return false;
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

        void submitData(ChargeResult cr)
        {
            /*
                         url: 'https://www.wolfinv.com/pk10/app/submitCharge.asp',
            data: {
              reqId: myThis.data.reqId,
              wxId: myThis.data.telNo,
              wxName: myThis.data.wxName,
              chargeAmt: myThis.data.chargeAmt,
              chargeAccount: myThis.data.chargeAccount,
              orderNum: myThis.data.orderNum,
              imgData: "已获取到base64",
              bankNo: myThis.data.bankNo,
              bankName:myThis.data.bankName,
              provider: 'littleFunction'    
                         */
            string urlM = "http://www.wolfinv.com/pk10/app/submitCharge.asp?reqId={0}&wxId={1}&wxName={2}&chargeAmt={3}&chargeAcount={4}&orderNum={5}&provider={6}&imgData={7}";
            if (!string.IsNullOrEmpty(actionDefine.submitUrl))
            {
                urlM = actionDefine.submitUrl;
            }
            string url = string.Format(urlM,
                cr.reqId,
                roomName,
                requestNike,
                cr.chargeAmt,
                cr.chargeAccount,
                cr.orderNum,
                "",
                cr.imgData
                );
            new WebClient().DownloadString(url);
        }
    }

    public class WebClientTo : WebClient
    {        /// <summary>        /// 过期时间        /// </summary>        
        public int Timeout { get; set; }
        public WebClientTo(int timeout)
        {
            Timeout = timeout;
        }
        /// <summary>        
        /// 重写GetWebRequest,添加WebRequest对象超时时间        ///
        /// </summary>        /// 
        /// <param name="address"></param>        /// 
        /// <returns></returns>        
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.Timeout = Timeout;
            request.ReadWriteTimeout = Timeout;
            return request;
        }
    }


    public class ChargeResult :JsonableClass<ChargeResult>
    {
        public string errcode { get; set; }
        public string msg { get; set; }
        public string chargeAmt { get; set; }
        public string reqId { get; set; }
        public string orderNum { get; set; }
        public string imgData { get; set; }
        public string chargeAccount { get; set; }
        public string respTime { get; set; }
    }
}
