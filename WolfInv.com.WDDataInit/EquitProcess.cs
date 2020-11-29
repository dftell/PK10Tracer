using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
//using WolfInv.com.StrategyLibForWD;
using System.Data;
using WolfInv.com.BaseObjectsLib;

namespace WolfInv.com.WDDataInit
{
    public class EquitProcess<T> where T : TimeSerialData
    {
        CommEquitAPI commAPI;
        const string strDataFolder = "vipdoc";
        const string strDataType = "lday";
        const string strDataName = "day";
        string sec_code;
        string sec_name;
        string sec_FullCode
        {
            get
            {
                if (sec_code.Contains("."))
                {
                    return sec_code;
                }
                else
                    return sec_code.WDCode();
            }
        }
        string market;

        
        public EquitProcess(CommEquitAPI api, string code, string name = null)
        {
            string[] codes = code.Split('.');
            EquitProcess_init(api, codes[0], codes[1], name);
        }

        public EquitProcess(CommEquitAPI api, string code, string mk, string name = null)
        {
            EquitProcess_init(api, code, mk, name);
        }
        public void EquitProcess_init(CommEquitAPI api,string code,string mk,string name=null)
        {
            commAPI = api;
            sec_code = code;
            market = mk;
            sec_name = name;
        }

        public string vipdocRoot;

        public string DayFileName
        {
            get
            {
                return string.Format("{4}\\{0}\\{1}\\{3}\\{1}{2}.day", strDataFolder,market,sec_code,strDataType, AppDomain.CurrentDomain.BaseDirectory+vipdocRoot ??"");
            }
        }

