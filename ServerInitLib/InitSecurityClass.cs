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

        public static MongoDataDictionary<XDXRData> getAllXDXRData(string DataType,List<string[]> codeGrp)
        {
            if(GlobalClass.TypeDataPoints[DataType].NeedLoadAllXDXR==0)
            {
                return null;
            }
            MongoDataDictionary<XDXRData> ret = null;
            ////Dictionary<string, List<XDXRData>> ret = new Dictionary<string, List<XDXRData>>();
            ////DateSerialCodeDataBuilder dcdb = new DateSerialCodeDataBuilder("CN_Stock_A",GlobalClass.TypeDataPoints["CN_Stock_A"].XDXRTable,codes);
            for (int i = 0; i < codeGrp.Count; i++)
            {
                string[] codes = codeGrp[i];
                XDXRReader reader = new XDXRReader(DataType, GlobalClass.TypeDataPoints[DataType].XDXRTable, codes);
                MongoDataDictionary<XDXRData> list = reader.GetAllCodeDateSerialDataList<XDXRData>(true);
                LogableClass.ToLog("获取到除权除息数据", string.Format("股票数：{0}；总条数：{1}",list.Count,list.Sum(p=>p.Value.Count)));
                if(ret == null)
                {
                    ret = list;
                }
                else
                {
                    foreach(string key in list.Keys)
                    {
                        if (!ret.ContainsKey(key))
                            ret.Add(key, list[key]);
                    }            
                }
            }
            return ret;
        }

        public static Dictionary<string,StockInfoMongoData> getAllCodes(string DataType)
        {
            if (GlobalClass.TypeDataPoints[DataType].NeedLoadAllCodes == 0)
            {
                return null;
            }
            List<string> ret = new List<string>();
            SecurityReader reader = new SecurityReader(DataType, GlobalClass.TypeDataPoints[DataType].StockListTable, null);
            MongoReturnDataList<StockInfoMongoData> mdl =  reader.GetAllCodeDataList< StockInfoMongoData>(true);
            if (mdl == null)
                return null;
            return mdl.ToDictionary(p => (p as ICodeData).code, p => (p as StockInfoMongoData));
        }

        public static List<string> getAllDateList(string DataType)
        {
            if (GlobalClass.TypeDataPoints[DataType].NeedLoadAllCodes == 0)
            {
                return null;
            }
            List<string> ret = new List<string>();
            SecurityIndexReader reader = new SecurityIndexReader(DataType, GlobalClass.TypeDataPoints[DataType].IndexDayTable, new string[] { GlobalClass.TypeDataPoints[DataType].DateIndex });
            MongoReturnDataList<StockMongoData> mdl = reader.GetAllTimeSerialList<StockMongoData>();
            if (mdl == null)
                return null;
            return mdl.Select(p => p.date).ToList();
        }
    }
}
