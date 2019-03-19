using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.PK10CorePress;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.WebRuleLib
{

    public abstract class WebRule
    {
        public abstract string IntsListToJsonString(List<InstClass> Insts);
        public abstract string IntsToJsonString(String ccs, int unit);
        public GlobalClass GobalSetting;
        protected WebRule(GlobalClass setting)
        {
            GobalSetting = setting;
        }
    }

}