        MongoReturnDataList<T> LocalData;
        public MongoReturnDataList<T> FullData;
        public class EquitUpdateResult
        {
            public string code;
            public string name;
            public bool succ;
            public bool Updated;
            public bool LoadedLocalData;
            public int LocalDataCount;
            public string LocalLastDate;
            public bool GetWebData;
            public string WebBegT;
            public int WebDataCount;
            public string WebDataLastDate;
            public string Msg;
            public string MsgDetail;
            public List<string> WebUrls=new List<string>();
        }
        public EquitUpdateResult updateData(bool noNeedToday=false,bool onlyLocal=false ,int localDataLen=1000,string fromdate=null,string todate=null)
        {
            EquitUpdateResult ret = new EquitUpdateResult();
            ret.code = this.sec_code;
            ret.name = this.sec_name;
            StockInfoMongoData simd = new StockInfoMongoData(sec_FullCode, sec_name);
            try
            {
                DateTime endT = DateTime.Today;
                if (noNeedToday)
                {
                    endT = endT.AddDays(-1);
                }
                LocalData = CollectCellFromTDX(simd,DayFileName, localDataLen,fromdate,todate);//获取数据
                if (LocalData == null)
                {
                    //ret.succ = false;
                    ret.Msg = "无法读取到本地数据";
                    ret.MsgDetail = "文件不存在";
                }
                else
                {
                    //LocalData.ForEach(a => a.Key = ret.code);
                    ret.LoadedLocalData = true;
                    ret.LocalDataCount = LocalData.Count;
                    if (LocalData.Count > 0)
                        ret.LocalLastDate = LocalData.Last().Expect;
                }
                DateTime begT = DateTime.Parse("1990-01-01");
                string expect = null;
                if (LocalData != null && LocalData.Count > 0)
                {
                    expect = LocalData.Last().Expect;
                    begT = expect.ToDate() ;
                }
                
                ret.succ = true;
                if (LocalData == null)
                {
                    LocalData = new MongoReturnDataList<T>(simd);
                }
                FullData = LocalData;
                if (onlyLocal)
                {
                    ret.succ = false;
                    return ret;
                }
                bool updated = false;
                if (noNeedToday)//不需要今天的数据
                {
                    if (LocalData.Count > 0)//&& LocalData.Last().Key.Equals(endT.ToString("yyyy-MM-dd")))//如果最后的数据等于今天的数据
                    {
                        DateTime maxDay = LocalData.Last().Expect.ToDate();
                        if (maxDay.Date >= endT.Date)
                        {
                            ret.succ = true;
                            ret.Updated = false;
                            return ret;
                        }
                    }
                }
                ret.WebBegT = begT.WDDate() ;
                MongoReturnDataList<T> wdData = commAPI.getSerialData<T>(simd, begT, endT,ref ret.WebUrls); 
                if(wdData!= null)
                {
                    ret.GetWebData = true;
                    ret.WebDataCount = wdData.Count;
                    if (ret.WebDataCount > 0)
                        ret.WebDataLastDate = wdData.Last().Expect;
                }
                if (LocalData.Count > 0 && wdData.Count == 1 && wdData.First().Expect == LocalData.Last().Expect)//不要更新
                {
                    ret.succ = true;
                    ret.Updated = false;
                    return ret;
                }
                foreach (string key in wdData.Keys)
                {
                    if (FullData.Where(a=>a.Expect ==  key).Count()>0)
                    {
                        ////foreach (var item in FullData.Where(a => a.Expect == key))
                        ////{
                        ////    //item.close  =  key.close;
                        ////}
                    }
                    else
                    {
                        FullData.Add(key, wdData[key]);
                    }
                }
                if (FullData.Count > 0)
                {
                    
                    ret.Updated = WriteToTDX(DayFileName, FullData.OrderBy(a => a.Expect).ToList());
                    if(!ret.Updated)
                    {
                        ret.Msg = "保存失败！";
                        ret.MsgDetail = "无法写入到文件！";
                    }
                }
            }
            catch (Exception ce)
            {
                ret.succ = false;
                ret.Updated = false;
                ret.Msg = ce.Message;
                ret.MsgDetail = ce.StackTrace;
                return ret;
            }
            ret.succ = true;
            return ret;
        }

        
        bool isBetween(string expect,string from=null,string to=null)
        {
            if(string.IsNullOrEmpty(from) && string.IsNullOrEmpty(to))
            {
                return true;
            }
            bool inRange = false;
            if(!string.IsNullOrEmpty(from))
            {
                if(expect.ToDate()>=from.ToDate())
                {
                    inRange = true;
                }
            }
            else
            {
                inRange = true;
            }
            if (!inRange)
                return false;
            inRange = false;
            if (!string.IsNullOrEmpty(to))
            {
                if (expect.ToDate() <= to.ToDate())
                {
                    inRange = true;
                }
            }
            else
            {
                inRange = true;
            }
            return inRange;
        }
        private MongoReturnDataList<T> CollectCellFromTDX(StockInfoMongoData simd, string filename,int defaultDays = 1000,string fromdate=null,string todate=null)// string Code,string Market="SH")
        {
            if(!string.IsNullOrEmpty(fromdate) &&!string.IsNullOrEmpty(todate))//如果提供了前后时间，默认长度无效
            {
                defaultDays = 0;
            }
            string path = filename;// string.Format("{2}\\Vipdoc\\{0}\\lday\\{0}{1}.day", Market, Market, ConstData.TDXPath);
            if (!File.Exists(path))
            {
                //return new MongoReturnDataList<T>();
                return null;
            }
            FileStream fs = File.OpenRead(path);
            BinaryReader br = new BinaryReader(fs);
            //共计40字节
            //日期 8字节 long
            //开盘价 4字节
            //最高价 4字节
            //最低价 4字节
            //收盘价 4字节
            //成交额度 4字节
            //成交量 8字节 long
            //前期价 4字节
            int sectorLen = 40;
            int days = (int)fs.Length / sectorLen;
            string[] allDates = new string[days];
            bool haveHeader = true;
            for (int i=0;i<days;i++)
            {
                string date = br.ReadInt32().ToString();
                DateTime res;
                bool succ = DateTime.TryParseExact(date, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out res);
                if(!succ)
                {
                    haveHeader = false;
                    break;
                }
                allDates[i] = date.WDDate("yyyyMMdd");
                if(i>0)
                {
                    if(allDates[i].ToDate()<=allDates[i-1].ToDate())
                    {
                        haveHeader = false;
                        break;
                    }
                }
            }
            if (!haveHeader)//如果没有头，关闭
            {
                br.Close();
                fs.Close();
                br = null;
                fs = null;
                return new MongoReturnDataList<T>(simd);
            }
            List<int> dates = new List<int>();
            bool lastInRange = false;
            for (int i=0;i<allDates.Length;i++)
            {
                bool succ = isBetween(allDates[i], fromdate, todate);
                if (succ)
                {
                    dates.Add(i);
                }
                else
                {
                    if(lastInRange)
                    {
                        break;
                    }
                }
                lastInRange = succ;
            }
            int[] useList = dates.ToArray();
            if(defaultDays>0)
            {
                if(string.IsNullOrEmpty(fromdate))//如果只有截止日期
                {
                    useList = dates.Skip(Math.Max(0,dates.Count - defaultDays)).ToArray();//取前面的
                }
                else
                {
                    useList = dates.Take(Math.Min(dates.Count, defaultDays)).ToArray();
                }
            }
            if(useList.Length== 0)
            {
                return new MongoReturnDataList<T>(simd);
            }
            int useDays = days;
            int skipDays = 0;
            useDays = useList.Length;
            skipDays = useList.First();
            br.ReadBytes((sectorLen-4)*skipDays);//头占了4个字节*日期数
            MongoReturnDataList<T> list = new MongoReturnDataList<T>(simd);
            try
            {
                for (int i = 0; i < useDays; i++)
                {
                    StockMongoData item = new StockMongoData();
                    item.Key = simd.Key;
                    //item.DataDate =DateTime.Parse(new string(br.ReadChars(8)));
                    item.Expect = allDates[skipDays + i];
                    item.date = item.Expect;
                    string currDate = br.ReadInt32().ToString().WDDate("yyyyMMdd");
                    if (!currDate.Equals(item.Expect))
                    {
                        break;
                    }
                    int open = br.ReadInt32();
                    int high = br.ReadInt32();
                    int low = br.ReadInt32();
                    int close = br.ReadInt32();
                    Single amount = br.ReadSingle();
                    //Int32 amount = br.ReadInt32();
                    decimal am = Convert.ToDecimal(amount);
                    long amstr = Convert.ToInt64(amount);
                    long vol = br.ReadInt64();
                    int preclose = br.ReadInt32();
                    //Decimal open = Convert.ToDecimal(br.ReadSingle());                 
                    item.open = Convert.ToDouble(open / 100m);
                    item.high = Convert.ToDouble(high / 100m);
                    item.low = Convert.ToDouble(low / 100m);
                    item.close = Convert.ToDouble(close / 100m);
                    item.amount = amstr;
                    item.preclose = Convert.ToDouble(preclose / 100m);
                    item.vol = vol;
                    //item.Code = stockInfo.Code;
                    //item.Name = stockInfo.Name;
                    list.Add(item as T);
                    item = null;
                }
            }
            catch (Exception ce)
            {

            }
            finally
            {
                br.Close();
                fs.Close();
                br = null;
                fs = null;
            }
            return list;
        }

