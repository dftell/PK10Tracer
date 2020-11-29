using System;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
using WAPIWrapperCSharp;
namespace WolfInv.com.StrategyLibForWD
{

    public class BaseDataProcess_ForWD : BaseDataProcess<TimeSerialData>
    {
        WindAPI w;
        protected BaseDataProcess_ForWD():base()
        {

        }

        public BaseDataProcess_ForWD(WindAPI _w) : base(new CommDataInterface_ForWD(_w))
        {
            w = _w;
        }

        public BaseDataProcess_ForWD(WindAPI _w,  Cycle cyc, PriceAdj rate) : base(new CommDataInterface_ForWD(_w), cyc, rate)
        {
            w = _w;
        }

        public BaseDataProcess_ForWD(WindAPI _w, CommDataBuilder_ForWD _gbc, Cycle cyc, PriceAdj rate) : base(new CommDataInterface_ForWD(_w), cyc,rate)
        {
            w = _w;
        }

        


        public override RunResultClass GetSetBaseData(string[] secCodes, DateTime EndT, params object[] datapointnames)
        {
            RunResultClass ret = new RunResultClass();
            BaseDataPointGuidClass gd = null;
            if (datapointnames.Length == 0)
                gd = new BaseDataPointGuidClass(true);
            else
            {
                gd = new BaseDataPointGuidClass(datapointnames);
            }
            GuidBuilder_ForWD gb = new GuidBuilder_ForWD(w, gd);
            gd.cycle = this.cycle;
            gd.priceAdj = this.prcAdj;
            ////////ret.Result = gb.getRecords(secCodes, EndT);
            ////////ret.Result.AddColumnByArray<DateTime>("DateTime", EndT);
            ret.Notice.Success = true;
            return ret;
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
            DateSerialGuidBuilder_ForWD gb = new DateSerialGuidBuilder_ForWD(w, gd);
            tab = gb.getRecords(secCode, begt, endt);
            ret.Table = tab;
            ret.Notice.Success = true;
            return ret;
        }

        
    }

}

