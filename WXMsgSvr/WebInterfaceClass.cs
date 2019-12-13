using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leestar54.WeChat.WebAPI;

using Leestar54.WeChat.WebAPI.Modal;

using Leestar54.WeChat.WebAPI.Modal.Response;

using System;

using System.Collections.Generic;

using System.IO;

using System.Linq;

using System.Text;

using System.Threading;

using System.Threading.Tasks;

using System.Windows.Forms;
using System.Runtime.InteropServices;
using WolfInv.com.LogLib;
using WolfInv.com.WinInterComminuteLib;
using WolfInv.com.WXMessageLib;
using System.Text.RegularExpressions;
using System.Drawing;

namespace WolfInv.com.WXMsgCom
{
    public delegate void MessageProcessedEvent(object sender,List<wxMessageClass> msgs);
    [Guid("9E69FAA3-1780-4E1C-9D4B-100BE7CED23B"),
 ClassInterface(ClassInterfaceType.None),
 ComSourceInterfaces(typeof(WebInterface))]
    public class WebInterfaceClass :RemoteServerClass, WebInterface
    {
        public static bool IPCCreated;
        public static bool ClientValid = false;
        public  bool Valid { get { return ClientValid; } }
        private static Client client;
        public MessageProcessedEvent MsgProcessCompleted;
        #region 专供外部连接,buffs当有连接后临时存储消息，当断开连接后30分钟后清空
        static bool Connected = false;
        static DateTime LastConnectTime;
        static List<wxMessageClass> MessageBuffs;
        int MaxDisconnectMinutes = 30;
        public string UnionId
        {
            get
            {
                return client?.user?.Uin;
            }
        }

        public string UserName
        {
            get
            {
                return client?.user?.UserName;
            }
        }

        public string UserNike
        {
            get
            {
                return client?.user?.NickName;
            }
        }

        void InjectBuffs(List<wxMessageClass> msgs)
        {
            lock (MessageBuffs)
            {
                if (IsConnected())
                {
                    MessageBuffs.AddRange(msgs);
                }
                else
                {
                    Connected = false;//主动置否
                    if (MessageBuffs.Count > 0)
                        MessageBuffs.Clear();
                }
            }
        }

        bool IsConnected()
        {
            if (!Connected)
                return false;
            if(DateTime.Now.Subtract(LastConnectTime).TotalMinutes>MaxDisconnectMinutes)
            {
                return false;
            }
            return true;
        }

        public List<wxMessageClass> getNewestMsg()
        {
            List<wxMessageClass> ret = new List<wxMessageClass>();
            lock(MessageBuffs)
            {
                if (MessageBuffs.Count > 0)
                {
                    ret.AddRange(MessageBuffs);
                    MessageBuffs.Clear();
                }
            }
            return ret;
        }
        #endregion
       

        

        public void HeartCheck()
        {
            Connected = true;
            LastConnectTime = DateTime.Now;
        }

        public static Dictionary<string, Contact> contactDict = new Dictionary<string, Contact>();

        private static QrCodeForm qrForm;
        static frm_MainWin frm;
        //private static string cookiePath = AppDomain.CurrentDomain.BaseDirectory + "autoLoginCookie";
        
        static bool DisplayByFrm;
        public WebInterfaceClass()
        {
            MessageBuffs = new List<wxMessageClass>();
            if(!IPCCreated)
            {
                try
                {
                    this.CreateChannel("wxmsg",true);
                    IPCCreated = true;
                }
                catch(Exception ce)
                {
                    IPCCreated = false;
                }
            }
            
        }

        private static string GetAssemblyPath()
        {
            string _CodeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            _CodeBase = _CodeBase.Substring(8, _CodeBase.Length - 8);    // 8是 file:// 的长度
            string[] arrSection = _CodeBase.Split(new char[] { '/' });
            string _FolderPath = "";
            for (int i = 0; i < arrSection.Length - 1; i++)
            {
                _FolderPath += arrSection[i] + "/";
            }
            return _FolderPath;
        }

