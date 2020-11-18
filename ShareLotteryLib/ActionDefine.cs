using System.Collections.Generic;

namespace WolfInv.com.ShareLotteryLib
{
    public class ActionDefine
    {
        public string actionName { get; set; }
        public List<string> regConditions { get; set; }
        public Dictionary<string,ResponseProcessItem> responseGroup { get; set; }
        public ActionDefine()
        {
            regConditions = new List<string>();
            responseGroup = new Dictionary<string, ResponseProcessItem>();
        }
        public string submitUrl { get; set; }
        public int typeIndex;
        public AskItemDefineClass AskDefine;
        public bool needAsk;
        public string regRule;
        public bool isOpenAsk;
        public bool noNeedLogin;
        public string useActionType;
        public string useBussinessType;
    }
    
}
