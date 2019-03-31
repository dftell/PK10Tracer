using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WolfInv.com.SecurityLib;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;

namespace WolfInv.com.StrategyLibForWD.Filters.StrategyFilters
{
    public class FirstPointFilter:CommFilterLogicBaseClass
    {
        public FirstPointFilter(CommSecurityProcessClass secinfo):base(secinfo)
        {

        }

        public FirstPointFilter(Cycle cyc, PriceAdj rate):base(cyc,rate)
        {
        }

        public override BaseDataTable GetData(int RecordCnt)
        {
            if (this.ExecStrategy == null)
            {
                throw (new Exception("必须先指定策略！"));
            }
            ////WindAPI wa = this.ExecStrategy.w;
            ////MACDTable data = new MACDTable();

            ////CommWDToolClass.GetMutliSerialData(wa,
            ////    this.secCode,
            ////    WDDayClass.Offset(wa,Endt,this.PassViewDays,this.Cycle),
            ////    this.Endt,
            ////    this.Cycle,
            ////    this.Rate,
            ////    true,
            ////    "MACD.DIFF,MACD.DEA,MACD.MACD"
            ////);
            return null;
        }
    }
}
