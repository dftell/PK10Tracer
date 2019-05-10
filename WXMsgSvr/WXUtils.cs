using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net;
using System.IO;
using System.Xml;
using System.Web.Script;
//using System.Web.Script.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.Timers;

namespace WXMsgSvr
{
    public delegate void DisplayMsgList(NewestMsgClass msg);
    public class WXUtils
    {
        public DisplayMsgList ReceivedMsg;
        public Timer SynckeyTimer;
        public static string UUIDUrlModel = "https://login.weixin.qq.com/jslogin?appid=wx782c26e4c19acffb&redirect_uri=https%3A%2F%.2Fwx.qq.com%2Fcgi-bin%2Fmmwebwx-bin%2Fwebwxnewloginpage&fun=new&lang=zh_CN&_={0}";
        public static string QCUrlModel = "https://login.weixin.qq.com/qrcode/{0}";
        public static string LoginUrlModel = "https://login.weixin.qq.com/cgi-bin/mmwebwx-bin/login?tip={0}&uuid={1}&_={2}";
        public static string PostUrlModel = "https://wx{0}.qq.com/cgi-bin/mmwebwx-bin/webwxinit?r={1}&lang=zh_CN&pass_ticket={2}";
        public static string ContactUrlModel = "https://wx{0}.qq.com/cgi-bin/mmwebwx-bin/webwxgetcontact?pass_ticket={1}&skey={2}";
        public static string JsonReqInfo = "{\"BaseRequest\":{\"Uin\":\"{0}\",\"Sid\":\"{1}\",\"Skey\":\"{2}\",\"DeviceID\":\"{3}\"}}";
        public int CheckLoginTip { get; set; }
        public string uuid { get; set; }
        public string wxsid { get; set; }
        public string skey { get; set; }
        public string pass_ticket{get;set;}
        public string Devid { get; set; }
        public bool HasNewMsg = false;
        public string wxuin { get; set; }
        public string Cookie { get; set; }
        long Stamp { get; set; }
        public string strHostFlg = "";
        public InitRequestClass baseReqObj { get; set; }
        //public string BaseRequest { get; set; }
        public string CurrResponse;
        WebClient wc = new WebClient();
        HttpClient hc = new HttpClient();
        public string LoginStatus { get; set; }
        public string DefaultParams { get; set; }
        public string webwx_data_ticket { get; set; }
        public string webwx_auth_ticket { get; set; }
        public WXUserInfo CurrUser { get; set; }

        public string NewestSyncCheckKey { get; set; }
        public SyncKeys syncObj { get; set; }
        public List<NewestMsgClass> MsgList;
        

        public WXContacter[] AllContacters { get; set; }


        public bool Logined;
        public bool Inited;
        public bool ShowImg;
        public bool ContactUpdated;
        public long CurrStamp
        {
            get
            {
                return GetTimeStamp();
            }
        }
        Action<Image> DisplayQRCodeFunc;
        Action<string, string, string> MsgNoticeFunc;
        public WXUtils(Action<Image> QRCodeFunc,Action<string,string,string> MsgFunc)
        {
            
            DisplayQRCodeFunc = QRCodeFunc;
            MsgNoticeFunc = MsgFunc;
            SynckeyTimer = new Timer();//在登录前为二维码刷新计时器，登录后为心跳包计时器
            SynckeyTimer.Interval = 60 * 1000;//二维码15秒钟更新一次，心跳包也使用此频率，也可以更改
            SynckeyTimer.AutoReset = true;
            SynckeyTimer.Elapsed += SynckeyTimer_Elapsed;
            SynckeyTimer.Enabled = true;
            SynckeyTimer_Elapsed(null, null);
        }

        private void SynckeyTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(!Logined)//展示二维码
            {
                if (ShowImg)
                    return;
                DisplayQRCodeFunc(getWx_Code());
            }
            else
            {
                if (!Inited)
                    return;
                synCheckGet(MsgNoticeFunc);
                NewestMsgClass ret = getCurrentMsgData(MsgNoticeFunc);
                if(ret != null)
                {
                    ReceivedMsg(ret);
                }

            }
        }

        public Image getWx_Code()
        {
            ShowImg = true;
            this.uuid = getUUID();
            string strTmp = Path.GetTempFileName();
            if(!getQR_Code(this.uuid, strTmp))
            {
                ShowImg = false;
                return null;
            }
            this.CheckLoginTip = 1;
            ShowImg = false;
            return Image.FromFile(strTmp);
        }
        //Function getUUID() As String
        public string getUUID()
        {
            this.Stamp = GetTimeStamp();
            string UUIDUrl = string.Format(UUIDUrlModel, this.Stamp);
            string strReq = null;
            try
            {
                strReq = wc.DownloadString(UUIDUrl);
            }
            catch(Exception ce)
            {
                return null;
            }
            if (strReq == null)
                return null;
            string[] Arr = strReq.Split(';');
            if (Arr.Length < 2)
                return null;
            string[] retArr = Arr[1].Split('"');
            if (retArr.Length < 2)
                return null;
            return retArr[1];
            
        }

