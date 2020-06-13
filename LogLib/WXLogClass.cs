using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace WolfInv.com.LogLib
{
    [Serializable]
    public class WXLogClass 
    {
        int deep;
        string FromUser;
        string ToUser;
        string Url;
        static WebClient wc;
        public string LogName { get { return FromUser; } }
        public WXLogClass(string strFromUser,string strToUser,string _url)
        {
            FromUser = strFromUser;
            ToUser = strToUser;
            Url = _url;
        }
        public string Log(string logname, string Topic, string msg, string DefaultUrl = null)
        {
            //ToUser = logname;
            return SendToWX(string.Format("来自用户[{0}]<{1}>的消息:[{2}]{3}", logname, getFuncName(), Topic, msg), DefaultUrl);
        }

        public string Log(string Topic, string Msg, string DefaultUrl = null)
        {
            return SendToWX(string.Format("来自用户[{0}]的消息:[{1}]{2}", FromUser,  Topic, Msg), DefaultUrl);
        }

        public string Log(string msg, string DefaultUrl = null)
        {
            return SendToWX(string.Format("来自用户[{0}]<{1}>的消息:{2}", FromUser, getFuncName(), msg), DefaultUrl);
        }

        public string LogImageUrl(string url,string DefaultUrl=null)
        {
            return SendToWXImageUrl(url, DefaultUrl);
        }

        public string LogImage(string image64,string defaultUrl=null)
        {
            return SendToWXImage(image64, defaultUrl);
        }

        string SendToWX(string Msg,string DefaultUrl=null)
        {
            if(DefaultUrl!=null)
            {
                Url = DefaultUrl;
            }
            if(wc == null)
                wc = new WebClient();
            string strUrl = string.Format("{0}?ToUser={1}&Msg={2}", Url, ToUser,string.Format("{0}",Msg));
            string ret = "";
            try
            {
                ret = wc.DownloadString(strUrl);
            }
            catch(Exception ce)
            {
                ret = ce.Message;
            }
            return ret;
        }

        string SendToWXImage(string image64,string DefaultUrl = null)
        {
            if (DefaultUrl != null)
            {
                Url = DefaultUrl;
            }
            if (wc == null)
                wc = new WebClient();
            string strUrl = string.Format("{0}", Url);
            string strPost = string.Format("ToUser={0}&Msg={1}&Type=image",ToUser, string.Format("{0}", image64));
            string ret = "";
            try
            {
                //ret = wc.DownloadString(strUrl);
                ret = PostData(strUrl, strPost, Encoding.UTF8);

            }
            catch (Exception ce)
            {
                ret = ce.Message;
            }
            return ret;
        }

        public static string PostData(string url, string Data, Encoding Encode)
        {
            string ret = "";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.ContentType = "application/x-www-form-urlencoded,multipart/form-data,application/json,application/xml";
            req.Method = "Post";
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(Data);
                Stream newStream = req.GetRequestStream();//创建一个Stream,赋值是写入HttpWebRequest对象提供的一个stream里面
                newStream.Write(byteArray, 0, byteArray.Length);
                newStream.Close();
                using (WebResponse wr = req.GetResponse())
                {
                    wr.GetResponseStream();
                    ret = new StreamReader(wr.GetResponseStream(), Encode).ReadToEnd();
                    wr.Close();
                }
            }
            catch (Exception ce)
            {
                return ce.Message;

                //throw ce;
            }
            return ret;
        }


        string SendToWXImageUrl(string msg,string DefaultUrl= null)
        {
            if (DefaultUrl != null)
            {
                Url = DefaultUrl;
            }
            if (wc == null)
                wc = new WebClient();
            string strUrl = string.Format("{0}?ToUser={1}&Msg={2}&Type=UrlImage", Url, ToUser, string.Format("{0}", msg));
            string ret = "";
            try
            {
                ret = wc.DownloadString(strUrl);
            }
            catch (Exception ce)
            {
                ret = ce.Message;
            }
            return ret;
        }
        string getFuncName()
        {
            var st = new System.Diagnostics.StackTrace();
            while (st.GetFrame(deep) == null)
            {
                deep--;

            }
            return st.GetFrame(deep).GetMethod().Name;
        }
    }
}
