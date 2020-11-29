using System;
using System.Text;

using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.GuideLib
{
       

    
    public abstract class CommGuidProcess<T> : CommDataBuilder<T>, ICommGuidProcess where T:TimeSerialData
    {
        public PriceAdj prcAdj;
        public Cycle cycle;
        protected CommDataBuilder<T> gbc;

        public CommGuidProcess():base()
        {

        }
        public CommGuidProcess(CommDataIntface<T> cdi):base(cdi)
        {

        }
        public CommGuidProcess(CommDataIntface<T> cdi, Cycle cyc, PriceAdj rate) : base(cdi)
        {

            cycle = cyc;
            prcAdj = rate;
        }
        public CommGuidProcess(CommDataIntface<T> cdi, CommDataBuilder<T> _gbc, Cycle cyc, PriceAdj rate):base(cdi)
        {
            gbc = _gbc;
            cycle = cyc;
            prcAdj = rate;
        }
        /// <summary>
        /// 获得多维数据
        /// </summary>
        /// <param name="secCodes"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public abstract RunResultClass getSetDataResult(string[] secCodes, DateTime dt);
        public abstract RunResultClass getSetDataResult(string[] secCodes, string DataPoints, DateTime dt);
        public abstract RunResultClass getSetDataResult(string secCodes, string DataPoints, DateTime dt);
        public abstract RunResultClass getSetDataResult(string[] secCodes, DateTime dt,params object[] DataPoints);
        public abstract RunResultClass getDateSerialResult(string secCode, DateTime begt,DateTime endt, params object[] DataPoints);
    }

   
}

