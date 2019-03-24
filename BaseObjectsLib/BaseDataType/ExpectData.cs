using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WolfInv.com.BaseObjectsLib
{
    public abstract class LottyData:TimeSerialData
    {

    }

    public class ExpectData: LottyData
    {
        //////public Int64 EId;
        //////public int MissedCnt;
        //////public string LastExpect;
        //////public string Expect;
        //////public string OpenCode;

        //////public DateTime OpenTime { get; set; }

        public string[] ValueList
        {
            get
            {
                string[] ret = OpenCode.Trim().Split(',');
                for (int i = 0; i < ret.Length; i++)
                {
                    ret[i] = ret[i].Trim().Substring(1);
                }
                return ret;
            }
        }
        public long LExpectNo
        {
            get { return long.Parse(Expect); }
        }
        public long ExpectIndex
        {
            get 
            {
                long v = LExpectNo;
                if (LExpectNo >= 344978) //34977
                {
                    v = v - 1;
                }
                if (LExpectNo >= 356395)//2013/4/16 356375-356395 missed
                {
                    v = v - 19;
                }
                return v;
            }
        }

        public override object Clone()
        {
            ExpectData ret = new ExpectData();
            ret.Expect = this.Expect;
            ret.OpenCode = this.OpenCode;
            ret.OpenTime = this.OpenTime;
            if(ret.CurrData!= null)
            {
                ret.CurrData = this.CurrData.Clone() as OneCycleData;
            }
            return ret;
        }

        public string PreExpect
        {
            get
            {
                if (Expect == null || Expect.Trim().Length == 0)
                    return null;
                return string.Format("{0}",Int64.Parse(Expect) - 1);
            }
        }

        public MongoDataDictionary<StockMongoData> DataList;
    }

}
