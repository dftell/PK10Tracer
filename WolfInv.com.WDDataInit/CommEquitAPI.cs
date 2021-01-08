using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
//using WolfInv.com.StrategyLibForWD;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.Http;
using WolfInv.com.LogLib;


namespace WolfInv.com.WDDataInit
{
    public abstract class CommEquitAPI
    {
        public string strModel;
        public abstract Dictionary<string, string> getAllEquit(DateTime date);
        public abstract MongoReturnDataList<T> getSerialData<T>(StockInfoMongoData info, DateTime useBeginTime, DateTime endTime,ref List<string> urls) where T : TimeSerialData;

        public EquitAPIFormat DataFormat;
        protected bool needDownload;
        protected HttpClient wb;
        

        public abstract Dictionary<string, string> getSetTable(string strResult);
        public abstract MongoReturnDataList<T> getSerialTable<T>(StockInfoMongoData info, string strResult) where T:TimeSerialData;

        protected void Init()
        {
            
        }
        
        protected async Task<string> getUrl(string url,Encoding encode,Action<string> prc = null)
        {
            try
            {
                if (wb == null)
                {
                    wb = new HttpClient();
                    System.Net.ServicePointManager.DefaultConnectionLimit = 512;

                }
                if (prc == null)
                {
                    HttpResponseMessage res = await wb.GetAsync(url);
                    if (res != null && res.IsSuccessStatusCode)
                    {
                        return await res.Content.ReadAsStringAsync();
                    }//return wb.DownloadString(url);
                }
                else
                {
                    Task<HttpResponseMessage> tk = wb.GetAsync(url);
                    tk.Start();
                    if(tk.IsCompleted)
                    {
                        return await tk.Result.Content.ReadAsStringAsync();
                    }
                }
            }
            catch(Exception ce)
            {

            }
            return null;
        }
    }

    public enum EquitAPIFormat
    {
        Table,Json,CSV
    }

    public class EquitAPI_Wind:CommEquitAPI
    {
        public override Dictionary<string, string> getAllEquit(DateTime date)
        {
            return new Dictionary<string, string>();
            /*
            MTable tab = CommWSSToolClass.getBkList(null, "a001010100000000", date, false);
            Dictionary<string, string> ret = new Dictionary<string, string>();
            if (tab == null || tab.Count == 0)
            {
                return ret;
            }
            for (int i = 0; i < tab.Count; i++)
            {
                ret.Add(tab[i, "wind_code"]?.ToString(), tab[i, "sec_name"]?.ToString());
            }
            return ret;
            */
        }

        public override Dictionary<string, string> getSetTable(string strResult)
        {
            return null;
        }

        public override MongoReturnDataList<T> getSerialTable<T>(StockInfoMongoData info, string strResult)
        {
            return null;
        }

        public override MongoReturnDataList<T> getSerialData<T>(StockInfoMongoData info, DateTime useBeginTime, DateTime endTime,ref List<string> urls)
        {
            return new MongoReturnDataList<T>(info,true);
            /*
            try
            {
                return CommWSDToolClass.GetSerialData(key, useBeginTime, endTime, BaseObjectsLib.Cycle.Day, BaseObjectsLib.PriceAdj.Fore);
            }
            catch (Exception ce)
            {
                return new ExpectList();
            }
            */
        }
    }

    public class EquitAPI_Sina : CommEquitAPI
    {
        public override Dictionary<string, string> getAllEquit(DateTime date)
        {
            return null;
        }

        public override MongoReturnDataList<T> getSerialData<T>(StockInfoMongoData info, DateTime useBeginTime, DateTime endTime, ref List<string> urls)
        {
            return null;
        }

        public override MongoReturnDataList<T> getSerialTable<T>(StockInfoMongoData info, string strResult)
        {
            return null;
        }

        public override Dictionary<string, string> getSetTable(string strResult)
        {
            return null;
        }
    }
    public class EquitAPI_NetEasy : CommEquitAPI
    {
        public override Dictionary<string, string> getAllEquit(DateTime date)
        {
            return null;
        }

