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
        public static ExpectList<T> CurrDataList;

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
        public void Log(string topic, string msg)
        {
            LogableClass.ToLog(this.ServiceName, topic, msg);
        }
    }
}
