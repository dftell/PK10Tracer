﻿using System.Text;
using System.Net;
using System.IO;
using WolfInv.com.BaseObjectsLib;
using System;
namespace WolfInv.com.SecurityLib
{
    public abstract class HtmlDataClass : IHtmlDataClass
    {
        protected HtmlDataClass(DataTypePoint dp)
        {
            dtp = dp;
            UseDataType = dtp.RuntimeInfo.DefaultUseDataType;
            dataUrl = dtp.RuntimeInfo.DefaultDataUrl;
        }
        protected DataTypePoint dtp = null;
        protected string dataUrl;
        //protected bool UseXmlMothed;
        protected string UseDataType;
        public ExpectList<T> getExpectList<T>() where T: TimeSerialData
        {
            ExpectList<T> ret = new ExpectList<T>(false);
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
                switch (UseDataType)
                {
                    case "XML":
                        {
                            ret = getXmlData<T>(htmltxt);
                            break;
                        }
                    case "JSON":
                        {
                            ret = getJsonData<T>(htmltxt);
                            break;
                        }
                    case "TXT":
                        {
                            ret = getTextData<T>(htmltxt);
                            break;
                        }
                    case "HTML":
                    default:
                        {
                            ret = getData<T>(htmltxt);
                            break;
                        }
                }
                for (int i = 0; i < ret.Count; i++)//期号标准化
                {
                    ret[i].Expect = DataReader<T>.getStdExpect(ret[i].Expect, dtp);
                }
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
                        dtp.RuntimeInfo.DefaultUseDataType = dtp.SubDataType;
                    }
                    else
                    {
                        dtp.RuntimeInfo.DefaultDataUrl = dtp.MainDataUrl;
                        dtp.RuntimeInfo.DefaultUseDataType = dtp.MainDataType;
                    }
                    //dtp.RuntimeInfo.DefaultUseXmlModel = dtp.RuntimeInfo.DefaultUseXmlModel==1?0:1;// dtp.SrcUseXml = (dtp.SrcUseXml == 1 ? 0 : 1);
                    //dtp.RuntimeInfo.DefaultUseDataType = dtp.RuntimeInfo.DefaultUseXmlModel == 1 ? 0 : 1;
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

        public abstract ExpectList<T> getData<T>(string strHtml) where T : TimeSerialData;

        public abstract ExpectList<T> getXmlData<T>(string strXml) where T : TimeSerialData;
        public abstract ExpectList<T> getJsonData<T>(string strXml) where T : TimeSerialData;

        public abstract ExpectList<T> getHisData<T>(string strHtml) where T : TimeSerialData;

        public static HtmlDataClass CreateInstance(DataTypePoint dtp)
        {
            HtmlDataClass ret = null;
            switch(dtp.DataType)
            {
                case "TXFFC":
                    {
                        ret = new TXFFC_HtmlDataClass(dtp);
                        break;
                    }
                case "CAN28":
                    {
                        ret = new CAN28_HtmlDataClass(dtp);
                        break;
                    }
                case "SCKL12"://四川快乐12
                    {
                        ret = new SCKL12_HtmlDataClass(dtp);
                        break;
                    }
                case "NLKL12"://辽宁快乐12
                    {
                        ret = new NLKL12_HtmlDataClass(dtp);
                        break;
                    }
                case "GDKL11"://广东快乐11
                    {
                        ret = new GDKL11_HtmlDataClass(dtp);
                        break;
                    }
                case "XYFT":
                    {
                        ret = new XYFT_HtmlDataClass(dtp);
                        break;
                    }
                case "CQSSC"://重庆时时彩
                    {
                        ret = new CQSSC_HtmlDataClass(dtp);
                        break;
                    }
                case "XJSSC"://新疆时时彩
                    {
                        ret = new XJSSC_HtmlDataClass(dtp);
                        break;
                    }
                case "TJSSC"://天津时时彩
                    {
                        ret = new TJSSC_HtmlDataClass(dtp);
                        break;
                    }
                case "JSK3"://江苏快三
                    {
                        ret = new JSK3_HtmlDataClass(dtp);
                        break;
                    }
                case "PK10":                
                default:
                    {
                        //ret = new PK10_HtmlDataClass(dtp);
                        ret = new NewPK10_HtmlDataClass(dtp);
                        break;
                    }
            }
            return ret;
        }

        public abstract ExpectList<T> getTextData<T>(string strXml) where T : TimeSerialData;
    }

}