        public override MongoReturnDataList<T> getSerialData<T>(StockInfoMongoData info, DateTime useBeginTime, DateTime endTime, ref List<string> urls)
        {
            return null;
        }

        public override MongoReturnDataList<T> getSerialTable<T>(StockInfoMongoData info, string strResult)
        {
            return null;
        }

        public override Dictionary<string, string> getSetTable(string strResult)
        {
            return null;
        }
    }

    public class EquitAPI_HeXun:CommEquitAPI
    {
        public EquitAPI_HeXun()
        {
            needDownload = true;
            strModel = "http://flashquote.stock.hexun.com/Quotejs/DA/{0}_{1}_DA.html";
            strModel = "http://webstock.quote.hermes.hexun.com/a/kline?code=sse{0}&start={1}&number={2}&type=5&callback=";


        }

        public override Dictionary<string, string> getAllEquit(DateTime date)
        {
            string strUrlForAllEquit = "http://quote.tool.hexun.com/hqzx/quote.aspx?type=2&market=0&sorttype=3&updown=up&page=1&count={0}";
            Dictionary<string, string> ret = new Dictionary<string, string>();
            string strRes = getUrl(string.Format(strUrlForAllEquit,10000),Encoding.Default)?.Result;
            if(strRes == null)
            {
                return ret;
            }
            return getSetTable(strRes);
        }

        

        public override Dictionary<string,string> getSetTable(string strResult)
        {
            string strReg = @"dataArr = ([\s\S]*);StockListPage";
            Dictionary<string, string> ret = new Dictionary<string, string>();
            Regex reg = new Regex(strReg);
            var matchs = reg.Matches(strResult);
            if (matchs.Count == 0)
                return ret;
            string strKeys = matchs[0].Groups[1].Value;
            string strJson = "{\"data\":"+strKeys+"}";
            HexunEquitClass res = HexunEquitClass.FromJson<HexunEquitClass>(strJson);
            if(res == null)
            {
                return ret;
            }
            res.data.ForEach(a => {
                if(!ret.ContainsKey(a[0].WDCode()))
                {
                    ret.Add(a[0].WDCode(), a[1]);
                }
            });
            return ret;
        }

        class HexunEquitClass:JsonableClass<HexunEquitClass>
        {
            public List<string[]> data;
        }

    

        public override MongoReturnDataList<T> getSerialTable<T>(StockInfoMongoData info, string strResult)
        {
            string startString = "hexundata(";
            string endString = ");";
            string strReg = @"hexunData\([\s\S]*)\);";
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>(info,true);
            if(strResult == null || !strResult.StartsWith(startString) || !strResult.EndsWith(endString))
            {
                return ret;
            }
            try
            {

                string strKeys = strResult.Substring(startString.Length);
                strKeys = strKeys.Substring(0, strKeys.Length - endString.Length);
                string strJson = strKeys;
                HexunSerialDataClass res = HexunSerialDataClass.FromJson<HexunSerialDataClass,string,Int64>(strJson,Int64.Parse,2);
                if (res == null || res.Data.Count <= 4)
                {
                    throw new Exception(strJson);
                    return ret;
                }
                int weight = (int)res.Data[res.Data.Count-1][0][0];
                for (int i = 0; i < res.Data[0].Count; i++)
                {
                    List<long> a = res.Data[0][i];
                    string datetime = a[0].ToString().WDDate("yyyyMMddHHmmss");
                    if (ret.Where(b=>b.Expect == datetime).Count()==0)
                    {
                        StockMongoData ed = new StockMongoData();
                        ed.Expect = datetime;
                        /*hexundata({ "KLine":[
                         * { "Time":"时间"},
                         * { "LastClose":"前收盘价"},
                         * { "Open":"开盘价"},
                         * { "Close":"收盘价"},
                         * { "High":"最高价"},
                         * { "Low":"最低价"},
                         * { "Volume":"成交量"},
                         * { "Amount":"成交额"}],
                         * "TABLE":[
                         * {"KLine[]":"K线列表"},
                         * {"BeginTime":"最早时间"},
                         * {"EndTime":"最后时间"},
                         * {"Total":"总数"},
                         * {"PriceWeight":"价格倍数"}],
                         * "Data":[[[19901221000000,
                         * 32030,
                         * 33630,
                         * 33630,
                         * 33630,
                         * 33630,
                         * 100,
                         * 1000],[19901224000000,33630,33630,35320,35320,33630,1700,23000]],19901221000000,20201110000000,7119,100]});
                        */
                        
                        //string DateTime = datetime;
                        //ed.date = datetime;
                        ed.preclose = Convert.ToDouble(a[1]) / weight;
                        ed.open = Convert.ToDouble(a[2]) / weight;
                        ed.close = Convert.ToDouble(a[3]) / weight;
                        ed.high = Convert.ToDouble(a[4]) / weight;
                        ed.low = Convert.ToDouble(a[5]) / weight;
                        ed.vol = Convert.ToDouble(a[6]);
                        ed.amount = Convert.ToInt64(a[7]);


                        ret.Add(ed as T);
                    }
                }
                var items = ret.OrderBy(a=>a.Expect);
                MongoReturnDataList<T> rres = new MongoReturnDataList<T>(info,true);
                foreach(var item in items)
                {
                    rres.Add(item);
                }
                return rres;
            }
            catch(Exception ce)
            {
                throw ce;
            }

            return ret;
        }

