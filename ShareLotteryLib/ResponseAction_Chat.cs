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
    /// 私聊类型，目前不属于彩票流程
    /// </summary>
    public class ResponseAction_Chat:ResponseActionClass
    {
        //public ResponseAction_Charge()
        //{

        //}
        //DbClass db;

        public ResponseAction_Chat(WXMsgProcess wxprs, wxMessageClass msg) : base(wxprs, msg)
        {
            ActionName = "私聊";
            //db = new DbClass(SystemSetting.DbHost, SystemSetting.DbUser, SystemSetting.DbPwd, SystemSetting.DbName);
        }

        protected override string getMsg()
        {
            string ret = string.Format("你好，我是机器人{0}，请问有什么能帮到您？",wxprocess.RobotNikeName);
            return ret;
        }

        public override string pureMsg
        {
            get
            {
                if (!string.IsNullOrEmpty(wxmsg.Msg))
                {
                    string msg = wxmsg.Msg.Replace(string.Format("{0}", wxmsg.ToNikeName), "")?.Trim()?.Replace(" ", "");
                    return msg;
                }
                return wxmsg.Msg;
            }
        }

        public override bool Response(ref ShareLotteryPlanClass optPlan)
        {
            answerMsg(null,requestUser,requestNike,false);
            return false;
        }

        public override bool ResponseAsk(TheAskWaitingUserAnswer ask)
        {
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

   

 }
