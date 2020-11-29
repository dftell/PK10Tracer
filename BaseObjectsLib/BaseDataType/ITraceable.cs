using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WolfInv.com.BaseObjectsLib
{
    public interface ISpecAmount<T> where T : TimeSerialData
    {
        double getChipAmount(double RestCash, ChanceClass<T> cc, AmoutSerials amts) ;
    }
    public interface ITraceChance<T>:ISpecAmount<T> where T : TimeSerialData
    {
        bool IsTracing { get; set; }
        bool CheckNeedEndTheChance(ChanceClass<T> cc, bool LastExpectMatched);
    }

    
}
