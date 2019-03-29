using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WolfInv.com.BaseObjectsLib
{

    public class SecBuffData
    {
    }

    ////public class WDSetBuffData : SecBuffData
    ////{

    ////}

    ////public class WDHisBuffData : SecBuffData
    ////{
    ////}
    //周期
    public enum Cycle
    {
        Day, Week, Month, Quarter, SemiYear, Year,Minute,Tick,Expect
    }
    //复权方式
    public enum PriceAdj
    {
        Fore, UnDo, Beyond, Target
    }

    public enum SecType
    {
        Equit, Index, Fund, Bond, Other
    }

}
