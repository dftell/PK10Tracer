using System;
using System.Collections.Generic;
using System.Linq;
namespace WolfInv.com.GuideLib
{
    public static class GuideToolClass
    {
        public static T[] MA<T>(this T[] arr, int arg)
        {
            int N = arg;
            T[] ret = new T[arr.Length];
            dynamic firstVal = arr[0];
            dynamic sum = default(T);
            for (int i = 0; i < ret.Length; i++)
            {
                if (i < N)
                {
                    ret[i] = (sum + arr[i]) / (i + 1);
                    sum += arr[i];
                }
                else
                {
                    firstVal = arr[i - N];
                    ret[i] = (sum + arr[i] - firstVal) / N;
                    sum += (arr[i] - firstVal);


                }
            }
            return ret;
        }

        public static T[] EMA<T>(this T[] arr, int N)
        {
            if (arr.Length == 1)
            {
                return arr;
            }
            T[] ret = new T[arr.Length];
            dynamic eval = arr[0];
            Single a = 2f / (N * 1f + 1f);
            for (int i = 0; i < arr.Length; i++)
            {
                dynamic currval = arr[i];

                if (i > 0)
                {
                    dynamic lv = ret[i - 1];
                    //EMAtoday = α * Pricetoday + (1 - α) * EMAyesterday;
                    // α=(N+1)/2
                    //eval = (2 * currval + (N - 1) * eval) / (N + 1);
                    eval = a * currval + (1 - a) * eval;
                }
                ret[i] = eval;
            }
            return ret;
        }

        public static T[] Times<T>(this T[] arr,T times)
        {
            T[] ret = new T[arr.Length];
            for(int i=0;i<arr.Length;i++)
            {
                dynamic res = arr[i] ;
                ret[i] = res * times;
            }
            return ret;
        }

        public static T[] Add<T>(this T[] arr, T times)
        {
            T[] ret = new T[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                dynamic res = arr[i];
                ret[i] = res + times;
            }
            return ret;
        }

        public class MACDCollection:List<MACDGuider>
        {
            public double[] MACDs
            {
                get
                {
                    return this.Select(a => a.MACD).ToArray();
                }
            }

            public double[] DEAs
            {
                get
                {
                    return this.Select(a => a.DEA).ToArray();
                }
            }

            public double[] DIFs
            {
                get
                {
                    return this.Select(a => a.DIF).ToArray();
                }
            }
        }

        public class MACDGuider
        {
            public double MACD;
            public double DEA;
            public double DIF;
        }
        public static MACDCollection MACD<T>(this T[] arr,int S=12,int L=26,int MID=9)
        {
            /*
DIF:EMA(CLOSE,S)-EMA(CLOSE,L);
DEA:EMA(DIF,MID);
MACD:(DIF-DEA)*2,COLORSTICK;
             */
            MACDCollection ret = new MACDCollection();
            T[] sEma = arr.EMA(S);
            T[] lEma = arr.EMA(L);
            T[] difArr = new T[arr.Length];
            for(int i=0;i<arr.Length;i++)
            {
                ret.Add(new MACDGuider());
                ret[i] = new MACDGuider();
                dynamic sa = sEma[i];
                dynamic la = lEma[i];
                dynamic dif = sa-la;
                difArr[i] = dif;
                ret[i].DIF = dif;
            }
            T[] deaArr = difArr.EMA(MID);            
            for (int i=0;i<arr.Length;i++)
            {
                dynamic difa = difArr[i];
                dynamic deaa = deaArr[i];
                ret[i].DEA = deaa;
                ret[i].MACD = (difa - deaa) * 2;
            }
            return ret;
        }

        public static T[] LastSector<T>(this T[] arr,int len)
        {
            if(len>=arr.Length)
            {
                return arr;
            }
            return arr.Skip(arr.Length - len).ToArray();
        }

        public static int LastMatchCondition<T>(this T[] arr,T[] cmpArr,Func<T[],T[],bool> func)
        {
            for (int i = arr.Length - 1; i >= 0; i--)
            {
                bool succ = func.Invoke(arr.Take(i).ToArray(), cmpArr.Take(i).ToArray());
                if (succ == true)
                {
                    return arr.Length-i;
                }
            }
            return -1;
        }

        public static int LastMatchCondition<T>(this T val, T[] cmpArr, Func<T[], T[], bool> func)
        {
            T[] arr = cmpArr.ToConst(val);
            return arr.LastMatchCondition(cmpArr, func);
        }



        /// <summary>
        /// 返回倒数第N个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static T Last<T>(this T[] arr,int position)
        {
            if (position <= 0)
                return default(T);
            if(position>arr.Length)
            {
                return default(T);
            }
            return arr[arr.Length - position];
        }
        public static bool CrossUp<T>(this T[] arr, T val)
        {
            return CrossUp(arr, ToConst(arr, val));
        }
        public static bool Cross<T>(this T[] arr, T val)
        {
            return CrossUp(arr, ToConst(arr, val));
        }

        public static bool Cross<T>(this T val,T[] arr)
        {
            return CrossUp(arr.ToConst(val), arr);
        }
        public static bool Cross<T>(this T[] arr, T[] arr1)
        {
            return CrossUp(arr, arr1);
        }
        public static bool CrossUp<T>(this T[] arr,T[] arr1) 
        {
            dynamic arr01 = arr.Last(1);
            dynamic arr02 = arr.Last(2);
            dynamic arr11 = arr1.Last(1);
            dynamic arr12 = arr1.Last(2);
            if (arr01 > arr11 && arr02 < arr12)
            {
                return true;
            }
            return false;
        }

        public static bool CrossDown<T>(this T[] arr,T val)
        {
            return CrossDown(arr, ToConst(arr, val));
        }

        public static bool CrossDown<T>(this T[] arr, T[] arr1)
        {
            dynamic arr01 = arr.Last(1);
            dynamic arr02 = arr.Last(2);
            dynamic arr11 = arr1.Last(1);
            dynamic arr12 = arr1.Last(2);
            
            if (arr01<arr11 && arr02>arr12)
            {
                return true;
            }
            return false;
        }

        public static double ToDbl<T>(this T val)
        {
            return double.Parse(val.ToString());
        }

        public static T[] ToConst<T>(this T[] arr,T val)
        {
            T[] ret = arr;
            for (int i = 0; i < ret.Length; i++)
                ret[i] = val;
            return ret;
        }

        public static T HHV<T>(this T[] arr)
        {
            return arr.Max();
        }

        public static T LLV<T>(this T[] arr)
        {
            return arr.Min();
        }
    }
}
