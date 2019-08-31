using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolfInv.com.BaseObjectsLib
{
    public class MongoDateTime
    {
        //public string BaseTime = "1970-01-01";
        public static DateTime BaseTime = new DateTime(1970, 1, 1);
        public static double Stamp(DateTime dt)
        {
            return dt.ToUniversalTime().Subtract(BaseTime).TotalMilliseconds/1000.00;
        }

        public static double Stamp(string strdt)
        {
            DateTime dt = new DateTime();
            DateTime.TryParse(strdt, out dt);
            return Stamp(dt);
        }

        public static DateTime StampToDate(double stamp)
        {
            DateTime ret = new DateTime();
            ret = BaseTime.ToLocalTime().AddMilliseconds((long)(stamp*1000.00));
            return ret;
        }
    }

    
}
