using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
namespace Strags
{
    public abstract class ChanceTraceClass : ITraceChance
    {
        bool _IsTracing;
        public bool IsTracing
        {
            get
            {
                return _IsTracing;
            }
            set
            {
                _IsTracing = value;
            }
        }
        public abstract bool CheckNeedEndTheChance(ChanceClass cc, bool LastExpectMatched);

        public abstract long getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts);
    }


}
