using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WolfInv.com.BaseObjectsLib
{
    /// <summary>
    /// 时序数据，所有彩票，股票，债券，期货等证券数据的基类
    /// </summary>
    [Serializable]
    public class TimeSerialData : MongoData
    {
        public TimeSerialData()
        {

        }
        public Int64 EId { get; set; }
        public int MissedCnt { get; set; }
        public string LastExpect { get; set; }
        public string Expect { get; set; }
        public string OpenCode { get; set; }

        public string Key { get { return "Lotty"; } }
        public string CurrTime { get {return OpenTime.ToString(); }  }

        public DateTime OpenTime { get; set; }

        //public abstract object Clone();
        public bool IsSecurity { get; set; }
        //public OneCycleData CurrData;
    }
}
