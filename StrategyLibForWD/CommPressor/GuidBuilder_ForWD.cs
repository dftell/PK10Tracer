using System;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.CFZQ_LHProcess;
using WolfInv.com.GuideLib;
using WAPIWrapperCSharp;
namespace WolfInv.com.StrategyLibForWD
{
    public class GuidBuilder_ForWD :GuidBuilder
    {
        public WindAPI w;
        public GuidBuilder_ForWD(WindAPI _w, GuidBaseClass guidClass) : base(new CommDataInterface_ForWD(_w), guidClass)
        {
            w = _w;
            strParamsStyle = "tradeDate={0};priceAdj={2};cycle={3};{1}";
        }
        public override MTable getRecords(string[] Sectors, DateTime dt)
        {
            if (gbc == null) return null;
            gbc.tradeDate = dt;
            WSSClass wsetobj;
            wsetobj = new WSSClass(w, string.Join(",", Sectors), gbc.GuidName, string.Format(strParamsStyle, dt.ToShortDateString().Replace("-", ""), gbc.strParam, gbc.priceAdj.ToString().Substring(0, 1), gbc.cycle.ToString().Substring(0, 1)));
            return WDDataAdapter.getTable(wsetobj.getDataSet(), gbc.GuidName, typeof(decimal));
        }
    }

}

