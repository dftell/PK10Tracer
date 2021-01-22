using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace WolfInv.com.GuideLib
{
    public class KLineNode:StockMongoData
    {

    }
    public class KLineData<T> where T : TimeSerialData
    {
        public delegate MongoDataDictionary<T> getSingleDataFunc(string code, string expect, int cnt);
        /// <summary>
        /// 判定是否停牌用
        /// </summary>
        public string Expect;
        public string code;
        public string name;
        bool isST;
        public List<T> orgList;
        public StockMongoData[] Datas;
        public int Length;
        public int[] Indies;
        public string[] Expects;
        public double[] Closes;
        public double[] Opens;
        public double[] Highs;
        public double[] Lows;
        public double[] Vols;
        public double[] RaiseRates;
        /// <summary>
        /// 低频数据内容
        /// </summary>
        public KlineDataItem[] lowCycleData;
        
        public double OpenRaiseRate;
        public double HighRaiseRate;
        public double LowRaiseRate;
        public Cycle LineCycle;
        public bool isLowCycle
        {
            get
            {
                return LineCycle > Cycle.Day  && LineCycle<Cycle.Year;
            }
        }
        public PriceAdj Adj;
        public getSingleDataFunc getSingleData;
        public KLineData(string useExpect,List<T> list,string _code,string _name,PriceAdj priceAdj=PriceAdj.Beyond,Cycle cyc = Cycle.Day)
        {
            LineCycle = cyc;
            Expect = useExpect;
            orgList = list.Where(a => a.Expect.ToDate() <= Expect.ToDate()).OrderBy(a => a.Expect).ToList();
            init(list, _code, _name, priceAdj,cyc);
            changeCycle();
        }

        public KLineData(string useExpect,MongoReturnDataList<T> list, PriceAdj priceAdj=PriceAdj.Beyond, Cycle cyc = Cycle.Day)
        {
            Expect = useExpect;
            if(list == null)
            {

            }
            orgList = list.Where(a=>a.Expect.ToDate()<=Expect.ToDate()).OrderBy(a=>a.Expect).ToList();
            init(orgList, list.SecInfo.code , list.SecInfo.name, priceAdj,cyc);
            changeCycle();
        }
        void init(List<T> list, string _code, string _name, PriceAdj priceAdj = PriceAdj.Beyond, Cycle cyc = Cycle.Day)
        {
            try
            {
                
                code = _code;
                name = _name;
                Datas = new StockMongoData[list.Count];
                LineCycle = cyc;
                Adj = priceAdj;
                if (list == null)
                    return;
                Length = list.Count;
                Indies = new int[list.Count];
                Expects = new string[list.Count];
                Closes = new double[list.Count];
                Opens = new double[list.Count];
                Highs = new double[list.Count];
                Lows = new double[list.Count];
                Vols = new double[list.Count];
                RaiseRates = new double[list.Count];
                dynamic rx = 1;
                if (priceAdj == PriceAdj.Fore)
                {
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        StockMongoData smd = list[i] as StockMongoData;
                        Datas[i] = smd;
                        Indies[i] = i;
                        if (i < list.Count - 1)
                        {
                            StockMongoData psmd = list[i + 1] as StockMongoData;
                            rx = rx * psmd.preclose / smd.close;
                            psmd = null;
                        }
                        else
                        {
                            OpenRaiseRate = 100 * (smd.open - smd.preclose) / smd.preclose;
                            HighRaiseRate = 100 * (smd.high - smd.preclose) / smd.preclose;
                            LowRaiseRate = 100 * (smd.low - smd.preclose) / smd.preclose;
                        }
                        Expects[i] = smd.Expect;
                        Closes[i] = (smd.close * rx);
                        Closes[i] = Closes[i].ToEquitPrice(2);
                        Opens[i] = (smd.open * rx);
                        Opens[i] = Opens[i].ToEquitPrice(2);
                        Highs[i] = (smd.high * rx);
                        Highs[i] = Highs[i].ToEquitPrice(2);
                        Lows[i] = (smd.low * rx);
                        Lows[i] = Lows[i].ToEquitPrice(2);
                        Vols[i] = (smd.vol / rx);
                        Vols[i] = Vols[i].ToEquitPrice(0);
                        RaiseRates[i] = 100 * (smd.close - smd.preclose) / smd.preclose;
                        smd = null;
                    }

                }
                else
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        StockMongoData smd = list[i] as StockMongoData;
                        Datas[i] = smd;
                        Indies[i] = i;
                        if (i > 0)
                        {
                            StockMongoData psmd = list[i - 1] as StockMongoData;
                            if (priceAdj == PriceAdj.UnDo || priceAdj == PriceAdj.Target)//不复权或者定点复权
                            {
                                rx = 1;
                            }
                            else
                            {
                                rx = rx * psmd.close / smd.preclose;
                            }
                            psmd = null;
                        }
                        Expects[i] = smd.Expect;
                        Closes[i] = (smd.close * rx);
                        Closes[i] = Closes[i].ToEquitPrice(2);
                        Opens[i] = (smd.open * rx);
                        Opens[i] = Opens[i].ToEquitPrice(2);
                        Highs[i] = (smd.high * rx);
                        Highs[i] = Highs[i].ToEquitPrice(2);
                        Lows[i] = (smd.low * rx);
                        Lows[i] = Lows[i].ToEquitPrice(2);
                        Vols[i] = (smd.vol / rx);
                        Vols[i] = Vols[i].ToEquitPrice(0);
                        RaiseRates[i] = 100 * (smd.close - smd.preclose) / smd.close;
                        if (i == list.Count - 1)
                        {
                            //OpenRaiseRate = 100 * (smd.open - smd.preclose) / smd.preclose;
                            OpenRaiseRate = 100 * (smd.open - smd.preclose) / smd.preclose;
                            HighRaiseRate = 100 * (smd.high - smd.preclose) / smd.preclose;
                            LowRaiseRate = 100 * (smd.low - smd.preclose) / smd.preclose;
                        }
                        smd = null;
                    }

                }
            }
            catch (Exception ce)
            {

            }
            finally
            {
                //GC.Collect();
            }
        }

        void changeToLineData(Dictionary<string, KlineDataItem> lowCycDates)
        {
            Length = lowCycDates.Count;
            Indies = new int[Length];
            Expects = new string[Length];
            Closes = new double[Length];
            Opens = new double[Length];
            Highs = new double[Length];
            Lows = new double[Length];
            Vols = new double[Length];
            RaiseRates = new double[Length];
            lowCycleData = new KlineDataItem[Length];
            int i = 0;
            KlineDataItem lastKdi = null;
            KlineDataItem preKdi = null;
            foreach (string key in lowCycDates.Keys)
            {
                preKdi = lastKdi;
                Indies[i] = i;
                Expects[i] = key;
                KlineDataItem kdi = lowCycDates[key];
                Closes[i] = kdi.close;
                Opens[i] = kdi.open;
                Highs[i] = kdi.high;
                Lows[i] = kdi.low;
                Vols[i] = kdi.vol;
                RaiseRates[i] = i == 0 ? 0 : 100 * (kdi.close - lastKdi.close) / lastKdi.close;
                kdi.index = i;//必须在这里再指定索引
                lowCycleData[i] = kdi;
                lastKdi = kdi;
                i++;
            }
            if (Length>1)
            {
                OpenRaiseRate = 100 * (Opens.Last() - Opens[Length - 2]) / Opens[Length - 2];
                HighRaiseRate = 100 * (Highs.Last() - Highs[Length - 2]) / Highs[Length - 2];
                LowRaiseRate = 100 * (Lows.Last() - Lows[Length - 2]) / Lows[Length - 2];
            }
        }

        void changeCycle()
        {
            if(LineCycle==Cycle.Day)//如果是日线，不变。整个KLINE只支持日线及以上数据
            {
                return;
            }
            toLowCycleLine(LineCycle);
        }

        void toLowCycleLine(Cycle cyc)
        {
            if (Expects.Length <= 1)
            {
                return;
            }
            var keyList = new List<int>();
            int lessInterVal = 0;
            switch(cyc)
            {
                //年
                case Cycle.Year:
                    {
                        lessInterVal = 365;
                        keyList = Expects.OrderBy(a=>a).Select(a => (int)a.ToDate().DayOfYear).ToList();
                        break;
                    }
                    ///季
                case Cycle.Quarter:
                    {
                        lessInterVal = 31;
                        keyList = Expects.OrderBy(a => a).Select(a => ((a.ToDate().Month % 3)>0?1:0)*a.ToDate().Day).ToList();
                        break;
                    }
                case Cycle.Month:
                    {
                        lessInterVal = 30;
                        keyList = Expects.OrderBy(a => a).Select(a => a.ToDate().Day).ToList();
                        break;
                    }
                case Cycle.Week:
                default:
                    {
                        lessInterVal = 7;
                        keyList = Expects.OrderBy(a => a).Select(a =>(int) a.ToDate().DayOfWeek).ToList();
                        break;
                    }
        }
            Dictionary<string,KlineDataItem> lowCycDates = new Dictionary<string, KlineDataItem>();
            int startIndex = 0;
            KlineDataItem useItem;
            for (int i = 1; i < keyList.Count; i++)
            {
                if (keyList[i - 1] > keyList[i])//如果前面一个日期的天数大于当前天数,一定是换周
                {
                    useItem = new KlineDataItem(startIndex,i- startIndex,Expects,Opens,Closes,Highs,Lows,Vols);
                    if (lowCycDates.ContainsKey(useItem.Expect))
                    {

                    }
                    else
                    {
                        lowCycDates.Add(useItem.Expect, useItem);
                    }
                    startIndex = i;
                    continue;
                }
                //长假处理
                DateTime currDate = Expects[i].ToDate();
                DateTime preDate = Expects[i - 1].ToDate();
                if(currDate.Subtract(preDate).TotalDays>lessInterVal)//当前日期数不可能是周末
                {
                    useItem = new KlineDataItem(startIndex, i - startIndex, Expects, Opens, Closes, Highs, Lows, Vols);
                    if (lowCycDates.ContainsKey(useItem.Expect))
                    {

                    }
                    else
                    {
                        lowCycDates.Add(useItem.Expect, useItem);
                    }
                    startIndex = i;
                    continue;
                }
            }
            if(!lowCycDates.ContainsKey(Expects.Last()))
            {
                useItem = new KlineDataItem(startIndex, Expects.Length - startIndex, Expects, Opens, Closes, Highs, Lows, Vols);
                if (lowCycDates.ContainsKey(useItem.Expect))
                {

                }
                else
                {
                    lowCycDates.Add(useItem.Expect, useItem);
                }
            }
            changeToLineData(lowCycDates);
        }

        public static KLineData<T> reGetKLineData(KLineData<T> klineData, string startDate)
        {
            bool isLowCycle = false;
            if (klineData.lowCycleData != null && klineData.lowCycleData.Length > 0)
            {
                isLowCycle = true;
            }
            int slDate = klineData.Expects.IndexOf(startDate);
            if (slDate < 0)
            {
                if (!isLowCycle)
                {
                    klineData = KLineData<T>.getKlineData(klineData, klineData.code, klineData.Expects.Last(), startDate, klineData.getSingleData);
                }
                else
                {
                    if (startDate.ToDate() < klineData.lowCycleData.First().Expects.First().ToDate())
                    {
                        klineData = KLineData<T>.getKlineData(klineData, klineData.code, klineData.Expects.Last(), startDate, klineData.getSingleData);
                    }
                }
            }
            return klineData;
        }

        public static KLineData<T> getKlineData(KLineData<T> kobj,string code, string expectNo, string strDate,getSingleDataFunc getSingleData)
        {
            int cnt = (int)expectNo.ToDate().Subtract(strDate.ToDate()).TotalDays;
            MongoDataDictionary<T> codeData = getSingleData(code, expectNo, cnt);
            if (codeData.Count == 0)//无法获取到数据
            {
                return null;
            }
            return new KLineData<T>(expectNo, codeData.First().Value,kobj.Adj,kobj.LineCycle);
        }

        public KLineData<StockMongoData> Ref(int N)
        {
            if(Datas.Length<N)
            {
                return new KLineData<StockMongoData>(this.Expects[Length-N-1], new List<StockMongoData>(),code,name);
            }
            KLineData<StockMongoData> ret = new KLineData<StockMongoData>(this.Expects[Length - N - 1], Datas.ToList().Take(Length-N).ToList(),code,name);
            return ret;

        }

        public bool IsStoped()
        {
            if (this.Expects.Last() == Expect)
                return false;
            return true;
        }

        public bool IsDownStop(int index = -1,double zf=-10.00)
        {
            if (index > this.Length || index < 0)
                return false;
            return IsDownStop(this[index].Expect,zf);
        }

        /// <summary>
        /// 是否是跌停
        /// </summary>
        /// <returns></returns>
        public bool IsDownStop(string date = null,double zf=-10.0)
        {
            if(string.IsNullOrEmpty(date))
                date = this.Expects.Last();
            if(code.StartsWith("688"))//科创板
            {
                zf = -20;
            }
            else if(code.StartsWith("3"))//创业板
            {
                if(date.ToDate()>="2020-08-24".ToDate())//创业板2020-08-24后涨跌幅调整为20
                {
                    zf = -20;
                }
            }
            if(this[date].close== (this[date].preclose * (100 + zf) / 100).ToEquitPrice(2))
            {
                return true;
            }
            return false;
        }

        public bool IsUpStop(int index = -1,double zf = 10.00)
        {
            if (index > this.Length || index < 0)
                return false;
            return IsUpStop(this[index].Expect,zf);
        }
        /// <summary>
        /// 是否是涨停
        /// </summary>
        /// <returns></returns>
        public bool IsUpStop(string date = null,double zf=10.0d)
        {
            if (string.IsNullOrEmpty(date))
                date = this.Expects.Last();
            if (code.StartsWith("688"))//科创板
            {
                zf = 20;
            }
            else if (code.StartsWith("3"))//创业板
            {
                if (date.ToDate() >= "2020-08-24".ToDate())//创业板2020-08-24后涨跌幅调整为20
                {
                    zf = 20;
                }
            }
            if (this[date].close == (this[date].preclose*(100+zf)/100).ToEquitPrice(2))//如果等于昨收价*(1+涨幅)的四舍五入值，就是涨停
            {
                return true;
            }
            return false;
        }

        public StockMongoData this[string key]
        {
            get
            {
                StockMongoData ret = null;
                int subindex = 0;
                int i = ExpectIndex(key, out subindex);
                if (i < 0)
                    return null;
                
                if (subindex<0)//日线
                {
                    ret = new StockMongoData()
                    {
                        open = Opens[i],
                        close = Closes[i],
                        high = Highs[i],
                        low = Lows[i],
                        vol = Vols[i],
                        Expect = Expects[i],
                        code = code,
                    };
                    if (i > 0)
                        ret.preclose = Closes[i - 1];
                    else
                        ret.preclose = Opens[0];
                }
                else
                {
                    var item = lowCycleData[i];
                    var itemIndex = i;
                    i = subindex;
                    ret = new StockMongoData()
                    {
                        open = item.opens[i],
                        close = item.closes[i],
                        high = item.highs[i],
                        low = item.lows[i],
                        vol = item.vols[i],
                        Expect = item.Expects[i],
                        code = code
                    };
                    if(itemIndex > 0)
                    {
                        ret.preclose = lowCycleData[itemIndex - 1].close;
                    }
                    else
                    {
                        ret.preclose = lowCycleData[0].open;
                    }
                }
                return ret;
                i = Expects.ToList().IndexOf(key);
                if(i < 0)//没有这个日期
                {
                    if (lowCycleData != null && lowCycleData.Length>0)//是低频数据
                    {
                        if (lowCycleData.First().Expects.First().ToDate() > key.ToDate())//如果日期早于最早的低频单元的开始日期
                        {
                            return null;
                        }
                        KlineDataItem item = lowCycleData.Where(a => a.Expect.ToDate() >= key.ToDate()).OrderBy(a => a.Expect.ToDate()).First();
                        i = item.Expects.IndexOf(key);
                        ret = new StockMongoData()
                        {
                            open = item.opens[i],
                            close = item.closes[i],
                            high = item.highs[i],
                            low = item.lows[i],
                            vol = item.vols[i],
                            Expect = item.Expects[i],
                            code = code

                        };
                        if(i>1)
                        {
                            ret.preclose = item.closes[i];
                        }
                        else
                        {
                            if(item.index>1)
                            {
                                ret.preclose = lowCycleData[item.index].closes.Last();
                            }
                            else
                            {
                                ret.preclose = lowCycleData[0].open;//错误，以开盘价代替
                            }
                        }
                        return ret;
                    }
                    return null;
                }
                ret= new StockMongoData()
                {
                    open = Opens[i],
                    close = Closes[i],
                    high = Highs[i],
                    low = Lows[i],
                    vol = Vols[i],
                    Expect = Expects[i],
                    code = code,
                };
                if(i>1)
                {
                    ret.preclose = Closes[i - 1];
                }
                else
                {
                    ret.preclose = Opens[i];//错误，以开盘价代替
                }
                return ret;
            }
        }

        public StockMongoData this[int index]
        {
            get
            {
                if(index >=0 && index<Datas.Length)
                {
                    return Datas[index];
                }
                return null;
            }
        }

        public int ExpectIndex(string  key,out int subIndex)
        {
            subIndex = -1;
            int i = Expects.ToList().IndexOf(key);
            if (i < 0)//没有这个日期
            {
                if (lowCycleData != null && lowCycleData.Length > 0)//是低频数据
                {
                    if (lowCycleData.First().Expects.First().ToDate() > key.ToDate() || lowCycleData.Last().Expect.ToDate()<key.ToDate())//如果日期早于最早的低频单元的开始日期
                    {
                        return -1;
                    }
                    KlineDataItem item = lowCycleData.Where(a => a.Expect.ToDate() >= key.ToDate()).OrderBy(a => a.Expect.ToDate()).First();
                    subIndex = item.Expects.IndexOf(key);
                    return item.index;
                }
                return -1;
            }
            if(lowCycleData != null && lowCycleData.Length>0)
            {
                subIndex = lowCycleData[i].Expects.Length-1;
            }
            return i;
        }
    }

    public class KlineDataItem
    {
        public string Expect;
        public int index;
        public string beginExpect;
        public string highExpect;
        public string lowExpect;
        public string[] Expects;
        public double[] closes;
        public double[] opens;
        public double[] highs;
        public double[] lows;
        public double[] vols;
        public double close;
        public double open;
        public double high;
        public double low;
        public double vol;
        public KlineDataItem(int index,int itemCount,string[] expects,double[] _opens,double[] _closes,double[] _highs,double[] _lows,double[] _vols)
        {
            Expects = new string[itemCount];
            closes = new double[itemCount];
            opens = new double[itemCount];
            highs = new double[itemCount];
            lows = new double[itemCount];
            vols = new double[itemCount];
            Expect = expects[index + itemCount-1];
            beginExpect = expects[index];
            Array.Copy(expects, index, Expects, 0, itemCount);
            Array.Copy(_closes, index, closes, 0, itemCount);
            Array.Copy(_opens, index, opens, 0, itemCount);
            Array.Copy(_highs, index, highs, 0, itemCount);
            Array.Copy(_lows, index, lows, 0, itemCount);
            Array.Copy(_vols, index, vols, 0, itemCount);
            open = opens.First();
            close = closes.Last();
            high = highs.Max();
            low = lows.Min();
            vol = vols.Sum();
            highExpect = Expects[highs.ToList().IndexOf(high)];
            lowExpect = Expects[lows.ToList().IndexOf(low)];
        }

    }



}
