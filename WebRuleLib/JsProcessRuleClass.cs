using System.Collections.Generic;
using WolfInv.com.BaseObjectsLib;
using System.Windows.Forms;
using mshtml;
using Gecko;
using WolfInv.com.WebCommunicateClass;

namespace WolfInv.com.WebRuleLib
{
    public class JsProcessRuleClass : WebRule
    {
        public JsProcessRuleClass(string name,IWebFlags wf, GlobalClass setting):base(name,wf,setting)
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

        public override string ToInstItem(InstClass ic)
        {
            return "";
        }

        protected override Dictionary<string, int> GetChanlesInfo(string url)
        {
            return new Dictionary<string, int>();
        }
    }
}
