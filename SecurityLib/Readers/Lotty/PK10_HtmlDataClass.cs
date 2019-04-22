using System;
using System.Xml;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public class PK10_HtmlDataClass : HtmlDataClass
    {
        public PK10_HtmlDataClass(DataTypePoint dp):base(dp)
        {

            //this.dataUrl = "https://www.52cp.cn/pk10/history";// 
            //GlobalClass.PK10_url; //"https://www.52cp.cn/pk10/history";
            this.dataUrl = GlobalClass.TypeDataPoints["PK10"].RuntimeInfo.DefaultDataUrl;
            //LogLib.LogableClass.ToLog("数据源url", this.dataUrl);
            this.UseXmlMothed = GlobalClass.TypeDataPoints["PK10"].RuntimeInfo.DefaultUseXmlModel==1;
        }
        public override ExpectList<T> getHistoryData<T>(string FolderPath, string filetype)
        {
            throw new NotImplementedException();
        }

        public override ExpectList<T> getHistoryData<T>(string strDate, int pageid)
        {
            throw new NotImplementedException();
        }

        protected override ExpectList<T> getData<T>(string strHtml)
        {
            ExpectList<T> ret = new ExpectList<T>();
            string startStr = "lg-history-table\">";
            string endStr = "</table>";
            string strXml = strHtml.Substring(strHtml.IndexOf(startStr)+startStr.Length);
            int endPos = strXml.IndexOf(endStr)+endStr.Length;
            strXml = strXml.Substring(0, endPos);
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(strXml);
                XmlNodeList rows = doc.SelectNodes("/table/tbody/tr");
                if (rows.Count == 0)
                    return ret;
                for (int i = rows.Count - 1; i >= 0; i--)
                {
                    XmlNodeList tdNodes = rows[i].SelectNodes("td");
                    XmlNodeList td2Nodes = tdNodes[2].SelectNodes("div");
                    ExpectData<T> ed = new ExpectData<T>();
                    ed.Expect = tdNodes[0].InnerText;
                    string strCode = string.Join(",",td2Nodes[0].InnerText.Replace("10", "0").ToCharArray());
                    ed.OpenCode = ChanceCodes(strCode);
                    ed.OpenTime = DateTime.Now.Date.Add(DateTime.Parse(tdNodes[1].InnerText).TimeOfDay);
                    ret.Add(ed);
                }
            }
            catch
            {
            }
            return ret;
        }

        string ChanceCodes(string codes)
        {
            string[] codeArr = codes.Split(',');
            for (int i = 0; i < codeArr.Length; i++)
            {
                if (codeArr[i] != "0")
                    codeArr[i] = "0" + codeArr[i];
                else
                    codeArr[i] = "10";
            }
            return string.Join(",", codeArr);
        }

        protected override ExpectList<T> getHisData<T>(string strHtml)
        {
            throw new NotImplementedException();
        }

        protected override ExpectList<T> getXmlData<T>(string strXml)
        {
            ExpectList<T> ret = new ExpectList<T>();
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(strXml);
                XmlNodeList rows = doc.SelectNodes("/xml/row");
                if (rows.Count == 0)
                    return ret;
                for (int i = rows.Count - 1; i >= 0; i--)
                {
                    ExpectData<T> ed = new ExpectData<T>();
                    ed.Expect = rows[i].Attributes["expect"].Value;
                    ed.OpenCode = rows[i].Attributes["opencode"].Value;
                    ed.OpenTime = DateTime.Parse(rows[i].Attributes["opentime"].Value);
                    ret.Add(ed as ExpectData<T>);
                }
            }
            catch(Exception e)
            {

                LogLib.LogableClass.ToLog(string.Format("非正常的xml数据,[{0}]:[{1}]",e.Message,e.StackTrace),strXml);
            }
            return ret;
        }
    }

}