        long GetTimeStamp()
        {
            DateTime starttime = new DateTime(1970, 1, 1);
            return (long)DateTime.Now.Subtract(starttime).TotalSeconds;
        }
        public bool getQR_Code(string uuid,string ImgPath)
        {
            if (uuid == null)
                return false;
            string QCUrl = string.Format(QCUrlModel, uuid);
            byte[] reqdata;
            try
            {
                reqdata = wc.DownloadData(QCUrl);
                FileStream fs = File.OpenWrite(ImgPath);
                fs.Write(reqdata, 0, reqdata.Length);
                fs.Flush();
                fs.Close();
                return true;
            }
            catch(Exception ce)
            {
                return false;
            }
        }

        public bool Login(out string strReURL,Action<string,string,string> MsgProc)
        {
            string url = string.Format(LoginUrlModel, CheckLoginTip, uuid,this.Stamp);
            strReURL = url;
            try
            {
                string strRet = wc.DownloadString(url);
                CurrResponse = strRet;//调试监控而已，无实际用途;
                string[] retArr = strRet.Split(';');
                string strStatus = retArr[0];
                string[] FlgArr = strStatus.Split('=');
                if (FlgArr.Length < 2)
                    return false;
                string strFlg = FlgArr[1];
                if(strFlg != "200")
                {
                    if(strFlg.Equals("408"))
                    {
                        LoginStatus = "未扫描";
                        CheckLoginTip = 1;
                    }
                    if(strFlg.Equals("201"))
                    {
                        LoginStatus = "未登录";
                        CheckLoginTip = 0;
                    }
                    MsgProc(LoginStatus, CheckLoginTip.ToString(), CurrResponse);
                    return false;
                }
                CheckLoginTip = 2;
                strReURL = retArr[1].Split('"')[1];
                DefaultParams = strRet;
                LoginStatus = "已登录";
                if (strReURL.IndexOf("wx2.")>0)
                {
                    strHostFlg = "2";
                }
                Logined = true;
                if(!this.Init(strReURL, MsgProc))
                {
                    Logined = false;
                    return false;
                }
                Inited = true;
            }
            catch(Exception ce)
            {
                MsgProc("错误", CheckLoginTip.ToString(), string.Format("{0}:{1}",ce.Message,ce.StackTrace));
                return false;
            }
            SynckeyTimer.Interval = 30 * 1000;
            MsgProc(LoginStatus, CheckLoginTip.ToString(), CurrResponse);
            return true;
            ////        Dim strurl As String
            ////'Sheet10.Cells(3, 11) = ""
            ////'Sheet10.Cells(4, 10) = Now()
            ////'iCheckLoginTip = Sheet10.Cells(4, 6).Text
            ////'uuid = Sheet10.Cells(4, 7).Text
            ////'strTimeStamp = Sheet10.Cells(4, 8).Text
            ////strurl = "https://login.weixin.qq.com/cgi-bin/mmwebwx-bin/login?tip=" & iCheckLoginTip & "&uuid=" & uuid + "&_=" & strTimeStamp
            ////'Sheet10.Cells(1, 5) = strUrl
            ////Dim httpreq As New XMLHTTP60
            ////httpreq.Open "get", strurl, False
            ////httpreq.setRequestHeader "contentType", "text/html;charset=UTF-8"
            ////httpreq.send
            ////'strheader = HttpReq.getAllResponseHeaders()
            ////strheader = httpreq.getResponseHeader("Content-Type")
            ////Dim ImgLen As Long
            ////ImgLen = httpreq.getResponseHeader("Content-Length")
            ////If httpreq.Status <> 200 Then
            ////    'MsgBox HttpReq.Status
            ////    Exit Function
            ////End If
            ////'MsgBox HttpReq.responseBody
            ////Dim strHtml As String
            ////strHtml = httpreq.responseText
            ////'Sheet10.Cells(2, 11) = strHtml
            ////Dim strflg As String
            ////Dim strstatus As String
            ////Dim strReDirUrl As String
            ////strstatus = Split(strHtml, ";")(0)
            ////strflg = Split(strstatus, "=")(1)
            ////'Sheet10.Cells(2, 11) = strflg
            ////'strLoginFlg = strflg
            ////If strflg<> "200" Then
            ////   If strflg = "408" Then
            ////        'Sheet10.Cells(3, 11) = "未扫描"
            ////        iCheckLoginTip = 1
            ////    End If
            ////    If strflg = "201" Then
            ////        'Sheet10.Cells(3, 11) = "未登陆"
            ////        iCheckLoginTip = 0
            ////    End If
            ////    'Sheet10.Cells(4, 6) = iCheckLoginTip
            ////    'strLoginStatus = iCheckLoginTip
            ////    Exit Function
            ////End If
            ////'Sheet10.Cells(5, 1) = ""
            ////strReDirUrl = Split(strHtml, ";")(1)
            ////strReURL = strReDirUrl
            ////If InStr(strReDirUrl, "wx2.") > 1 Then
            ////    'Sheet10.Cells(5, 1) = "2"
            ////    strHostFlg = "2"
            ////End If
            ////'strHostFlg = Sheet10.Cells(5, 1)
            ////strUserUrl = Split(strReDirUrl, """")(1)
            ////strReURL = strUserUrl
            ////url_Params = strUserUrl
            ////'Sheet10.Cells(3, 11) = "已登陆"
            ////'Sheet10.Cells(2, 5) = strUserUrl
            ////wait_login = True
        }

