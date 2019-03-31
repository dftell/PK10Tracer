using System;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace WolfInv.com.SecurityLib
{
    public class GuidBuilder_ForMG :GuidBuilder
    {
        public MongoDataReader w;
        public GuidBuilder_ForMG(MongoDataReader _w, GuidBaseClass guidClass) : base(new CommDataInterface_ForMG(_w), guidClass)
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

