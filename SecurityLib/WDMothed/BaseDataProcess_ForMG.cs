using System;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;

namespace WolfInv.com.SecurityLib
{

    public class BaseDataProcess_ForMG : BaseDataProcess
    {
        MongoDataReader w;
        protected BaseDataProcess_ForMG():base()
        {

        }

        public BaseDataProcess_ForMG(MongoDataReader _w) : base(new CommDataInterface_ForMG(_w))
        {
            w = _w;
        }

        public BaseDataProcess_ForMG(MongoDataReader _w,  Cycle cyc, PriceAdj rate) : base(new CommDataInterface_ForMG(_w), cyc, rate)
        {
            w = _w;
        }

        public BaseDataProcess_ForMG(MongoDataReader _w, CommDataBuilder_ForMG _gbc, Cycle cyc, PriceAdj rate) : base(new CommDataInterface_ForMG(_w), cyc,rate)
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
            GuidBuilder_ForMG gb = new GuidBuilder_ForMG(w, gd);
            gd.cycle = this.cycle;
            gd.priceAdj = this.prcAdj;
            ret.Result = gb.getRecords(secCodes, EndT);
            ret.Result.AddColumnByArray<DateTime>("DateTime", EndT);
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
            DateSerialGuidBuilder_ForMG gb = new DateSerialGuidBuilder_ForMG(w, gd);
            tab = gb.getRecords(secCode, begt, endt);
            ret.Result = tab;
            ret.Notice.Success = true;
            return ret;
        }

        
    }

}

