using System;
using System.Data;
//using MathNet.Numerics;
//using MathNet.Numerics.Statistics;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.GuideLib
{
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
}
