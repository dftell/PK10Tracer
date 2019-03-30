using System;
using WolfInv.com.CFZQ_LHProcess;
using WAPIWrapperCSharp;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace WolfInv.com.StrategyLibForWD
{
    /// <summary>
    /// 多维指标工厂类
    /// </summary>
    public class GuidBuilder_ForWD : CommDataBuilder_ForWD
    {
        public GuidBuilder_ForWD(WindAPI _w, GuidBaseClass guidClass):base(_w,guidClass)
        {
            strParamsStyle = "tradeDate={0};priceAdj={2};cycle={3};{1}";
        }
        public MTable getRecords(string[] Sectors, DateTime dt)
        {
            if (gbc == null) return null;
            gbc.tradeDate = dt;
            WSSClass wsetobj;
            wsetobj = new WSSClass(w,string.Join(",", Sectors),gbc.GuidName,string.Format(strParamsStyle,dt.ToShortDateString().Replace("-",""),gbc.strParam,gbc.priceAdj.ToString().Substring(0,1),gbc.cycle.ToString().Substring(0,1)));
            return WDDataAdapter.getTable(wsetobj.getDataSet(), gbc.GuidName, typeof(decimal));
        }
    }

}

