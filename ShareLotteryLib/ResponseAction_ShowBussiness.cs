using WolfInv.com.WXMessageLib;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using WolfInv.Com.WCS_Process;
using System.Net;
using System.Text;
using System;
using System.Web;
using System.Data;
using System.IO;
using System.Xml;

namespace WolfInv.com.ShareLotteryLib
{
    public class ResponseAction_queryBussiness : ResponseActionClass
    {
        public ResponseAction_queryBussiness(WXMsgProcess wxprs, wxMessageClass msg) : base(wxprs, msg)
        {
            ActionName = "显示业务结果";
        }

        protected override string getMsg()
        {
            string m = 
@"当前合买信息如下:
{0}
目前认购信,共计:{1}人{2}份
{3}
";
            string ret = string.Format(m, 
                currPlan.ToBasePlanInfo(),
                currPlan.subscribeList.GroupBy(a=>a.betWxName).Count(),
                currPlan.subscribeList.Sum(a=>a.subscribeShares), 
                currPlan.ToGroupedSubscribeInfo());
            return ret;
        }

        public override bool Response(ref ShareLotteryPlanClass optPlan)
        {
            Regex regTr = new Regex(actionDefine.regRule);
            MatchCollection mcs = regTr.Matches(pureMsg);
            string matchKey = null;
            string content = null;
            bussinessConfigClass bcc = null;
            Dictionary<string, string> allList = null;
            matchResult mr = null;
            string useBussinessType = actionDefine.useBussinessType;
            matchKey = useBussinessType;
            if (mcs.Count == 1 && mcs[0].Groups.Count > actionDefine.typeIndex + 1)
            {
                if (string.IsNullOrEmpty(matchKey))
                    matchKey = mcs[0].Groups[actionDefine.typeIndex].Value;//用作获取信息类型，去业务列表中匹配改信息
                if (WXMsgProcess.bcDic.ContainsKey(matchKey))
                {
                    bcc = WXMsgProcess.bcDic[matchKey];
                }
                if (bcc == null)
                {
                    answerMsg(string.Format("未找到{0}的业务配置！", matchKey));
                    return false;
                }
                //mr.action = mcs[0].Groups[1].Value;
                mr = getBussinessFormat(mcs[0].Groups, bcc);
                submitData("",mr,bcc);
                return true;
            }
            answerMsg("未提供查询业务的关键字！");
            return false;
        }

        void submitData(string btype, matchResult mr, bussinessConfigClass define)
        {
            string urlM = "http://www.wolfinv.com/pk10/app/QueryBussiness.asp?key={0}&keys={1}&src={2}&action={3}&{4}";
            if (!string.IsNullOrEmpty(actionDefine.submitUrl))
            {
                urlM = actionDefine.submitUrl;
            }
            string pars = string.Join("&", mr.items.Select(a => { return string.Format("{0}={1}", a.Key, a.Value); }));
            string keys = string.Join(",", define.condition.Select(a => string.Format("{0}|{1}|{2}", a.i, a.o, a.l)));
            string urlReq = string.Format(urlM, define.keyCol, HttpUtility.UrlEncode(keys), define.reqSrc, mr.action, pars);
            string url = urlReq;
            string ret = "";
            try
            {
                WebClient wc = new WebClient();
                wc.Encoding = Encoding.UTF8;
                ret = wc.DownloadString(url);
            }
            catch (Exception ce)
            {
                ret = string.Format("{0}:{1}", ce.Message, url);
            }
            //CITMSUser existUser = GlobalShare.GlobalData.getUserByWxId(wxmsg.FromUserNam, !DisableMultiTaskProcess);

            string xml = ret;
            try
            {
                DataSet ds = new DataSet();
                byte[] array = Encoding.UTF8.GetBytes(xml);
                MemoryStream stream = new MemoryStream(array);
                stream.Position = 0;
                XmlTextReader reader = new XmlTextReader(stream);
                ds.ReadXml(reader);
                if(ds.Tables.Count==0)
                {
                    answerMsg("未查找到满足条件的数据！");
                    return;
                }
                Dictionary<string, string> viewDic = define.item.Where(a => a.hiden == false).OrderBy(a=>a.id).ToDictionary(a => a.reqInfo, a => a.title);
                List<string> res = new List<string>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    List<string> strRow = new List<string>();
                    DataRow dr = ds.Tables[0].Rows[i];
                    for (int c = 0; c < dr.Table.Columns.Count; c++)
                    {
                        DataColumn col = dr.Table.Columns[c];
                        if (viewDic.ContainsKey(col.ColumnName))
                        {
                            strRow.Add(string.Format("{0}:{1}", viewDic[col.ColumnName], dr[col.ColumnName]));
                        }
                    }
                    res.Add(string.Format("第{0}条数据:\n{1}", i + 1, string.Join(this.SwtichLine, strRow)));
                }
                ret = string.Join(this.SwtichLine, res);
            }
            catch (Exception ce)
            {
                ret = ce.Message;
            }
            answerMsg(ret);

        }
    }


}
