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
       

        public static MongoDataDictionary<T> getAllSerialData(string codes=null)
        {
            
            if (_AllEquitSerialData == null || string.IsNullOrEmpty(codes))
                return _AllEquitSerialData;
            string[] arr = codes.Split(';');
            MongoDataDictionary<T> ret = new MongoDataDictionary<T>(true);
            for(int i=0;i<arr.Length;i++)
            {
                if(_AllEquitSerialData.ContainsKey(arr[i].WDCode()))
                {
                    ret.TryAdd(arr[i].WDCode(),_AllEquitSerialData[arr[i].WDCode()]);
                }
            }
            return ret;
        }

        public static MongoDataDictionary<T> _AllEquitSerialData = new MongoDataDictionary<T>(true);
        public MongoReturnDataList<T> getEquitSerialData(string key,string name)
        {
            if(_AllEquitSerialData.ContainsKey(key))
            {
                return _AllEquitSerialData[key];
            }
            return new MongoReturnDataList<T>(new StockInfoMongoData(key,name),true);
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
                _AllEquitSerialData = new MongoDataDictionary<T>(true);
            string[] names = key.Split('.');
            if(names.Length<2)
            {
                return;
            }
            EquitProcess<T> epr = new EquitProcess<T>(rcbd.currAPI,key,names[1], name);
            epr.vipdocRoot = vipDocRoot;
            var res = epr.updateData(noNeedToday, onlyLocal,localDataLen, fromdate, todate);
            //processCnt++;
            
                
            //lock (_AllEquitSerialData)
            //{
                if (!_AllEquitSerialData.ContainsKey(key))
                    _AllEquitSerialData.TryAdd(key, epr.FullData);
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
            _AllEquitSerialData = new MongoDataDictionary<T>(true);
            var items = _AllEquits.Keys.ToList();
            WolfTaskClass.MultiTaskProcess<string, string, string, string>
                (
                    items,_AllEquits,new Dictionary<string,string>(),
                    (key,val,eqts,list,notice)=>
                    {
                        
                        string[] names = key.Split('.');
                        string mk = "";
                        if (names.Length > 1)
                            mk = names[1];
                        EquitProcess<T> epr = new EquitProcess<T>(rcbd.currAPI, key, mk, val);
                        epr.vipdocRoot = vipDocRoot;
                        try
                        {
                            var res = epr.updateData(noNeedToday, onlyLocal, localDataLen, fromdate, todate);
                            processCnt++;
                            finishedMsg?.Invoke(processCnt, allCnt, res);
                            notice?.Invoke(key);
                            if (processCnt >= allCnt)//内部计算，如果处理完成了，就将状态置为已加载完成
                            {
                                Loaded = true;
                                Loading = false;
                            }
                            if (!_AllEquitSerialData.ContainsKey(key))
                                _AllEquitSerialData.TryAdd(key, epr.FullData);
                            else
                                _AllEquitSerialData[key] = epr.FullData;
                            //GC.Collect();
                        }
                        finally
                        {
                            epr = null;
                        }
                    },
                    (key) => {
                        //Task.Factory.StartNew(() =>
                        //{
                            
                        //});
                    },
                    threadCnt, grpCnt, true
                );

            return;

           
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
                    AllDays = AllDays.OrderBy(a => a).ToArray();
                    _AllEquitSerialData = null;
                }
            }
            // AllDays = rcbd.getAllDays(DateTime.Parse("1990-01-01"), DateTime.Today);
        }
    }


}
