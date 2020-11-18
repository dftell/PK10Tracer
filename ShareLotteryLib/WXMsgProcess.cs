using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WolfInv.com.WXMessageLib;
using System.Xml;
using XmlProcess;
using System.Runtime.Serialization;
using WolfInv.com.LogLib;
using System.Net;
using WolfInv.Com.WCS_Process;
using Group = WolfInv.Com.WCS_Process.Group;

namespace WolfInv.com.ShareLotteryLib
{
    [Serializable]
    public class WXMsgProcess :LogClass//: ISerializable
    {
        #region 运行时变量
        public Dictionary<string, Dictionary<string, TheAskWaitingUserAnswer>> waitingAsk { get; set; }//所有群等待回复的问题
        public Dictionary<string, Dictionary<string, TheAskWaitingUserAnswer>> historyAsk { get; set; }//所有群历史回复的问题

        public static Dictionary<string, ActionDefine> ActionDic;

        public static Dictionary<string,bussinessConfigClass> bcDic;

        public Action<string> LoginPress;

        #endregion

        public string RobotUnionId { get; set; }
        public string RobotNikeName { get; set; }
        public string RobotUserName { get; set; }

        public Action<string, ShareLotteryPlanClass> SharePlanChanged;

        public Action<string, wxMessageClass> MsgChanged;

        public Func<string, string, string> SendMsg;
        public Func<string, string, string> SendImgMsg;
        public Func<string, string, string> SendUrlImgMsg;
        public ShareLotteryPlanCollection AllPlan { get; set; }
        public CITMSUser currChatUser;
        public string ProcessName { get; set; }
        public WXMsgProcess()
        {
            ProcessName = "默认设置";
            waitingAsk = new Dictionary<string, Dictionary<string, TheAskWaitingUserAnswer>>();
            historyAsk = new Dictionary<string, Dictionary<string, TheAskWaitingUserAnswer>>();
        }

        #region 序列化
        ////public void GetObjectData(SerializationInfo info, StreamingContext context)
        ////{
        ////    try
        ////    {
        ////        info.AddValue("WXMsgProcess_ProcessName", ProcessName);
        ////        info.AddValue("WXMsgProcess_SendMsg", SendMsg, typeof(Func<string, string, string>));
        ////        info.AddValue("WXMsgProcess_SharePlanChanged", SharePlanChanged, typeof(Action<string, ShareLotteryPlanClass>));
        ////        info.AddValue("WXMsgProcess_MsgChanged", MsgChanged, typeof(Action<string, wxMessageClass>));
        ////        info.AddValue("WXMsgProcess_AllPlan", AllPlan, typeof(ShareLotteryPlanCollection));
        ////    }
        ////    catch(Exception ce)
        ////    {

        ////    }

        ////    //base.GetObjectData(info, context);

        ////    //info.AddValue("WXMsgProcess_Value", Value);
        ////}
        ////protected WXMsgProcess(SerializationInfo info, StreamingContext context)
        ////{
        ////    try
        ////    {
        ////        ProcessName = info.GetString("WXMsgProcess_ProcessName");
        ////        SendMsg = (Func<string, string, string>)info.GetValue("WXMsgProcess_SendMsg", typeof(Func<string, string, string>));
        ////        SharePlanChanged = (Action<string, ShareLotteryPlanClass>)info.GetValue("WXMsgProcess_SharePlanChanged",  typeof(Action<string, ShareLotteryPlanClass>));
        ////        MsgChanged = (Action<string, wxMessageClass>)info.GetValue("WXMsgProcess_MsgChanged", typeof(Action<string, wxMessageClass>));
        ////        AllPlan = (ShareLotteryPlanCollection)info.GetValue("WXMsgProcess_AllPlan", typeof(ShareLotteryPlanCollection));
        ////    }
        ////    catch(Exception ce)
        ////    {

        ////    }
        ////}
        #endregion
        ////public void ReceiveMsg()
        ////{
        ////    RefreshMsg(sender, msgs);
        ////}

