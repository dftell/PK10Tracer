using System;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;

namespace WolfInv.com.SecurityLib
{
    public class TDayGuildBuilder_ForMG : CommDataBuilder_ForMG
    {
        public TDayGuildBuilder_ForMG(MongoDataReader _w, GuidBaseClass guidClass)
            : base(_w, guidClass)
        {
            strParamsStyle = "Period={0};";//Days=Weekdays
        }

        public MTable getRecords(DateTime begdt, DateTime enddt)
        {
            if (gbc == null) return null;
            WTDaysClass wsetobj;
            wsetobj = new WTDaysClass(w, begdt, enddt, string.Format(strParamsStyle, gbc.cycle.ToString().Substring(0,1)));
            return WDDataAdapter.getTable(wsetobj.getDataSet());
        }

        public MTable getRecords(DateTime endt, int N)
        {
            if (gbc == null) return null;
            WTDaysOffsetClass wsetobj;
            wsetobj = new WTDaysOffsetClass(w, endt, N, string.Format(strParamsStyle, gbc.cycle.ToString().Substring(0, 1)));
            return WDDataAdapter.getTable(wsetobj.getDataSet());
        }

        public MTable getRecordsCount(DateTime begdt, DateTime enddt)
        {
            if (gbc == null) return null;
            WTDaysCountClass wsetobj;
            wsetobj = new WTDaysCountClass(w, begdt, enddt);
            return WDDataAdapter.getTable(wsetobj.getDataSet());
        }
    }

}

