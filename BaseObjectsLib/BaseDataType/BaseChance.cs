using System;
namespace WolfInv.com.BaseObjectsLib
{
    public class BaseChance<T> : DisplayAsTableClass where T : TimeSerialData
    {
        public BaseChance()
        {

        }
        /// <summary>
        /// db字段
        /// </summary>
        /// 
        //int _chanceindex;
        public Int64? ChanceIndex { get; set; }
        public string ChanceCode { get; set; }
        public int ChipCount { get; set; }
        public string ExpectCode { get; set; }
        public double UnitCost { get; set; }
        public DateTime ExecDate { get; set; }
        public double Cost { get; set; }
        public double Profit { get; set; }
        public double Gained { get; set; }
        public int HoldTimeCnt { get; set; }
        public int CurrTimes { get; set; }
        public int InputTimes { get; set; }
        public int IsEnd { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int ChanceType { get; set; }
        public Int64 BaseCost { get; set; }
        
        
        
        /// <summary>
        /// json用是否可以跟踪
        /// </summary>



        public bool Closed { get; set; }

        /// <summary>
        /// json用单注金额
        /// </summary>
        

        public string SignExpectNo { get; set; }
        public string EndExpectNo { get; set; }
        public string strInputTimes { get; set; }
        
        
        
        
        
        public double BaseAmount = 1;
    }


    
}
