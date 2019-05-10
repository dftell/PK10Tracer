using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace WolfInv.com.LogLib
{
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
        public string Log(string logname, string Topic, string msg)
        {
            //ToUser = logname;
            return SendToWX(string.Format("来自用户[{0}]<{1}>的消息:[{2}]{3}", logname, getFuncName(), Topic, msg));
        }

        public string Log(string Topic, string Msg)
        {
            return SendToWX(string.Format("来自用户[{0}]<{1}>的消息:[{2}]{3}", FromUser, getFuncName(), Topic, Msg));
        }

        public string Log(string msg)
        {
            return SendToWX(string.Format("来自用户[{0}]<{1}>的消息:{2}", FromUser, getFuncName(), msg));
        }

        string SendToWX(string Msg)
        {
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
