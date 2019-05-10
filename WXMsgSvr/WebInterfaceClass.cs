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

namespace WolfInv.com.WXMsgCom
{

    [Guid("9E69FAA3-1780-4E1C-9D4B-100BE7CED23B"),
 ClassInterface(ClassInterfaceType.None),
 ComSourceInterfaces(typeof(WebInterface))]
    public class WebInterfaceClass :RemoteServerClass, WebInterface
    {
        static bool IPCCreated;
        public static bool ClientValid = false;
        public  bool Valid { get { return ClientValid; } }
        private static Client client;

        public static Dictionary<string, Contact> contactDict = new Dictionary<string, Contact>();

        private static QrCodeForm qrForm;
        static frm_MainWin frm;
        //private static string cookiePath = AppDomain.CurrentDomain.BaseDirectory + "autoLoginCookie";
        
        static bool DisplayByFrm;
        public WebInterfaceClass()
        {
            if(!IPCCreated)
            {
                this.CreateChannel("wxmsg");
                IPCCreated = true;
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

                client.LoginComplete += Client_LoginComplete; ;

                client.DelContactListComplete += Client_DelContactListComplete; ;

                client.ModContactListComplete += Client_ModContactListComplete;

                client.GetContactComplete += Client_GetContactComplete; ;
                client.BatchGetContactComplete += Client_GetContactComplete;
                client.MPSubscribeMsgListComplete += Client_MPSubscribeMsgListComplete; ;

                client.LogoutComplete += Client_LogoutComplete; ;
                if (DisplayByFrm)
                {
                    frm = new frm_MainWin(client);
                    client.ReceiveMsg += frm.RefreshMsg;
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

            Console.WriteLine("接收修改联系人信息");

            foreach (var item in e.Result)

            {

                contactDict[item.UserName] = item;

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
            LogableClass.ToLog("获取联系人列表（包括公众号，联系人）", "总数：" + e.Result.Count);
            Console.WriteLine("获取联系人列表（包括公众号，联系人），总数：" + e.Result.Count);

            foreach (var item in e.Result)

            {

                if (!contactDict.Keys.Contains(item.UserName))

                {

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
            if(DisplayByFrm)
                frm.RefreshContact(sender, e);

        }



        private static void Client_BatchGetContactComplete(object sender, TEventArgs<List<Contact>> e)

        {

            Console.WriteLine("拉取联系人信息，总数：" + e.Result.Count);

            foreach (var item in e.Result)

            {

                if (!contactDict.Keys.Contains(item.UserName))

                {

                    contactDict.Add(item.UserName, item);

                }

            }

        }



        private static void Client_LoginComplete(object sender, TEventArgs<User> e)
        {
            string cookie = client.GetLastCookie();
            Console.WriteLine("登陆成功：" + e.Result.NickName);
            Console.WriteLine("========已记录cookie，下次登陆将推送提醒至手机，取代扫码========");
            LogableClass.ToLog("登陆成功：" + e.Result.NickName,"========已记录cookie，下次登陆将推送提醒至手机，取代扫码========");
            using (StreamWriter sw = new StreamWriter(cookiePath, false))
            {
                sw.WriteLine(cookie);
            }
            try
            {
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
