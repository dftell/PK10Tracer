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
public partial class AddBussiness : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        string msg = null;
        Response.ContentEncoding = Encoding.UTF8;
        initServerWCS.reponseWrite = (string m) => { Response.Write(m); };
        bool succ = initServerWCS.Init(this,out msg);
        if(!succ)
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
        DataCondition dc = new DataCondition();
        try
        {
            foreach (string key in Request.QueryString.AllKeys)
            {
                if (key.Trim().ToLower().Equals("src"))
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
            
            Dictionary<string, string> keyDic = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(strKeys))
            {
                string[] keys = strKeys.Split(',');
                for (int i = 0; i < keys.Length; i++)
                {
                    string val = null;
                    string[] kp = keys[i].Split('|');
                    if (reqList.ContainsKey(kp[0]))
                    {
                        val = reqList[kp[0]];
                        
                        DataCondition sdc = new DataCondition();
                        sdc.Datapoint = new DataPoint(kp[0]);
                        sdc.value = val;
                        if (!keyDic.ContainsKey(kp[0]))
                        {
                            keyDic.Add(kp[0], val);
                        }
                        if (kp.Length > 1)
                        {
                            sdc.strOpt = kp[1];
                        }
                        if (kp.Length > 2)
                        {
                            sdc.Logic = kp[2]?.Trim().ToLower() == "or" ? ConditionLogic.Or : ConditionLogic.And;
                        }
                        if (dc.SubConditions == null)
                        {
                            dc.SubConditions = new List<DataCondition>();
                        }
                        dc.SubConditions.Add(sdc);
                    }

                }
            }
            //需要查询？
            DataSource dsrc = GlobalShare.getDataSource(strSrc);
            if (dsrc == null)
            {
                Response.Write(string.Format("不存在数据源{0}！", strSrc));
                return;
            }
            
            if (string.IsNullOrEmpty(strKeyCol))
            {
                Response.Write(string.Format("未指定数据源{0}的关键字！", strSrc));
                return;
            }
            DataSet ds = DataSource.InitDataSource(dsrc,dc.SubConditions,out msg, GlobalShare.DebugMode);
            List<UpdateData> existList = DataSource.DataSet2UpdateData(ds, strSrc);
            if (existList == null)
                existList = new List<UpdateData>();
            DataRequestType reqType = DataRequest.getReqType(strAction);
            if (reqType == DataRequestType.Add)
            {
                if (existList.Count > 0)
                {
                    Response.Write(string.Format("系统已经存在{0}的数据！", string.Join(",", keyDic.Select(a => string.Format("{0}为{1}", GlobalShare.DataPointMappings[a.Key].Text, a.Value)))));
                    return;
                }
            }
            string strKeyValue = null;
            if (reqType == DataRequestType.Update || reqType == DataRequestType.Delete)
            {
                if (existList.Count == 0)
                {
                    Response.Write(string.Format("系统不存在{0}的数据！", string.Join(",", keyDic.Select(a => string.Format("{0}为{1}", GlobalShare.DataPointMappings[a.Key].Text, a.Value)))));
                    return;
                }
                if (existList.Count > 1)
                {
                    Response.Write(string.Format("系统存在多条{0}的数据！", string.Join(",", keyDic.Select(a => string.Format("{0}为{1}", GlobalShare.DataPointMappings[a.Key].Text, a.Value)))));
                    return;
                }
                if (existList[0].Items.ContainsKey(strKeyCol))
                {
                    strKeyValue = existList[0].Items[strKeyCol].value;
                }
            }


            UpdateData ud = DataSource.Dictionary2UpdateData(reqList, dsrc);
            ud.keydpt = new DataPoint(strKeyCol);
            ud.keyvalue = reqList.ContainsKey(strKeyCol) ? reqList[strKeyCol] : null;
            ud.ReqType = reqType;
            if (strKeyValue != null)
                ud.keyvalue = strKeyValue;
            msg = GlobalShare.DataCenterClientObj.UpdateDataList(dsrc, new DataCondition(), ud, reqType);
            if (msg != null)
            {
                Response.Write(string.Format("保存失败:{0}!", msg));
                return;
            }
            Response.Write("succ");
        }
        catch(Exception ce)
        {
            Response.Write(string.Format("{0}:{1}",ce.Message,ce.StackTrace));
        }
    }

    
    
}