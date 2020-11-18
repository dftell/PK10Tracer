using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WolfInv.com.WXMPLib;
public partial class WXMainFrame : System.Web.UI.Page
{
    public string CurrUrl = "";
    public string NonStr = "";
    public long TimeStamp = 0;
    public string Sign = "";
    public string AppId = "";
    public string jsTicket = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            CurrUrl = HttpContext.Current.Request.Url.ToString().Split('#')[0];
            NonStr = Guid.NewGuid().ToString("N");
            TimeStamp = AccessTockenClass.GetTimeStamp(DateTime.UtcNow);
            Sign = AccessTockenClass.getJsApiToken(CurrUrl, NonStr, TimeStamp)?.ToLower();
            AppId = AccessTockenClass.AppID;
            jsTicket = AccessTockenClass.JsToken;
        }
    }
}