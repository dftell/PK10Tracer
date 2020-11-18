using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// EventPress_Text 的摘要说明
/// </summary>
public class EventPress_Text:EventPressClass
{
    public EventPress_Text(wxmessage wxobj,string evt, string text):base(wxobj,evt,text)
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public override string getResult()
    {
        return string.Format(Message_Text, wxobj.FromUserName, wxobj.ToUserName, "Text", "我们正在努力建设自动机器人！");
    }
}