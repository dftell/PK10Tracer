using System;
using System.Collections.Generic;
using System.Data;
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
using WolfInv.Com.MetaDataCenter;
public partial class QueryBussiness : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.ContentEncoding = Encoding.UTF8;
        string msg = null;
        Response.ContentEncoding = Encoding.UTF8;
        initServerWCS.reponseWrite = (string m) => { Response.Write(m); };
        bool succ = initServerWCS.Init(this, out msg);
        if (!succ)
        {
            Response.Write(msg);
            Response.End();
            return;
        }
        Dictionary<string, string> allItems = new Dictionary<string, string>();
        string strKeyCol = "";
        string strKeys = "";
        string strSrc = "";
        string strAction = "";
        Dictionary<string, string> reqList = new Dictionary<string, string>(); 
        
        foreach(string key in Request.QueryString.AllKeys)
        {
            if(key.Trim().ToLower().Equals("src"))
            {
                strSrc = Request.QueryString[key];
                continue;
            }
            if (key.Trim().ToLower().Equals("key"))
            {
                strKeyCol = Request.QueryString[key];
                continue;
            }
            if (key.Trim().ToLower().Equals("keys"))
            {
                strKeys = Request.QueryString[key];
                continue;
            }
            if (key.Trim().ToLower().Equals("action"))
            {
                strAction = Request.QueryString[key];
                continue;
            }
            if (!reqList.ContainsKey(key))
                reqList.Add(key, Request.QueryString[key]);
        }
        DataCondition dc = new DataCondition();
        if(!string.IsNullOrEmpty(strKeys))
        {
            string[] keys = strKeys.Split(',');
            for (int i = 0; i < keys.Length; i++)
            {
                string val = null;
                string[] kp = keys[i].Split('|');
                if(reqList.ContainsKey(kp[0]))
                {
                    val = reqList[kp[0]];
                    DataCondition sdc = new DataCondition();
                    sdc.Datapoint = new DataPoint(kp[0]);
                    sdc.value = val;
                    sdc.strOpt = kp[1];
                    sdc.Logic = kp[2]?.Trim().ToLower()=="or"?ConditionLogic.Or:ConditionLogic.And;
                    if(dc.SubConditions == null)
                    {
                        dc.SubConditions = new List<DataCondition>();
                    }
                    dc.SubConditions.Add(sdc);
                }
                
            }
        }
        //需要查询？
        if (!GlobalShare.mapDataSource.ContainsKey(strSrc))
        {
            Response.Write(string.Format("不存在数据源{0}！",strSrc));
            return;
        }
        DataSource dsrc = GlobalShare.mapDataSource[strSrc];
        if (string.IsNullOrEmpty(strKeyCol))
        {
            Response.Write(string.Format("未指定数据源{0}的关键字！", strSrc));
            return;
        }
        DataSet ds = DataSource.InitDataSource(dsrc,dc.SubConditions,out msg, GlobalShare.DebugMode);
        if (msg == null)
        {
            Response.Write(ds?.GetXml() ?? "");
        }
        else
        {
            Response.Write(msg);
        }
        Response.End();
    }

    
    
}