using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WolfInv.com.BaseObjectsLib
{
    /// <summary>
    /// 时序数据，所有彩票，股票，债券，期货等证券数据的基类
    /// </summary>
    public abstract class TimeSerialData : DetailStringClass, ICloneable
    {
        public Int64 EId;
        public int MissedCnt;
        public string LastExpect;
        public string Expect;
        public string OpenCode;

        public DateTime OpenTime { get; set; }

        public abstract object Clone();
        public bool IsSecurity;
        public OneCycleData CurrData;
    }
}
