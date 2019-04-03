using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.PK10CorePress;
using WolfInv.com.BaseObjectsLib;
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
        public abstract bool CheckNeedEndTheChance<T>(ChanceClass<T> cc, bool LastExpectMatched) where T : TimeSerialData;
        public abstract long getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts);
        public abstract long getChipAmount<T>(double RestCash, ChanceClass<T> cc, AmoutSerials amts) where T : TimeSerialData;
    }


}
