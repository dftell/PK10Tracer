using System.Collections.Generic;
using WolfInv.com.WebCommunicateClass;

namespace WolfInv.com.WebRuleLib
{
    public interface ILotteryRule
    {
        string IntsListToJsonString(List<InstClass> Insts);
        string IntsToJsonString(string ccs, int unit);
    }
}