        static string CurrPath
        {
            get
            {
                return GetAssemblyPath();
            }
        }
        private static string cookiePath = CurrPath + "autoLoginCookie";

        

        string cookie
        {
            get
            {
                try
                {
                    LogableClass.ToLog("Cookie路径", cookiePath);
                    if (File.Exists(cookiePath))
                    {

                        StreamReader sr = new StreamReader(cookiePath, Encoding.Default);

                        string ret = sr.ReadLine();

                        sr.Close();
                        return ret;
                    }
                }
                catch(Exception ce)
                {
                    LogableClass.ToLog("获取Cookie", string.Format("{0}:{1}", cookiePath, ce.Message));
                    return ce.Message;
                }
                return "";
            }
        }

        
        public string Init()
        {
            try
            {
                ////Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

                ////AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                LogableClass.ToLog("实例化接口", "开始");
                if (client == null)
                    client = new Client();
                LogableClass.ToLog("启动二维码", "开始");
                if (DisplayByFrm)
                    qrForm = new QrCodeForm();

                LogableClass.ToLog("指定响应事件", "开始");



                //获取登陆之后记录的cookie，实现推送手机端登陆，取代扫码

                //若不需要，注释掉以下代码即可





                client.ExceptionCatched += Client_ExceptionCatched; ;

                client.GetLoginQrCodeComplete += Client_GetLoginQrCodeComplete; ;

                client.CheckScanComplete += Client_CheckScanComplete; ;

                client.LoginComplete += Client_LoginComplete;

                client.DelContactListComplete += Client_DelContactListComplete; ;

                client.ModContactListComplete += Client_ModContactListComplete;

                client.GetContactComplete += Client_GetContactComplete; ;
                client.BatchGetContactComplete += Client_BatchGetContactComplete; ;
                client.MPSubscribeMsgListComplete += Client_MPSubscribeMsgListComplete; ;

                client.LogoutComplete += Client_LogoutComplete; ;
                if (DisplayByFrm)
                {
                    frm = new frm_MainWin(client);
                    client.ReceiveMsg += RefreshMsg;
                    this.MsgProcessCompleted += frm.RefreshMsg;
                    
                }
                else
                {
                    client.ReceiveMsg += Client_ReceiveMsg;
                }



                //client.GetContactComplete += frm.RefreshContact;

                Console.WriteLine("小助手启动");
                LogableClass.ToLog("小助手启动", "开始");




                //获取群成员详情，需要我们主动调用，一般用不到，因为群里已经包含Member基本信息。

                //Contact chatRoom = contactDict["群UserName"];

                //string listStr = string.Join(",", chatRoom.MemberList);

                //client.GetBatchGetContactAsync(listStr, chatRoom.UserName);

                return "";
            }
            catch(Exception ce)
            {
                LogableClass.ToLog("错误", "初始化接口失败！", string.Format("{0}:{1}", ce.Message, ce.StackTrace));
                return string.Format("{0}:{1}", ce.Message, ce.StackTrace);
                
            }

        }

        

        public string Start()
        {
            LogableClass.ToLog("接口", "启动");
            try
            {
                LogableClass.ToLog("cookie", cookie);
                client.Start(cookie);
                LogableClass.ToLog("接口已启动", "完毕");
                if (DisplayByFrm)
                    Application.Run(frm);
                else
                {
                    while(true)
                    {
                    }
                }
            }
            catch (Exception ce)
            {
                LogableClass.ToLog("错误", "接口启动失败！", string.Format("{0}:{1}", ce.Message, ce.StackTrace));
                return string.Format("{0}:{1}", ce.Message, ce.StackTrace);
            }
            //
            return "";
        }

        public void Stop()
        {
            client.Logout();
        }


        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)

        {

            Console.WriteLine(e.ToString());

        }



