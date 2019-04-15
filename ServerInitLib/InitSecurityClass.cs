using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.PK10CorePress;
using WolfInv.com.LogLib;
using System.Data;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.SecurityLib;
using System.Threading;
using System.Threading.Tasks;
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

        public static MongoDataDictionary<XDXRData> getAllXDXRDataAsync(string DataType,List<string[]> codeGrp)
        {
            if(GlobalClass.TypeDataPoints[DataType].NeedLoadAllXDXR==0)
            {
                return null;
            }
            DateTime now = DateTime.Now;
            //MongoDataDictionary<XDXRData> ret = new MongoDataDictionary<XDXRData>();
            ////DateSerialCodeDataBuilder dcdb = new DateSerialCodeDataBuilder("CN_Stock_A",GlobalClass.TypeDataPoints["CN_Stock_A"].XDXRTable,codes);
            SecurityGrpReader<XDXRData> sgr = new SecurityGrpReader<XDXRData>();
            sgr.CheckEvent = (a,b,c,d,f)=> { };
            MongoDataDictionary<XDXRData> data = sgr.GetResult(codeGrp, (a) =>
            {
                XDXRReader reader = new XDXRReader(GlobalClass.TypeDataPoints[DataType].DataType, GlobalClass.TypeDataPoints[DataType].XDXRTable, a);
                MongoDataDictionary<XDXRData> res = reader.GetAllCodeDateSerialDataList<XDXRData>(true);
                return res;
            }, GlobalClass.TypeDataPoints[DataType].MaxThreadCnt, 1);

            return data;
        }

        
        
        static void FillDic(ref MongoDataDictionary<XDXRData> ret, MongoDataDictionary<XDXRData> list)
        {
            lock (ret)
            {
                if (ret.Count == 0)
                {
                    ret = list;
                }
                else
                {
                    foreach (string key in list.Keys)
                    {
                        if (!ret.ContainsKey(key))
                            ret.Add(key, list[key]);
                    }
                }
            }
        }

        static async Task<MongoDataDictionary<XDXRData>> Exec(XDXRReader reader)
        {
            //Func<XDXRReader,MongoDataDictionary<XDXRData>> action = delegate (XDXRReader rd)
            Func<MongoDataDictionary<XDXRData>> action = delegate ()
            {
                MongoDataDictionary<XDXRData> list = reader.GetAllCodeDateSerialDataList<XDXRData>(true);
                LogableClass.ToLog("获取到除权除息数据", string.Format("股票数：{0}；总条数：{1}", list.Count, list.Sum(p => p.Value.Count)));
                return list;
            };
            return await Task.Run(action);
            //return await Task.Run(action);
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

        public static List<string> getStockIndexAllDateList(string DataType)
        {
            if (GlobalClass.TypeDataPoints[DataType].NeedLoadAllCodes == 0)
            {
                return null;
            }
            List<string> ret = new List<string>();
            SecurityIndexReader reader = new SecurityIndexReader(DataType, GlobalClass.TypeDataPoints[DataType].IndexDayTable, new string[] { GlobalClass.TypeDataPoints[DataType].DateIndex });
            MongoReturnDataList<StockIndexMongoData> mdl = reader.GetAllTimeSerialList<StockIndexMongoData>();
            if (mdl == null)
                return null;
            return mdl.Select(p => p.date).ToList();
        }

        public static List<string> getStockAllDateList(string DataType)
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
