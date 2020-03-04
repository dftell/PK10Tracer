using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.LogLib;
namespace WolfInv.com.SecurityLib
{
    public class EquitDayExpectReader : CommExpectReader
    {
        public EquitDayExpectReader()
            : base()
        {
            this.strDataType = "EquitDay";
            InitTables();
        }

        
    }

    public class EquitTimeExpectReader : CommExpectReader
    {
        public EquitTimeExpectReader()
            : base()
        {
            this.strDataType = "EquitTime";
            InitTables();
        }
        
    }
}
