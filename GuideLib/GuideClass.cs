using System.Collections.Generic;
using System.Text;
namespace WolfInv.com.GuideLib
{

    public enum BasePriceAndVol
    {
        windcode, sec_name,   open, high, low, close, volume,amt
    }

    public enum BaseDataPoint
    {
        windcode, sec_name, riskadmonition_date, ipo_date, open, high, low, close, volume, trade_status, susp_days, maxupordown, sec_type, pct_chg
    }
    /// <summary>
    /// 指标类
    /// </summary>
    public abstract class GuideClass
    {
        protected Dictionary<string, double> OrgData;
        protected GuideResultSet BaseSet;
        protected GuideClass()
        {
        }
        public GuideClass(Dictionary<string, double> data)
        {
            OrgData = data;
            BaseSet = new GuideResultSet(OrgData);

        }
        public abstract GuideResultSet GetResult(params int[] inParams);
    }
}
