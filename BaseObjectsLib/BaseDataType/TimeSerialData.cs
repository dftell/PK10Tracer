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
        int t_ExpectSerialByType = 1;
        /// <summary>
        /// 1,int 2,date
        /// </summary>
        public int ExpectSerialByType//1,int 2,date
        {
            get { return t_ExpectSerialByType; }
            set { t_ExpectSerialByType = value; }
        }
        /// <summary>
        /// 如果是date，序号位数
        /// </summary>
        public int ExpectSerialLong { get; set; }//
        public Int64 EId { get; set; }
        public int MissedCnt { get; set; }
        public string LastExpect { get; set; }
        protected string _expect;
        public string Expect { get {return _expect; } set { _expect = value; } }
        public string OpenCode { get; set; }

        protected string _key = "Lotty";
       
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }
        public string KeyName { get; set; }
        string _currTime = null;
        public string CurrTime {
            get
            {
                return _currTime;
            }
            set
            {
                _currTime = value;
                if (value != null)
                {
                    DateTime dt;
                    bool suc = DateTime.TryParse(value, out dt);
                    if(suc)
                        OpenTime = dt;
                }
            }
        }

        public DateTime OpenTime { get; set; }

        //public abstract object Clone();
        protected bool _IsSecurity = false;
        public bool IsSecurity { get; set; }
        //public OneCycleData CurrData;
        public bool Disalbe = false;

        public virtual TimeSerialData Clone()
        {
            TimeSerialData ret = new TimeSerialData();
            ret.Expect = this.Expect;
            ret.Key = this.Key;
            return ret;
        }
    }
}
