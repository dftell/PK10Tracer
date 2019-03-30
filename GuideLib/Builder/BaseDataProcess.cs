using System;

using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace WolfInv.com.GuideLib
{
    public abstract class  BaseDataProcess : CommGuidProcess
    {
        public BaseDataProcess()
        {

        }
        public BaseDataProcess(CommDataIntface cdi)
            : base(cdi)
        { }
        public BaseDataProcess(CommDataIntface cdi, Cycle cyc, PriceAdj rate)
            : base(cdi)
        {
            this.cycle = cyc;
            this.prcAdj = rate;
        }

        public abstract RunResultClass GetSetBaseData(string[] secCodes, DateTime EndT, params object[] datapointnames);
        

        public RunResultClass GetSetBaseData( string[] secCodes, DateTime EndT)
        {
            return GetSetBaseData(secCodes, EndT, new object[0] { });
        }

        public RunResultClass GetSetBaseData(string secCodes, string strFields, DateTime EndT)
        {
            return GetSetBaseData(secCodes.Split(','), EndT, strFields);
        }

        public RunResultClass GetSetBaseData( string secCodes, DateTime EndT)
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

        //public abstract RunResultClass getDateSerialResult(string secCode, DateTime begt, DateTime endt, params object[] DataPoints);
        
    }

}

