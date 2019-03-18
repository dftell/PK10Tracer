using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.CFZQ_LHProcess;
using System.Data;
//using MathNet.Numerics;
//using MathNet.Numerics.Statistics;
using WAPIWrapperCSharp;
using System.Reflection;
namespace WolfInv.com.StrategyLibForWD
{
    public enum MACDColor { None,Green, Red }
    public class MACDDataItem:BaseDataItemClass
    {
        public DateTime Date
        {
            get
            {
                return this.DateTime;
            }
        }
        public decimal DIFF, DEA, MACD;
        public int RowId;
        public MACDColor Color;
        //临时状态
        public bool IsAreaStart;
        public bool IsAreaEnd;
        public MACDDataItem()
        {
        }

        public MACDDataItem(DataRow dr)
        {
            Type t = this.GetType();
            this.FillByDataRow(dr);

            ////Date = (DateTime)dr["Date"];
            ////Open = (decimal)dr["Open"];
            ////High = (decimal)dr["High"];
            ////Low = (decimal)dr["Low"];
            ////Close = (decimal)dr["Close"];
            ////DIFF = (decimal)dr["DIFF"];
            ////DEA = (decimal)dr["DEA"];
            ////MACD = (decimal)dr["MACD"];
            ////Color = MACDColor.None;
        }

        

        ////public T getDataPointByName<T>(string name)
        ////{
        ////}
    }

    public class MACDTable :BaseDataTable
    {
        public MACDTable()
        {
            //InitTableStructure("Date", typeof(DateTime),
                //"Open", typeof(decimal),
                //"High", typeof(decimal),
                //"Low", typeof(decimal),
                //"Close", typeof(decimal),
                //"DIFF", typeof(decimal),
                //"DEA", typeof(decimal),
                //"MACD", typeof(decimal));
        }

        public MACDTable(MTable dt):base(dt)
        { 
        }

        public MACDTable(DataTable dt)
            : base(dt)
        {
        }


        public MACDTable(List<MACDDataItem> _data):this()
        {
            this.Table.Rows.Clear();
            for (int i = 0; i < _data.Count; i++)
            {
                DataRow dr = this.Table.NewRow();
                this.Table.Rows.Add(_data[i].FillRow(dr));
            }
        }

        public new MACDDataItem this[int rowid]
        {
            get
            {
                MACDDataItem item = new MACDDataItem(this.Table.Rows[rowid]);
                item.RowId = rowid;
                return item;
            }
        }

        public List<decimal> MACDs
        {
            get
            {
                return getDataPoints(":", "MACD");
            }
        }

        public List<decimal> DEAs
        {
            get
            {
                return getDataPoints(":", "DEA");
            }
        }

        public List<decimal> DIFFs
        {
            get
            {
                return getDataPoints(":", "DIFF");
            }
        }

        public List<decimal> Opens
        {
            get
            {
                return getDataPoints(":", "Open");
            }
        }

        public List<decimal> Highs
        {
            get
            {
                return getDataPoints(":", "High");
            }
        }

        public List<decimal> Lows
        {
            get
            {
                return getDataPoints(":", "Low");
            }
        }
        
        public List<decimal> Closes
        {
            get
            {
                return getDataPoints(":", "Close");
            }
        }

        List<decimal> getDataPoints( string pointName)
        {
            return getDataPoints(":",pointName);
        }

        List<decimal> getDataPoints(string rows, string pointName)
        {
            return this[rows, pointName].ToList<decimal>();

        }
         List<decimal> getDataPoints(int From,int To,string pointName)
        {
            List<decimal> rets = new List<decimal>();
            string rows = string.Format("{0}:{1}", From, To);
            return this[rows, pointName].ToList<decimal>();
        }

         public MACDTable  AvaliableData
         {
             get
            {
                if (_AvaliableData == null)
                {
                    if (this.Count > 0)
                    {
                        _AvaliableData = new MACDTable(this.Select("TRADE_STATUS='交易'"));
                    }
                }
                return (MACDTable)_AvaliableData;
            }
         }

         List<MACDDataItem> _list;
         public List<MACDDataItem> ItemList
         {
             get
             {
                 if (_list == null)
                 {
                     _list = new List<MACDDataItem>();
                     for (int i = 0; i < this.Count; i++)
                         _list.Add(this[i]);
                 }
                 return _list;
             }
         }
    }

    public class MACDAreaClass
    {
        public MACDTable Data;
        public DateTime BegT;
        public DateTime EndT;
        public int Len;
        public bool UpDir;//上涨
        public MACDTable OutData;
        public decimal Summ;
        /// <summary>
        /// MACD极值
        /// </summary>
        public decimal MaxinMACD;
        /// <summary>
        /// 价格极值
        /// </summary>
        public decimal MaxinPrice;
        /// <summary>
        /// 开收极值
        /// </summary>
        public decimal MaxinOCPrice;
        /// <summary>
        /// 收盘极值
        /// </summary>
        public decimal MaxinClose;

        public MACDAreaClass()
        {
            Init();
        }

        public MACDAreaClass(MACDTable _data)
        {
            Data = _data;
            Init();
        }

