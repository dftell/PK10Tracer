using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProbMathLib
{
    public class FixCountMethods
    {
        static Dictionary<string, Dictionary<int, Int64>> buffs_Val = new Dictionary<string, Dictionary<int, long>>();
        static Dictionary<string, Dictionary<int, Int64>> buffs_Sum = new Dictionary<string, Dictionary<int, long>>();
        static string str_model_val = "val_{0}_{1}_{2}_{3}_{4}";
        static string str_model_sum = "val_{0}_{1}_{2}_{3}_{4}";
        /// <summary>
        /// 获得机会需要下注的数量
        /// </summary>
        /// <param name="Chips">注数</param>
        /// <param name="HoldTimeCnt">持有次数</param>
        /// <param name="Odds">赔率</param>
        /// <param name="MinWinRate">最小盈利比例</param>
        /// <returns></returns>
        public static Int64 getTheAmount(int Chips, int HoldTimeCnt, double Odds, double MinWinRate)
        {
            return getTheAmount(Chips, HoldTimeCnt, 1, Odds, MinWinRate);
        }
        /// <summary>
        /// 获得机会需要下注的数量
        /// </summary>
        /// <param name="Odds"></param>
        /// <param name="MinWinRate"></param>
        /// <param name="Chips">注数</param>
        /// <param name="HoldTimeCnt">持有次数</param>
        /// <param name="FirstAmount"></param>
        /// <param name="Odds">赔率</param>
        /// <param name="MinWinRate">最小盈利比例</param>
        /// <returns></returns>
        public static Int64 getTheAmount(int Chips, int HoldTimeCnt, int FirstAmount,double Odds, double MinWinRate)
        {
            //y= sum(n-1)/odds
            if (HoldTimeCnt < 1) return 0;
            if (HoldTimeCnt == 1) return FirstAmount;
            string strKey = string.Format(str_model_val, Chips, HoldTimeCnt, FirstAmount, Odds, MinWinRate);
            Dictionary<int, Int64> Vals = new Dictionary<int, long>();
            if (!buffs_Val.ContainsKey(strKey))
            {
                buffs_Val.Add(strKey, Vals);
            }
            Vals = buffs_Val[strKey];
            if (Vals.ContainsKey(HoldTimeCnt))
            {
                return Vals[HoldTimeCnt];
            }
            Int64 LastSum = getSum(Chips, HoldTimeCnt - 1, FirstAmount, Odds, MinWinRate);//前值
            //Int64 TryRet = (Int64)Math.Floor(LastSum / Odds);//保本
            //(y*odds-sum(n-1)-y*Chips)/(sum(n-1)+y*Chips)>minwinrate
            //y*odds/(sum(n-1)+y*Chips)-1>minwinrate
            //y*odds/(sum(n-1)+y*Chips)>minwinrate+1
            //y*0dds>(minwinrate+1)*(sum(n-1)+y*Chips)
            //y*odds-(minwinrate+1)*y*Chips>(minwinrate+1)*sum(n-1)
            //y*(odds-(minwinrate+1)*Chips)>(minwinrate+1)*sum(n-1)
            //y>(miwinrate+1)*sum(n-1)/(odds-(minwinrate+1)*Chips)


            //y*odds-sum(n-1)-y>minwinrate*sum(n-1)+minwinrate*y
            //y*(odds-minwinrate-1)>minwinrate*sum(n-1)+sum(n-1)
            //y>(minwinrate*sum(n-1)+sum(n-1))/(odds-minwinrate-1)
            Int64 TryRet = (Int64)Math.Floor((MinWinRate + 1) * LastSum / (Odds - (MinWinRate + 1)*Chips));//试算
            if (TryRet < FirstAmount)//判断是否小于第一次的数值，支持某些网站有最小金额限制
                TryRet = FirstAmount;
            Int64 LastRet = TryRet;
            double RealWr = (double)((LastRet * Odds) / (LastSum + LastRet * Chips) - 1);
            while (RealWr < MinWinRate)//微调，如果取整后所得盈亏比例小于最小盈亏比例，每次加一，直到满足条件为欸止
            {
                LastRet++;
                RealWr = (double)((LastRet * Odds) / (LastSum + LastRet * Chips) - 1);
            }
            Vals.Add(HoldTimeCnt, LastRet);
            return LastRet;

        }

        static Int64 getSum(int Chips, int HoldTimeCnt, int FirstAmount, double Odds, double MinWinRate)
        {
            if(HoldTimeCnt <1) return 0;
            if (HoldTimeCnt == 1) return FirstAmount*Chips;
            return getSum(Chips, HoldTimeCnt - 1, FirstAmount, Odds, MinWinRate) + getTheAmount(Chips, HoldTimeCnt, FirstAmount, Odds, MinWinRate)*Chips;
        }
    }
}
