using System.Text;
using System.Net;
using System.IO;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public abstract class HtmlDataClass
    {
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
            catch
            {
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