        public void RefreshMsg(object sender, List<wxMessageClass> msgs,bool disableMutliTask=false)
        {

            string msg = null;
            bool sysValid = GlobalShare.InitAndVirLogin(string.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath)?AppDomain.CurrentDomain.BaseDirectory:AppDomain.CurrentDomain.RelativeSearchPath,out msg);
            if(!sysValid)
            {
                for(int i=0;i<msgs.Count;i++)
                    SendMsg?.Invoke(string.Format("{0}", msg), msgs[i].FromUserNam);
                return;
            }
            wxMessageClass wxmsg = null;
            try
            {

                for (int i = 0; i < msgs.Count; i++)
                {
                    wxmsg = msgs[i];
                    //if(wxmsg.FromMemberNikeName == null)
                    //{
                    //    continue;
                    //}
                    if (SystemSetting.saveMsg == "1")//如果保存记录
                    {
                        Task.Run(() =>
                        {
                            saveMsg(wxmsg);
                        });
                    }
                    if (!wxmsg.IsAtToMe)
                    {
                        if (wxmsg.ToUserName == RobotUserName && SystemSetting.allProviteChat == "1")//个人对我说的话，转入私聊
                        {
                            ProcessProviteMsg(wxmsg);
                        }
                        //if(wxmsg.ToUserName == )
                        continue;
                    }
                    MsgChanged?.Invoke(wxmsg.FromMemberNikeName, wxmsg);//更新消息
                    ShareLotteryPlanClass optPlan = null;
                    if (!ProcessOneMsg(wxmsg, ref optPlan,disableMutliTask))//不是群内对我说的话
                    {   
                       
                        continue;
                    }
                    if (optPlan != null)
                    {
                        AllPlan.setCurrPlan(optPlan);
                        SharePlanChanged?.Invoke(optPlan.wxRootName, optPlan);
                    }
                }
            }
            catch (Exception ce)
            {
                SendMsg?.Invoke(string.Format("{0}:{1}", ce.Message, ce.StackTrace),wxmsg.FromUserNam);
            }
        }


