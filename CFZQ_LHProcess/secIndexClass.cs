using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace CFZQ_LHProcess
{
    public class secIndexBuilder
    {
        SecIndexClass _sic = null;
        public secIndexBuilder(SecIndexClass sic)
        {
            _sic = sic;
        }
        public DataTable getRecords(DateTime dt)
        {
            WSETCommIndexClass wsetobj;
            wsetobj = new WSETCommIndexClass(_sic.SummaryCode, dt);
            return WDDataAdapter.getRecords(wsetobj.getDataSet());
        }

    }
}
