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
        public static bool isNAN<T>(this T val)
        {
            dynamic v = val;
            if (val is double)
            {
                
                return double.IsNaN(v);
            }
            if(val is float)
            {
                return float.IsNaN(v);
            }
            return false;
        }
        public static T[] MA<T>(this T[] arr, int arg)
        {
            if(arr.Length<=arg)
            {
                return arr;
            }
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
            if (arr.Length <=N)
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
            try
            {
                return func.Invoke(Narr);
            }
            finally
            {
                Narr = null;
            }
        }

        public static int LastMatchCondition<T>(this T[] arr, T[] cmpArr, Func<T[], T[], bool> func)
        {
            try
            {
                for (int i = arr.Length - 1; i >= 0; i--)
                {
                    T[] arr1 = arr.Take(i).ToArray();
                    T[] arr2 = cmpArr.Take(i).ToArray();
                    bool succ = func.Invoke(arr1, arr2);
                    arr1 = null;
                    arr2 = null;
                    if (succ == true)
                    {
                        return arr.Length - i;
                    }
                }
                return -1;
            }
            finally
            {
                cmpArr = null;
                //GC.Collect();
            }
        
        }

        ////public static int LastMatchCondition<T>(this T val, T[] cmpArr, Func<T[], T[], bool> func)
        ////{
        ////    T[] arr = val.ToConst(cmpArr);
        ////    return arr.LastMatchCondition(cmpArr, func);
        ////}
        public static int LastMatchCondition<T>(this T[] cmpArr, T val, Func<T[], T[], bool> func)
        {
            T[] arr = val.ToConst(cmpArr);
            return cmpArr.LastMatchCondition(arr, func);
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
            dynamic arr01;
            dynamic arr02;
            dynamic arr11;
            dynamic arr12;
            try
            {
                arr01 = arr.Last(1);
                arr02 = arr.Last(2);
                arr11 = arr1.Last(1);
                arr12 = arr1.Last(2);
                if (arr01 > arr11 && arr02 < arr12)
                {
                    return true;
                }
                return false;
            }
            finally
            {
                arr01 = null;
                arr02 = null;
                arr11 = null;
                arr12 = null;
                arr = null;
                arr1 = null;
                //GC.Collect();
            }
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

        public static T[] ToConst<T>(this T[] arr,T val)
        {
            T[] ret = new T[arr.Length];//
            for (int i = 0; i < arr.Length; i++)
                ret[i] = val;
            return ret;
        }

        public static long ToWeight<T>(this T val,long weight)
        {
            dynamic v = val;
            try
            {
                return (long)(weight * v);
            }
            finally
            {
                v = null;
            }
        }

        public static T ToSimpleNumber<T>(this T val,int levelStepLen=10)
        {
            dynamic v = val;
            v =  Math.Floor((double)v / levelStepLen);
            return v;
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
                T[] temp1 = arr1.FirstSector(baseCnt + i);
                T[] temp2 = arr2.FirstSector(baseCnt + i);
                bool succ = func.Invoke(temp1, temp2);
                temp1 = null;
                temp2 = null;
                if (succ)
                    matchCnt++;
            }
            return matchCnt;
        }

        public static T[] MaxMinArray<T>(this T[] arr, int minSplitLength, bool needSerialData = false)
        {
            MaxMinElementClass<T>[] mmarr = getMaxMinElementList(arr);
            MaxMinElementClass<T>[] tmp = MaxMinArray<T>(mmarr, minSplitLength,false);
            try
            {
                while (tmp.Length < mmarr.Length)
                {
                    mmarr = tmp;
                    tmp = MaxMinArray<T>(mmarr, minSplitLength, false);
                }
                tmp = MaxMinArray<T>(tmp, minSplitLength, true);
                if (needSerialData)
                {
                    return getVirturlFullSerial<T>(tmp);
                }
                return tmp.Select(a => a.Value).ToArray();
            }
            finally
            {
                mmarr = null;
            }
        }

        public static MaxMinElementClass<T>[] MaxMinArray<T>(this T[] arr, int minSplitLength)
        {
            MaxMinElementClass<T>[] mmarr = getMaxMinElementList(arr);
            try
            {
                
                MaxMinElementClass<T>[] tmp = MaxMinArray<T>(mmarr, minSplitLength, false);
                while (tmp.Length < mmarr.Length)//为偶数，再算一次
                {
                    mmarr = tmp;
                    tmp = MaxMinArray<T>(mmarr, minSplitLength, false);
                }
                tmp = MaxMinArray<T>(tmp, minSplitLength, true);
                return tmp;
            }
            finally
            {
                mmarr = null;
            }
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
            try
            {
                if (arr.Length < 3)
                    return arr;
                if (!SimpleFilter)
                    return MaxMinArrayUseRange(arr, minInter);
                List<MaxMinElementClass<T>> ret = new List<MaxMinElementClass<T>>();
                ret.Add(arr.First());
                for (int i = 1; i < arr.Length - 1; i++)
                {
                    MaxMinElementClass<T> pre = arr[i - 1];
                    MaxMinElementClass<T> curr = arr[i];
                    MaxMinElementClass<T> next = arr[i + 1];

                    dynamic preval = pre.WeightValue;
                    dynamic currval = curr.WeightValue;
                    dynamic nextval = next.WeightValue;
                    if ((preval - currval) * (currval - nextval) > 0)//中间值
                    {
                        continue;
                    }
                    ret.Add(arr[i]);
                }
                ret.Add(arr.Last());
                return ret.ToArray();
            }
            finally
            {
                arr = null;
            }
        }

        public static MaxMinElementClass<T>[] getMaxMinElementList<T>(this T[] arr)
        {
            try
            {
                MaxMinElementClass<T>[] ret = new MaxMinElementClass<T>[arr.Length];
                T[] ema2 = arr.EMA(2);//区分3个，所以要2
                for (int i = 0; i < arr.Length; i++)
                {
                    ret[i] = new MaxMinElementClass<T>(i, arr[i], ema2[i]);
                }
                ema2 = null;
                return ret;
            }
            finally
            {
                arr = null;
            }
        }

        static MaxMinElementClass<T>[] MaxMinArrayUseRange<T>(MaxMinElementClass<T>[] arr, int minInter)
        {
            if (arr.Length < 3)
                return arr;
            if(arr.Length == 3)
            {
                dynamic mid = arr[1].Value;
                dynamic se = arr[0].Value;
                dynamic ee = arr[2].Value;
                if((mid>se && mid>ee)||(mid<se && mid<ee))//如果中间值为峰值
                {
                    return arr;
                }
                return new MaxMinElementClass<T>[]{ arr[0],arr[2]};
            }
            if(arr.Length == 4)//总共只有4个元素
            {
                //如果本身就只有4个元素，根据第一个和最后一个的趋势判定趋势，为上，取大值，否则，取小值
                dynamic lastVal = arr[3].Value;
                dynamic lastWeight = arr[3].WeightValue;
                dynamic firstVal = arr[0].Value;
                dynamic firstWeight = arr[0].WeightValue;
                //如果最后一个大于第一个，或者相等但是最后一个权重大于第一个
                bool isUp = (lastVal > firstVal)||(lastVal==firstVal &&lastWeight>firstWeight);
                dynamic secondVal = arr[1].Value;
                dynamic secondWeight = arr[1].WeightValue;
                dynamic threeVal = arr[2].Value;
                dynamic threeWeight = arr[2].WeightValue;
                bool bigIs2 = secondVal > threeVal || (secondVal == threeVal && secondWeight > threeWeight);
                int bigIndex = (int)(1 + Math.Pow(0, bigIs2 ? 1 :0));
                int smallIndex = (int)(1 + Math.Pow(0, bigIs2 ? 0 : 1));
                MaxMinElementClass<T> bigVal = arr[bigIndex];
                MaxMinElementClass<T> smallVal = arr[smallIndex];
                secondVal = null;
                secondWeight = null;
                threeVal = null;
                threeWeight = null;
                lastVal = null;
                lastWeight = null;
                firstVal = null;
                firstWeight = null;
                return new MaxMinElementClass<T>[] { arr[0], isUp?bigVal:smallVal , arr[3] };

            }

            List<MaxMinElementClass<T>> ret = new List<MaxMinElementClass<T>>();
            ret.Add(arr.First());
            try
            {
                for (int i = 1; i < arr.Length - 1; i++)
                {
                    MaxMinElementClass<T> curr = arr[i];
                    if (curr.Value.isNAN())
                    {
                        continue;
                    }
                    var Items = arr.Where(a => (!a.Value.isNAN() && a.index >= curr.index - minInter && a.index <= curr.index + minInter && a.index != curr.index));
                    if (Items.Count() == 0)
                    {
                        ret.Add(arr[i]);
                        continue;
                    }
                    dynamic currVal = curr.Value;
                    dynamic currWeight = curr.WeightValue;
                    dynamic maxval = Items.Max(a => a.Value); //先去获得极值
                    dynamic minval = Items.Min(a => a.Value);
                    //再定位最大最小值的确定值
                    var maxItem = Items.Where(a =>
                    {
                        dynamic val = a.Value;
                        return val == maxval;
                    }).OrderBy(a => a.WeightValue).Last();//最大值
                    var minItem = Items.Where(a =>
                    {
                        dynamic val = a.Value;
                        return val == minval;
                    }).OrderBy(a => a.WeightValue).First();//最小值
                    if ((currVal < minval || (currVal == minval && currWeight < minItem.WeightValue))
                        || (currVal > maxval || (currVal == maxval && currWeight > maxItem.WeightValue)))//极值
                    {
                        ret.Add(arr[i]);
                    }
                    minItem = null;
                    Items = null;
                }
                
                ret.Add(arr.Last());
                return ret.ToArray();
            }
            finally
            {
                arr = null;
            }
        }
        

    }

    public class MaxMinElementClass<T>
    {
        public int index;
        public T Value;
        /// <summary>
        /// EMA值
        /// </summary>
        public T WeightValue;

        public MaxMinElementClass(int i,T v,T weight)
        {
            index = i;
            Value = v;
            WeightValue = weight;
        }

        
    }
}
