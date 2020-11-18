using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;

namespace WolfInv.com.StrategyLibForWD
{
    [Serializable]
    public class ExpectList:Dictionary<string,ExpectData>, ISerializable
    {
        public ExpectList()
        {

        }

        protected ExpectList(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //his = info.GetBoolean("Test_Value");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            //info.AddValue("Test_Value", Value);
        }


        public static ExpectList getList(BaseDataTable dt)
        {
            ExpectList list = new ExpectList();
            try
            {
                //ExpectList el = new ExpectList();
                
                List<ExpectData> Datas = new DisplayAsTableClass().FillByTable<ExpectData>(dt.GetTable());
                Datas.ForEach(a =>
                {
                    if (a.DateTime != null)
                        a.DateTime = DateTime.Parse(a.DateTime).ToString("yyyy-MM-dd");
                });
                bool isSerialData = true;
                if (Datas.Count > 0)
                {
                    string date = Datas.First().DateTime;
                    var items = Datas.Where(a => DateTime.Parse(a.DateTime).ToString("yyyy-MM-dd").Equals(date));
                    if (items.Count() == Datas.Count)
                    {
                        isSerialData = false;
                    }
                }
                if (isSerialData)
                {
                    Datas.ForEach(a =>
                    {
                        if (!list.ContainsKey(a.DateTime))
                        {
                            list.Add(a.DateTime, a);
                        }
                    });
                }
                else
                {
                    Datas.ForEach(a =>
                    {
                        if (!list.ContainsKey(a.windcode))
                        {
                            list.Add(a.windcode, a);
                        }
                    });
                }
            }
            catch(Exception ce)
            {
                return list;
            }
            return list;
        }

        public string[] Date
        {
            get
            {
                return this.Keys.Select(a=>a).ToArray();
            }
        }

        public double[] Close
        {
            get
            {
                return this.Values.Select(a => a.CLOSE).ToArray();
            }
        }
        public double[] Open
        {
            get
            {
                return this.Values.Select(a => a.OPEN).ToArray();
            }
        }

        public double[] High
        {
            get
            {
                return this.Values.Select(a => a.HIGH).ToArray();
            }
        }

        public double[] Low
        {
            get
            {
                return this.Values.Select(a => a.LOW).ToArray();
            }
        }

        public double[] Volume
        {
            get
            {
                return this.Values.Select(a => a.VOLUME).ToArray();
            }
        }

        
        public static bool isSameDate(DateTime date1,DateTime date2)
        {
            if(date1.Year == date2.Year && date1.DayOfYear == date2.DayOfYear)
            {
                return true;
            }
            return false;
        }

        public List<ExpectData> Serials
        {
             get
            {
                return this.Values.ToList();
            }
        }
        public ExpectList LastDays(int N)
        {
            int len = Math.Min(this.Count, N);
            ExpectList ret = new ExpectList();
            var items = this.Skip(len - N);
            foreach(var item in items)
            {
                ret.Add(item.Key, item.Value);
            }
            return ret;
        }

        public ExpectList AfterDate(string Date)
        {
            DateTime cprDate = Date.ToDate();
            if(cprDate>this.Last().Key.ToDate() || cprDate<this.First().Key.ToDate())
            {
                return new ExpectList();
            }
            int index = this.Keys.ToList().Where(a=>a.ToDate()<Date.ToDate()).Count();
            if (index < 0)
                return new ExpectList();
            ExpectList ret = new ExpectList();
            foreach(var key in this.Skip(index))
            {
                ret.Add(key.Key, key.Value);
            }
            return ret;
        }

        
    }

    public delegate T[] paramObjs<T>(T[] data, params object[] args);

    

    [Serializable]
    public class ExpectData: DisplayAsTableClass
    {
        public ExpectData()
        {

        }
        //windcode, sec_name,   open, high, low, close, volume,amt
        public string windcode;
        public string sec_name;
        public long amt;
        public string Code
        {
            get
            {
                return windcode;
            }
            set
            {
                windcode = value;
            }
        }
        public string Location;
        public string Name
        {
            get
            {
                return sec_name;
            }
            set
            {
                sec_name = value;
            }
        }
        public string Time;//hexun
        public string DateTime
        { 
            get
            {
                return Time;
            }
            set
            {
                Time = value;
            }
        }
        public double OPEN;
        public double HIGH;
        public double LOW;
        public double CLOSE;
        public double VOLUME;
        public double LastPrice;//hexun
        public int PriceWeight=1;//hexun
        public long AMOUNT
        {
            get
            {
                return amt;
            }
            set
            {
                amt = value;
            }
        }

        
    }
}