        bool ProcessOneMsg(wxMessageClass wxmsg,ref ShareLotteryPlanClass optPlan,bool disableMutliTask=false)
        {
            ActionDefine define = null;
            ActionType curraction = checkTheAction(wxmsg,ref define);
            currChatUser = GlobalShare.GlobalData?.getUserByWxId(wxmsg.FromUserNam,!disableMutliTask);
            if(!define.noNeedLogin && currChatUser == null)//如果需要登录，但是该用户未有微信账号对应用户信息，那就证明没有登陆
            {

                string msg = string.Format(@"{0}此操作需要你的用户信息，请先登录!",disableMutliTask?"":"@"+wxmsg.FromMemberNikeName+" ");
                SendMsg(msg, wxmsg.FromUserNam);
                return false;
            }
            if(currChatUser != null)
            {
                this.LoginPress?.Invoke(currChatUser.wxOpenId);
            }
            ResponseActionClass rac = null;
            bool isOpenAskResult = false;
            if (waitingAsk!= null)
            {
                if(waitingAsk.ContainsKey(wxmsg.FromUserNam) && waitingAsk[wxmsg.FromUserNam].ContainsKey(wxmsg.FromMemberUserName))
                {
                    TheAskWaitingUserAnswer askObj = waitingAsk[wxmsg.FromUserNam][wxmsg.FromMemberUserName];
                    //如果回复的人就是等待回答清单中的人，首先判断是否是回复，如果不是回复，宣布首先回答我的问题，其他问题无效，等下再问
                    if ((curraction!= ActionType.ValidateInfo && curraction!= ActionType.Undefined) || (curraction == ActionType.Undefined && !askObj.isOpenAsk))//如果有非开放式问题
                    {
                        
                        string msg = string.Format(@"@{0} 请您先回答我刚才提出的问题！在回答之前我不会接受你其他任何请求.
{1}",wxmsg.FromMemberNikeName,askObj.askMsg);
                        SendMsg(msg,wxmsg.FromUserNam);
                        return false;
                    }
                    else //如果是不可识别的答案，只处理开放式问题,把最后一个问题提出来
                    {
                        rac = new ResponseAction_ValidateInfo(this,wxmsg);
                        isOpenAskResult = true;
                    }
                }
            }
            if(!isOpenAskResult)            
                rac = ResponseActionClass.CreateAction(curraction, this, wxmsg);
            
            if (rac == null)
            {
                SendMsg("系统已重置！", wxmsg.FromUserNam);
                optPlan = null;
                return false;
            }
            rac.DisableMultiTaskProcess = disableMutliTask;
            rac.actionDefine = define;
            if (curraction == ActionType.Undefined && !isOpenAskResult)
                rac.currPlan = null;
            else
            {
                rac.currPlan = AllPlan.getCurrRoomPlan(wxmsg.FromUserNam);
            }
            return rac.Response(ref optPlan);
            if (curraction == ActionType.Undefined)
                return false;
        }

        bool ProcessProviteMsg(wxMessageClass wxmsg)
        {
            ResponseAction_Chat rc = new ResponseAction_Chat(this, wxmsg);
            ShareLotteryPlanClass opt = null;
            rc.Response(ref opt);
            return true;
        }

        public ActionType checkTheAction(wxMessageClass wxmsg,ref ActionDefine define)
        {

            define = new ActionDefine();
            define.noNeedLogin = true;
            string myname = string.Format("@{0}", wxmsg.AtMemberNikeName[0]);
            string msg = wxmsg.Msg.Replace(myname, "").Trim().Replace(" ", "");
            ////string strtest = @"\[[\s\S]*?]";
            ////Regex regtest = new Regex(strtest);
            
            ////MatchCollection tmcs = regtest.Matches(msg);
            
            if (ActionDic == null)
            {
                ActionDic = getActionDictionary();
            }
            if(bcDic == null)
            {
                bussinessProcess.Load();
                bcDic = bussinessProcess.bussinessList==null?new Dictionary<string, bussinessConfigClass>():bussinessProcess.bussinessList.configs.ToDictionary(a=>a.matchKey,a=>a);
            }

            Type t = typeof(ActionType);
            Array arr = Enum.GetValues(t);
            Dictionary<string, int> actDic = new Dictionary<string, int>();
            foreach (int i in arr)
            {
                string strName = Enum.GetName(t, i);
                if (!actDic.ContainsKey(strName))
                    actDic.Add(strName, i);
            }
            
            foreach (string strName in ActionDic.Keys)
            {
                //string strName = Enum.GetName(t, myCode);//获取名称
                ActionDefine dic = ActionDic[strName];
                if(dic == null)
                {
                    continue;
                }
                
                string useActionType = dic.useActionType;
                if(string.IsNullOrEmpty(useActionType))
                {
                    useActionType = dic.actionName;
                }
                if (!actDic.ContainsKey(useActionType))
                {
                    continue;
                }
                int myCode = actDic[useActionType];
                for (int i = 0; i < dic.regConditions.Count; i++)
                {
                    Regex regTr = new Regex(dic.regConditions[i]);
                    MatchCollection mcs = regTr.Matches(msg);
                    if (mcs.Count > 0)
                    {
                        define = dic;
                        if ((ActionType)myCode == ActionType.ValidateInfo)
                        {
                            if (mcs.Count > 1)
                            {
                                continue;
                            }
                        }
                        return (ActionType)myCode;
                    }
                }
            }
            return ActionType.Undefined;
        }

        Dictionary<string,ActionDefine> getActionDictionary()
        {
            Dictionary<string, ActionDefine> ret = new Dictionary<string, ActionDefine>();
            string xml = TextFileComm.getFileText("MsgActionDic.xml","xml");
            if(xml == null)
                return ret;
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                xmldoc.LoadXml(xml);
                XmlNode root = xmldoc.SelectSingleNode("root");
                XmlNodeList nlist = root.SelectNodes("action");
                foreach(XmlNode node in nlist)
                {
                    ActionDefine ad = new ActionDefine();
                    
                    string type = XmlUtil.GetSubNodeText(node, "@type");
                    ad.submitUrl = XmlUtil.GetSubNodeText(node, "@submitUrl");
                    ad.useActionType = XmlUtil.GetSubNodeText(node, "@useActionType"); 
                    ad.useBussinessType = XmlUtil.GetSubNodeText(node, "@useBussinessType"); 
                    ad.isOpenAsk = XmlUtil.GetSubNodeText(node, "@isOpenAsk").ToLower().Trim() == "1" || XmlUtil.GetSubNodeText(node, "@isOpenAsk").ToLower().Trim() == "true";
                    ad.noNeedLogin = XmlUtil.GetSubNodeText(node, "@noNeedLogin").ToLower().Trim() == "true"|| XmlUtil.GetSubNodeText(node, "@noNeedLogin").ToLower().Trim() == "1";
                    ad.actionName = type;
                    ad.needAsk = XmlUtil.GetSubNodeText(node, "@needAsk").ToLower().Trim() == "1" || XmlUtil.GetSubNodeText(node, "@needAsk").ToLower().Trim() == "true";
                    int.TryParse(XmlUtil.GetSubNodeText(node, "@typeIndex"), out ad.typeIndex);
                    //int.TryParse(XmlUtil.GetSubNodeText(node, "@actionIndex"), out ad.actionIndex);
                    if (ad.needAsk)
                        getAskItems(node.SelectSingleNode("ask/item"),ref ad.AskDefine);
                    ad.regRule = XmlUtil.GetSubNodeText(node, "@regRule");
                    List<string> slist = new List<string>();
                    XmlNodeList snodes = node.SelectNodes("item");
                    foreach(XmlNode snode in snodes)
                    {
                        slist.Add(XmlUtil.GetSubNodeText(snode, "."));
                    }
                    ad.regConditions = slist;
                    snodes = node.SelectNodes("response/case");
                    foreach(XmlNode snode in snodes)
                    {
                        ResponseProcessItem rpi = new ResponseProcessItem();
                        rpi.condition = XmlUtil.GetSubNodeText(snode, "condition");
                        rpi.isUrl = XmlUtil.GetSubNodeText(snode, "isUrl");
                        rpi.type = XmlUtil.GetSubNodeText(snode, "type");
                        rpi.msg = XmlUtil.GetSubNodeText(snode, "msg");
                        rpi.paramList = XmlUtil.GetSubNodeText(snode, "paramList");
                        if(!ad.responseGroup.ContainsKey(rpi.condition))
                        {
                            ad.responseGroup.Add(rpi.condition, rpi);
                        }
                    }
                    if(!ret.ContainsKey(type))
                    {
                        ret.Add(type, ad);
                    }
                }
            }
            catch(Exception ce)
            {
                return ret;
            }
            return ret;
        }

