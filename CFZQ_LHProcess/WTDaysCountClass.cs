using System;
using WAPIWrapperCSharp;
namespace WolfInv.com.CFZQ_LHProcess
{
    public class WTDaysCountClass : WDBuilder, iWDReader
    {
        DateTime _From;
        DateTime _To;
        public WTDaysCountClass(WindAPI _w, DateTime From, DateTime To)
            : base(_w)
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
        }



        public WindData getDataSet()
        {
            WindData wd = w.tdayscount(_From, _To,"");
            if (wd.errorCode != 0) throw (new WDErrorException(w, wd.errorCode));
            return wd;
        }

        public long getDataSetCount()
        {
            return 1;
        }
    }

}
