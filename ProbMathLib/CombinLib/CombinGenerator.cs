using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WolfInv.com.ProbMathLib
{
    public class CombinGenerator
    {
        public static string ResortNumString(string arrs,string splitor=",")
        {
            return string.Join(splitor, ResortNumString(arrs.Split(splitor.ToCharArray())));
        }

        public static string[] ResortNumString(string[] arr)
        {
            List<int> ret = new List<int>();
            //Array.Copy(arr, 0, ret, 0, arr.Length);
            arr.ToList().ForEach(a => {
                int val = -1;
                int.TryParse(a, out val);
                ret.Add(val);
            });

            var res = ret.OrderBy(a => a);
            List<string> rs = new List<string>();
            foreach(var a in res)
            {
                rs.Add(a.ToString());
            }
            return rs.ToArray();
        }



        public static List<string> Generate(string[] strs,int N=-1,string splitor="")
        {
            List<string> ret = new List<string>();
            if(N==-1)
            {
                N = strs.Length;
            }
            if (strs.Length < N)
            {
                return ret ;
            }
            int Len = strs.Length;
            for(int i=0;i<Len-N+1;i++)
            {
                string strFirst = strs[i];
                if(N== 1)
                {
                    ret.Add(strFirst);
                    continue;
                }
                
                int nextindex = i+1;
                if(nextindex>Len)
                {
                    continue;
                }
                string[] nextArr = new string[Len - 1];
                Array.Copy(strs, nextindex, nextArr,0,Len- nextindex);
                List<string> nextlist = Generate(nextArr, N - 1,splitor);
                nextlist.ForEach(a => {
                    if(a!=null)
                        ret.Add(string.Format("{0}{1}{2}", strFirst, splitor, a));
                });
            }
            return ret;
        }

        //a b c d e f g h
        //a b
        public class CombinComparer
        {

        }
        

        
    }

    public class CombinClass : List<string>
    {

        public int Len { get; set; }
        public string[] OrgArrange { get; set; }
        string Splitor = ",";
        public CombinClass()
        {

        }
        


        public CombinClass(string arrs, int len = -1, string splitor = ",")
        {
            Init(arrs.Split(splitor.ToCharArray()), len, splitor);
        }

        public static CombinClass CreateNumCombin(int AllNum,int SelNum,string splitor=",",int leftint=1,char padval='0')
        {
            string[] arr = new string[AllNum];
            for(int i=0;i<AllNum;i++)
            {
                arr[i] = (i + 1).ToString();
                if(leftint>1)
                {
                    arr[i] = arr[i].PadLeft(leftint, padval);
                }
            }
            return new CombinClass(arr, SelNum, splitor);
        }

        public static string[] CreateNumArr(int AllNum, string splitor = ",", int leftint = 1, char padval = '0')
        {
            string[] arr = new string[AllNum];
            for (int i = 0; i < AllNum; i++)
            {
                arr[i] = (i + 1).ToString();
                if (leftint > 1)
                {
                    arr[i] = arr[i].PadLeft(leftint, padval);
                }
            }
            return arr;
        }

        public CombinClass(string[] arr, int len = -1, string splitor = ",")//使用前必须排序
        {
            Init(arr, len, splitor);
        }

        void Init(string[] arr, int len = -1, string splitor = ",")
        {
            Len = len;
            if (Len < 0)
            {
                Len = arr.Length;
            }
            Splitor = splitor;
            OrgArrange = arr;
            List<string> ret = CombinGenerator.Generate(arr, Len, splitor);
            //ret.AddRange(ret);
            this.AddRange(ret);
        }

        public CombinClass Union(CombinClass obj)
        {
            CombinClass ret = new CombinClass();
            if (obj == null)
                return this;
            ret.AddRange(obj);
            this.ForEach(a => {
                if (!ret.Contains(a))
                    ret.Add(a);
            });
            return ret;
        }

        public CombinClass Reconvert(string[] s,string splitor=",")
        {
            if(s==null || s.Length == 0)
            {
                return this;
            }
            if(this.Count>0 && s.Length>0)
            {
                if (this[0].Split(splitor.ToCharArray()).Length >= s.Length)
                    return this;
            }
            CombinClass ret = new CombinClass();
            for(int i=0;i<this.Count;i++)
            {
                string[] curritem = this[i].Split(splitor.ToCharArray());
                ret.Add(string.Join(splitor, getReconvertString(s, curritem)));
            }
            return ret;
        }

        public static string[] getReconvertString(string[] strAll,string[] substr)
        {
            List<string> ret = new List<string>();
            List<string> sublist = substr.ToList();
            strAll.ToList().ForEach(a=> {
                if(!sublist.Contains(a))
                {
                    ret.Add(a);
                }
            });
            return ret.ToArray();
        }

        public CombinClass GetSubCombin(int lng)
        {
            CombinClass ret = new CombinClass();
            this.ForEach(a => {
                CombinClass subcmb = new CombinClass(a,lng);
                ret = ret.Union(subcmb);
            });
            return ret;
        }

        public CombinClass Subtract(CombinClass obj)
        {
            CombinClass ret = new CombinClass();
            if (obj == null || obj.Count == 0)
                return this;
            this.ForEach(a => {
                if (!obj.Contains(a))
                    ret.Add(a);
            });
            return ret;
        }

        public CombinClass Inner(CombinClass obj)
        {
            CombinClass ret = new CombinClass();
            if (obj == null)
                return ret;
            this.ForEach(a => {
                if (obj.Contains(a))
                {
                    ret.Add(a);
                }
            });
            return this;
        }
    }

}
