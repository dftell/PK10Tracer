using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using LogLib;
using PK10CorePress;
using ExchangeLib;
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
