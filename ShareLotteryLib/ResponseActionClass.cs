using System;
using System.Collections.Generic;
using WolfInv.com.WXMessageLib;
using System.Text.RegularExpressions;
using System.Linq;
using WolfInv.com.ShareLotteryLib;
using WolfInv.Com.WCS_Process;
namespace WolfInv.com.ShareLotteryLib
{
    /// <summary>
    /// 相应微信问题的行为基类
    /// </summary>
    public abstract class ResponseActionClass
    {
        public bool DisableMultiTaskProcess;
        public bool fromWXMP
        {
            get
            {
                return DisableMultiTaskProcess;
            }
        }
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
                        GlobalShare.resetSystem();
                        ShareLotteryPlanClass.Reset();
                        WXMsgProcess.ActionDic = null;
                        WXMsgProcess.bcDic = null;
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
                case ActionType.ShowSystemInfo:
                    {
                        ret = new ResponseAction_ShowSystemInfo(wxprs, msg);
                        break;
                    }
                case ActionType.QueryBussiness:
                    {
                        ret = new ResponseAction_queryBussiness(wxprs, msg);
                        break;
                    }
                case ActionType.CreateUserInfo:
                case ActionType.BindUserInfo:
                case ActionType.AddBussiness:
                    {
                        ret = new ResponseAction_AddBussiness(wxprs, msg);
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
                    wxprocess.SendMsg?.Invoke(fromWXMP?msg:string.Format("@{0} {1}", toNikeName ?? wxmsg.FromMemberNikeName, msg), toUser ?? wxmsg.FromUserNam);
                }
                else
                    if (isUrl)
                        wxprocess.SendUrlImgMsg?.Invoke(msg, toUser ?? wxmsg.FromUserNam);
                    else
                        wxprocess.SendImgMsg?.Invoke(msg, toUser ?? wxmsg.FromUserNam);
            }
            catch (Exception ce)
            {
                wxprocess.SendMsg?.Invoke(fromWXMP?ce.Message:string.Format("@{0} {1}", toNikeName ?? wxmsg.FromMemberNikeName, ce.Message), toUser ?? wxmsg.FromUserNam);
            }
        }

        protected virtual string getMsg()
        {
            string ret = "无法识别的内容！";
            return ret;
        }

        protected void FillAsk(AskItemDefineClass items,ref MutliLevelData data)
        {
            if (items == null)
                return;
            if (items.subItems == null)
            {
                return;
            }
            if (data == null)
            {
                data = new MutliLevelData();
            }
            
            for (int i = 0; i < items.subItems.Count; i++)
            {
                var a = items.subItems[i];
                if (data.SubList != null)
                {
                    MutliLevelData sobj = null;
                    FillAsk(a,ref sobj);
                    //if(sobj != null)
                    data.AddSub(a.value, a.text, sobj);
                }
            }
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

        protected string SwtichLine
        {
            get
            {
                return DisableMultiTaskProcess ? "\n" : "<br>";
            }
        }

        protected matchResult getBussinessFormat(GroupCollection grps, bussinessConfigClass bcc)
        {
            matchResult ret = new matchResult();
            Dictionary<int, string[]> comboList = new Dictionary<int, string[]>();
            int cnt = 0;
            for(int i=0;i<bcc.item.Count;i++)
            {
                bussinessConfigItemClass bcic = bcc.item[i];
                if (bcic.id > cnt)
                {

                    cnt = bcic.id;
                }
                else
                    bcic.id = cnt + 1;
                string val = "";
                if (bcic.regIndex < grps.Count)
                {
                    val = grps[bcic.regIndex].Value;//match result
                }       
                if(bcic.refIndex>0 && bcic.refIndex < grps.Count)
                {
                    if(bcic.regIndex>=0 && bcic.regIndex< comboList[bcic.refIndex].Length)
                    {
                        val = comboList[bcic.refIndex][bcic.regIndex];
                    }
                }
                if(!string.IsNullOrEmpty(bcic.defaultValue) && string.IsNullOrEmpty(val))
                {
                    
                    val = bcic.defaultValue;
                    if (bcic.dataType.ToLower().Equals("date"))
                    {
                        string ddate = bcic.defaultValue.ToLower().Trim();
                        DateTime now = DateTime.Now;
                        if (ddate.Equals("today"))
                        {
                            val = now.ToString("yyyy-MM-dd");
                        }
                        else if (ddate.Equals("tomorrow"))
                        {
                            val = now.AddDays(1).ToString("yyyy-MM-dd");
                        }
                        else if (ddate.Equals("yesterday"))
                        {
                            val = now.AddDays(-1).ToString("yyyy-MM-dd");
                        }
                        else if (ddate.Equals("nextweekfirst"))
                        {
                            val = now.AddDays(7 - (int)now.DayOfWeek+1).ToString("yyyy-MM-dd");
                        }
                        else if (ddate.Equals("nextmonthfirst"))
                        {
                            val = now.AddMonths(1).AddDays(now.Day-1).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            val = now.ToString("yyyy-MM-dd");
                        }
                    }
                    if(bcic.defaultValue.StartsWith("{") && bcic.defaultValue.EndsWith("}"))//user info or system params
                    {
                        string defaultVal = bcic.defaultValue.Trim();
                        defaultVal = defaultVal.Substring(1, defaultVal.Length - 2);
                        string[] vals = defaultVal.Split('.');
                        if(vals[0].Equals("userinfo"))
                        {
                            if (vals[1].ToLower().Equals("wxid"))
                            {
                                val = wxmsg.FromUserNam;
                            }
                            else
                            {
                                if (wxprocess.currChatUser != null)
                                {
                                    val = wxprocess.currChatUser.getUserInfo(vals[1]);
                                }
                                else
                                {

                                }
                            }
                        }
                        else if(vals[0].Equals("sysinfo"))
                        {
                            if(SystemSetting.SysParams.ContainsKey(vals[1]))
                            {
                                val = SystemSetting.SysParams[vals[1]];
                            }
                        }
                        else
                        {

                        }
                    }
                }
                val = val ?? "";
                if (bcic.noNull && string.IsNullOrEmpty(val))
                {
                    ret.msgs.Add(string.Format("{0}值不能为空!", bcic.title));
                }
                if (bcic.maxLen>0 && val.Length>bcic.maxLen)
                {
                    ret.msgs.Add(string.Format("{0}的值[{1}]长度大于{2}!", bcic.title, val, bcic.maxLen));
                }
                if (bcic.minLen > 0 && val.Length < bcic.minLen)
                {
                    ret.msgs.Add(string.Format("{0}的值[{1}]长度小于{2}!", bcic.title, val, bcic.minLen));
                }
                string strText = val;
                if (!string.IsNullOrEmpty(bcic.sysType))//公司/部门职能和成员是四大系统类型
                {
                    //判定三种系统类型
                    List<string> parentS = new List<string>();
                    if (!string.IsNullOrEmpty(bcic.parentRefId))
                    {
                        string[] ids = bcic.parentRefId.Split(bcic.parentRefSplit.ToArray());
                        for (int l = 0; l < ids.Length; l++)
                        {
                            int idx = 0;
                            if (int.TryParse(ids[l], out idx))
                            {
                                if (idx > 0)
                                {
                                    parentS.Add(ret.items.Values.ToList()[idx - 1]);//序号，所以要减1
                                }
                                else
                                {
                                    parentS.Clear();
                                    break;
                                }
                            }
                            else//如果没有匹配完整，全部取消
                            {
                                parentS.Clear();
                                break;
                            }
                                    
                        }
                    }
                    SystemDataClass sdc = GlobalShare.GlobalData?.getDefault(bcic.sysType, val, parentS.ToArray()); 
                    if(sdc != null && !string.IsNullOrEmpty(sdc.code))//结果不为空才替换
                    {
                        strText = sdc.name;
                        val = sdc.code;
                    }
                }
                if (!ret.items.ContainsKey(bcic.reqInfo))
                    ret.items.Add(bcic.reqInfo,val);
                if (!bcic.hiden)
                {
                    if (!ret.displayItems.ContainsKey(bcic.title))
                    {

                        ret.displayItems.Add(bcic.title, strText);
                    }
                }
                if (bcic.isKey)
                {
                    if(!ret.keys.ContainsKey(bcic.reqInfo))
                        ret.keys.Add(bcic.reqInfo, val);
                }
                if (bcic.isActionInfo)
                {
                    ret.action = val;
                }
                if(bcic.isTypeInfo)
                {
                    ret.type = val;
                }
                if(bcic.isUserInfo)
                {
                    ret.user = val;
                }
                if(bcic.isCombo)
                {
                    string[] cols = val.Split(bcic.splitStrings.ToCharArray());
                    comboList.Add(bcic.regIndex,cols);
                }
                
            }
            ret.success = true;
            return ret;
        }

        public class matchResult
        {
            public bool success;
            public string user;
            public string type;
            public string action;
            public List<string> msgs = new List<string>();
            public Dictionary<string,string> items = new Dictionary<string, string>();
            public Dictionary<string, string> keys = new Dictionary<string, string>();
            
            public Dictionary<string, string> displayItems = new Dictionary<string, string>();
        }
    }


}
