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
            return ExpectList.getList(new BaseDataTable(res));
        }

        

        public static MTable GetMutliSerialData(WindAPI w, string code, DateTime begt, DateTime endt, Cycle cyc, PriceAdj prcAdj, bool IncludeBaseData, string args)
        {

            RunNoticeClass ret = new RunNoticeClass();
            MTable mtab = new MTable();
            if (IncludeBaseData)
            {
                BaseDataProcess bp = new BaseDataProcess_ForWD(w, cyc, prcAdj);
                RunResultClass bret = bp.getDateSerialResult(code, begt, endt, new object[0] { });
                if (!bret.Notice.Success)
                {
                    mtab.Union(bret.Result);
                    //return new BaseDataTable();
                }
            }
            Dictionary<string, HashSet<string>> guids = CommWSSToolClass.getMutliValueGuid(args.Split(','));
            foreach (string key in guids.Keys)
            {
                MutliValueGuidProcess_ForWD cgp = new MutliValueGuidProcess_ForWD(w, key, guids[key].ToArray<string>());
                RunResultClass cret = cgp.getDateSerialResult(code, begt, endt, new object[0] { });

            }
            return mtab;
        }

        
    }

    public static class WDEquitCodeExtenel
    {
        public static string WDCode(this string code)
        {
            string ret = code;
            if(code.Split('.').Length>1)
                return ret;
            string flg = "SZ";
            if(code.StartsWith("6"))
            {
                flg = "SH";
            }
            return string.Format("{0}.{1}",code,flg);
        }
    }
}
