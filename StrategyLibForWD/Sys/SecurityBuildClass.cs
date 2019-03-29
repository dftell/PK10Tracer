using System;
using System.Collections.Generic;
using WAPIWrapperCSharp;
using WolfInv.com.CFZQ_LHProcess;
using System.Threading;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.StrategyLibForWD
{
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
}
