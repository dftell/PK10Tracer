using System;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace WolfInv.com.SecurityLib
{
    /// <summary>
    /// 日期序列指标工厂类
    /// </summary>
    public class DateSerialGuidBuilder_ForMG : CommDataBuilder_ForMG
    {
        //WindData wd = w.wsd("600011.SH", "MACD", "2017-02-05", "2018-03-06", "MACD_L=26;MACD_S=12;MACD_N=9;MACD_IO=1;Fill=Previous");
        public DateSerialGuidBuilder_ForMG(MongoDataReader _w, GuidBaseClass guidClass):base(_w,guidClass)
        {
            strParamsStyle = "priceAdj={1};Period={2};Fill=Previous;{0}";
         }
        public MTable getRecords(string Sector, DateTime begdt, DateTime enddt)
        {
            if (gbc == null) return null;
            
            //WSDClass wsetobj;
            //wsetobj = new WSDClass(w,Sector,gbc.GuidName,begdt,enddt,string.Format(strParamsStyle,gbc.strParam,gbc.priceAdj.ToString().Substring(0,1),gbc.cycle.ToString().Substring(0,1)));
            //return WDDataAdapter.getTable(wsetobj.getDataSet(),gbc.GuidName,typeof(decimal));
        }
    }

}

