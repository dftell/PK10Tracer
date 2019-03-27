using System.Text;
using System.Net;
using System.IO;
using WolfInv.com.BaseObjectsLib;
using System;
namespace WolfInv.com.SecurityLib
{
    public abstract class HtmlDataClass
    {
        protected HtmlDataClass(DataTypePoint dp)
        {
            dtp = dp;
        }
        protected DataTypePoint dtp = null;
        protected string dataUrl;
        protected bool UseXmlMothed;
        public ExpectList getExpectList()
        {
            ExpectList ret = new ExpectList();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(dataUrl);
            req.Method = "Get";
            string htmltxt = null;
            try
            {
                using (WebResponse wr = req.GetResponse())
                {
                    wr.GetResponseStream();
                    htmltxt = new StreamReader(wr.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();
                    wr.Close();
                }
                if(UseXmlMothed)
                    return getXmlData(htmltxt);
                else
                    return getData(htmltxt);
            }
            catch(Exception ce)
            {
                LogLib.LogableClass.ToLog("接收数据错误", ce.Message);
                //切换主备host
                if(dtp.RuntimeInfo == null)
                {
                    dtp.RuntimeInfo = new DataPointBuff();
                }
                if(dtp.RuntimeInfo.DefaultDataUrl.Equals(dtp.MainDataUrl))
                {
                    dtp.RuntimeInfo.DefaultDataUrl = dtp.SubDataUrl;
                }
                else
                {
                    dtp.RuntimeInfo.DefaultDataUrl = dtp.MainDataUrl;
                }
                dtp.SrcUseXml = (dtp.SrcUseXml==1?0:1);
            }
            return ret;
        }

        public abstract ExpectList getHistoryData(string FolderPath,string filetype);

        public abstract ExpectList getHistoryData(string strDate, int pageid);

        protected abstract ExpectList getData(string strHtml);

        protected abstract ExpectList getXmlData(string strXml);

        protected abstract ExpectList getHisData(string strHtml);
    }

}
