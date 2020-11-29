using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using WolfInv.com.StrategyLibForWD;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.WinInterComminuteLib;

namespace WolfInv.com.WDDataInit
{
    /// <summary>
    /// 初始化万德数据
    /// </summary>
    public class WDDataInit<T>: RemoteServerClass where T:TimeSerialData
    {
        public static string vipDocRoot;
        public static bool Debug = false;
        public static DateTime[] AllDays;
        public static bool Loading;
        static Dictionary<string, string> _AllEquits;

        public static Dictionary<string, string> AllSecurities
        {
            get
            {
                return _AllEquits;
            }
            set
            {
                _AllEquits = value;
            }
        }
       

        public static MongoDataDictionary<T> getAllSerialData()
        {
            return _AllEquitSerialData;
        }

        public static MongoDataDictionary<T> _AllEquitSerialData;
        public MongoReturnDataList<T> getEquitSerialData(string key,string name)
        {
            if(_AllEquitSerialData.ContainsKey(key))
            {
                return _AllEquitSerialData[key];
            }
            return new MongoReturnDataList<T>(new StockInfoMongoData(key,name));
        }
        public static bool Loaded;
        
        static Dictionary<string, DateTime> t_onMarkDays;
        static receiveBasicData rcbd = new receiveBasicData();
        static List<Task> tasks = new List<Task>();
        static MongoDataDictionary<StockMongoData> localData;
        static string ErrorMsg;
        static int processCnt;
        public static Action<int, int,EquitProcess<T>.EquitUpdateResult> finishedMsg;
        public static bool OnLine;
        

        public static string getErrorMsg()
        {
            return ErrorMsg;
        }

        public static bool Init()
        {
            
            
            OnLine = true;
            if(AllDays == null || AllDays?.Length == 0)
            {
                OnLine = false;
                //ErrorMsg = "交易日期为空！";
                //return false;
            }
            _AllEquits = rcbd.loopGetAllEquits();
            if (_AllEquits == null || _AllEquits.Count == 0)
            {
                _AllEquits = rcbd.getLocalEquits();
            }
            InitExchangeDays();
            //loadAllEquitSerials();
            return true;
        }

        public static void loadEquitSerial(string key,string name, bool noNeedToday = false, bool onlyLocal = false,int localDataLen=1000,string fromdate=null,string todate=null)
        {
            if(_AllEquitSerialData == null)
                _AllEquitSerialData = new MongoDataDictionary<T>();
            string[] names = key.Split('.');
            EquitProcess<T> epr = new EquitProcess<T>(rcbd.currAPI, names[0],names[1], name);
            epr.vipdocRoot = vipDocRoot;
            var res = epr.updateData(noNeedToday, onlyLocal,localDataLen, fromdate, todate);
            processCnt++;
            
                
            //lock (_AllEquitSerialData)
            //{
                if (!_AllEquitSerialData.ContainsKey(key))
                    _AllEquitSerialData.Add(key, epr.FullData);
                else
                    _AllEquitSerialData[key] = epr.FullData;
            //}
        }

        public static void loadAllEquitSerials(int threadCnt=10,int grpCnt=50, bool noNeedToday=false,bool onlyLocal=false,int localDataLen=1000, string fromdate = null, string todate = null, bool sync = false)
        {
            Loading = true;
            processCnt = 0;
            if(Debug)
            {
                _AllEquits = new Dictionary<string, string>();
                _AllEquits.Add("002624.SZ", "中国平安");
            }
            int allCnt = _AllEquits.Count;
            _AllEquitSerialData = new MongoDataDictionary<T>();
            var items = _AllEquits.Keys.ToList();
            int step = grpCnt>1?grpCnt:(int)Math.Sqrt(items.Count);
            int index = 0;
            List<List<string>> grps = new List<List<string>>();
            tasks = new List<Task>();
            while (index < items.Count)
            {
                
                List<string> batchItems = items.Skip(index).Take(step).ToList();
                if (batchItems.Count == 0)
                    break;
                index += step;
                Task tk = Task.Factory.StartNew(
                    (obj) =>
                    {
                        List<string> list = obj as List<string>;
                        for (int s = 0; s < list.Count; s++)
                        {
                            string key = list[s];
                            string name = null;
                            if (_AllEquits.ContainsKey(key))
                            {
                                name = _AllEquits[key];
                            }
                            string[] names = key.Split('.');
                            if (names.Length < 2)
                            {
                                return;
                            }
                            EquitProcess<T> epr = new EquitProcess<T>(rcbd.currAPI, names[0], names[1], name);
                            epr.vipdocRoot = vipDocRoot;
                            var res = epr.updateData(noNeedToday, onlyLocal, localDataLen,fromdate,todate);
                            processCnt++;
                            finishedMsg?.Invoke(processCnt, allCnt, res);
                            if (processCnt >= allCnt)//内部计算，如果处理完成了，就将状态置为已加载完成
                            {
                                Loaded = true;
                                Loading = false;
                            }
                            lock (_AllEquitSerialData)
                            {
                                if (!_AllEquitSerialData.ContainsKey(key))
                                    _AllEquitSerialData.Add(key, epr.FullData);
                                else
                                    _AllEquitSerialData[key] = epr.FullData;
                            }
                        }
                    }, batchItems
                    );
                if(tasks.Count< threadCnt)
                {
                    tasks.Add(tk);
                }
                else
                {
                    tasks.Add(tk);
                    int finished = Task.WaitAny(tasks.ToArray());
                    tasks.RemoveAt(finished);
                    
                }
                //Task.WaitAll(tasks.ToArray());
            }
            
            
            if (sync)
            {
                Task.WaitAll(tasks.ToArray());
                Loading = false;
                Loaded = true;
            }
        }

        static void InitExchangeDays()
        {
            AllDays = new DateTime[] { };
            loadEquitSerial("000001.SH", "上证综指", false, false, 100000);
            if(_AllEquitSerialData != null)
            {
                if(_AllEquitSerialData.ContainsKey("000001.SH"))
                {
                    AllDays = _AllEquitSerialData["000001.SH"].Select(a => a.Expect.ToDate()).ToArray();
                    _AllEquitSerialData = null;
                }
            }
            // AllDays = rcbd.getAllDays(DateTime.Parse("1990-01-01"), DateTime.Today);
        }
    }


}
