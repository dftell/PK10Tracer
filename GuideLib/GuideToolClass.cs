using System;
using System.Collections.Generic;
using System.Linq;
namespace WolfInv.com.GuideLib
{
    public class MACDCollection : List<MACDGuider>
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

        public static T[] Times<T>(this T[] arr, T times)
        {
            T[] ret = new T[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                dynamic res = arr[i];
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


        public static MACDCollection MACD<T>(this T[] arr, int S = 12, int L = 26, int MID = 9)
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
            for (int i = 0; i < arr.Length; i++)
            {
                ret.Add(new MACDGuider());
                ret[i] = new MACDGuider();
                dynamic sa = sEma[i];
                dynamic la = lEma[i];
                dynamic dif = sa - la;
                difArr[i] = dif;
                ret[i].DIF = dif;
            }
            T[] deaArr = difArr.EMA(MID);
            for (int i = 0; i < arr.Length; i++)
            {
                dynamic difa = difArr[i];
                dynamic deaa = deaArr[i];
                ret[i].DEA = deaa;
                ret[i].MACD = (difa - deaa) * 2;
            }
            return ret;
        }

        public static T[] LastSector<T>(this T[] arr, int len)
        {
            if (len >= arr.Length)
            {
                return arr;
            }
            return arr.Skip(arr.Length - len).ToArray();
        }

        public static T[] FirstSector<T>(this T[] arr, int len)
        {
            if (len >= arr.Length)
            {
                return arr;
            }
            return arr.Take(len).ToArray();
        }

        public static dynamic Ref<T>(this Func<T[][], dynamic> func, int N, params T[][] arr)
        {
            T[][] Narr = new T[arr.Length][];
            for (int i = 0; i < arr.Length; i++)
            {
                Narr[i] = arr[i].FirstSector(N);
            }
            return func.Invoke(Narr);
        }

        public static int LastMatchCondition<T>(this T[] arr, T[] cmpArr, Func<T[], T[], bool> func)
        {
            for (int i = arr.Length - 1; i >= 0; i--)
            {
                bool succ = func.Invoke(arr.Take(i).ToArray(), cmpArr.Take(i).ToArray());
                if (succ == true)
                {
                    return arr.Length - i;
                }
            }
            return -1;
        }

        public static int LastMatchCondition<T>(this T val, T[] cmpArr, Func<T[], T[], bool> func)
        {
            T[] arr = val.ToConst(cmpArr);
            return arr.LastMatchCondition(cmpArr, func);
        }

        public static T ToEquitPrice<T>(this T price, int defaultDec = 3)
        {
            dynamic val = price;
            return Math.Round(val, defaultDec);
        }


        /// <summary>
        /// 返回倒数第N个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static T Last<T>(this T[] arr, int position)
        {
            if (position <= 0)
                return default(T);
            if (position > arr.Length)
            {
                return default(T);
            }
            return arr[arr.Length - position];
        }
        public static bool CrossUp<T>(this T[] arr, T val)
        {
            return CrossUp(arr, ToConst(val, arr));
        }
        public static bool Cross<T>(this T[] arr, T val)
        {
            return CrossUp(arr, ToConst(val, arr));
        }

        public static bool Cross<T>(this T val, T[] arr)
        {
            return CrossUp(val.ToConst(arr), arr);
        }
        public static bool Cross<T>(this T[] arr, T[] arr1)
        {
            return CrossUp(arr, arr1);
        }
        public static bool CrossUp<T>(this T[] arr, T[] arr1)
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

        public static bool CrossDown<T>(this T[] arr, T val)
        {
            return CrossDown(arr, ToConst(val, arr));
        }

        public static bool CrossDown<T>(this T[] arr, T[] arr1)
        {
            dynamic arr01 = arr.Last(1);
            dynamic arr02 = arr.Last(2);
            dynamic arr11 = arr1.Last(1);
            dynamic arr12 = arr1.Last(2);

            if (arr01 < arr11 && arr02 > arr12)
            {
                return true;
            }
            return false;
        }

        public static double ToDbl<T>(this T val)
        {
            return double.Parse(val.ToString());
        }

        public static T[] ToConst<T>(this T val, T[] arr)
        {
            T[] ret = new T[arr.Length];//
            for (int i = 0; i < arr.Length; i++)
                ret[i] = val;
            return ret;
        }

        public static T HHV<T>(this T[] arr, int N = -1)
        {
            if (N <= 0)
            {
                N = arr.Length;
            }
            return arr.LastSector(N).Max();
        }

        public static T LLV<T>(this T[] arr, int N = -1)
        {
            if (N <= 0)
            {
                N = arr.Length;
            }
            return arr.LastSector(N).Min();
        }

        public static int Count<T>(this T[] arr1, Func<T[], T[], bool> func, T[] arr2, int N)
        {
            int matchCnt = 0;
            if (N > arr1.Length)
                return 0;
            int baseCnt = arr1.Length - N;
            for (int i = 0; i < N; i++)
            {
                bool succ = func.Invoke(arr1.FirstSector(baseCnt + i), arr2.FirstSector(baseCnt + i));
                if (succ)
                    matchCnt++;
            }
            return matchCnt;
        }

        public static T[] MaxMinArray<T>(this T[] arr, int minSplitLength, bool needSerialData = false)
        {
            MaxMinElementClass<T>[] mmarr = getMaxMinElementList(arr);
            MaxMinElementClass<T>[] tmp = MaxMinArray<T>(mmarr, minSplitLength,false);
            while (tmp.Length< mmarr.Length)
            {
                mmarr = tmp;
                tmp = MaxMinArray<T>(mmarr, minSplitLength,false);
            }
            tmp = MaxMinArray<T>(tmp, minSplitLength, true);
            if(needSerialData)
            {
                return getVirturlFullSerial<T>(tmp);
            }
            return tmp.Select(a => a.Value).ToArray();
        }

        public static MaxMinElementClass<T>[] MaxMinArray<T>(this MaxMinElementClass<T>[] arr, int minSplitLength)
        {
            MaxMinElementClass<T>[] mmarr = arr;// MaxMinElementClass<T>.getMaxMinElementList(arr);
            MaxMinElementClass<T>[] tmp = MaxMinArray<T>(mmarr, minSplitLength, false);
            while (tmp.Length < mmarr.Length)
            {
                mmarr = tmp;
                tmp = MaxMinArray<T>(mmarr, minSplitLength, false);
            }
            tmp = MaxMinArray<T>(tmp, minSplitLength, true);
            return tmp;//.Select(a => a.Value).ToArray();
        }

        public static T[] getVirturlFullSerial<T>(MaxMinElementClass<T>[] mmarr)
        {
            List<T> ret = new List<T>();
            ret.Add(mmarr[0].Value);
            for(int i=1;i<mmarr.Length;i++)
            {
                if(mmarr[i].index == ret.Count)
                {
                    ret.Add(mmarr[i].Value);
                    continue;
                }
                ret.AddRange(getSerialArr<T>(ret.Last(), mmarr[i].Value, mmarr[i].index+1 - ret.Count));

            }
            return ret.ToArray();
        }

        static T[] getSerialArr<T>(T from,T to,int len)
        {
            dynamic fromVal = from;
            dynamic toVal = to;
            dynamic diffVal = (toVal - fromVal) / len;
            T[] ret = new T[len];
            for (int i=0;i<len;i++)
            {
                ret[i] = fromVal + (i + 1) * diffVal;
            }
            return ret.ToArray();
        }



        static MaxMinElementClass<T>[] MaxMinArray<T>(this MaxMinElementClass<T>[] arr,int minInter,bool SimpleFilter)
        {
            if (arr.Length < 3)
                return arr;
            if(!SimpleFilter)
                return MaxMinArrayUseRange(arr, minInter);
            List<MaxMinElementClass<T>> ret = new List<MaxMinElementClass<T>>();
            ret.Add(arr.First());
            for (int i = 1; i < arr.Length-1; i++)
            {
                MaxMinElementClass<T> pre = arr[i - 1];
                MaxMinElementClass<T> curr = arr[i];
                MaxMinElementClass<T> next = arr[i + 1];
                
                dynamic preval = pre.Value;
                dynamic currval = curr.Value;
                dynamic nextval = next.Value;
                if ((preval - currval) * (currval - nextval) > 0)//中间值
                {
                    continue;
                }
                ret.Add(arr[i]);
            }
            ret.Add(arr.Last());
            return ret.ToArray();
        }

        public static MaxMinElementClass<T>[] getMaxMinElementList<T>(this T[] arr)
        {
            MaxMinElementClass<T>[] ret = new MaxMinElementClass<T>[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                ret[i] = new MaxMinElementClass<T>(i, arr[i]);
            }
            return ret;
        }

        static MaxMinElementClass<T>[] MaxMinArrayUseRange<T>(MaxMinElementClass<T>[] arr, int minInter)
        {
            if (arr.Length < 3)
                return arr;
            List<MaxMinElementClass<T>> ret = new List<MaxMinElementClass<T>>();
            ret.Add(arr.First());
            for (int i = 1; i < arr.Length - 1; i++)
            {
                MaxMinElementClass<T> curr = arr[i];
                var Items = arr.Where(a => (a.index > curr.index -minInter && a.index < curr.index + minInter && a.index != curr.index));
                if (Items.Count() == 0)
                {
                    ret.Add(arr[i]);
                    continue;
                }
                dynamic currval = curr.Value;
                dynamic maxval = Items.Max(a => a.Value);
                dynamic minval = Items.Min(a => a.Value);
                if (currval>=minval && currval<=maxval)//中间值
                {
                    continue;
                }
                ret.Add(arr[i]);
            }

            ret.Add(arr.Last());
            return ret.ToArray();
        }


    }

    public class MaxMinElementClass<T>
    {
        public int index;
        public T Value;
        public MaxMinElementClass(int i,T v)
        {
            index = i;
            Value = v;
        }

        
    }
}
