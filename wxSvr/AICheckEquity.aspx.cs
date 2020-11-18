using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AICheckEquity : System.Web.UI.Page
{
    string strModel = @"{""succ"":[succ],""msg"":""[msg]"",""result"":""[result]""}";
    protected void Page_Load(object sender, EventArgs e)
    {
        return;
        string strJson = getRequest();
        string strCode = Request["code"];
        string strDate = Request["date"];
        string strRate = Request["rate"];
        string strPrice = Request["price"];
        if(string.IsNullOrEmpty(strJson) && string.IsNullOrEmpty(strCode))
        {
            Response.Write(getResponse(false,"无法接收到请求信息！"));
            Response.End();
            return;
        }

    }

    string getResponse(bool succed,string msg,string res=null)
    {
        return strModel.Replace("[succ]", (succed ? 1 : 0).ToString()).Replace("[msg]", msg).Replace("[result]", res);
    }

    string getRequest()
    {
        try
        {
            StreamReader str = new StreamReader(Request.InputStream, Encoding.UTF8);
            return str.ReadToEnd();
        }
        catch (Exception ce)
        {
            return null;
        }
    }
}