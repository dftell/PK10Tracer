using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using WAPIWrapperCSharp;
using CFZQ_LHProcess;
using System.Threading;
namespace StrategyLibForWD
{
    public class SystemGlobal
    {
        public delegate void barDelegate(int no);
        public static DateTime OnMarketDate;//最早上市时间
        public static WindAPI w;
        public static int GroupCnt;
        public static int EquitCnt;
        public static Dictionary<SecType, Dictionary<string,SecurityInfo>> AllSecSet;
        public static int FinishedQquitCnt;

        static void Init()
        {
            GroupCnt = 10;
            ThreadPool.SetMaxThreads(1,1);
            AllSecSet = new Dictionary<SecType, Dictionary<string, SecurityInfo>>();
            FinishedQquitCnt = 0;
            InitEquites();
            InitIndices();
        }
        public SystemGlobal(WindAPI _w)
        {
            w = _w;
            try
            { w.start(); }
            catch
            { }
            
            Init();
        }
        public void Invoke()
        {

        }
        static void InitEquites()
        {
            AllMarketEquitClass aec = new AllMarketEquitClass();
            MTable dt =  CommWDToolClass.getBkList(w, aec.SummaryCode, DateTime.Today,false);
            string[] equitcodes = dt["WIND_CODE"].ToList<string>().ToArray();
            BaseDataTable bdt = CommWDToolClass.GetBaseData(w,
                equitcodes,
                DateTime.Today,
                Cycle.Day,
                PriceAdj.Beyond,
                new object[0] { });//获得所有股票的基本信息
            OnMarketDate = bdt[BaseDataPoint.ipo_date.ToString().ToUpper()].ToList<DateTime>().Min<DateTime>();//所有股票中的最早的IPO日期
            EquitCnt = bdt.Count;
            Dictionary<string, SecurityInfo> alllist = new Dictionary<string, SecurityInfo>();
            Dictionary<int, List<SecurityInfo>> grps = new Dictionary<int,List<SecurityInfo>>();
            for (int i = 0; i < bdt.Count; i++)
            {
                BaseDataItemClass info = bdt[i] as BaseDataItemClass;
                SecurityInfo si = new SecurityInfo();
                si.secType = SecType.Equit;
                si.BaseInfo = info;
                si.DateIndex = new Dictionary<DateTime, int>();
                if (alllist.ContainsKey(si.BaseInfo.WindCode))
                {
                    throw (new Exception("初始化系统出现重复股票！"));
                }
                int t = i%GroupCnt;
                if (!grps.ContainsKey(t))
                {
                    grps.Add(t, new List<SecurityInfo>());
                }
                grps[t].Add(si);
                alllist.Add(si.BaseInfo.WindCode, si);
            }
            int cnt = 0;
            foreach (int k in grps.Keys)
            {
                if(cnt++ == 0)
                    ThreadPool.QueueUserWorkItem(new WaitCallback(runBuild), grps[k]);
            }
            if (!AllSecSet.ContainsKey(SecType.Equit))
            {
                AllSecSet.Add(SecType.Equit,alllist);
            }
            
        }

        static void runBuild(object si)
        {
            SecurityBuildClass sbc = new SecurityBuildClass(w, (List<SecurityInfo>)si);
            try
            {
                sbc.GetDateTimeInfo();
            }
            catch (Exception ce)
            {

                return;
            }
        }

        static void InitIndices()
        {
            List<Type> indexTypes = CommIndexTool.Allndices();
        } 
    }

    public class SecurityBuildClass : WDBuilder
    {
        public List<SecurityInfo> Infos; 
        public SecurityBuildClass(WindAPI _w)
            : base(_w)
        {
        }
        public SecurityBuildClass(WindAPI _w,List<SecurityInfo> si)
            : base(_w)
        {
            Infos = si;
        }
        public void GetDateTimeInfo()
        {
            //object _Infos
            DateTime begt = SystemGlobal.OnMarketDate;
            for (int s = 0; s< Infos.Count; s++)
            {
                SecurityInfo Info = Infos[s];
                if (Info.secType == SecType.Equit)
                {
                    begt = Info.BaseInfo.Ipo_date;
                }
                DateTime[] list;
                try
                {
                    list = WDDayClass.getTradeDates(w, Info.BaseInfo.WindCode, begt, System.DateTime.Today, Cycle.Day);
                
                }
                catch (Exception ce)
                {
                    WDErrorException we = null;
                    if(ce is WDErrorException) we=(WDErrorException)ce;
                    return ;
                }
                Info.DateIndex = new Dictionary<DateTime, int>();
                for (int i = 0; i < list.Length; i++)
                {
                    Info.DateIndex.Add(list[i], i);
                }
                Info.DateIndexFinished = true;
                Thread.Sleep(100);
            }
        }
    }

    public class SecurityInfo
    {
        public SecType secType;
        public BaseDataItemClass BaseInfo;
        /// <summary>
        /// 日期索引
        /// </summary>
        public Dictionary<DateTime,int> DateIndex;
        public bool DateIndexFinished;
        public SecurityInfo()
        {
            DateIndex = new Dictionary<DateTime, int>();
        }
    }

    public class IndexInfo:SecurityInfo
    {
        public string IndexCode;
        public List<SecurityInfo> Secuities;
        public IndexInfo()
        {
            Secuities = new List<SecurityInfo>();
        }
    }
}
