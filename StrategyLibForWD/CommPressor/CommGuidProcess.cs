using System;
using System.Text;
using WolfInv.com.CFZQ_LHProcess;
using WolfInv.com.SecurityLib;
using WAPIWrapperCSharp;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.StrategyLibForWD
{
 

    
    public abstract class CommGuidProcess:WDBuilder
    {
        public PriceAdj prcAdj;
        public Cycle cycle;
        protected CommDataBuilder gbc;
        public CommGuidProcess(WindAPI _w):base(_w)
        {

        }
        public CommGuidProcess(WindAPI _w,CommDataBuilder _gbc, Cycle cyc, PriceAdj rate):base(_w)
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

