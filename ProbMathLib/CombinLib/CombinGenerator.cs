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

}
