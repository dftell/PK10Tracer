using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// EventPressClass 的摘要说明
/// </summary>
public abstract class EventPressClass
{
    protected wxmessage wxobj;
    string strEvent;
    string strKey;
    public EventPressClass(wxmessage wxo,string evt,string key)
    {
        wxobj = wxo;
        strEvent = evt;
        strKey = key;
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public abstract string getResult();

    

    protected string Message_Text
    {
        get
        {
            return @"<xml><ToUserName><![CDATA[{0}]]></ToUserName><FromUserName><![CDATA[{1}]]></FromUserName><CreateTime>{2}</CreateTime><MsgType><![CDATA[{3}]]></MsgType><Content><![CDATA[{4}]]></Content></xml>";
        }
    }
}
public enum EventType
{
    Text,Link,Redirect,Image, voice, ShortVideo, Music,News,Location
}
public class EventResponseResult
{
    public EventType EventResponseType;
    public long msgId;

}

public class ResponseResult
{
    public string FromUser;
}