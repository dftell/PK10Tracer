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
        /// <summary>
        /// 判定是否停牌用
        /// </summary>
        string Expect;
        string code;
        string name;
        bool isST;
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
        public double OpenRaiseRate;
        public double HighRaiseRate;
        public double LowRaiseRate;
        public Cycle LineCycle;
        public KLineData(string useExpect,List<T> list,string _code,string _name,PriceAdj priceAdj=PriceAdj.Beyond,Cycle cyc = Cycle.Day)
        {
            LineCycle = cyc;
            Expect = useExpect;
            init(list, _code, _name, priceAdj,cyc);
        }

        public KLineData(string useExpect,MongoReturnDataList<T> list, PriceAdj priceAdj=PriceAdj.Beyond, Cycle cyc = Cycle.Day)
        {
            Expect = useExpect;
            if(list == null)
            {

            }
            init(list, list.SecInfo.code , list.SecInfo.name, priceAdj,cyc);
            changeCycle();
        }
        void init(List<T> list, string _code, string _name, PriceAdj priceAdj = PriceAdj.Fore, Cycle cyc = Cycle.Day)
        {
            try
            {
                code = _code;
                name = _name;
                Datas = new StockMongoData[list.Count];
                LineCycle = cyc;
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
            int i = 0;
            KlineDataItem lastKdi = null;
            KlineDataItem preKdi = null;
            foreach (string key in lowCycDates.Keys)
            {
                preKdi = lastKdi;
                Indies[i] = i;
                Expects[i] = key;
                KlineDataItem kdi = lowCycDates[key];
                Closes[i] = kdi.closes.Last();
                Opens[i] = kdi.opens.First();
                Highs[i] = kdi.highs.Max();
                Lows[i] = kdi.lows.Min();
                Vols[i] = kdi.vols.Sum();
                RaiseRates[i] = i == 0 ? 0 : 100 * (kdi.closes.Last() - lastKdi.closes.Last()) / lastKdi.closes.Last();
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
                        keyList = Expects.Select(a => (int)a.ToDate().DayOfYear).ToList();
                        break;
                    }
                    ///季
                case Cycle.Quarter:
                    {
                        lessInterVal = 31;
                        keyList = Expects.Select(a => ((a.ToDate().Month % 3)>0?1:0)*a.ToDate().Day).ToList();
                        break;
                    }
                case Cycle.Month:
                    {
                        lessInterVal = 30;
                        keyList = Expects.Select(a => a.ToDate().Day).ToList();
                        break;
                    }
                case Cycle.Week:
                default:
                    {
                        lessInterVal = 7;
                        keyList = Expects.Select(a =>(int) a.ToDate().DayOfWeek).ToList();
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
                    lowCycDates.Add(Expects[i - 1],useItem);
                    startIndex = i;
                    continue;
                }
                //长假处理
                DateTime currDate = Expects[i].ToDate();
                DateTime preDate = Expects[i - 1].ToDate();
                if(currDate.Subtract(preDate).TotalDays>lessInterVal)//当前日期数不可能是周末
                {
                    useItem = new KlineDataItem(startIndex, i - startIndex, Expects, Opens, Closes, Highs, Lows, Vols);
                    lowCycDates.Add(Expects[i - 1], useItem);
                    startIndex = i;
                    continue;
                }
            }
            if(lowCycDates.ContainsKey(Expects.Last()))
            {
                useItem = new KlineDataItem(startIndex, Expects.Length - startIndex, Expects, Opens, Closes, Highs, Lows, Vols);
                lowCycDates.Add(Expects.Last(), useItem);
            }
            changeToLineData(lowCycDates);
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
                int i = Expects.ToList().IndexOf(key);
                if(i < 0)
                {
                    return null;
                }
                return new StockMongoData()
                {
                    open = Opens[i],
                    close = Closes[i],
                    high = Highs[i],
                    low = Closes[i],
                    vol = Vols[i],
                    Expect = Expects[i],
                    code = code,
                    preclose = Closes[i - 1]
                };
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
    }

    public class KlineDataItem
    {
        public string Expect;
        public string beginExpect;
        public string[] Expects;
        public double[] closes;
        public double[] opens;
        public double[] highs;
        public double[] lows;
        public double[] vols;
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
        }

    }



}
