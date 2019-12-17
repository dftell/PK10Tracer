using System;
using System.Collections.Generic;
using WolfInv.com.WXMessageLib;

namespace WolfInv.com.ShareLotteryLib
{
    /// <summary>
    /// 相应微信问题的行为基类
    /// </summary>
    public abstract class ResponseActionClass
    {
        protected WXMsgProcess wxprocess;
        protected wxMessageClass wxmsg;
        public string Msg;
        public string requestUser;
        public string requestNike;
        public string roomId;
        public string roomName;
        public ShareLotteryPlanClass currPlan;
        public string ActionName;
        public TheAskWaitingUserAnswer lastAsk;
        public List<object> Buffs = new List<object>();
        public ActionDefine actionDefine;
        /*Ready,//筹备中
        Subscribing,//正在认购中
        Paying,//认购完成，正在缴款中
        Paied,//缴款完成，等待开奖
        Opened,//开奖完成，返奖
        Completed //结束*/
        

        protected ResponseActionClass(WXMsgProcess wxprs, wxMessageClass msg)
        {
            wxprocess = wxprs;
            wxmsg = msg;
            Msg = pureMsg;
            requestUser = wxmsg.FromMemberUserName;
            requestNike = wxmsg.FromMemberNikeName;
            roomId = wxmsg.FromUserNam;
            roomName = wxmsg.FromNikeName;
        }

        public virtual string pureMsg
        {
            get
            {
                string msg = wxmsg.Msg.Replace(string.Format("@{0}", wxmsg.AtMemberNikeName[0]), "").Trim();
                return msg;
            }
        }
        
        
        public static ResponseActionClass CreateAction(ActionType act, WXMsgProcess wxprs, wxMessageClass msg)
        {
            ResponseActionClass ret = null;
            switch(act)
            {
                case ActionType.ResetSystem:
                    {
                        ShareLotteryPlanClass.Reset();
                        WXMsgProcess.ActionDic = null;
                        break;
                    }
                case ActionType.ApplyCreate:
                    {
                        ret = new ResponseAction_ApplyCreate(wxprs, msg);
                        break;
                    }
                case ActionType.SubmitNewInfo:
                    {
                        ret = new ResponseAction_SubmitNewInfo(wxprs, msg);
                        break;
                    }
                case ActionType.CancelCurr:
                    {
                        ret = new ResponseAction_CancelCurr(wxprs, msg);
                        break;
                    }
                case ActionType.SubcribeShares:
                    {
                        ret = new ResponseAction_SubcribeShares(wxprs, msg);
                        break;
                    }
                case ActionType.ModifyPlan:
                    {
                        ret = new ResponseAction_ModifyStatus(wxprs, msg, SharePlanStatus.Ready);
                        break;
                    }
                case ActionType.AppendShares:
                    {
                        ret = new ResponseAction_ModifyStatus(wxprs, msg, SharePlanStatus.Paied);
                        break;
                    }
                case ActionType.JdUnion:
                    {
                        ret = new ResponseAction_JdUnion(wxprs, msg);
                        break;
                    }
                case ActionType.ShowPlan:
                    {
                        ret = new ResponseAction_ShowPlan(wxprs, msg);
                        break;
                    }
                case ActionType.EndTheSubscribe:
                    {
                        ret = new ResponseAction_ModifyStatus(wxprs, msg,SharePlanStatus.Paying);
                        break;
                    }
                case ActionType.DeclareResult:
                    {
                        ret = new ResponseAction_ModifyStatus(wxprs, msg, SharePlanStatus.Opened);
                        break;
                    }
                case ActionType.DeclareProfit:
                    {
                        ret = new ResponseAction_ModifyStatus(wxprs, msg, SharePlanStatus.Opened);
                        break;
                    }
                case ActionType.ClosePlan:
                    {
                        ret = new ResponseAction_ClosePlan(wxprs, msg);
                        break;
                    }
                case ActionType.Charge:
                    {
                        ret = new ResponseAction_Charge(wxprs, msg);
                        break;
                    }
                case ActionType.ValidateInfo:
                    {
                        ret = new ResponseAction_ValidateInfo(wxprs, msg);
                        break;
                    }
                case ActionType.ManualInstructs:
                    {
                        ret = new ResponseAction_ManualIntructs(wxprs, msg);
                        break;
                    }
                case ActionType.Undefined:
                default:
                    {
                        ret = new ResponseAction_Undefined(wxprs, msg);
                        break;
                    }

            }
            return ret;
        }

        public virtual void answerMsg(string msg = null,string toUser=null,string toNikeName=null,bool isImg=false,bool isUrl=false)
        {
            try
            {
                if (string.IsNullOrEmpty(msg))
                    msg = getMsg();
                if (!isImg)
                {
                    
                        wxprocess.SendMsg?.Invoke(string.Format("@{0} {1}", toNikeName ?? wxmsg.FromMemberNikeName, msg), toUser ?? wxmsg.FromUserNam);
                }
                else
                    if (isUrl)
                        wxprocess.SendUrlImgMsg?.Invoke(msg, toUser ?? wxmsg.FromUserNam);
                    else
                        wxprocess.SendImgMsg?.Invoke(msg, toUser ?? wxmsg.FromUserNam);
            }
            catch (Exception ce)
            {
                wxprocess.SendMsg?.Invoke(string.Format("@{0} {1}", toNikeName ?? wxmsg.FromMemberNikeName, ce.Message), toUser ?? wxmsg.FromUserNam);
            }
        }

        protected virtual string getMsg()
        {
            string ret = "无法识别的内容！";
            return ret;
        }

        public virtual bool Response(ref ShareLotteryPlanClass optPlan)
        {
            optPlan = null;
            answerMsg();
            return true;
        }

        public virtual bool ResumeResponse()
        {

            return true;
        }

        public virtual bool ResponseAsk(TheAskWaitingUserAnswer ask)
        {
            return true;
        }
    }


}