        public bool Init(string url,Action<string,string,string> NoticeFunc)
        {

            string cookie = GetCookie(url, NoticeFunc);
            if(cookie == null)
            {
                NoticeFunc("初始化微信","获取基本信息","获取Cookie失败！");
                return false;
            }
            NoticeFunc("初始化微信", "获取基本信息", "成功！");
            this.Cookie = cookie;
            if(!InitConnectInfo(NoticeFunc))
            {
                NoticeFunc("初始化微信", "获取用户数据", "初始化连接信息失败！");
                return false;
            }
            NoticeFunc("初始化微信", "获取用户数据", "成功！");
            if(getContactList(NoticeFunc) == false)
            {
                NoticeFunc("初始化微信", "获取联系人列表", "失败！");
                return false;
            }
            ContactUpdated = true;
            ////NewestMsgClass nmsg = getCurrentMsgData(NoticeFunc);
            ////HasNewMsg = true;
            ////ReceivedMsg(nmsg);
            Inited = true;
            return true;
            ////            Sub init(strurl As String, sB As StatusBar)
            ////    On Error Resume Next
            ////    sB.Panels(2).Text = "获取Cookie。。。"
            ////    If Not getCookie(strurl) Then
            ////        MsgBox "无法获得基本信息！"
            ////        Exit Sub
            ////    End If
            ////    sB.Panels(2).Text = "初始化用户信息。。。"
            ////    If Not initSelf() Then
            ////        MsgBox "无法初始化！"
            ////        Exit Sub
            ////    End If
            ////    sB.Panels(2).Text = "获取联系人信息。。。"
            ////    If Not getContactList() Then
            ////        MsgBox "无法获取联系人列表！"
            ////        Exit Sub
            ////    End If
            ////    ListContactor getMyAllContactors()
            ////    'BindToDDL
            ////    'getMsgData
            ////End Sub
        }

