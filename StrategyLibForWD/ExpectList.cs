using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;

namespace WolfInv.com.StrategyLibForWD
{
    public class ExpectList:List<ExpectData>
    {
        public ExpectList()
        {

        }

        public static ExpectList getList(BaseDataTable dt)
        {
            ExpectList el = new ExpectList();
            List<ExpectData> list = new DisplayAsTableClass().FillByTable<ExpectData>(dt.GetTable());
            list.ForEach(a => el.Add(a));
            return el;
        }

        public string[] Date
        {
            get
            {
                return this.Select(a => a.DateTime.ToString("yyyy-MM-dd")).ToArray();
            }
        }

        public double[] Close
        {
            get
            {
                return this.Select(a => a.CLOSE).ToArray();
            }
        }
        public double[] Open
        {
            get
            {
                return this.Select(a => a.OPEN).ToArray();
            }
        }

        public double[] High
        {
            get
            {
                return this.Select(a => a.HIGH).ToArray();
            }
        }

        public double[] Low
        {
            get
            {
                return this.Select(a => a.LOW).ToArray();
            }
        }

        public double[] Volume
        {
            get
            {
                return this.Select(a => a.VOLUME).ToArray();
            }
        }

        

        public ExpectList LastDays(int N)
        {
            int len = Math.Min(this.Count, N);
            ExpectList ret = new ExpectList();
            for(int i=this.Count-len;i<this.Count;i++)
            {
                ret.Add(this[i]);
            }
            return ret;
        }

        
    }

    public delegate T[] paramObjs<T>(T[] data, params object[] args);

    

    public static class Extends
    {
        public static T[] fullFuncs<T>(paramObjs<T> func, T[] data, params object[] ars)
        {
            return func(data, ars);
        }
        public static T[] Get<T>(this T[] data, int N, paramObjs<T> func)
        {            
            return func(data,N);
        }

        public static BaseDataTable NDay<T>(this ExpectList data,int N, params object[] objs)
        {
            ExpectList useData = data.LastDays(N);
            BaseDataTable ret = new BaseDataTable();
            List<string> cols = new List<string>();
            List<T[]> funcs = new List<T[]>();
            int colcnt = 0;
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] is string)
                {
                    cols.Add((string)objs[i]);
                    colcnt++;
                }
                else
                {
                    if(i>=2*colcnt)
                    {
                        break;
                    }
                    funcs.Add(objs[i] as T[]);
                }
            }
            List<Single[]> vals = new List<float[]>();
            for (int i=0;i<cols.Count;i++)
            {
                ret.GetTable().Columns.Add(cols[i]);
                
            }
            
            for(int i=0;i<useData.Count;i++)
            {
                DataRow dr = ret.GetTable().NewRow();
                for(int c=0;c<cols.Count;c++)
                {
                    dr[c] = funcs[c][i];
                }
                ret.GetTable().Rows.Add(dr);
            }
            return ret;
        }

    }

        public class ExpectData: DisplayAsTableClass
    {

        public DateTime DateTime;
        public double OPEN;
        public double HIGH;
        public double LOW;
        public double CLOSE;
        public double VOLUME;
        
    }
}
