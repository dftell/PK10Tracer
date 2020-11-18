using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
//using WolfInv.com.StrategyLibForWD;
using System.Data;
using WolfInv.com.BaseObjectsLib;

namespace WolfInv.com.WDDataInit
{
    public class EquitProcess<T> where T:TimeSerialData
    {
        CommEquitAPI commAPI;
        const string strDataFolder = "vipdoc";
        const string strDataType = "lday";
        const string strDataName = "day";
        string sec_code;
        string sec_name;
        string market;

        public string FullCode
        {
            get
            {
                return string.Format("{0}.{1}", sec_code, market);
            }
        }
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
        public struct EquitUpdateResult
        {
            public string code;
            public string name;
            public bool succ;
            public bool Updated;
            public string Msg;
            public string MsgDetail;
        }
        public EquitUpdateResult updateData(bool noNeedToday=false,bool onlyLocal = false,int localDataLen = 1000)
        {
            EquitUpdateResult ret = new EquitUpdateResult();
            ret.code = this.sec_code;
            ret.name = this.sec_name;
            try
            {
                DateTime endT = DateTime.Today;
                if (noNeedToday)
                {
                    endT = endT.AddDays(-1);
                }
                LocalData = CollectCellFromTDX(this.sec_code,DayFileName, localDataLen);//获取数据
                if (LocalData == null)
                {

                    //ret.succ = false;
                    ret.Msg = "无法读取到本地数据";
                    ret.MsgDetail = "文件不存在";

                }
                else
                {
                    LocalData.ForEach(a => a.Key = ret.code);
                }
                DateTime begT = DateTime.Parse("1990-01-01");
                string expect = null;
                if (LocalData != null && LocalData.Count > 0)
                {
                    expect = LocalData.Last().Expect;
                    begT = expect.ToDate() ;
                }

                FullData = LocalData;
                ret.succ = true;
                if (onlyLocal)
                {
                    if(LocalData == null)
                    {
                        ret.succ = false;
                        FullData = new MongoReturnDataList<T>();
                    }
                    return ret;
                }
                bool updated = false;
                if (noNeedToday)//不需要今天的数据
                {
                    if (LocalData.Count > 0)//&& LocalData.Last().Key.Equals(endT.ToString("yyyy-MM-dd")))//如果最后的数据等于今天的数据
                    {
                        DateTime maxDay = LocalData.Last().Expect.ToDate();
                        if (maxDay >= endT)
                        {
                            ret.succ = true;
                            ret.Updated = false;
                            return ret;
                        }
                    }

                }
                MongoReturnDataList<T> wdData = commAPI.getSerialData<T>(FullCode, begT, endT);
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
                        foreach (var item in FullData.Where(a => a.Expect == key))
                        {
                            //item.close  =  key.close;
                        }
                    }
                    else
                    {
                        FullData.Add(key, wdData[key]);
                    }
                }
                if (FullData.Count > 0)
                {
                    ret.Updated = WriteToTDX(DayFileName, FullData);
                    if(!ret.Updated)
                    {
                        ret.Msg = "保存失败！";
                        ret.Msg = "无法写入到文件！";
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

        

        private MongoReturnDataList<T> CollectCellFromTDX(string key,string filename,int defaultDays = 1000)// string Code,string Market="SH")
        {
            
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
            int useDays = days;
            int skipDays = 0;
            if(defaultDays>0)
            {
                if (defaultDays < days)
                {
                    useDays = defaultDays;
                    skipDays = days - defaultDays;
                }
            }
            
            br.ReadBytes(sectorLen*skipDays);
            MongoReturnDataList<T> list = new MongoReturnDataList<T>();
            for (int i = 0; i < useDays; i++)
            {               
                StockMongoData item = new StockMongoData();
                //item.DataDate =DateTime.Parse(new string(br.ReadChars(8)));
                long date = br.ReadInt64();
                long year = date / 10000;
                int month = int.Parse(date.ToString().Substring(4, 2));
                int day = int.Parse(date.ToString().Substring(6, 2));
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
                item.date  = new DateTime((int)year, month, day).WDDate();
                item.Expect = item.date;
                item.open = Convert.ToDouble(open / 100m);
                item.high = Convert.ToDouble(high / 100m);
                item.low = Convert.ToDouble(low / 100m);
                item.close = Convert.ToDouble(close / 100m);
                item.amount = amstr;
                item.preclose = preclose;
                item.vol = vol;
                //item.Code = stockInfo.Code;
                //item.Name = stockInfo.Name;
                list.Add( item as T);
                item = null;
            }
            br.Close();
            fs.Close();
            br = null;
            fs = null;
            return list;
        }

        public bool WriteToTDX(string fl, MongoReturnDataList<T> el)
        {
            try
            {
                var items = el.OrderBy(a => a.Expect);
                FileStream fs = new FileStream(fl, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                double lastprice = 0;
                byte[] bs = new byte[32 * items.Count()];
                int i = 0;
                foreach (var data1 in items)
                {
                    StockMongoData data = data1 as StockMongoData;
                    string dt = data.Expect.WDDate("yyyy-MM-dd","yyyyMMdd");
                    long ldt = long.Parse(dt);
                     
                    bw.Write(ldt);
                    bw.Write(Convert.ToInt32(data.open * 100));
                    bw.Write(Convert.ToInt32(data.high * 100));
                    bw.Write(Convert.ToInt32(data.low * 100));
                    bw.Write(Convert.ToInt32(data.close * 100));
                    bw.Write(Convert.ToSingle(data.amount));
                    bw.Write(Convert.ToInt64(data.vol));
                    bw.Write(Convert.ToInt32(lastprice * 100));
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
