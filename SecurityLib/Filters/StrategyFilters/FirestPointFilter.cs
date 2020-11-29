using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WolfInv.com.SecurityLib;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;

namespace WolfInv.com.SecurityLib
{
    public class FirstPointFilter<T> : CommFilterLogicBaseClass<T> where T:TimeSerialData
    {
        public FirstPointFilter(CommSecurityProcessClass<T> secinfo):base(secinfo)
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

        public override CommSecurityProcessClass<T> ExecFilter(CommStrategyInClass Input)
        {
            return base.ExecFilter(Input);
        }
    }
}
