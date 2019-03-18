using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFZQ_LHProcess;
using WAPIWrapperCSharp;
namespace StrategyLibForWD.Filters.StrategyFilters
{
    public class FirestPointFilter:FilterLogicBaseClass
    {
        public FirestPointFilter(SecurityProcessClass secinfo):base(secinfo)
        {

        }

        public FirestPointFilter(Cycle cyc, PriceAdj rate):base(cyc,rate)
        {
        }

        public override BaseDataTable GetData(int RecordCnt)
        {
            if (this.ExecStrategy == null)
            {
                throw (new Exception("必须先指定策略！"));
            }
            WindAPI wa = this.ExecStrategy.w;
            MACDTable data = new MACDTable();

            CommWDToolClass.GetMutliSerialData(wa,
                this.secCode,
                WDDayClass.Offset(wa,Endt,this.PassViewDays,this.Cycle),
                this.Endt,
                this.Cycle,
                this.Rate,
                true,
                "MACD.DIFF,MACD.DEA,MACD.MACD"
            );
            return null;
        }
    }
}
