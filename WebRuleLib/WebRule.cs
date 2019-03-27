using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.WebCommunicateClass;
using WolfInv.com.BaseObjectsLib;
using System.Windows.Forms;
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

        public abstract bool IsVaildWeb(HtmlDocument doc);

        public abstract bool IsLogined(HtmlDocument doc);

        public abstract double GetCurrMoney(HtmlDocument doc);
    }

    public class WebRuleBuilder
    {
        public static WebRule Create(GlobalClass gc)
        {
            if(gc.ForWeb.ToLower().IndexOf("kcai")>=0)
            {
                return new Rule_ForKcaiCom(gc);
            }
            ////else if()
            ////{

            ////}
            return null;
        }
    }
}
