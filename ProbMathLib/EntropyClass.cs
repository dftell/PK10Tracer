using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProbMathLib
{
    public class EntropyClass
    {
        /// <summary>
        /// 获得熵
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double GetEntropy(double[] p)
        {
            double sum = 0;
            for(int i=0;i<p.Length;i++)
            {
                sum += p[i]*Math.Log10(p[i]);
            }
            return -1*sum;
        }

        /// <summary>
        /// 贝叶斯公式
        /// </summary>
        /// <param name="P(A)">P(A)</param>
        /// <param name="P(B|A)">P(B|A)</param>
        /// <param name="P(B)">P(B)</param>
        /// <returns></returns>
        public static double GetBayes(double A, double BA,double B)
        {
            return A * BA / B;
        }

        /// <summary>
        /// 贝叶斯公式
        /// </summary>
        /// <param name="Atrue">prob of A is True</param>
        /// <param name="Afalse">prob of A is False</param>
        /// <param name="Btrue">prob of B is True</param>
        /// <param name="Bfalse">prob of B is False</param>
        /// <returns></returns>
        public static double GetBayes(double At, double Af,double Bt, double Bf)
        {
            return At * Bt / (At * Bt + Af * Bf);
        }
    }

   
}
