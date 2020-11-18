using System;
using WAPIWrapperCSharp;
namespace WolfInv.com.CFZQ_LHProcess
{
    public class WTDaysClass : WDBuilder,iWDReader
    {
        DateTime _From;
        DateTime _To;
        string _params;
        public WTDaysClass(WindAPI _w,DateTime From,DateTime To,string strParams):base(_w)
        {
            if (From.CompareTo(To) > 0)
            {
                _From = To;
                _To = From;
            }
            else
            {
                _From = From;
                _To = To;
            }
            _params = strParams;
        }



        public WindData getDataSet()
        {
            WindData wd = w.tdays(_From.ToString("yyyy-MM-dd"), _To.ToString("yyyy-MM-dd"), _params);
            //if (wd.errorCode != 0) throw (new WDErrorException(w, wd.errorCode));
            return wd;
        }

        public long getDataSetCount()
        {
            return 1;
        }
    }

}
