using System;
using System.Text;

using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.GuideLib
{
       

    
    public abstract class CommGuidProcess : CommDataBuilder, ICommGuidProcess
    {
        public PriceAdj prcAdj;
        public Cycle cycle;
        protected CommDataBuilder gbc;

        public CommGuidProcess():base()
        {

        }
        public CommGuidProcess(CommDataIntface cdi):base(cdi)
        {

        }
        public CommGuidProcess(CommDataIntface cdi, Cycle cyc, PriceAdj rate) : base(cdi)
        {

            cycle = cyc;
            prcAdj = rate;
        }
        public CommGuidProcess(CommDataIntface cdi, CommDataBuilder _gbc, Cycle cyc, PriceAdj rate):base(cdi)
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