        public bool InitConnectInfo(Action<string,string,string> NoticeFunc)
        {
            try
            {
                string Devid = GetDevId() ;
                this.Devid = Devid;
                string strPostData = JsonReqInfo.Replace("{0}",wxuin).Replace("{1}", wxsid).Replace("{2}", skey).Replace("{3}", Devid);
                wc.Headers.Add("Content-Type", "application/json; charset=UTF-8");
                wc.Headers.Add("Cookie", this.Cookie);
                string postUrl = string.Format(PostUrlModel, strHostFlg, GetTimeStamp(), pass_ticket);
                byte[] res = wc.UploadData(postUrl, "Post", Encoding.UTF8.GetBytes(strPostData));
                string strRes = System.Text.Encoding.UTF8.GetString(res);
                JavaScriptSerializer js = new JavaScriptSerializer();
                WXUserInfo wxui = js.Deserialize<WXUserInfo>(strRes);
                if (wxui != null)
                    CurrUser = wxui;

                //this.BaseRequest = strPostData;
                this.baseReqObj = new InitRequestClass().LoadFromJson<InitRequestClass>(strPostData);

                //this.syncCheckKey = wxui.SyncKey.ToJson();
                this.NewestSyncCheckKey = wxui.SyncKey.ToString();
                this.syncObj = wxui.SyncKey;
                return true;
            }
            catch(Exception ce)
            {
                NoticeFunc("发送请求出现错误！", ce.Message, ce.StackTrace);
                return false;
            }
            ////            Function initSelf() As Boolean
            ////    On Error Resume Next
            ////    Dim postUrl As String
            ////''    strStamp = Sheet10.Cells(4, 8)
            ////''    strHostFlg = Sheet10.Cells(5, 1)
            ////    postUrl = "https://wx" & strHostFlg & ".qq.com/cgi-bin/mmwebwx-bin/webwxinit?r=" & getTimeStap() & "&lang=zh_CN&pass_ticket=" & pass_ticket
            ////    '{"BaseRequest":{"Uin":"UUUU","Sid":"SSSS","Skey":"KKKK","DeviceID":"e123456789012345"}}
            ////    Dim strModel As String
            ////    Dim strPost As String
            ////    strModel = "{""BaseRequest"":{""Uin"":""UUUU"",""Sid"":""SSSS"",""Skey"":""KKKK"",""DeviceID"":""eDDDD""}}"
            ////    strPost = Replace(strModel, "UUUU", wxuin)
            ////    strPost = Replace(strPost, "SSSS", wxsid)
            ////    strPost = Replace(strPost, "KKKK", skey)
            ////    strPost = Replace(strPost, "DDDD", Right("00000000000" & strStamp, 15))
            ////    paramData = strPost
            ////    Dim httpreq As New XMLHTTP60
            ////    Dim strm As New Stream
            ////    httpreq.Open "post", postUrl, False
            ////    httpreq.setRequestHeader "contentType", "application/json; charset=UTF-8"
            ////    httpreq.setRequestHeader "Cookie", cookie
            ////    httpreq.send strPost
            ////    If httpreq.Status <> 200 Then
            ////        Exit Function
            ////    End If
            ////    Dim strRet As String
            ////    strRet = httpreq.responseText
            ////    If Err.Number <> 0 Then
            ////        MsgBox Err.Description
            ////        Err.Clear
            ////    End If
            ////    Dim js As New JsonClass
            ////    Set globeobj = js.GetJsonVal(strRet, "")
            ////    Dim lpos As Long
            ////    lpos = InStr(strRet, "SyncKey")
            ////    If lpos > 0 Then
            ////        Dim strkey As String
            ////        Dim sUser As String
            ////        strkey = Right(strRet, Len(strRet) - lpos)
            ////        DecodeSyncKey(strkey)
            ////        lpos = InStr(strkey, """User"": {")
            ////        sUser = Right(strkey, Len(strkey) - lpos)


            ////        Set SelfBean = New WXUser
            ////        SelfBean.LoadDataBuyJson globeobj.User
            ////'''        Sheet10.Cells(9, 12) = Trim(SelfBean.JsonObj.UserName)
            ////'''        Sheet10.Cells(9, 13) = Trim(SelfBean.JsonObj.NickName)
            ////    End If
            ////    'Sheet10.Cells(3, 12) = strPost
            ////    'Sheet10.Cells(4, 12) = strRet
            ////    'Sheet10.Cells(2, 11) = postUrl
            ////    'Sheet10.Cells(3, 10) = Cookie
            ////    'Sheet10.Cells(3, 9) = SyncKey
            ////    initSelf = True
            ////End Function
        }
        public string GetCookie(string ConnectUrl,Action<string,string,string> NoticeFunc)
        {
            string strUrl = ConnectUrl + "&fun=new";
            try
            {
                WebRequest wr = WebRequest.Create(strUrl);
                //string ret = wc.DownloadString(strUrl);
                WebResponse webresp = wr.GetResponse();
                StreamReader sr = new StreamReader(webresp.GetResponseStream());
                string ret = sr.ReadToEnd();
                sr.Close();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(ret);
                XmlNode node = doc.SelectSingleNode("/error/ret");
                if(node == null || node.InnerText== "1")
                {
                    NoticeFunc("获取Cookie", "重链接回复错误", node?.InnerText);
                    return null;
                }
                XmlNodeList nodes = doc.SelectNodes("/error/*");
                this.wxsid = doc.SelectSingleNode("/error/wxsid").InnerText;
                this.skey = doc.SelectSingleNode("/error/skey").InnerText;
                this.wxuin = doc.SelectSingleNode("/error/wxuin").InnerText;
                this.pass_ticket = doc.SelectSingleNode("/error/pass_ticket").InnerText;
                string strCookie = webresp.Headers["Set-Cookie"];
                string[] cookieArr = strCookie.Split(';');
                for (int i = 0; i < cookieArr.Length; i++)
                { 
                    
                    if(cookieArr[i].IndexOf("webwx_data_ticket")>=0)
                        this.webwx_data_ticket = cookieArr[i];
                    if (cookieArr[i].IndexOf("webwx_auth_ticket") >= 0)
                        this.webwx_auth_ticket = cookieArr[i];
                }
                webresp.Close();
                return strCookie;
            }
            catch
            {
                return null;
            }
            
            ////            Function getCookie(strurl As String) As Boolean
            ////    On Error Resume Next
            ////    'Dim strUrl As String
            ////    strurl = strUserUrl & "&fun=new"
            ////    Dim httpreq As New XMLHTTP60
            ////    httpreq.Open "get", strurl, False
            ////    httpreq.setRequestHeader "contentType", "text/html;charset=UTF-8"
            ////    httpreq.send
            ////    Dim strCookie As String
            ////    If httpreq.Status <> 200 Then
            ////        MsgBox httpreq.Status
            ////        Exit Function
            ////    End If
            ////    Dim xmldoc As New DOMDocument60
            ////    xmldoc.LoadXML httpreq.responseText
            ////    Dim xmlnodes As IXMLDOMNodeList
            ////    Dim retnode As IXMLDOMNode
            ////    Set retnode = xmldoc.SelectSingleNode("/error/ret")
            ////    If retnode Is Nothing Or retnode.Text = 1 Then
            ////        MsgBox "重链接回复错误！"
            ////        Exit Function
            ////    End If
            ////    Set xmlnodes = xmldoc.SelectNodes("/error/*")
            ////    skey = xmlnodes(2).Text
            ////    wxsid = xmlnodes(3).Text
            ////    wxuin = xmlnodes(4).Text
            ////'''    For I = 1 To 4
            ////'''        Sheet10.Cells(1, 11 + I) = xmlnodes(I + 1).nodeName
            ////'''        Sheet10.Cells(2, 11 + I) = xmlnodes(I + 1).Text
            ////'''    Next
            ////    pass_ticket = xmlnodes(5).Text
            ////    strCookie = httpreq.getResponseHeader("Set-Cookie")
            ////    Dim cookieArr() As String
            ////    cookieArr = Split(strCookie, ";")
            ////'    Sheet10.Cells(3, 11) = strCookie
            ////    scookie = scookie & "wxpluginkey=" & getTimeStap() + "; "
            ////    scookie = scookie & cookieArr(0) & ";"
            ////    scookie = scookie & "wxsid=" & wxsid & ";"
            ////    scookie = scookie & "mm_lang=zh_CN; MM_WX_NOTIFY_STATE=1; MM_WX_SOUND_STATE=1;"
            ////    cookie = scookie
            ////'    Sheet10.Cells(1, 1) = Cookie
            ////    getCookie = True
            ////End Function
        }

