using WolfInv.com.BaseObjectsLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WolfInv.com.GuideLib.LinkGuid
{
    public class EMA: BaseLinkGuideClass
    {
        int N = 5;
        public EMA(double[] inData):base(inData)
        {

        }
        public EMA(double[] inData, params object[] args) : base(inData,args)
        {
            if (args == null)
                return;
            if (args.Length < 1)
                return;
            if (args[0] is int)
            {
                N = (int)args[0];
            }
            
        }

        public override Matrix GetResult()
        {
            if (N <= 0)
                return null;
            List<double[]> list = new List<double[]>();
            double alpha = 2.0 / (1.0 + N * 1.0);
            for (int i = 0; i < orgList.Length; i++)
            {
                double[] useArr = new double[i + 1];
                Array.Copy(orgList, useArr, i + 1);
                if(i<=5)
                {
                    list.Add(new double[] { useArr.Average() });
                    continue;
                }
                double CurrVal = orgList[i];
                double PreVal = list[list.Count-1][0];
                list.Add(new double[] { alpha * CurrVal + (1 - alpha) * list[list.Count-1][0] });
            }
            return new Matrix(list);
        }

        double GetEMAValue_del(double[] arr,int N)
        {
            if (arr.Length <= 1)
                return arr.Average();
            double alpha = 2.0 / (1.0 + N*1.0);
            double NewestVal = arr[arr.Length - 1];
            double[] newarr = new double[arr.Length - 1];
            Array.Copy(arr, newarr, arr.Length - 1);
            double preVal =  newarr.Average();
            return alpha * NewestVal + (1 - alpha) * preVal;
        }

        public override double[] GetNext(Matrix OldData, double NewData)
        {
            double alpha = (2.0 / (1.0 + N));
            if (GTMinCnt())
            {
                double NewVal = alpha * NewData + (1 - alpha) * OldData[OldData.rowNum - 1, 0];
                return new double[]{ NewVal};
            }
            return new double[] { (OldData.ToList().Select(a => a[0]).Sum() + NewData) / (OldData.rowNum + 1.0) };
        }

        protected override bool GTMinCnt()
        {
            if (orgList.Length >= 1) return true;
            return false;
        }
    }
}
