using System.Collections.Generic;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.Strags;
namespace PK10Server
{
    public class CurrInfo<T> where T:TimeSerialData
    {
        public string CurrExpect;
        public string ExecExpect
        {
            get
            {
                return (long.Parse(CurrExpect) + 1).ToString();
            }
        }

        public Dictionary<string, Dictionary<string, List<ChanceClass<T>>>> CurrChances;

        public CurrInfo()
        {
            CurrChances = new Dictionary<string,Dictionary<string,List<ChanceClass<T>>>>();
        }
    }

    public class GlobalSettingClass
    {
        public SettingClass CurrSetting;
        public List<StragClass> CurrStags;
    }
}
