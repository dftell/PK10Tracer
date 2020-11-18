using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WolfInv.Com.WCS_Process;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using WolfInv.com.ShareLotteryLib;

/// <summary>
/// AutoAskRobot 的摘要说明
/// </summary>
public class AutoAskRobot
{
    wxmessage wxobj;
    string strInput;
    public AutoAskRobot(wxmessage wxo,string txt)
    {
        strInput = txt;
        wxobj = wxo;
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public string getAsk()
    {
        return "很抱歉！武府小旺无法识别您说的内容！我正在努力进化中！";
    }
}

public class initServerWCS
{
    public static Action<string> reponseWrite;
    public static bool Init(Page page,out string msg,string folder=".")
    {
        msg = null;
        GlobalShare.InitTextProcess = reponseWrite;
        if(GlobalShare.AppPath == null)
        {
            
            return GlobalShare.InitAndVirLogin(page.Server.MapPath(folder),out msg);
        }
        return true;
    }
}