        private static void Client_ModContactListComplete(object sender, TEventArgs<List<Contact>> e)

        {
            try
            {

                Console.WriteLine("接收修改联系人信息");

                foreach (var item in e.Result)

                {
                    contactDict[item.UserName] = item;
                }
            }
            catch(Exception ce)
            {

            }
        }



        private static void Client_DelContactListComplete(object sender, TEventArgs<List<DelContactItem>> e)

        {

            Console.WriteLine("接收删除联系人信息");

        }



        private static void Client_ReceiveMsg(object sender, TEventArgs<List<AddMsg>> e)
        {
            try
            {
                ClientValid = true;
                LogableClass.ToLog("接收到消息条数:", e.Result.Count.ToString());
                foreach (var item in e.Result)

                {

                    switch (item.MsgType)

                    {

                        case MsgType.MM_DATA_TEXT:

                            if (contactDict.Keys.Contains(item.FromUserName))
                            {
                                if (item.FromUserName.StartsWith("@@"))
                                {
                                    //群消息，内容格式为[群内username];<br/>[content]，例如Content=@ffda8da3471b87ff22a6a542c5581a6efd1b883698db082e529e8e877bef79b6:<br/>哈哈
                                    string[] content = item.Content.Split(new string[] { ":<br/>" }, StringSplitOptions.RemoveEmptyEntries);
                                    Console.WriteLine(contactDict[item.FromUserName].NickName + "：" + contactDict[item.FromUserName].MemberDict[content[0]].NickName + "：" + content[1]);
                                }
                                else
                                {
                                    Console.WriteLine(contactDict[item.FromUserName].NickName + "：" + item.Content);
                                }
                            }

                            else
                            {

                                //不包含（一般为群）则需要我们主动拉取信息

                                client.GetBatchGetContactAsync(item.FromUserName);

                            }



                            

                            break;

                        case MsgType.MM_DATA_HTML:

                            break;

                        case MsgType.MM_DATA_IMG:

                            break;

                        case MsgType.MM_DATA_PRIVATEMSG_TEXT:

                            break;

                        case MsgType.MM_DATA_PRIVATEMSG_HTML:

                            break;

                        case MsgType.MM_DATA_PRIVATEMSG_IMG:

                            break;

                        case MsgType.MM_DATA_VOICEMSG:

                            break;

                        case MsgType.MM_DATA_PUSHMAIL:

                            break;

                        case MsgType.MM_DATA_QMSG:

                            break;

                        case MsgType.MM_DATA_VERIFYMSG:

                            //自动加好友，日限额80个左右，请勿超限额多次调用，有封号风险

                            //client.VerifyUser(item.RecommendInfo);

                            break;

                        case MsgType.MM_DATA_PUSHSYSTEMMSG:

                            break;

                        case MsgType.MM_DATA_QQLIXIANMSG_IMG:

                            break;

                        case MsgType.MM_DATA_POSSIBLEFRIEND_MSG:

                            break;

                        case MsgType.MM_DATA_SHARECARD:

                            break;

                        case MsgType.MM_DATA_VIDEO:

                            break;

                        case MsgType.MM_DATA_VIDEO_IPHONE_EXPORT:

                            break;

                        case MsgType.MM_DATA_EMOJI:

                            break;

                        case MsgType.MM_DATA_LOCATION:

                            break;

                        case MsgType.MM_DATA_APPMSG:
                            {

                            }
                            break;

                        case MsgType.MM_DATA_VOIPMSG:

                            break;

                        case MsgType.MM_DATA_STATUSNOTIFY:

                            switch (item.StatusNotifyCode)

                            {

                                case StatusNotifyCode.StatusNotifyCode_READED:

                                    break;

                                case StatusNotifyCode.StatusNotifyCode_ENTER_SESSION:

                                    break;

                                case StatusNotifyCode.StatusNotifyCode_INITED:

                                    break;

                                case StatusNotifyCode.StatusNotifyCode_SYNC_CONV:

                                    //初始化的时候第一次sync会返回最近聊天的列表

                                    client.GetBatchGetContactAsync(item.StatusNotifyUserName);

                                    break;

                                case StatusNotifyCode.StatusNotifyCode_QUIT_SESSION:

                                    break;

                                default:

                                    break;

                            }

                            break;

                        case MsgType.MM_DATA_VOIPNOTIFY:

                            break;

                        case MsgType.MM_DATA_VOIPINVITE:

                            break;

                        case MsgType.MM_DATA_MICROVIDEO:

                            break;

                        case MsgType.MM_DATA_SYSNOTICE:

                            break;

                        case MsgType.MM_DATA_SYS:

                            //系统消息提示，例如完成好友验证通过，建群等等，提示消息“以已经通过了***的朋友验证请求，现在可以开始聊天了”、“加入了群聊”

                            //不在字典，说明是新增，我们就主动拉取加入联系人字典

                            if (!contactDict.Keys.Contains(item.FromUserName))

                            {

                                client.GetBatchGetContactAsync(item.FromUserName);

                            }

                            break;

                        case MsgType.MM_DATA_RECALLED:

                            break;

                        default:

                            break;

                    }

                }

            }

            catch (Exception err)

            {

                Console.WriteLine("异常：" + err.Message);

            }

        }

