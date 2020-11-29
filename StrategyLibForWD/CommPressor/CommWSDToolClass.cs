using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAPIWrapperCSharp;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;

namespace WolfInv.com.StrategyLibForWD
{
    public class CommWSDToolClass
    {
        static WindAPI commW;
        static CommWSDToolClass()
        {
            connectWd();
        }

        static void connectWd()
        {
            commW = new WindAPI();
        }

        public static ExpectList GetSerialData(string code, DateTime begt, DateTime endt, Cycle cyc = Cycle.Day, PriceAdj prcAdj = PriceAdj.Fore,  bool IncludeBaseData=true, string args=null)
        {
            if (commW == null)
            {
                connectWd();
            }
            if (commW == null)
                return null;

            var res = GetMutliSerialData(commW, code,begt, endt, cyc, prcAdj, IncludeBaseData,args);
            if (res == null)
            {
                return new ExpectList();
            }
            CommWSSToolClass.rewriteDate(res, endt);
            return ExpectList.getList(new BaseDataTable(res));
        }

        

        public static MTable GetMutliSerialData(WindAPI w, string code, DateTime begt, DateTime endt, Cycle cyc, PriceAdj prcAdj, bool IncludeBaseData, string args)
        {

            RunNoticeClass ret = new RunNoticeClass();
            MTable mtab = new MTable();
            if (IncludeBaseData)
            {
                BaseDataProcess<TimeSerialData> bp = new BaseDataProcess_ForWD(w, cyc, prcAdj);
                RunResultClass bret = bp.getDateSerialResult(code, begt, endt, new object[0] { });
                if (bret.Notice.Success)
                {
                    mtab.Union(bret.Table);
                    //return new BaseDataTable();
                }
            }
            if (!string.IsNullOrEmpty(args))
            {
                Dictionary<string, HashSet<string>> guids = CommWSSToolClass.getMutliValueGuid(args.Split(','));
                foreach (string key in guids.Keys)
                {
                    MutliValueGuidProcess_ForWD cgp = new MutliValueGuidProcess_ForWD(w, key, guids[key].ToArray<string>());
                    RunResultClass cret = cgp.getDateSerialResult(code, begt, endt, new object[0] { });
                    mtab.Union(cret.Table);
                }
            }
            return mtab;
        }

        
    }
}
