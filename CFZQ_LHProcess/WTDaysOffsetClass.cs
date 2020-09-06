using System;
using WAPIWrapperCSharp;
namespace WolfInv.com.CFZQ_LHProcess
{
    public class WTDaysOffsetClass : WDBuilder, iWDReader
    {
        DateTime _From;
        int _Days;
        string _params;
        public WTDaysOffsetClass(WindAPI _w, DateTime From, int days, string strParams)
            : base(_w)
        {
            _From = From;
            _Days = days;
            _params = strParams;
        }

        public WindData getDataSet()
        {
            //WindData wd = w.tdaysoffset("2018-03-11", 2, "Period=W")
            WindData wd = w.tdaysoffset(_From, _Days, _params);
            if (wd.errorCode != 0) throw (new WDErrorException(w, wd.errorCode));
            return wd;
        }

        public long getDataSetCount()
        {
            return 1;
        }
    }

}
