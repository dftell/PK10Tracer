using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.WebRuleLib
{
    public class WebRuleBuilder
    {
        public static WebRule Create(string webId,IWebFlags webflag, GlobalClass gc)
        {
            string val = webId;// gc.InstFormat.ToLower().Trim();
            WebRule ret = null;
            switch(val)
            {
                case "kcai":
                case "guosen":
                    {
                        ret = new Rule_ForKcaiCom(webId, webflag, gc);
                        break;
                    }
                case "ashc": //傲视皇朝
                case "jhc"://金皇朝
                    {
                        ret = new ASHC_WebRule(webId, webflag, gc);
                        break;
                    }
                default:
                    {
                        ret = new JsProcessRuleClass(webId, webflag, gc); 
                        break;
                    }
            }

            return ret;
        }

        
    }
}
