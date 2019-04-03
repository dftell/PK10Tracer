using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.GuideLib.LinkGuid
{
    public abstract class BaseLinkGuideClass:List<double>
    {
        protected object[] _args;
        protected double[] orgList;
        protected BaseLinkGuideClass()
        {
            orgList = new double[0] ;
        }

        public BaseLinkGuideClass(double[] inData)
        {
            if (inData == null || inData.Length == 0)
                return;
            orgList = inData;

        }

        public void ResetData(double[] data)
        {
            orgList = data;
        }

        public  BaseLinkGuideClass(double[] inData, params object[] args)
        {
            if (inData == null || inData.Length == 0)
                return;
            orgList= inData;
            _args = args;
        }

        public abstract Matrix GetResult();

        public Matrix GetResult(Matrix OldData,double NewData,bool UseSameLength = false)
        {
            List<double[]> list = OldData.ToList();
            if (UseSameLength)
            {
                if (GTMinCnt())
                    list.RemoveAt(0);
            }
            list.Add(GetNext(OldData, NewData));
            Matrix ret = new Matrix(list);
            List<double> NewList = new List<double>();
            NewList.AddRange(orgList);
            NewList.Add(NewData);
            orgList = NewList.ToArray();
            return ret;
        }

        public abstract double[] GetNext(Matrix OldData, double NewData);

        protected abstract bool GTMinCnt();
    }

    

   
}
