using System.Collections.Generic;
using System.Data;
//using MathNet.Numerics;
//using MathNet.Numerics.Statistics;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.GuideLib
{
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
}