        void getAskItems(XmlNode node,ref AskItemDefineClass pobj)
        {
            string msg = XmlUtil.GetSubNodeText(node, "@text");
            if(string.IsNullOrEmpty(msg))
            {
                return;
            }
            string val = XmlUtil.GetSubNodeText(node, "@value");
            bool isOpenAsk = XmlUtil.GetSubNodeText(node, "@isOpenAsk") == "1";
            if(pobj == null)
            {
                pobj = new AskItemDefineClass();
            }
            pobj.text = msg;
            pobj.value = val;
            pobj.isOpenAsk = isOpenAsk;
            XmlNodeList nodes = node.SelectNodes("item");
            foreach(XmlNode snode in nodes)
            {
                AskItemDefineClass sobj = null;
                getAskItems(snode,ref sobj);
                if(sobj != null)
                {
                    if(pobj.subItems == null)
                        pobj.subItems = new List<AskItemDefineClass>();
                    pobj.subItems.Add(sobj);
                }
            }
        }

        public bool InjectAsk(TheAskWaitingUserAnswer ask)
        {
            
            lock (waitingAsk)
            {
                if (waitingAsk == null)
                {
                    waitingAsk = new Dictionary<string, Dictionary<string, TheAskWaitingUserAnswer>>();
                }
                if (!waitingAsk.ContainsKey(ask.roomName))
                {
                    waitingAsk.Add(ask.roomName, new Dictionary<string, TheAskWaitingUserAnswer>());
                }
                if (waitingAsk[ask.roomName].ContainsKey(ask.userName))
                {
                    //
                    //waitingAsk[ask.roomName][ask.userName].Closed = true;
                    //
                    CopyToHistoryAsks(waitingAsk[ask.roomName][ask.userName]);
                    CloseCurrAsk(ask);
                    return false;
                }
                else
                {
                    waitingAsk[ask.roomName].Add(ask.userName, ask);
                }
                return true;
            }
        }

