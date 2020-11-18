using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using WolfInv.com.ShareLotteryLib;
using WolfInv.com.WXMessageLib;
using WolfInv.Com.WCS_Process;

public partial class wx : System.Web.UI.Page
{
    static ShareLotteryPlanCollection plancolls;
    const string _token = "HappyShareETech";
    private const string _myOpenid = "wx8e34dcb86f5c900b";
    string postStr = "";

    void resp(string msg)
    {
        Response.Write(msg);
    }

    void Logined(string str)
    {
        //Session.Mode = System.Web.SessionState.SessionStateMode.InProc;
        //Page.EnableViewState = true;
        //enableSessionState = true;
        SendMsg(str, "登录成功！");
        if (string.IsNullOrEmpty( Session["wxid"]?.ToString()))
        {
            
            Session["wxid"] = str;
            
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        wxmessage wx = GetWxMessage();
        string msg = null;
        Response.ContentEncoding = Encoding.UTF8;

        initServerWCS.reponseWrite = resp;
        bool succ = initServerWCS.Init(this, out msg);
        if (!succ)
        {
            Response.Write(msg);
            Response.End();
            return;
        }

        InitColls();
        //************** 验证成为开发者的时候将此代码注释 ***********//
        //对微信的信息进行处理和应用
        
        if(WXOpera(wx)==false)
            Valid();
        //Response.End();
        //***********  验证成为开发者之后将此代码注释 *************//
        //string httpMethod = Request.HttpMethod.ToLower();
        //if (httpMethod == "post")
        //{
        //    //第一次验证的时候开启
        //    FirstValid();
        //}
        //else
        //{
        //    Valid();  //如果不是post请求就去做开发者验证
        //}
    }

    

    void InitColls()
    {
        if (plancolls == null)
        {
            plancolls = new ShareLotteryPlanCollection();
            plancolls.MsgProcess.LoginPress += Logined;
            plancolls.MsgProcess.SendMsg += SendMsg;
            plancolls.MsgProcess.SendImgMsg += SendImgMsg;
            plancolls.MsgProcess.SendUrlImgMsg += SendUrlImgMsg;
            //plancolls.MsgProcess.MsgChanged += refreshMsg;
        }
        //plancolls.MsgProcess.SharePlanChanged += refreshTab;

    }

    string SendMsg(string content,string toUser)
    {
        string res = string.Format(Message_Text
            , toUser,plancolls.MsgProcess.RobotUserName, DateTime.Now.Ticks, content);
        System.Web.HttpContext.Current.Response.Write(res??"");
        //Response.End();
        return res;
    }

    string SendImgMsg(string content, string toUser)
    {
        return "";
    }

    string SendUrlImgMsg(string content, string toUser)
    {
        return "";
    }

    void refreshMsg(string id, wxMessageClass wxmsg)
    {
        List<wxMessageClass> msgs = new List<wxMessageClass>();
        msgs.Add(wxmsg);
        plancolls.MsgProcess.RefreshMsg(null,msgs,true);//disableMutliTask必须要是true,防止多线程
    }

    /// <summary>
    /// 验证成为开发者
    /// </summary>
    private void Valid()
    {
        string echoStr = Request.QueryString["echoStr"]?.ToString();
        if (CheckSignature())
        {
            if (!string.IsNullOrEmpty(echoStr))
            {
                Response.Write(echoStr);
                Response.End();
            }
        }
    }

    /// <summary>
    /// 验证微信签名
    /// </summary>
    /// * 将token、timestamp、nonce三个参数进行字典序排序
    /// * 将三个参数字符串拼接成一个字符串进行sha1加密
    /// * 开发者获得加密后的字符串可与signature对比，标识该请求来源于微信。
    /// <returns></returns>
    private bool CheckSignature()
    {
        string signature = Request.QueryString["signature"]?.ToString();
        string timestamp = Request.QueryString["timestamp"]?.ToString();
        string nonce = Request.QueryString["nonce"]?.ToString();
        string[] ArrTmp = { _token, timestamp, nonce };
        Array.Sort(ArrTmp);     //字典排序
        string tmpStr = string.Join("", ArrTmp);
        tmpStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
        tmpStr = tmpStr.ToLower();
        if (tmpStr == signature)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 第一次验证配置
    /// </summary>
    private void FirstValid(wxmessage wx)
    {
        Stream s = System.Web.HttpContext.Current.Request.InputStream;
        byte[] b = new byte[s.Length];
        s.Read(b, 0, (int)s.Length);
        postStr = Encoding.UTF8.GetString(b);
        if (!string.IsNullOrEmpty(postStr))
        {
            ResponseMsg(wx,postStr);
        }
    }

    /// <summary>
    /// 返回信息结果(微信信息返回)
    /// </summary>
    /// <param name="weixinXML"></param>
    private void ResponseMsg(wxmessage wx,string weixinXML)
    {
        //回复消息的部分:你的代码写在这里
        string content = "欢迎进入武府量化投资公众号！";
        string res = sendTextMessage(wx, content);
        Response.Write(res);
        Response.End();
    }

    /// <summary>
    /// 微信操作
    /// </summary>
    private bool WXOpera(wxmessage wx)
    {
        if (wx == null)
            return false;
        string res = "";
        if(string.IsNullOrEmpty(wx.EventName) && string.IsNullOrEmpty(wx.MsgType))//无事件也无类型
        {
            return false;
        }
        string strMsgType = wx.MsgType?.Trim().ToLower();
        switch(strMsgType)
        {
            case "event":
                {
                    string strEvent = wx.EventName?.Trim().ToLower();
                    switch (strEvent)
                    {
                        case "subscribe":
                            {
                                //刚关注时的时间，用于欢迎词
                                string content = "您好，欢迎关注武府量化投资公众号!";
                                res = sendTextMessage(wx, content);
                                HttpContext.Current.Response.Write(res);
                                HttpContext.Current.Response.End();
                                return true;

                            }
                        case "unsubscribe":
                            {
                                //刚关注时的时间，用于欢迎词
                                string content = "您好，我们正在不断完善公众号内容！期待您能再次关注我们！";
                                res = sendTextMessage(wx, content);
                                HttpContext.Current.Response.Write(res);
                                HttpContext.Current.Response.End();
                                return true;
                            }
                        
                        case "miniprogram":
                            {
                                string content = "您好，我们正在不断完善小程序内容！期待您能再次关注我们！";
                                res = sendTextMessage(wx, content);
                                HttpContext.Current.Response.Write(res);
                                HttpContext.Current.Response.End();
                                return true;
                            }
                        default:
                            {
                                string content = wx.Xml.Replace("]", "-") + "无法识别的事件，我们正在努力完善我们的功能！";
                                res = sendTextMessage(wx, content);
                                HttpContext.Current.Response.Write(res);
                                HttpContext.Current.Response.End();
                                break;
                            }
                    }
                    break;
                }
            case "image":
                {
                    string content = "文字识别功能正在开发中...";
                    res = sendTextMessage(wx, content);
                    HttpContext.Current.Response.Write(res);
                    HttpContext.Current.Response.End();
                    return true;
                }
            case "text":
            default:
                {
                    //SendMsg(wx.Xml, wx.FromUserName);
                    //return true;
                    plancolls.MsgProcess.RobotUnionId = wx.ToUserName;
                    plancolls.MsgProcess.RobotNikeName = "武府小智";
                    plancolls.MsgProcess.RobotUserName = wx.ToUserName;
                    
                    wxMessageClass wmsg = new wxMessageClass();
                    wmsg.Msg = wx.Content;
                    wmsg.OrgMsg = wx.Xml;
                    wmsg.FromUserNam = wx.FromUserName;
                    wmsg.FromNikeName = wx.FromUserName;
                    wmsg.FromMemberUserName = wx.FromUserName;
                    wmsg.FromMemberNikeName = wx.FromUserName;
                    wmsg.ToUserName = wx.ToUserName;
                    wmsg.AtMemberUserName = wx.ToUserName;
                    wmsg.AtMemberNikeName = new string[] { wx.ToUserName };
                    wmsg.IsAtToMe = true;
                    refreshMsg(wx.ToUserName, wmsg);
                    return true;
                    break;
                    {
                        EventPress_Text et = new EventPress_Text(wx, wx.EventKey, wx.Content);
                        string content = et.getResult();
                        HttpContext.Current.Response.Write(content);
                        HttpContext.Current.Response.End();
                        return true;
                    }
                    break;
                }
        }
        return false;
    }
    /// <summary>
    /// 获取和设置微信类中的信息
    /// </summary>
    /// <returns></returns>
    private wxmessage GetWxMessage()
    {
        wxmessage wx = new wxmessage();
        try
        {
            
            StreamReader str = new StreamReader(Request.InputStream, Encoding.UTF8);
            XmlDocument xml = new XmlDocument();
            xml.Load(str);
            str.Close();
            str.Dispose();
            wx.ToUserName = xml.SelectSingleNode("xml").SelectSingleNode("ToUserName").InnerText;
            wx.FromUserName = xml.SelectSingleNode("xml").SelectSingleNode("FromUserName").InnerText;
            wx.MsgType = xml.SelectSingleNode("xml").SelectSingleNode("MsgType").InnerText;
            if (wx.MsgType.Trim().ToLower() == "text")
            {
                wx.Content = xml.SelectSingleNode("xml").SelectSingleNode("Content").InnerText;
            }
            if (wx.MsgType.Trim() == "event")
            {
                wx.EventName = xml.SelectSingleNode("xml").SelectSingleNode("Event").InnerText;
                wx.EventKey = xml.SelectSingleNode("xml").SelectSingleNode("EventKey").InnerText;
            }
            wx.Xml = xml.OuterXml;
            return wx;
        }
        catch(Exception ce)
        {
            SendMsg(string.Format("{0}:{1}", ce.Message, ce.StackTrace), wx.FromUserName);
            return null;
        }
    }
    /// <summary>  
    /// 发送文字消息  
    /// </summary>  
    /// <param name="wx" />获取的收发者信息  
    /// <param name="content" />内容  
    /// <returns></returns>  
    private string sendTextMessage(wxmessage wx, string content)
    {
        string res = string.Format(Message_Text,
             wx.FromUserName, wx.ToUserName, DateTime.Now.Ticks, content);
        return res;
    }
    /// <summary>
    /// 普通文本消息
    /// </summary>
    private static string Message_Text
    {
        get
        {
            return @"<xml><ToUserName><![CDATA[{0}]]></ToUserName><FromUserName><![CDATA[{1}]]></FromUserName><CreateTime>{2}</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[{3}]]></Content></xml>";
        }
    }
}