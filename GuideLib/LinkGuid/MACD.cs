using WolfInv.com.BaseObjectsLib;
using System;
using System.Collections.Generic;
using System.Linq;
namespace WolfInv.com.GuideLib.LinkGuid
{
    public class MACD: BaseLinkGuideClass
    {
        int S = 12;
        int L = 26;
        int M = 9;
        List<double> DIFList = new List<double>();
        List<double> DEAList = new List<double>();
        List<double> MACDList = new List<double>();
        public MACD(double[] inData):base(inData)
        {

        }
        public MACD(double[] inData,object[] args) : base(inData,args)
        {
            if (args == null)
                return;
            if (args.Length < 3)
                return;
            if (args[0] is int)
            {
                S = (int)args[0];
            }
            if (args[1] is int)
            {
                L = (int)args[0];
            }
            if (args[2] is int)
            {
                M = (int)args[0];
            }
        }
        EMA SEMA = null;
        EMA LEMA = null;
        EMA DEAEMA = null;
        Matrix MS = null;
        Matrix ML = null;
        Matrix MM = null;

        void initEMAs()
        {
            SEMA = null;
            LEMA = null;
            DEAEMA = null;
            MS = null;
            ML = null;
            MM = null;
        }
        public override Matrix GetResult()
        {
            initEMAs();
            if (S <= 0 || L<=0||M<=0)
                return null;
            //DIFList = new List<double>();
            
            int cnt = orgList.Length;
            SEMA = new EMA(orgList, S);
            LEMA = new EMA(orgList, L);
            MS = SEMA.GetResult();
            ML = LEMA.GetResult();
            DIFList = new List<double>();
            for(int i=0;i<MS.rowNum;i++)
            {
                DIFList.Add(MS[i, 0] - ML[i, 0]);
            }
            DEAEMA = new EMA(DIFList.ToArray(), M);
            MM = DEAEMA.GetResult();
            DEAList = MM.ToList().Select(a=>a[0]).ToList();
            MACDList = new List<double>();
            List<double[]> ret = new List<double[]>();
            for(int i=0;i<DIFList.Count;i++)
            {
                MACDList.Add(2 * (DIFList[i] - DEAList[i]));
                ret.Add(new double[] { DIFList[i],DEAList[i],MACDList[i]});
            }
            return new Matrix(ret);
        }

        public override double[] GetNext(Matrix OldData, double NewData)
        {
            double[] newSVal = SEMA.GetNext(MS, NewData);
            double[] newLVal = LEMA.GetNext(ML, NewData);
            double diff = newSVal[0] - newLVal[0];
            double[] newDEAVal = DEAEMA.GetNext(MM, diff);
            double dea = newDEAVal[0];
            MS.AddRow(newSVal);
            ML.AddRow(newLVal);
            MM.AddRow(newDEAVal);
            return new double[] { diff, dea, 2 * (diff - dea) };
        }

        protected override bool GTMinCnt()
        {
            return true;
        }
    }
}
