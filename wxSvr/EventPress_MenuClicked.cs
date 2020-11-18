using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// EventPress_MenuClicked 的摘要说明
/// </summary>
public class EventPress_MenuClicked: EventPressClass
{
    
    string strMenuKey;
    public EventPress_MenuClicked(wxmessage wx,string eventType, string key):base(wx,eventType,key)
    {
        wxobj = wx;
        string strMnuKey = key.ToLower();
        switch(eventType)
        {
            case "view":
                {
                    responseView();
                    break;
                }
            case "click":
                {
                    responseClick();
                    break;
                }
            default:
                {
                    getResult();
                    break;
                }
            
        }
        
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public override string getResult()
    {
        return string.Format(Message_Text, wxobj.FromUserName, wxobj.ToUserName, "Text", "");
    }

    public string responseView()
    {
        ViewPressClass vp = new ViewPressClass();
        return vp.getUrl(strMenuKey);
    }

    public string responseClick()
    {
        ViewPressClass vp = new ViewPressClass();
        return vp.getUrl(strMenuKey);
    }
}