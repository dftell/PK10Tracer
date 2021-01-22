using System.Collections.Generic;
using System.ComponentModel;
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
        [Description("日线")] Day,
        [Description("周线")] Week,
        [Description("月线")] Month,
        [Description("系线")] Quarter,
        [Description("半年线")] SemiYear,
        [Description("年线")] Year,
        [Description("笔")] Tick,
        [Description("分钟线")] Minute,
        [Description("小时线")] Hour,
        [Description("期线")] Expect
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
