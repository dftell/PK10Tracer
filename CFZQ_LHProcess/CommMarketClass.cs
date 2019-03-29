using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.CFZQ_LHProcess;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.CFZQ_LHProcess
{
    public class CommMarketClass
    {
        public static MTable GetMarketsStocks(string indexname, DateTime endt, int MinOnMarketDays, bool NextDay, bool ExcludeST, bool MAFilter)
        {
            MTable ret = new MTable();
            if(indexname == null || indexname.Trim().Length==0) throw new Exception("市场，板块，指数不能为空！");
            string[] IndexList = indexname.Split(';');
            if (IndexList.Length == 1)//指数或者个股
            {

            }
            return ret;
        }
    }
}
