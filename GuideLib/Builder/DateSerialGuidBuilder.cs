using System;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace WolfInv.com.GuideLib
{
    /// <summary>
    /// 日期序列指标工厂类
    /// </summary>
    public abstract class DateSerialGuidBuilder : CommDataBuilder
    {
        //WindData wd = w.wsd("600011.SH", "MACD", "2017-02-05", "2018-03-06", "MACD_L=26;MACD_S=12;MACD_N=9;MACD_IO=1;Fill=Previous");
        public DateSerialGuidBuilder(CommDataIntface cdi, GuidBaseClass guidClass):base(cdi,guidClass)
        {
            //strParamsStyle = "priceAdj={1};Period={2};Fill=Previous;{0}";
        }
        public abstract MTable getRecords(string Sector, DateTime begdt, DateTime enddt); 
 
    }

}

