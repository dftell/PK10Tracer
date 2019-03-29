using System;
using WAPIWrapperCSharp;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace WolfInv.com.StrategyLibForWD
{
    public class BaseDataProcess : CommGuidProcess
    {
        public BaseDataProcess(WindAPI _w)
            : base(_w)
        { }
        public BaseDataProcess(WindAPI _w,Cycle cyc, PriceAdj rate)
            : base(_w)
        {
            this.cycle = cyc;
            this.prcAdj = rate;
        }

         RunResultClass GetSetBaseData(string[] secCodes, DateTime EndT, params object[] datapointnames)
        {
            RunResultClass ret = new RunResultClass();
            BaseDataPointGuidClass gd = null;
            if (datapointnames.Length == 0)
                gd = new BaseDataPointGuidClass(true);
            else
            {
                gd = new BaseDataPointGuidClass(datapointnames);
            }
            GuidBuilder gb = new GuidBuilder(w, gd);
            gd.cycle = this.cycle;
            gd.priceAdj = this.prcAdj;
            ret.Result = gb.getRecords(secCodes, EndT);
            ret.Result.AddColumnByArray<DateTime>("DateTime",EndT);
            ret.Notice.Success = true;
            return ret;
        }

         RunResultClass GetSetBaseData( string[] secCodes, DateTime EndT)
        {
            return GetSetBaseData(secCodes, EndT, new object[0] { });
        }

        RunResultClass GetSetBaseData(string secCodes, string strFields, DateTime EndT)
        {
            return GetSetBaseData(secCodes.Split(','), EndT, strFields);
        }

        RunResultClass GetSetBaseData( string secCodes, DateTime EndT)
        {
            return GetSetBaseData(secCodes.Split(','), EndT, new object[0] { });
        }

        public override RunResultClass getSetDataResult(string[] secCodes, DateTime dt)
        {
            return this.GetSetBaseData(secCodes, dt);
        }

        public override RunResultClass getSetDataResult(string[] secCodes, string DataPoints, DateTime dt)
        {
            return this.GetSetBaseData(secCodes, dt, DataPoints);
        }

        public override RunResultClass getSetDataResult(string secCodes, string DataPoints, DateTime dt)
        {
            return this.GetSetBaseData(secCodes.Split(','), dt, DataPoints);
        }

        public override RunResultClass getSetDataResult(string[] secCodes, DateTime dt, params object[] DataPoints)
        {
            return this.GetSetBaseData(secCodes, dt,DataPoints);
        }

        public override RunResultClass getDateSerialResult(string secCode, DateTime begt, DateTime endt, params object[] DataPoints)
        {
            RunResultClass ret = new RunResultClass();
            MTable tab = new MTable();
            BaseDataPointGuidClass gd;
            if (DataPoints.Length > 0)
                gd = new BaseDataPointGuidClass(DataPoints);
            else
                gd = new BaseDataPointGuidClass(true);
            gd.cycle = this.cycle;
            gd.priceAdj = this.prcAdj;
            DateSerialGuidBuilder gb = new DateSerialGuidBuilder(w, gd);
            tab = gb.getRecords(secCode, begt, endt);
            ret.Result = tab;
            ret.Notice.Success = true;
            return ret;
        }
    }

}

