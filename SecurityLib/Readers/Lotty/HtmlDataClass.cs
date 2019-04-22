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
        public ExpectList<T> getExpectList<T>() where T: TimeSerialData
        {
            ExpectList<T> ret = new ExpectList<T>();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(dataUrl);
            //LogLib.LogableClass.ToLog(dataUrl, UseXmlMothed.ToString());
            req.Method = "Get";
            string htmltxt = "";
            try
            {
                using (WebResponse wr = req.GetResponse())
                {
                    wr.GetResponseStream();
                    htmltxt = new StreamReader(wr.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();
                    wr.Close();
                }
                if(UseXmlMothed)
                    return getXmlData<T>(htmltxt);
                else
                    return getData<T>(htmltxt);
            }
            catch(Exception ce)
            {
               
                LogLib.LogableClass.ToLog(string.Format("主机连接错误！url:{0};接收到数据:{1}",dataUrl,htmltxt), ce.Message);
                //切换主备host
                if(dtp.RuntimeInfo == null)
                {
                    dtp.RuntimeInfo = new DataPointBuff();
                }
                if (dtp.AutoSwitchHost==1)
                {
                    if (dtp.RuntimeInfo.DefaultDataUrl.Equals(dtp.MainDataUrl))
                    {
                        dtp.RuntimeInfo.DefaultDataUrl = dtp.SubDataUrl;
                    }
                    else
                    {
                        dtp.RuntimeInfo.DefaultDataUrl = dtp.MainDataUrl;
                    }
                    dtp.RuntimeInfo.DefaultUseXmlModel = dtp.RuntimeInfo.DefaultUseXmlModel==1?0:1;// dtp.SrcUseXml = (dtp.SrcUseXml == 1 ? 0 : 1);
                    LogLib.LogableClass.ToLog("切换到主机", dtp.RuntimeInfo.DefaultDataUrl);
                }
                else
                {
                    LogLib.LogableClass.ToLog("未设置自动切换到主机", "等待下次看是否能恢复！");
                }
            }
            return ret;
        }

        public abstract ExpectList<T> getHistoryData<T>(string FolderPath,string filetype) where T : TimeSerialData;

        public abstract ExpectList<T> getHistoryData<T>(string strDate, int pageid) where T : TimeSerialData;

        protected abstract ExpectList<T> getData<T>(string strHtml) where T : TimeSerialData;

        protected abstract ExpectList<T> getXmlData<T>(string strXml) where T : TimeSerialData;

        protected abstract ExpectList<T> getHisData<T>(string strHtml) where T : TimeSerialData;
    }

}
