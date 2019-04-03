using System.Linq;
using WolfInv.com.BaseObjectsLib;
using System;
using System.Collections.Generic;
namespace WolfInv.com.GuideLib.LinkGuid
{
    public class MA: BaseLinkGuideClass
    {
        int N = 5;

        public MA(double[] inData):base(inData)
        {

        }
        public MA(double[] inData, params object[] args) :base(inData,args)
        {
            if (args == null)
                return;
            if (args.Length < 1)
                return;
            if(args[0] is int)
            {
                N = (int)args[0];
            }
            
        }

        public override Matrix GetResult()
        {
            if (N <= 0)
                return null;
            List<double[]> list = new List<double[]>();
            for(int i=0;i<orgList.Length;i++)
            {
                int UseN = i >= N - 1 ? N : i + 1;
                double[] calcArr =new double[UseN];
                Array.Copy(orgList, i, calcArr, 0, UseN);
                list.Add(new double[] { calcArr.Average()});
            }
            return new Matrix(list);
        }

        public override double[] GetNext(Matrix OldData, double NewData)
        {
            double[] ret = new double[1];
            double old = OldData[0, 0];
            int DivCnt = N;
            if (!GTMinCnt())
            {
                old = 0;
                DivCnt = orgList.Length + 1;
            }
            double diff = NewData-old;
            ret[0] = diff / DivCnt; 
            return ret;
        }

        protected override bool GTMinCnt()
        {
            if(orgList.Length >= N)
            {
                return true;
            }
            return false;
        }
    }
}
