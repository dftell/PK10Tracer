using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProbMathLib
{
    public class ProbMath
    {
        public static double CalculateStdDev(IEnumerable<double> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                //  计算平均数   
                double avg = values.Average();
                //  计算各数值与平均数的差值的平方，然后求和 
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                //  除以数量，然后开方
                ret = Math.Sqrt(sum / values.Count());
            }
            return ret;
        }

        public static double CalculateStdDev(IEnumerable<int> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                //  计算平均数   
                double avg = (double)values.Average();
                //  计算各数值与平均数的差值的平方，然后求和 
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                //  除以数量，然后开方
                ret = Math.Sqrt(sum / values.Count());
            }
            return ret;
        }

        public static decimal GetFactorial(int N)
        {
            return GetFactorial(N, N);
        }

        public static decimal GetFactorial(int N, int M)
        {
            if (N == 0) return 1;
            decimal ret = 1;
            for (int i = N-M+1; i <= N; i++)
            {
                ret = ret * i;
            }
            return ret;
        }

        /// <summary>
        /// 计算泊松分布
        /// </summary>
        /// <param name="N"></param>
        /// <param name="NormalWinRate"></param>
        /// <returns></returns>
        public static double Poission(int N, double NormalWinRate)
        {
            return GetPoission(N, NormalWinRate, 1);
        }
        /// <summary>
        /// 计算泊松分布
        /// </summary>
        /// <param name="N">此后的N期中</param>
        /// <param name="NormalWinRate">正常胜率</param>
        /// <param name="ExpectMatchNeedTimes">期望第多少次命中，默认是1</param>
        /// <returns></returns>
        public static double GetPoission(int N, double NormalWinRate,int ExpectMatchNeedTimes)
        {
            //P(M)= 1;
            double ExpectWinRate = 0;
            if(ExpectMatchNeedTimes >0)
                ExpectWinRate = 1 / ExpectMatchNeedTimes;//期望第ExpectMatchNeedTimes次内的概率 
            return Math.Pow(Math.E, -1 * (NormalWinRate * N)) * Math.Pow(NormalWinRate * N, ExpectWinRate) / (long)GetFactorial(ExpectMatchNeedTimes);
        }

        public static double GetBinomial(int N,int K,double p)
        {
            double ret = (long)GetCombination(N,K) * Math.Pow(p,K)*Math.Pow(1-p,N-K);
            
            return ret;
        }
        
        public static decimal GetCombination(int N, int M)
        {
            if (M < 15)
            {
                return GetFactorial(N, M) / GetFactorial(M, M);
            }
            HashSet<int> Pn = getP(N, M);
            HashSet<int> Pm = getP(M, M);
            HashSet<int> Rm = new HashSet<int>();
            HashSet<int> Rn = new HashSet<int>();
            Pm.ToList().ForEach(p => Rm.Add(p));
            Pn.ToList().ForEach(p => Rn.Add(p));
            foreach (int key in Rm)
            {
                if (Rn.Contains(key))
                {
                    Pm.Remove(key);
                    Pn.Remove(key);
                }
            }
            List<int> Lm = new List<int>();
            List<int> Ln = new List<int>();
            Rm.ToList().ForEach(p => Lm.Add(p));
            Rn.ToList().ForEach(p => Ln.Add(p));
            decimal ret = 1;
            CalcList(ref Ln, ref Lm);
            CalcList(ref Lm, ref Ln);
            CalcList(ref Ln, ref Lm);
            Ln.ForEach(p => ret = ret * p);
            Lm.ForEach(p => ret = ret / p);
            CalcList(ref Ln, ref Lm);
            CalcList(ref Lm, ref Ln);
            return ret;
        }

        static HashSet<int> getP(int N, int M)
        {
            HashSet<int> ret = new HashSet<int>();
            for (int i = N - M + 1; i <= N; i++)
            {
                ret.Add(i);
            }
            return ret;
        }

        static void CalcList(ref List<int> Ln,ref List<int> Lm)
        {
            for (int i = Ln.Count - 1; i >= 0; i--)
            {
                int Ni = Ln[i];
                if (Ni == 1) continue;
                for (int j = Lm.Count - 1; j >= 0; j--)
                {
                    Ni = Ln[i]; //一定要重新读取
                    int Mi = Lm[j];
                    if (Mi == 1) continue;
                    bool NDev = false;
                    if (Ni % Mi == 0)
                    {
                        Ln[i] = Ni / Mi;
                        //Ni = Ln[i];
                        Lm.RemoveAt(j);
                        //ret = Ln[i];
                        NDev = true;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static double GetGeometric()
        {
            return 0;
        }
    }

    
}
