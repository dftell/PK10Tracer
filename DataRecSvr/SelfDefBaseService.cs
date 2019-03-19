using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using WolfInv.com.LogLib;
using WolfInv.com.PK10CorePress;
using WolfInv.com.ExchangeLib;
using WolfInv.com.BaseObjectsLib;
namespace DataRecSvr
{
    public  class SelfDefBaseService : ServiceBase
    {
        public static string CurrExpectNo;
        public string ServiceName;
        public static ExpectList CurrDataList;

        public ExpectList CurrData
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

        public Dictionary<string, Dictionary<string, ExchangeChance>> CurrChances;
        public void Log(string topic, string msg)
        {
            LogableClass.ToLog(this.ServiceName, topic, msg);
        }
    }
}
