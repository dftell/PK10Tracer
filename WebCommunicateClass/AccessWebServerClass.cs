using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
namespace WolfInv.com.WebCommunicateClass
{
    public class AccessWebServerClass
    {
        public static string GetData(string url)
        {
            return GetData(url, Encoding.UTF8);
        }
        public static string GetData(string url, Encoding Encode)
        {
            string ret = "";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "Get";
            try
            {
                using (WebResponse wr = req.GetResponse())
                {
                    wr.GetResponseStream();
                    ret = new StreamReader(wr.GetResponseStream(), Encode).ReadToEnd();
                    wr.Close();
                }
            }
            catch (Exception ce)
            {
                return null;
                    
                    //throw ce;
            }
            return ret;
        }

        public static string PostData(string url,string Data,Encoding Encode)
        {
            string ret = "";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
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
    }
}
