using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.PK10CorePress;
using WolfInv.com.BaseObjectsLib;
namespace Strags
{
    public abstract class ChanceTraceClass : ITraceChance<TimeSerialData>
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
        public abstract bool CheckNeedEndTheChance(ChanceClass<TimeSerialData> cc, bool LastExpectMatched);
        public abstract bool CheckNeedEndTheChance(ChanceClass cc, bool LastExpectMatched);
        public abstract double getChipAmount(double RestCash, ChanceClass<TimeSerialData> cc, AmoutSerials amts);
        public abstract double getChipAmount<T>(double RestCash, ChanceClass<TimeSerialData> cc, AmoutSerials amts);
    }


}
