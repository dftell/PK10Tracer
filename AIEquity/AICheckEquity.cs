using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIEquity
{
    public class AICheckEquity
    {
        public AICheckEquity()
        {

        }

        public AIInvestSuggestClass Check(string code,string date,double price,double holdRate, int riskLevel)
        {
            AIInvestSuggestClass ret = new AIInvestSuggestClass();
            return ret;
        }
    }

    public class AIInvestSuggestClass
    {
        public bool Succ;
        public string msg;
        public InvSuggestType InvSugg;

    }

    public enum InvSuggestType
    {
        None,Hold,Buy, Add,Full,Sale,Reduce,Clear
    }
}
