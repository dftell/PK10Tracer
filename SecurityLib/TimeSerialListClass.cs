using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WolfInv.com.SecurityLib
{
    
    public delegate decimal CalcFunc(params object[] args);

    public class TimeSerialListClass
    {
        public BaseDataType DataType { get; set; }
        public Cycle UseCycle { get; set; }

        public TimeSerialListClass()
        {
            DataType = BaseDataType.Equit;
            UseCycle = Cycle.Day;
        }
        public TimeSerialListClass(BaseDataType type, Cycle cyc)
        {
            DataType = type;
            UseCycle = cyc;
        }

        public DataTable NDays(string secCode,int days,string DataPointName, CalcFunc func)
        {
            DataTable dt = new DataTable();
            
            return dt;
        }

        public DataTable NDays(string secCode, int days, params object[] args)
        {
            DataTable dt = new DataTable();
            return dt;
        }
    }
}
