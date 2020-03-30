using System.Collections.Generic;
using WolfInv.com.BaseObjectsLib;
using System.Windows.Forms;
using mshtml;
using Gecko;

namespace WolfInv.com.WebRuleLib
{
    public class JsProcessRuleClass : WebRule
    {
        public JsProcessRuleClass(string name,GlobalClass setting):base(name,setting)
        {
            
        }
        public override string getChargeAmt(HtmlDocument doc)
        {
            return "0";
        }

        public override string getChargeNum(HtmlDocument doc)
        {
            return "0";
        }

        public override string getChargeQCode(HtmlDocument doc)
        {
            return "";
        }

        
        public override string getErr_Msg(HtmlDocument doc)
        {
            return null;
        }

        public override string getRealUrl(string html)
        {
            return null;
        }

        public override object getVerCodeImage(HtmlDocument doc)
        {
            return null;
        }
        

        protected override Dictionary<string, int> GetChanlesInfo(string url)
        {
            return new Dictionary<string, int>();
        }
    }
}