        public bool WriteToTDX(string fl, List<T> el)
        {
            try
            {
                var items = el.OrderBy(a => a.Expect);
                FileStream fs = new FileStream(fl, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                double lastprice = 0;
                byte[] bs = new byte[32 * items.Count()];
                int i = 0;
                foreach(var data1 in items)//先写头
                {
                    string dt = data1.Expect.WDDate("yyyy-MM-dd", "yyyyMMdd");
                    int dtlng = int.Parse(dt);
                    bw.Write(dtlng);
                }
                foreach (var data1 in items)
                {
                    StockMongoData data = data1 as StockMongoData;
                    string dt = data.Expect.WDDate("yyyy-MM-dd","yyyyMMdd");
                    int ldt = int.Parse(dt);
                     
                    bw.Write(ldt);
                    bw.Write(Convert.ToInt32(data.open * 100));
                    bw.Write(Convert.ToInt32(data.high * 100));
                    bw.Write(Convert.ToInt32(data.low * 100));
                    bw.Write(Convert.ToInt32(data.close * 100));
                    bw.Write(Convert.ToSingle(data.amount));
                    bw.Write(Convert.ToInt64(data.vol));
                    bw.Write(Convert.ToInt32(data.preclose * 100));
                    lastprice = data.close;
                    /**/
                    
                    i++;
                }

                // 设置当前流的位置为流的开始 
                bw.Flush();
                bw.Close();
                fs.Close();
                return true;
            }
            catch(Exception ce)
            {
                throw ce;
                return false;
            }
        }

        
    }
}
