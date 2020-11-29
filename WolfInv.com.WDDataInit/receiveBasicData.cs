using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
//using WolfInv.com.StrategyLibForWD;
using WolfInv.com.BaseObjectsLib;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Threading;
using System.Linq;
namespace WolfInv.com.WDDataInit
{

    public class receiveBasicData
    {
        Dictionary<string, CommEquitAPI> AllAPIs;
        Dictionary<string,CommEquitAPI> useAPI;
        const string strDataFolder = "vipdoc";
        static string strMarkets = "sz,sh";
        const string strDataType = "lday";
        const string strDataName = "day";
        AsyncOperation asyncOperation;
        
        public CommEquitAPI currAPI
        {
            get
            {
                if (useAPI == null || useAPI.Count == 0)
                    return null;
                return useAPI.First().Value;
            }
        }
        public receiveBasicData()
        {
            InitAPIs();
            asyncOperation = AsyncOperationManager.CreateOperation(null);
            //如果是非周末
            DateTime today = DateTime.Now;
            if (today.DayOfWeek != DayOfWeek.Sunday && today.DayOfWeek != DayOfWeek.Saturday)
            {

            }
        }

        void InitAPIs()
        {
            AllAPIs = new Dictionary<string, CommEquitAPI>();
            AllAPIs.Add("hexun", new EquitAPI_HeXun());
            AllAPIs.Add("sina", new EquitAPI_Sina());
            AllAPIs.Add("neteasy", new EquitAPI_NetEasy());
            AllAPIs.Add("wind", new EquitAPI_Wind());
            useAPI = new Dictionary<string, CommEquitAPI>();
        }

        

        public Dictionary<string,string> loopGetAllEquits()
        {
            Dictionary<string, CommEquitAPI> loopApis = AllAPIs;
            DateTime today = DateTime.Today;
            lock (useAPI)
            {
                if (useAPI != null && useAPI.Count > 0)
                {
                    loopApis = useAPI;
                }
                foreach (string key in loopApis.Keys)
                {
                    CommEquitAPI api = loopApis[key];
                    var res = api.getAllEquit(today);
                    if (res != null && res.Count > 0)
                    {
                        useAPI = new Dictionary<string, CommEquitAPI>();
                        useAPI.Add(key, api);
                        return res;
                    }
                }
                if(useAPI == null)
                {
                    throw new Exception("所有的接口都无法使用！");
                }
            }
            return new Dictionary<string, string>();
        }

        public MongoReturnDataList<T> loopGetEquitSerialData<T>(StockInfoMongoData simd,DateTime begt,DateTime endt) where T:TimeSerialData
        {
            Dictionary<string, CommEquitAPI> loopApis = AllAPIs;
            DateTime today = DateTime.Today;
            //lock (useAPI)
            //{
                if (useAPI != null && useAPI.Count > 0)
                {
                    loopApis = useAPI;
                }
                foreach (string key in loopApis.Keys)
                {
                    CommEquitAPI api = loopApis[key];
                List<string> urls = null;
                    var res = api.getSerialData<T>(simd, begt,endt,ref urls);
                    if (res != null && res.Count > 0)
                    {
                        useAPI = new Dictionary<string, CommEquitAPI>();
                        useAPI.Add(key, api);
                        return res;
                    }
                }
            //}
            return new MongoReturnDataList<T>(simd);
        }

        public Dictionary<string, string> getLocalEquits()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            string[] mkts = strMarkets.Split(',');
            string model = @"{0}\{1}\{2}\{3}";
            for(int i=0;i<mkts.Length;i++)
            {
                string strMarket = mkts[i];
                string path = string.Format(model, AppDomain.CurrentDomain.BaseDirectory, strDataFolder, strMarket, strDataType);
                string[] files = Directory.GetFiles(path);
                for(int f = 0;f<files.Length;f++)
                {
                    string strReg = @"(\d{6}).day$";
                    Regex reg = new Regex(strReg);
                    MatchCollection ms = reg.Matches(files[f]);
                    if (ms.Count == 0)
                        continue;
                    string code  = ms[0].Value.Replace("day", strMarket.ToUpper());
                    if(!ret.ContainsKey(code))
                    {
                        ret.Add(code, "");
                    }
                }
            }
            return ret;
         }





        
        /*
        public DateTime[] getAllDays(DateTime from,DateTime to)
        {
            try
            {
                return WDDayClass.getTradeDates(null, from, to);
            }
            catch
            {
                return null;
            }
        }
        */

    }
}
