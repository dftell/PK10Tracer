using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.PK10CorePress
{
    [Serializable]
    public class ExpectList : BaseObjectsLib.ExpectList<TimeSerialData>
    {
        public ExpectList():base(false)
        {
        }
        public ExpectList(Dictionary<string, MongoReturnDataList<TimeSerialData>> _data) : base(_data,false,false)
        {
        }


        public ExpectList(DataTable dt) : base(dt,false)
        {
        }

        public static ExpectList getExpectList<T>(ExpectList<T> el) where T:TimeSerialData
        {
            ExpectList ret = new ExpectList();
            for (int i = 0; i < el.Count; i++)
            {
                ExpectData ed = new ExpectData().GetExpectData(el[i]);
                ret.Add(ed);
            }
            return ret;
        }
        

        public new ExpectData this[int i]
        {
            get
            {
                ExpectData ret = new ExpectData();
                ret.OpenCode = base[i].OpenCode;
                ret.OpenTime = base[i].OpenTime;
                ret.Expect = base[i].Expect;
                return ret;
                return base[i]  as ExpectData;
                //return base[i].CopyTo<ExpectData>();
            }
            set
            {
                if (value == null)
                    return;
                base[i].Expect = value.Expect;
                base[i].OpenTime = value.OpenTime;
                base[i].OpenCode = value.OpenCode;
                //base[i] = value.CopyTo<ExpectData>();
                //base[i] = new ExpectData().GetExpectData(value);
            }
        }

        public ExpectData LastData
        {
            get
            {
                // return base.LastData.CopyTo<ExpectData>();
                return  new ExpectData().GetExpectData(base.LastData);
            }
        }

        public ExpectList getSubArray(int FromIndex, int len)
        {
            ExpectList ret = new ExpectList();
            ExpectList<TimeSerialData> tmp = base.getSubArray(FromIndex, len);
            foreach (ExpectData<TimeSerialData> a in tmp)
            {
                ExpectData<TimeSerialData> item = a as ExpectData<TimeSerialData>;
                ret.Add(new ExpectData().GetExpectData<TimeSerialData>(item));
            }
            return ret;// as ExpectList;
        }

        public ExpectList FirstDatas(int RecLng)
        {
            //ExpectList ret = new ExpectList();
            //base.FirstDatas(RecLng).ForEach(a => ret.Add(a.CopyTo<ExpectData>()));
            //return ret;
            ExpectList ret = new ExpectList();
            if (RecLng == this.Count) return this;
            if (RecLng > this.Count)
                throw new Exception("请求长度超出目标列表长度！");
            for (int i = 0; i < RecLng; i++)
            {
                ret.Add(this[i]);
            }
            return ret;
        }

        public ExpectList LastDatas(int RecLng,bool ResortTime)
        {
            ////ExpectList ret = new ExpectList();
            ////ExpectList<TimeSerialData> tmp = base.LastDatas(RecLng, ResortTime);
            //////tmp.ForEach(a => ret.Add(new ExpectData(a)));
            ////for (int i = 0; i < tmp.Count; i++)
            ////    ret.Add(tmp[i]);
            ////return ret;
            
            if (RecLng >= this.Count) return this;
            if (RecLng > this.Count)
            {
                throw new Exception("请求长度超出目标列表长度！");
            }
            ExpectList ret = new ExpectList();
            for (int i = Math.Max(0,this.Count - RecLng); i < this.Count; i++)
            {
                ret.Add(this[i]);
            }
            return ret;
        }

    }
    [Serializable]
    public class ExpectData : BaseObjectsLib.ExpectData<TimeSerialData>
    {
        //////public Int64 EId;
        //////public int MissedCnt;
        //////public string LastExpect;
        //////public string Expect;
        //////public string OpenCode;

        //////public DateTime OpenTime { get; set; }

        public ExpectData():base(false)
        {

        }

        public ExpectData(TimeSerialData T):base(T,false)
        {

        }

        public ExpectData GetExpectData<T>(ExpectData<T> data) where T : TimeSerialData
        {
            return getData<T>(data);
        }
        public static ExpectData getData<T>(ExpectData<T> data) where T:TimeSerialData
        {
            ExpectData ret = new ExpectData();
            ret.Expect = data.Expect;
            ret.OpenCode = data.OpenCode;
            ret.OpenTime = data.OpenTime;
            return ret;
        }
    


        public virtual string[] ValueList
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

        //////public override object Clone()
        //////{
        //////    ExpectData ret = new ExpectData();
        //////    ret.Expect = this.Expect;
        //////    ret.OpenCode = this.OpenCode;
        //////    ret.OpenTime = this.OpenTime;
        //////    if(ret.CurrData!= null)
        //////    {
        //////        ret.CurrData = this.CurrData.Clone();
        //////    }
        //////    return ret;
        //////}

        public string PreExpect
        {
            get
            {
                if (Expect == null || Expect.Trim().Length == 0)
                    return null;
                return string.Format("{0}", Int64.Parse(Expect) - 1);
            }
        }

        
    }

    public class Combin_ExpectData<T>: ExpectData<T> where T:TimeSerialData
    {
        public Combin_ExpectData():base(false)
        {

        }
        public Combin_ExpectData(ExpectData<T> ed):base(false)
        {
            this.OpenCode = ed.OpenCode;
            this.Expect = ed.Expect;
            this.OpenTime = ed.OpenTime;
        }
        public string[] ValueList
        {
            get
            {
                string[] ret = OpenCode.Trim().Split(',');
                return ret;
            }
        }
    }
}
