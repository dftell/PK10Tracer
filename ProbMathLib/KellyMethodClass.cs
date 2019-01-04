using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProbMathLib
{
    public class KellyMethodClass
    {
        //f*=(p*rW-q*rL)/（rLrW）f*=（bp-q)/b
        /// <summary>5/9.75=0.51282
        /// (b*p-q)>0 => b*p-(1-p)>0 = >b*p-1+p>0=>b*p+p>1=>p>1/(1+b)
        /// 1/(1+(9.75-5)/5)=0.338983 3.33898*1.95>
        /// 0.5128 0.48718 0.51282*0.95 *1.005 - 0.48718=0.002434895/
        /// </summary>
        /// <param name="Chips"></param>
        /// <param name="TotalCnt"></param>
        /// <param name="Odds"></param>
        /// <returns></returns>
        /// // b*p-q>0
        public static double KellyFormula(int Chips,int TotalCnt,double Odds,double MinRate)
        {
            double p, q, b;
            b = (double)Odds/Chips;
            p = (double) Chips/ Odds*MinRate;//Chips/Odd*MinRate
            q = 1 - p;
            return (double)(b * p - q) / b;
            
        }

        public static double KellyFormula(int Chips, double Odds,double SpecP)
        {
            double p, q, b;
            b = (double)Odds / Chips;
            p = SpecP;
            q = 1 - p;
            return (double)(b * p - q) / b;

        }
    }
}
