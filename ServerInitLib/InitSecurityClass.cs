using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.PK10CorePress;

using WolfInv.com.LogLib;
using System.Data;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.SecurityLib;

namespace WolfInv.com.ServerInitLib
{
    public class InitSecurityClass:LogableClass
    {
        public InitSecurityClass()
        {
            
        }
        public static Dictionary<Cycle,List<string>> getTypeAllTimes()
        {
            Dictionary<Cycle, List<string>> ret = new Dictionary<Cycle, List<string>>();
            Cycle[] cycs = new Cycle[3] { Cycle.Day,Cycle.Week,Cycle.Minute};
            foreach(Cycle cyc in cycs)
            {
                //NoSqlDataReader reader = new NoSqlDataReader("CN_Stock_A",GlobalClass.TypeDataPoints["CN_Stock_A"].IndexDayTable,new string[] { "000001" },cyc);
                
                //ret.Add(cyc, list.Select(p => p.date).ToList());
            }
            
            return ret;
        }

        public static MongoDataDictionary getAllXDXRData(string[] codes)
        {
            ////Dictionary<string, List<XDXRData>> ret = new Dictionary<string, List<XDXRData>>();
            ////DateSerialCodeDataBuilder dcdb = new DateSerialCodeDataBuilder("CN_Stock_A",GlobalClass.TypeDataPoints["CN_Stock_A"].XDXRTable,codes);
            XDXRReader reader = new XDXRReader("CN_Stock_A", GlobalClass.TypeDataPoints["CN_Stock_A"].XDXRTable, codes);
            MongoDataDictionary list = reader.GetAllCodeDateSerialDataList(true);
            return list;
        }
    }
}
