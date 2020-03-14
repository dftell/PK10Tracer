using System;
using System.Collections.Generic;
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
            return SendToWX(string.Format("来自用户[{0}]<{1}>的消息:[{2}]{3}", FromUser, getFuncName(), Topic, Msg), DefaultUrl);
        }

        public string Log(string msg, string DefaultUrl = null)
        {
            return SendToWX(string.Format("来自用户[{0}]<{1}>的消息:{2}", FromUser, getFuncName(), msg), DefaultUrl);
        }

        public string LogImageUrl(string url,string DefaultUrl=null)
        {
            return SendToWXImageUrl(url, DefaultUrl);
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
