using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using WolfInv.com.LogLib;
using WolfInv.com.ExchangeLib;
using WolfInv.com.BaseObjectsLib;
namespace DataRecSvr
{
    public  class SelfDefBaseService<T> : ServiceBase where T:TimeSerialData
    {
        public static string CurrExpectNo;
        public string ServiceName;
        static ExpectList<T> CurrDataList;

        /// <summary>
        /// 专为外部程序调用而设，调用前先设置gc
        /// </summary>
        /// <param name="gc"></param>
        public void setGlobalClass(GlobalClass gc)
        {
            Program.gc = gc;
        }

        public void setAllSettingConfig(WolfInv.com.ServerInitLib.ServiceSetting<TimeSerialData> setting)
        {
            Program.AllServiceConfig = setting;
        }

        //protected WXLogClass wxl = new WXLogClass("服务器管理员",GlobalClass.LogUser, GlobalClass.LogUrl);
        public ExpectList<T> CurrData
        {
            get
            {
                return CurrDataList;
            }
            set
            {
                CurrDataList = value;
            }
        }

        public Dictionary<string, Dictionary<string, ExchangeChance<T>>> CurrChances;
        public void Log(string topic, string msg,bool ToWXMsg=false)
        {
            LogableClass.ToLog(this.ServiceName, topic, msg);
            if(ToWXMsg)
                Program.AllServiceConfig.wxlog.Log(this.ServiceName, topic, msg, string.Format(Program.gc.WXLogUrl, Program.gc.WXSVRHost));
        }
    }
}
