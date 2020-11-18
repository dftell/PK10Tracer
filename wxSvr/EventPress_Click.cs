using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// EventPress_Click 的摘要说明
/// </summary>
public class EventPress_Click:EventPressClass
{
    public EventPress_Click(wxmessage wxobj, string evt,string strKey, string text) : base(wxobj, evt,strKey)
    {
        
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public override string getResult()
    {
        return null;
    }

}