        public void RefreshMsg(object sender, TEventArgs<List<AddMsg>> e)
        {
            try
            {
                List<AddMsg> msg = e.Result;
                string dicitems = "FromUserName,FromUserNikeName,FromMemberName,FromMemerNikeName,ToUserName,ToUserNikeName,AtNikeNames,Message,MsgTime";
                string[] itemArr = dicitems.Split(',');
                List<wxMessageClass> messages = new List<wxMessageClass>();
                Dictionary<string, Contact> AllUsers = contactDict;
                //List<string> msgs = tbox.Lines.ToList();
                //List<ListViewItem> msgs = new List<ListViewItem>();
                for (int i = 0; i < msg.Count; i++)
                {

                    AddMsg wxmsg = msg[i];
                    if ( wxmsg.MsgType != MsgType.MM_DATA_TEXT )
                    {
                        continue;
                    }                    
                    string fromName = wxmsg.FromUserName;
                    string NickName = fromName;
                    if (AllUsers.ContainsKey(fromName))//联系人发来的
                    {
                        ////if(fromName.StartsWith("@@"))
                        ////{
                        ////    client.GetBatchGetContactAsync(string.Join(",",AllUsers[fromName].MemberDict.Keys.ToArray()),fromName);
                        ////}
                        NickName = AllUsers[fromName].NickName;
                        if (AllUsers[fromName].MemberList.Count > 0)//是群
                        {

                            if (!AllUsers[fromName].DisplayNikeName)//所有的群都显示昵称
                            {
                                client.GetBatchGetContactAsync(string.Join(",", AllUsers[fromName].MemberList.Select(a => a.UserName)), fromName);
                                AllUsers[fromName].DisplayNikeName = true;
                            }
                        }
                    }
                    else
                    {

                        client.GetBatchGetContactAsync(fromName);
                        continue;
                    }
                    string ToName = wxmsg.ToUserName;
                    string ToNameNike = null;
                    if (AllUsers.ContainsKey(ToName))
                    {
                        ToNameNike = AllUsers[ToName].NickName;
                    }

                    //string[] strMsg = new string[5];
                    wxMessageClass wmsg = new wxMessageClass();

                    if (wxmsg.MsgType == MsgType.MM_DATA_IMG)
                    {
                        string strImg = wxmsg.Content;

                    }

                    if (fromName.StartsWith("@@"))
                    {
                        
                        string[] content = wxmsg.Content.Split(new string[] { ":<br/>" }, StringSplitOptions.RemoveEmptyEntries);
                        content[1] = content[1].Replace("<br/>", "\r\n");
                        string memName = "";
                        if (1 == 2 && AllUsers.ContainsKey(content[0]))
                        {
                            memName = AllUsers[content[0]].DisplayName;
                            if (string.IsNullOrEmpty(memName))
                            {
                                memName = AllUsers[content[0]].NickName;
                            }
                        }
                        else
                        {
                            if (AllUsers[fromName].MemberDict.ContainsKey(content[0]))
                                memName = AllUsers[fromName].MemberDict[content[0]].DisplayName;
                            if (string.IsNullOrEmpty(memName))
                            {
                                memName = AllUsers[fromName].MemberDict[content[0]].NickName;
                            }
                        }

                        //strMsg = string.Format("[{0}]{1}:{2}", NickName, memName, string.Join("",content.Skip(1).ToArray()));
                        //strMsg = new string[] { DateTime.Now.ToShortTimeString(), NickName, memName, "", string.Join("", content.Skip(1).ToArray()) };
                        wmsg.FromUserNam = fromName;
                        wmsg.FromNikeName = NickName;
                        wmsg.FromMemberUserName = content[0];
                        wmsg.FromMemberNikeName = memName;
                        wmsg.CreateTime = wxmsg.CreateTime;
                        wmsg.Msg = content[1];
                        wmsg.OrgMsg = wxmsg.Content;
                        Regex regTr = new Regex(@"@(.*?) ");
                        List<string> atlist = new List<string>();
                        MatchCollection mcs = regTr.Matches(wmsg.Msg);
                        for (int mi = 0; mi < mcs.Count; mi++)
                        {
                            atlist.Add(mcs[mi].Value.Trim().Replace("@", ""));
                        }
                        wmsg.AtMemberNikeName = atlist.ToArray();
                        if (wmsg.AtMemberNikeName != null && wmsg.AtMemberNikeName.Length == 1)
                        {
                            var matchnicks = AllUsers[fromName].MemberDict.Where(a => a.Value.DisplayName == wmsg.AtMemberNikeName[0] || a.Value.NickName == wmsg.AtMemberNikeName[0]);
                            foreach (var mem in matchnicks)
                            {
                                if (mem.Value.UserName == client.user.UserName)
                                {
                                    wmsg.IsAtToMe = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        wmsg.FromUserNam = fromName;
                        wmsg.FromNikeName = NickName;
                        wmsg.FromMemberUserName = null;
                        wmsg.FromMemberNikeName = null;
                        wmsg.ToUserName = ToName;
                        wmsg.ToNikeName = ToNameNike;
                        wmsg.CreateTime = wxmsg.CreateTime;
                        wmsg.Msg = wxmsg.Content.Replace("<br/>", "\r\t");
                        wmsg.OrgMsg = wxmsg.Content;
                    }
                    messages.Add(wmsg);
                    //msgs.Add(new ListViewItem(strMsg));
                }
                if (messages.Count > 0)
                {
                     
                    Task.Run(() => {
                        MsgProcessCompleted?.Invoke(this, messages);
                    });
                    Task.Run(() => {
                        InjectBuffs(messages);
                    });
                    //?.Invoke(this, messages);
                    ////if(MsgProcessCompleted_ForExtralInterfaceInvoke!=null)
                    ////{
                    ////    MsgProcessCompleted_ForExtralInterfaceInvoke.ForEach(
                    ////        a => {
                    ////            a.Invoke(this, messages);
                    ////        }
                    ////        );
                    ////}
                }
            }
            catch(Exception ce)
            {
                MessageBox.Show(ce.Message);
            }
            //tbox.Lines = msgs.ToArray(); 
            //tbox.Focus();
            //tbox.Select(tbox.Text.Length, 0);
            //tbox.ScrollToCaret();
            
          
        }



        private static void Client_LogoutComplete(object sender, TEventArgs<User> e)
        {

            Console.WriteLine("已登出");

            Application.Exit();

        }



        private static void Client_MPSubscribeMsgListComplete(object sender, TEventArgs<List<MPSubscribeMsg>> e)

        {

            Console.WriteLine("获取公众号文章，总数：" + e.Result.Count);

        }



        private static void Client_GetContactComplete(object sender, TEventArgs<List<Contact>> e)

        {
            try
            {
                LogableClass.ToLog("获取联系人列表（包括公众号，联系人）", "总数：" + e.Result.Count);
                Console.WriteLine("获取联系人列表（包括公众号，联系人），总数：" + e.Result.Count);

                foreach (var item in e.Result)
                {
                    if (!contactDict.Keys.Contains(item.UserName))
                    {

                        if (item.UserName.StartsWith("@@"))
                        {
                            if (item.MemberList.Count == 0)
                            {

                                string msg = string.Format("{0}获取到的群成员数竟然是0", item.NickName);
                                client.GetBatchGetContactAsync(item.UserName);//如果成员数是0，重新获取
                                continue;
                            }
                            client.GetBatchGetContactAsync(string.Join(",", item.MemberList.Select(a => a.UserName)), item.UserName);

                        }
                        contactDict.Add(item.UserName, item);

                    }




                    //联系人列表中包含联系人，公众号，可以通过参数做区分

                    if (item.VerifyFlag != 0)

                    {

                        //个人号

                    }

                    else

                    {

                        //公众号

                    }

                }

                //如果获取完成

                if (client.IsFinishGetContactList)
                {



                }
                if (DisplayByFrm)
                    frm.RefreshContact(sender, e);
            }
            catch(Exception ce)
            {
                MessageBox.Show(ce.Message);
            }
        }

        

        private static void Client_BatchGetContactComplete(object sender, TEventArgs<List<Contact>> e)
        {
            try
            {
                Console.WriteLine("拉取联系人信息，总数：" + e.Result.Count);
                bool IsRoomId = false;
                string roomid = null;
                if (e.Result.Count == 0)
                    return;
                string msg = null;
                Contact item = e.Result.First();
                if (e.Result.Count == 1 && item.UserName.StartsWith("@@"))
                {
                    msg = string.Format("{0}是老群！", item.NickName);
                    client.GetBatchGetContactAsync(string.Join(",", item.MemberList.Select(a => a.UserName)), item.UserName);
                    if (!contactDict.Keys.Contains(item.UserName))
                    {
                        if (item.MemberCount > 0)
                        {
                            contactDict.Add(item.UserName, item);
                        }
                    }
                    return;
                }
                if (e.Result.Count > 0 && string.IsNullOrEmpty(e.Result.First().EncryChatRoomId) == false)
                {
                    IsRoomId = true;
                    roomid = getRooId(e.Result, contactDict);

                    if (roomid == null && item.UserName.StartsWith("@@"))//可能是群，漏过了
                    {

                        client.GetBatchGetContactAsync(string.Join(",", item.MemberList.Select(a => a.UserName)), item.UserName);
                        return;
                    }
                    else if (roomid == null && !item.UserName.StartsWith("@@"))
                    {
                        msg = string.Format("{0}有群聊id并且是用户类型但是却找不到对应的群！", item.NickName);
                        return;
                    }
                }
                foreach (var vi in e.Result)
                {
                    item = vi;
                    if (IsRoomId && !string.IsNullOrEmpty(roomid))//回传的群成员
                    {
                        //var items = contactDict.Where(a => a.Value.MemberDict.ContainsKey(item.UserName));
                        if (!contactDict.ContainsKey(roomid))
                        {
                            break;
                        }
                        Contact ct = contactDict[roomid];
                        if (!ct.MemberDict.ContainsKey(item.UserName))
                        {
                            continue;
                        }
                        ct.MemberDict[item.UserName].NickName = item.NickName;
                        ct.MemberDict[item.UserName].DisplayName = item.DisplayName;
                        ct.DisplayNikeName = true;
                    }
                    else//第一次回传的
                    {
                        if (!contactDict.Keys.Contains(item.UserName))
                        {
                            contactDict.Add(item.UserName, item);
                            if (item.UserName.StartsWith("@@"))//如果是群，并且
                            {

                                if (item.MemberList.Count == 0)
                                {
                                    msg = string.Format("群{0}成员数竟然是0！", item.NickName);
                                    client.GetBatchGetContactAsync(string.Join(",", item.MemberList.Select(a => a.UserName)), item.UserName);
                                    continue;
                                }
                                //item.DisplayNikeName = true;
                                client.GetBatchGetContactAsync(string.Join(",", item.MemberList.Select(a => a.UserName)), item.UserName);
                            }
                        }
                        else//群内传回来的不处理
                        {
                            ////bool disname = contactDict[item.UserName].DisplayNikeName;
                            ////item.DisplayNikeName = disname;
                            ////contactDict[item.UserName] = item;
                        }
                    }
                }
            }
            catch(Exception ce)
            {
                MessageBox.Show(ce.Message);
            }

        }

        static string getRooId(List<Contact> list,Dictionary<string,Contact> currlist)
        {
            string ret = null;
            try
            {
                lock (currlist)
                {
                    
                    int cnt = list.Count;
                    Dictionary<string, Contact> flt = new Dictionary<string, Contact>();
                    currlist.Values.ToList().ForEach(a =>
                    {
                        if (!a.UserName.StartsWith("@@"))
                        {

                            return;
                        }
                        if (a.MemberList.Count == 0)
                            return;
                        if (a.MemberList.Count >= cnt)
                        {
                            if (!flt.ContainsKey(a.UserName))
                            {
                                flt.Add(a.UserName, a);
                            }
                            return;
                        }

                    }
                    );
                    //Dictionary<string, int> fltcnt = flt.ToDictionary(a => a.Key, a => a.Value.MemberCount);
                    if (flt.Count() == 1)
                    {
                        return flt.First().Value.UserName;
                    }
                    foreach (var a in flt)
                    {
                        int matchcnt = 0;
                        list.ForEach(b =>
                        {
                            if (a.Value.MemberDict.ContainsKey(b.UserName))
                            {
                                matchcnt++;
                            }
                        });
                        if (matchcnt == cnt)
                            return a.Value.UserName;
                    }
                    return ret;
                }
            }
            catch(Exception ce)
            {

            }
            return null;
        }


        private static void Client_LoginComplete(object sender, TEventArgs<User> e)
        {
            try
            {
                string cookie = client.GetLastCookie();
                Console.WriteLine("登陆成功：" + e.Result.NickName);
                Console.WriteLine("========已记录cookie，下次登陆将推送提醒至手机，取代扫码========");
                LogableClass.ToLog("登陆成功：" + e.Result.NickName, "========已记录cookie，下次登陆将推送提醒至手机，取代扫码========");
                using (StreamWriter sw = new StreamWriter(cookiePath, false))
                {
                    sw.WriteLine(cookie);
                }

                qrForm.Invoke(new Action(() =>
                {
                    if (DisplayByFrm)
                    {
                        qrForm.Close();
                    }
                }));
            }
            catch
            {
            }
        }



        private static void Client_CheckScanComplete(object sender, TEventArgs<System.Drawing.Image> e)

        {

            Console.WriteLine("用户已扫码");

            qrForm.SetPic(e.Result);

        }



        private static void Client_GetLoginQrCodeComplete(object sender, TEventArgs<System.Drawing.Image> e)

        {

            Console.WriteLine("已获取登陆二维码");

            qrForm.SetPic(e.Result);

            qrForm.ShowDialog();

        }



        private static void Client_ExceptionCatched(object sender, TEventArgs<Exception> e)

        {

            if (e.Result is GetContactException)

            {

                Console.WriteLine("获取好友列表异常：" + e.Result.ToString());

                return;

            }



            if (e.Result is OperateFailException)

            {

                Console.WriteLine("异步操作异常：" + e.Result.ToString());

                return;

            }



            Console.WriteLine("异常：" + e.Result.ToString());
            ToLog("异常：" + e.Result.ToString());

        }

        public string SendMsg(string str,string ToUser)
        {
            if (Valid)
            {
                string RToUser = ToUser;
                if(!contactDict.ContainsKey(ToUser))
                {

                    List<Contact> res = getContactListByName(ToUser);
                    if (res.Count == 0)
                    {
                        return "不存在该用户！";
                    }
                    if(res.Count >1)
                    {
                        return "存在多个用户！";
                    }
                    RToUser = res[0].UserName;
                }   
                return client.SendMsg(str, RToUser).ToJson();
            }
            else
            {

                return "服务未启动，开始启动服务！123";
            }
        }

        public FileInfo GetImageFromBase64(string base64string)
        {
            string img = base64string;
            var strhead = "data:image/png;base64,";
            if(img.StartsWith(strhead))
            {
                img = base64string.Substring(strhead.Length);
            }
            try
            {
                byte[] b = Convert.FromBase64String(img);
                

                //MemoryStream ms = new MemoryStream(b);
                string ret = Path.GetTempFileName() + ".jpg";
                
                File.WriteAllBytes(ret,b);

                FileInfo fi = new FileInfo(ret);
                return fi;
            }
            catch(Exception ce)
            {

            }
            return null;
        }

        public string SendImgMsg(string strBase64, string ToUser)
        {
            if (Valid)
            {
                string RToUser = ToUser;
                if (!contactDict.ContainsKey(ToUser))
                {

                    List<Contact> res = getContactListByName(ToUser);
                    if (res.Count == 0)
                    {
                        return "不存在该用户！";
                    }
                    if (res.Count > 1)
                    {
                        return "存在多个用户！";
                    }
                    RToUser = res[0].UserName;
                }
                FileInfo strFileName = GetImageFromBase64(strBase64);
                if (strFileName != null)
                    return client.SendMsg(strFileName, RToUser).ToJson();
                else
                    return "无法保存图片";
            }
            else
            {

                return "服务未启动，开始启动服务！123";
            }
        }

        public string SendUrlImgMsg(string url,string ToUser)
        {
            HttpClient hc = new HttpClient();
            try
            {
                Image img = hc.GetImage(url);
                string ret = Path.GetTempFileName() + ".jpg";
                img.Save(ret);
                FileInfo strFileName = new FileInfo(ret);
                if (Valid)
                {
                    string RToUser = ToUser;
                    if (!contactDict.ContainsKey(ToUser))
                    {

                        List<Contact> res = getContactListByName(ToUser);
                        if (res.Count == 0)
                        {
                            return "不存在该用户！";
                        }
                        if (res.Count > 1)
                        {
                            return "存在多个用户！";
                        }
                        RToUser = res[0].UserName;
                    }
                    if (strFileName != null)
                        return client.SendMsg(strFileName, RToUser).ToJson();
                    else
                        return "无法保存图片";
                }
                else
                {

                    return "服务未启动，开始启动服务！123";
                }
            }
            catch(Exception ce)
            {
                return ce.Message;
            }
            
        }

        public void SetDisplayMethod(bool bDisplayByFrm)
        {
            DisplayByFrm = bDisplayByFrm;
        }

        public static List<Contact> getContactListByName(string ToUser)
        {
            List<Contact> ret = new List<Contact>();
            IEnumerable<Contact> eres = WebInterfaceClass.contactDict.Values.Where(a => (a.UserName ==ToUser || a.DisplayName == ToUser || a.NickName == ToUser || a.RemarkName == ToUser));
            if (eres == null)
            {
                return ret;
            }
            return eres.ToList();
        }
        
    }

   
}