        class HexunSerialDataClass:JsonableClass<string>
        {
            //public string[] KLine;
            [ChangeType(typeof(List<List<long>>), typeof(long))]
            public List<List<List<long>>> Data { get; set; }
            //public string[] TABLE;
        }

        class SerialData:List<listObject>
        {
            
        }

        class listObject:List<long>
        {

        }

        public override MongoReturnDataList<T> getSerialData<T>(StockInfoMongoData info, DateTime useBeginTime, DateTime endTime,ref List<string> urls)
        {
            urls = new List<string>();
            string strSerialModel = "http://webstock.quote.hermes.hexun.com/a/kline?code={3}{0}&start={1}&number={2}&type=5&callback=hexunData";
            string[] codes = info.code.Split('.');
            string mkt = codes[1].ToUpper().Equals("SZ") ? "SZSE" : "SSE";
            string url = string.Format(strSerialModel, codes[0], useBeginTime.ToString("yyyyMMddHHmmss"),Math.Max(1,endTime.Subtract(useBeginTime).TotalDays+1),mkt);
            urls.Add(url);
            string data = getUrl(url, Encoding.UTF8)?.Result;
            MongoReturnDataList<T> ret = getSerialTable<T>(info,data);
            if (ret == null || ret.Count == 0)
            {
                throw new Exception(data);
                return new MongoReturnDataList<T>(info,true);
            }
            DateTime lastDate;
            if (!DateTime.TryParse(ret.Last().Expect,out lastDate))
            {
               // LogableClass.ToLog("调试",string.Format("{0}:{1},请求:{2}",ret.Last().Key,ret.Last()?.ToDetailString(),url),data);
            }
            lastDate = ret.Last().Expect.ToDate();//最后日期
            int totalCnt = 0;
            while(endTime.Subtract(lastDate).TotalDays>5)//非常差距
            {
                totalCnt = ret.Count;
                string wurl = string.Format(strSerialModel, codes[0], lastDate.ToString("yyyyMMddHHmmss"), Math.Max(1, endTime.Subtract(lastDate).TotalDays+1),mkt);
                urls.Add(wurl);
                data = getUrl(wurl, Encoding.UTF8)?.Result;
                MongoReturnDataList<T> tmpList = getSerialTable<T>(info,data);
                if(tmpList == null || tmpList.Count<=1)
                {
                    return ret;
                }
                foreach(string c in tmpList.Keys)
                {
                    if(ret.ContainKey(c))
                    {
                        ret[c] = tmpList[c];
                    }
                    else
                    {
                        ret.Add(c, tmpList[c]);
                    }
                }                
                lastDate = tmpList.Last().Expect.ToDate();
                if(ret.Count == totalCnt)
                {
                    break;
                }
            }
            return ret;
        }
    }
}
