using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
using Strags;
namespace PK10Server
{
    public class CurrInfo
    {
        public string CurrExpect;
        public string ExecExpect
        {
            get
            {
                return (long.Parse(CurrExpect) + 1).ToString();
            }
        }

        public Dictionary<string, Dictionary<string, List<ChanceClass>>> CurrChances;

        public CurrInfo()
        {
            CurrChances = new Dictionary<string,Dictionary<string,List<ChanceClass>>>();
        }
    }

    public class GlobalSettingClass
    {
        public SettingClass CurrSetting;
        public List<StragClass> CurrStags;
    }
}