        public void CopyToHistoryAsks(TheAskWaitingUserAnswer ask)
        {
            string roomId = ask.roomName;
            string requestUser = ask.userName;
            ask.Closed = true;
            lock (historyAsk)
            {
                if (historyAsk == null)
                {
                    historyAsk = new Dictionary<string, Dictionary<string, TheAskWaitingUserAnswer>>();
                }
                if (!historyAsk.ContainsKey(roomId))
                {
                    historyAsk.Add(roomId, new Dictionary<string, TheAskWaitingUserAnswer>());
                }
                if (historyAsk[roomId].ContainsKey(ask.askId))
                {
                    throw new Exception("历史上存在相同编号的疑问！");
                }
                else
                {
                    historyAsk[roomId].Add(ask.askId, ask);
                }
            }
        }

        public void CloseCurrAsk(TheAskWaitingUserAnswer ask)
        {
            string roomId = ask.roomName;
            string requestUser = ask.userName;
            ask.Closed = true;
            lock(waitingAsk)
            {
                if(waitingAsk.ContainsKey(roomId) && waitingAsk[roomId].ContainsKey(requestUser))
                {
                    waitingAsk[roomId].Remove(requestUser);
                }
            }
        }

        public TheAskWaitingUserAnswer getLastAsk(string roomid,string username)
        {
            if(waitingAsk.ContainsKey(roomid) && waitingAsk[roomid].ContainsKey(username))
            {
                return waitingAsk[roomid][username];
            }
            return null;
        }

        public bool setLastAsk(TheAskWaitingUserAnswer ask)
        {
            string roomid = ask.roomName;
            string username = ask.userName;
            if (waitingAsk.ContainsKey(roomid) && waitingAsk[roomid].ContainsKey(username))
            {
                waitingAsk[roomid][username] = ask;
                return true;
            }
            InjectAsk(ask);
            return false;//做正常的操作，但是报否
        }

        public void saveMsg(wxMessageClass msg)
        {
            /*
             wxmsgid	
             msgFromUser	
             msgFromNike	
             msgFromMember	
             msgFromMemberNike	
             msgToUser	
             msgToNike	
             msgIsInRoomToMe	
             msgIsProviteToMe	
             orgContent	
             msgTime	
             providerUnionId	
             providerUserName	
             providerNike
1	test	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
             */
            string urlM = "http://www.wolfinv.com/pk10/app/submitWXChatMsg.asp?" +
                "msgFromUser={0}" +
                "&msgFromNike={1}" +
                "&msgFromMember={2}" +
                "&msgFromMemberNike={3}" +
                "&msgToUser={4}" +
                "&msgToNike={5}" +
                "&msgIsInRoomToMe={6}" +
                "&msgIsProviteToMe={7}" +
                "&orgContent={8}" +
                "&msgTime={9}" +
                "&providerUnionId={10}" +
                "&providerUserName={11}" +
                "&providerNike={12}";
            string url = string.Format(urlM,
                msg.FromUserNam,
                msg.FromNikeName,
                msg.FromMemberUserName,
                msg.FromMemberNikeName,
                msg.ToUserName,
                msg.ToNikeName,
                msg.IsAtToMe?1:0,
                msg.ToUserName == RobotUserName?1:0,
                msg.OrgMsg,
                msg.CreateTime,
                RobotUnionId,
                RobotUserName,
                RobotNikeName

                );
            try
            {
                WebClient wc = new WebClient();
                wc.Encoding = Encoding.UTF8;
                string res = wc.DownloadString(url);
            }
            catch(Exception ce)
            {

            }
        }
    }

    public class ResponseProcessItem
    {
        public string condition { get; set; }
        public string type { get; set; }
        public string isUrl { get; set; }
        public string msg { get; set; }
        public string paramList { get; set; }

    }
    
}