        public List<MACDAreaClass> SplitSubMACDAreas()
        {
            var Arr = from n in Data.ItemList 
                      orderby n.RowId 
                      select n;
            List<MACDAreaClass> ret = new List<MACDAreaClass>();
            MACDDataItem firstObj = Arr.First<MACDDataItem>();
            firstObj.Color = firstObj.MACD > 0 ? MACDColor.Red : MACDColor.Green;
            firstObj.IsAreaStart = true;
            MACDDataItem endObj = Arr.Last<MACDDataItem>();
            endObj.Color = endObj.MACD > 0 ? MACDColor.Red : MACDColor.Green;
            endObj.IsAreaEnd = true;
            for (int i = 1; i < Len - 1; i++)
            {
                MACDDataItem CurrObj = Arr.ElementAt<MACDDataItem>(i);
                MACDDataItem NextObj = Arr.ElementAt<MACDDataItem>(i+1);
                MACDDataItem PreObj = Arr.ElementAt<MACDDataItem>(i - 1);
                if (CurrObj.MACD > 0 && PreObj.MACD <=0) //MACD上穿0线
                {
                    CurrObj.Color = MACDColor.Red;
                    CurrObj.IsAreaStart = true;
                    PreObj.Color = MACDColor.Green;
                    PreObj.IsAreaEnd = true;
                    continue;
                }
                if (CurrObj.MACD < 0 && PreObj.MACD >= 0) //MACD下穿0线
                {
                    CurrObj.Color = MACDColor.Green;
                    CurrObj.IsAreaStart = true;
                    PreObj.Color = MACDColor.Red;
                    PreObj.IsAreaEnd = true;
                    continue;
                }
            }
            var KeyArr = from n in Arr 
                         where n.Color != MACDColor.None
                         select n; //上下穿切换点
            int KeyCnt = KeyArr.Count<MACDDataItem>();
            var startArr = from Item in KeyArr
                           let begDate = Item.Date
                           let endDate = 0
                           where Item.IsAreaStart=false
                           select new { Item,begDate};
            var endArr = from n in startArr
                           let Item =n.Item
                           let begDate = n.begDate
                           let endDate = n.Item.Date
                           where n.Item.IsAreaEnd=false
                           select new { Item,begDate,endDate};
            int Keyindex = -1;
            bool Skip = false;
            foreach (var key in endArr)
            {
                Keyindex++;
                if (Skip)
                {
                    Skip = false;
                    continue;
                }
                var subAreaArr = from n in Arr
                                 where n.Date >= key.begDate 
                                 where n.Date <= key.endDate
                                 select n;
                MACDAreaClass subObj = new MACDAreaClass(new MACDTable(subAreaArr as List<MACDDataItem>));
                //subObj.UpDir = subAreaArr.First<MACDDataItem>().Color == MACDColor.Red;
                if (subObj.Len < 3)
                {
                    if (Keyindex > 0 && Keyindex < KeyCnt-1)
                    {
                        var UnionArea = from n in Arr
                                        where n.Date >= ret[ret.Count - 1].BegT && n.Date <= key.endDate
                                        select n;
                        MACDAreaClass unionAreaObj = new MACDAreaClass(new MACDTable(UnionArea as List<MACDDataItem>));
                        ret[ret.Count - 1] = unionAreaObj;
                        Skip = true;
                        continue;
                    }
                }
                ret.Add(subObj);
            }
            List<MACDAreaClass> lastRes = new List<MACDAreaClass>();
            if(ret.Count>0) lastRes.Add(ret[0]);
            int cnt = 0;
            for (int i = 1; i < ret.Count; i++)
            {
                if (ret[i].UpDir == ret[cnt].UpDir)//方向相同，合并
                {
                    lastRes[lastRes.Count] = this.MergeArea(lastRes[lastRes.Count - 1], ret[i]);
                }
                else
                {
                    cnt++;
                    lastRes.Add(ret[i]);
                }
            }
            return lastRes;
        }

        public void SplitRedColorSubAreas()
        {
        }

        public List<MACDAreaClass> SplitMACDAreasByDEA(bool isRed)
        {
            MACDTable Arr = Data;
            List<MACDAreaClass> ret = new List<MACDAreaClass>();
            if (!isRed)
            {
                var tArr = from n in Arr.ItemList
                           where n.DEA>=0 
                           orderby  n.RowId descending
                           select n;
                if(tArr.Count<MACDDataItem>() ==0) //如果没有DEA小于0的，返回一个空结构
                    return ret;
                int lastUDEAid = tArr.First<MACDDataItem>().RowId;//最后一个
            }
            return ret;
        }

        public MACDAreaClass MergeArea(MACDAreaClass A1, MACDAreaClass A2)
        {
            MACDTable tmp = new MACDTable(A1.Data.GetTable());
            tmp.Contact(A2.Data);
            MACDAreaClass ret = new MACDAreaClass(tmp);
            return ret;
        }

        void Init()
        {
            if (this.Data == null) return;
            Len = Data.Count;
            BegT = Data[0].Date;
            MACDDataItem LastItem = Data[Len - 1<0?0:(Len-1)];
            EndT = LastItem.Date;
            UpDir = false;
            for (int i = 0; i < Len; i++)//初始化该区初始方向
            {
                decimal vMacd = Data[Len - i].MACD;
                if (vMacd == 0) continue;
                UpDir = vMacd > 0;
                break;
            }
            Summ = Data.MACDs.Sum();
            MaxinMACD = UpDir ? Data.MACDs.Max() : Data.MACDs.Min();
            MaxinPrice = UpDir ? Data.Highs.Max() : Data.Lows.Min();
            MaxinClose = UpDir ? Data.Closes.Max() : Data.Closes.Min();
            MaxinOCPrice = UpDir ? Math.Max(Data.Opens.Max(), Data.Closes.Max()) : Math.Min(Data.Opens.Min(), Data.Closes.Min());
        }
    }

    public class MACDCommTool
    {
        public static FilterResult IsFirstBuyPoint(MACDDataItem secinfo, Delegate CallBackFunc)
        {
            FilterResult ret = new FilterResult();
            if (secinfo.DEA >= 0)
            {
                ret.Msg.Append("第一买点，DEA必须在0线以下！");
                return ret;
            }
            if (secinfo.pct_chg  > 10)
            {
                ret.Msg.Append("涨幅太大！");
                return ret;
            }
            
            return ret;
        }
    }
}
