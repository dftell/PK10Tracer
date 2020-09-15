using System;

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

        public static T[] MACD<T>(this T[] arr,int S=12,int L=26,int MID=9)
        {
            /*
DIF:EMA(CLOSE,S)-EMA(CLOSE,L);
DEA:EMA(DIF,MID);
MACD:(DIF-DEA)*2,COLORSTICK;
             */
            T[] sEma = arr.EMA(S);
            T[] lEma = arr.EMA(L);
            T[] difArr = new T[arr.Length];
            for(int i=0;i<arr.Length;i++)
            {
                dynamic sa = sEma[i];
                dynamic la = lEma[i];
                dynamic dif = sa-la;
                difArr[i] = dif;
            }
            T[] deaArr = difArr.EMA(MID);
            T[] ret = new T[arr.Length];
            for (int i=0;i<arr.Length;i++)
            {
                dynamic difa = difArr[i];
                dynamic deaa = deaArr[i];
                ret[i] = (difa - deaa) * 2;
            }
            return ret;
        }
    }
}
