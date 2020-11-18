using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.LogLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public class EquitDayExpectReader<T> : CommExpectReader<T> where T:TimeSerialData
    {
        public EquitDayExpectReader()
            : base()
        {
            this.strDataType = "EquitDay";
            InitTables();
        }

        
    }

    public class EquitTimeExpectReader<T>: CommExpectReader<T> where T : TimeSerialData
    {
        public EquitTimeExpectReader()
            : base()
        {
            this.strDataType = "EquitTime";
            InitTables();
        }
        
    }
}
