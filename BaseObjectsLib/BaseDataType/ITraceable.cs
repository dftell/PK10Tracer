using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WolfInv.com.BaseObjectsLib
{
    public interface ISpecAmount
    {
        Int64 getChipAmount<T>(double RestCash, ChanceClass<T> cc, AmoutSerials amts) where T : TimeSerialData;
    }
    public interface ITraceChance:ISpecAmount
    {
        bool IsTracing { get; set; }
        bool CheckNeedEndTheChance<T>(ChanceClass<T> cc, bool LastExpectMatched) where T : TimeSerialData;
    }

    
}