        public string GetNewCookie()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("wxpluginkey={0};", this.CurrStamp));
            sb.Append(string.Format("{0};{1};", webwx_data_ticket, webwx_auth_ticket));
            sb.Append(string.Format("wxsid={0};", this.wxsid));
            sb.Append("mm_lang=zh_CN; MM_WX_NOTIFY_STATE=1; MM_WX_SOUND_STATE=1;");
            return sb.ToString();
        }
        public bool getContactList(Action<string, string, string> NoticeFunc)
        {
            try
            {
                string url = string.Format(ContactUrlModel, strHostFlg,this.pass_ticket,this.skey);
                string strPostData = JsonReqInfo.Replace("{0}", wxuin).Replace("{1}", wxsid).Replace("{2}", skey).Replace("{3}", "123456789012345");

                wc.Headers.Add("Content-Type", "application/json; charset=UTF-8");
                wc.Headers.Add("Cookie", GetNewCookie());
                byte[] retBys = wc.UploadData(url,Encoding.UTF8.GetBytes(this.baseReqObj.ToJson()));
                string strRet = Encoding.UTF8.GetString(retBys);
                string tmpCookie = wc.Headers["Cookie"];
                this.Cookie = tmpCookie;
                JavaScriptSerializer js = new JavaScriptSerializer();
                ContactListOfUser retobj = js.Deserialize<ContactListOfUser>(strRet);
                AllContacters = retobj.MemberList;
            }
            catch(Exception e)
            {
                NoticeFunc("获取联系列表错误", e.Message, e.StackTrace);
                return false;
            }
            ////        postUrl = "https://wx" & strHostFlg & ".qq.com/cgi-bin/mmwebwx-bin/webwxgetcontact"
            ////'{"BaseRequest":{"Uin":"UUUU","Sid":"SSSS","Skey":"KKKK","DeviceID":"e123456789012345"}}
            ////paramData = strPost
            ////Dim httpreq As New XMLHTTP60
            ////httpreq.Open "post", postUrl, False
            ////httpreq.setRequestHeader "contentType", "application/json; charset=UTF-8"
            ////httpreq.setRequestHeader "Cookie", cookie
            ////httpreq.send paramData
            ////Dim ContacterList As Dictionary
            ////If httpreq.Status <> 200 Then
            ////    Exit Function
            ////End If
            ////Dim wxc As New WXContactor
            ////Set FriendList = wxc.LoadListByText(httpreq.responseText)
            ////getContactList = True
            
            return true;
        }

        public string GetDevId()
        {
            int val = new Random().Next();
            return "e" + val.ToString().PadLeft(15,'0');
        }

        public static string SyncUrlModel= "https://webpush.wx{0}.qq.com/cgi-bin/mmwebwx-bin/synccheck{1}";
        ////// 参数          示例值        说明
        //////deviceid    |e547171618594402        //////参考5中的生成方式
        //////sid         |+FhlgkGS3wD/GKQw        //////公参中的值
        //////skey        |/@crypt_8b4f09cc_1b827f84b1535b6be801f00427499050        //////公参中的值
        //////synckey     |1_700722177|2_700724323|3_700724315|1000_1520925834        //////微信初始化后获取的4个key，这些key会随着每次获取最新消息（参见9）后的返回值更新，其目的在于每次同步消息后记录一个当前同步的状态。
        //////uin         |211722515        //////公参中的值
        public long synCheckGet(Action<string, string, string> NoticeFunc)
        {
            HasNewMsg = false;
            string reqParamsModel = "?deviceid={0}&sid={1}&skey={2}&synckey={3}&uin={4}";
            string url = string.Format(SyncUrlModel,strHostFlg,string.Format(reqParamsModel, this.Devid,this.wxsid,this.skey, this.NewestSyncCheckKey, this.wxuin));
            try
            {

                wc.Headers.Add("Content-Type", "application/json; charset=UTF-8");
                wc.Headers.Add("Cookie", GetNewCookie());
                byte[] retArr = wc.UploadData(url,"Post",new byte[0] { });
                string strRet = Encoding.UTF8.GetString(retArr);
                JavaScriptSerializer js = new JavaScriptSerializer();
                synccheckResult retobj = js.Deserialize<synccheckResult>(strRet.Replace("window.synccheck=", ""));
                string msg = null;
                if(!retobj.Success(out msg))
                {
                    NoticeFunc("心跳检查", "获取心跳包错误", msg);
                    return -1;
                }
                if(retobj.SelStatus() == SelectorStatus.New_Msg)
                {
                    HasNewMsg = true ;
                }
                return 1;
            }
            catch(Exception ce)
            {
                return -1;
            }
            return 1;
            ////            Function synCheckGet() As Integer
            ////    On Error Resume Next
            ////    strStamp = getTimeStap()
            ////    skey = Sheet10.cells(2, 12).Text
            ////    wxsid = Sheet10.cells(2, 13).Text
            ////    wxuin = Sheet10.cells(2, 14).Text
            ////    pass_ticket = Sheet10.cells(2, 15).Text
            ////    SyncKey = Sheet10.cells(1, 8)
            ////    syncCheckKey = Sheet10.cells(2, 8)
            ////    strHostFlg = Sheet10.cells(5, 1)
            ////    Dim url As String
            ////    url = "https://webpush.wx" & strHostFlg & ".qq.com/cgi-bin/mmwebwx-bin/synccheck?r=" & strStamp & "&skey=" & skey & "&sid=" & wxsid & "&uin=" & wxuin & "&deviceid=e" & Right("000000000000" & getTimeStap(), 15) & "&synckey=" + syncCheckKey + "&_=" + getTimeStap()
            ////    Dim httpreq As New XMLHTTP60
            ////    httpreq.Open "post", url, False
            ////    httpreq.setRequestHeader "contentType", "text/html;charset=UTF-8"
            ////    httpreq.setRequestHeader "Cookie", cookie
            ////    httpreq.send
            ////    If httpreq.Status <> 200 Then
            ////        synCheckGet = -1
            ////        Exit Function
            ////    End If
            ////    Dim js As New JsonClass
            ////    Dim retobj As Object
            ////    Dim strRet As String
            ////    Dim strArr() As String
            ////    strRet = httpreq.responseText
            ////    strArr = Split(strRet, "=")
            ////    Set retobj = js.GetJsonVal(strArr(1), "")
            ////    If retobj.retcode <> "0" Then
            ////'        synCheckGet = -1
            ////'        MsgBox "心跳包错误！[" & strRet & "]" & Url
            ////'        StopTimerGetMsg
            ////'        refreshCode
            ////        Exit Function
            ////    End If
            ////    If retobj.selector <> 2 And retobj.selector <> 6 Then
            ////        synCheckGet = -1
            ////        'MsgBox "无消息！"
            ////        Exit Function
            ////    End If
            ////    synCheckGet = 1
            ////End Function
        }

        public bool SendMsg(String FromUserName,String ToUserName, String Content)
        {
            return true;
            ////        Function SendMsg(FromUserName As String, ToUserName As String, Content As String) As Boolean
            ////    On Error Resume Next
            ////    If Len(Trim(FromUserName)) = 0 Or Len(Trim(ToUserName)) = 0 Then Exit Function
            ////    If Len(Trim(Content)) = 0 Then
            ////        MsgBox " 内容不能为空!"
            ////        Exit Function
            ////    End If
            ////    Dim postUrl As String
            ////    Dim strModel As String
            ////    Dim strPost As String
            ////    strStamp = getTimeStap()
            ////    skey = Sheet10.cells(2, 12).Text
            ////    wxsid = Sheet10.cells(2, 13).Text
            ////    wxuin = Sheet10.cells(2, 14).Text
            ////    pass_ticket = Sheet10.cells(2, 15).Text
            ////    SyncKey = Sheet10.cells(1, 8).Text
            ////    syncCheckKey = Sheet10.cells(2, 8).Text
            ////    cookie = Sheet10.cells(1, 1).Text
            ////    strHostFlg = Sheet10.cells(5, 1)
            ////    postUrl = "https://wx" & strHostFlg & ".qq.com/cgi-bin/mmwebwx-bin/webwxsendmsg?pass_ticket=" & pass_ticket
            ////    strModel = "{""BaseRequest"":{""Uin"":""UUUU"",""Sid"":""SSSS"",""Skey"":""KKKK"",""DeviceID"":""eDDDD""}," _
            ////    & """Msg"":{""Type"":1,""Content"":""{Content}"",""FromUserName"":""{FromUserName}"",""ToUserName"":""{toUserName}""," _
            ////    & """LocalID"":""TTTT"",""ClientMsgId"":""TTTT""},""Scene"":0,""rr"":TTTT}"
            ////    strPost = Replace(strModel, "UUUU", wxuin)
            ////    strPost = Replace(strPost, "SSSS", wxsid)
            ////    strPost = Replace(strPost, "KKKK", skey)
            ////    strPost = Replace(strPost, "DDDD", Right("111111111111111" & strStamp, 15))
            ////    strPost = Replace(strPost, "TTTT", strStamp)
            ////    strPost = Replace(strPost, "{FromUserName}", FromUserName)
            ////    strPost = Replace(strPost, "{Content}", Content)
            ////    strPost = Replace(strPost, "{toUserName}", ToUserName)
            ////    Sheet10.cells(11, 12) = strPost
            ////    tempData = "refreshTimes=5; login_frequency=2; " & cookie
            ////    Dim jc As New JsonClass
            ////    Set testobj = jc.GetJsonVal(strPost, "")
            ////    Dim httpreq As New XMLHTTP60
            ////    httpreq.Open "post", postUrl, False
            ////    httpreq.setRequestHeader "contentType", "application/json; charset=UTF-8"
            ////    httpreq.setRequestHeader "Cookie", tempData
            ////    httpreq.send strPost
            ////    If httpreq.Status<> 200 Then
            ////        ReDim ret(0)
            ////        Exit Function
            ////    End If
            ////    Dim strRet As String
            ////    strRet = httpreq.responseText
            ////    Set obj = jc.GetJsonVal(strRet, "")
            ////    If obj.BaseResponse.ret = 0 Then
            ////        SendMsg = True
            ////    End If
            ////End Function
        }

        public static string NewestMsgUrlModel = "https://wx{0}.qq.com/cgi-bin/mmwebwx-bin/webwxsync?sid={1}&skey={2}&uin={3}";
        public NewestMsgClass getCurrentMsgData(Action<string, string, string> NoticeFunc)
        {
            if(!HasNewMsg)
            {
                return null;
            }
            string url = string.Format(NewestMsgUrlModel, strHostFlg, this.wxsid, this.skey, this.wxuin);
            NewMsgRequest nmr = new NewMsgRequest();
            nmr.BaseRequest = this.baseReqObj.BaseRequest;
            nmr.SyncKey = this.syncObj;// new SyncKeys().LoadFromJson<SyncKeys>(this.syncCheckKey);
            nmr.rr = -1*this.GetTimeStamp();
            try
            {
                wc.Headers.Add("Content-Type", "application/json; charset=UTF-8");
                wc.Headers.Add("Cookie", this.Cookie);
                byte[] byteRet = wc.UploadData(url, Encoding.UTF8.GetBytes(nmr.ToJson()));
                string strRet = Encoding.UTF8.GetString(byteRet);
                NewestMsgClass retobj = new NewestMsgClass().LoadFromJson<NewestMsgClass>(strRet);
                string msg = null;
                if (retobj.SyncCheckKey!= null)
                {
                    //this.syncCheckKey = retobj.SyncCheckKey.ToJson();
                    this.syncObj = retobj.SyncCheckKey;
                    this.NewestSyncCheckKey = syncObj.ToString();
                }
                return retobj;
            }
            catch (Exception ce)
            {
                NoticeFunc("获取最新消息", ce.Message, ce.StackTrace);
                return null;
            }
            //////            Function getCurrentMsgData() As WXMsg()
            //////    Dim postUrl As String
            //////    Dim ret() As WXMsg
            //////    strHostFlg = Sheet10.cells(5, 1)
            //////    postUrl = "https://wx" & strHostFlg & ".qq.com/cgi-bin/mmwebwx-bin/webwxsync?sid=" & wxsid & "&skey=" & skey
            //////    Dim strModel As String
            //////    Dim strPost As String
            //////    strStamp = getTimeStap()
            //////    skey = Sheet10.cells(2, 12).Text
            //////    wxsid = Sheet10.cells(2, 13).Text
            //////    wxuin = Sheet10.cells(2, 14).Text
            //////    pass_ticket = Sheet10.cells(2, 15).Text
            //////    SyncKey = Sheet10.cells(1, 8)
            //////    syncCheckKey = Sheet10.cells(2, 8)
            //////    cookie = Sheet10.cells(1, 1).Text
            //////    strModel = "{""BaseRequest"":{""Uin"":""UUUU"",""Sid"":""SSSS"",""Skey"":""KKKK"",""DeviceID"":""eDDDD""},""SyncKey"":{NNNN},""rr"":RRRR}"
            //////    strPost = Replace(strModel, "UUUU", wxuin)
            //////    strPost = Replace(strPost, "SSSS", wxsid)
            //////    strPost = Replace(strPost, "KKKK", skey)
            //////    strPost = Replace(strPost, "DDDD", Right("00000000000" & strStamp, 15))
            //////    strPost = Replace(strPost, "NNNN", SyncKey)
            //////    strPost = Replace(strPost, "RRRR", strStamp)
            //////    Sheet10.cells(8, 11) = strPost
            //////    'paramData = strPost
            //////    Dim jc As New JsonClass
            //////    Set obj = jc.GetJsonVal(strPost, "")
            //////    If obj Is Nothing Then
            //////        MsgBox "构建请求错误！" & strPost
            //////       ' Exit Function
            //////    End If
            //////    Dim httpreq As New XMLHTTP60
            //////    httpreq.Open "post", postUrl, False
            //////    httpreq.setRequestHeader "contentType", "application/json; charset=UTF-8"
            //////    httpreq.setRequestHeader "Cookie", cookie
            //////    httpreq.send strPost
            //////    If httpreq.Status <> 200 Then
            //////        ReDim ret(0)
            //////        getCurrentMsgData = ret
            //////        Exit Function
            //////    End If
            //////    Dim Msg As New WXMsg

            //////    Set obj = jc.GetJsonVal(httpreq.responseText, "")
            //////    If obj.BaseResponse.ret <> 0 Then
            //////        MsgBox obj.retcode
            //////        Exit Function
            //////    End If
            //////    getCurrentMsgData = Msg.LoadMsgsByText(httpreq.responseText)
            //////End Function
        }

    }

    public class BaseRequestClass:ToJsonClass
    {
        public long Uin { get; set; }
        public string Sid { get; set; }
        public string skey { get; set; }
        public string DeviceID { get; set; }
    }

    public class NewMsgRequest:ToJsonClass
    {
        public BaseRequestClass BaseRequest { get; set; }
        public SyncKeys SyncKey { get; set; }
        public long rr { get; set; }
    }

    public class InitRequestClass:ToJsonClass
    {
        public BaseRequestClass BaseRequest { get; set; }
    }

    public class ToJsonClass
    {
        public string ToJson()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(this);
        }

        public T LoadFromJson<T>(string strjs)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<T>(strjs);
        }
    }
}
