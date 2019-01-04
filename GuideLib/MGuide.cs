using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProbMathLib;
using BaseObjectsLib;
namespace GuideLib
{
    [Serializable]
    public abstract class MGuide:DetailStringClass
    {
        protected double[][] _LastResult;
        protected double[] OrgData;
        public MGuide(double[] data)
        {
            OrgData = data;
        }

        public double[][] LastValues
        {
            get
            {
                return _LastResult;
            }
        }

        public abstract double[][] getData();

        public abstract double[][] getLastData();

        public abstract double[] CurrValues{get;}

        public abstract double[][] Ref(int Cycs);

        public double[][] BuffData
        {
            get
            {
                return _LastResult;
            }
        }

        public bool IsUpCross(int SpecSubLineId)
        {
            if (_LastResult.Length < SpecSubLineId) return false;
            if (_LastResult[SpecSubLineId].Length <= 1) return false;
            int len = _LastResult[SpecSubLineId].Length;
            double currVal = OrgData[len - 1];
            double currRet = _LastResult[SpecSubLineId][len - 1];
            double preVal = OrgData[len - 2];
            double preRet = _LastResult[SpecSubLineId][len - 2];
            return (currVal >= currRet && preVal <= preRet);
        }
        public bool IsDownCross(int SpecSubLineId)
        {

            if (_LastResult.Length < SpecSubLineId) return false;
            if (_LastResult[SpecSubLineId].Length <= 1) return false;
            int len = _LastResult[SpecSubLineId].Length;
            double currVal = OrgData[len - 1];
            double currRet = _LastResult[SpecSubLineId][len - 1];
            double preVal = OrgData[len - 2];
            double preRet = _LastResult[SpecSubLineId][len - 2];
            return (currVal <= currRet && preVal >= preRet);
        }
        public bool IsUpCross()
        {
            return IsUpCross(0);
        }
        public bool IsDownCross()
        {
            return IsDownCross(0);
        }

        public bool IsUp(int SpecSubLineId)
        {
            if (_LastResult.Length < SpecSubLineId) return false;
            if (_LastResult[SpecSubLineId].Length <= 1) return false;
            int len = _LastResult[SpecSubLineId].Length;
            double currVal = OrgData[len - 1];
            double currRet = _LastResult[SpecSubLineId][len - 1];
            return (currVal > currRet );
        }
        public bool IsDown(int SpecSubLineId)
        {

            if (_LastResult.Length < SpecSubLineId) return false;
            if (_LastResult[SpecSubLineId].Length <= 1) return false;
            int len = _LastResult[SpecSubLineId].Length;
            double currVal = OrgData[len - 1];
            double currRet = _LastResult[SpecSubLineId][len - 1];
            return (currVal < currRet );
        }

        public bool IsUp()
        {
            return IsUp(0);
        }
        public bool IsDown()
        {
            return IsDown(0);
        }
        
    }

    /// <summary>
    /// 均线
    /// </summary>
    public class MA : MGuide
    {
        int _N;
        public MA(double[] data, int N)
            : base(data)
        {
            _N = N;
            getData();
            //this._LastResult = 
        }

        public override double[][] getData()
        {
            int N = _N;
            double firstVal = 0;
            double sum = 0;
            List<double> ret = new List<double>();
            for (int i = 0; i < this.OrgData.Length; i++)
            {
                if (i < N)
                {
                    sum += OrgData[i];
                    ret.Add(sum/(i+1));
                    firstVal = OrgData[0];
                    continue;
                }
                //firstVal = OrgData[i-N-1];
                sum = sum - firstVal + OrgData[i];
                ret.Add(sum /N);
                firstVal = OrgData[i - N];
            }
            double[][] r = new double[1][];
            r[0] = ret.ToArray();
            _LastResult = r;
            return r;
        }

        public override double[] CurrValues
        {
            get 
            {
                if (this._LastResult.Length != 1)
                    return new double[] { double.NaN };
                if(this._LastResult[0].Length == 0)
                {
                    return new double[] { double.NaN };
                }
                return new double[] { this._LastResult[0][this._LastResult[0].Length - 1] };
            
            }
        }

        public override double[][] getLastData()
        {
            throw new NotImplementedException();
        }

        public override double[][] Ref(int Cycs)
        {
            throw new NotImplementedException();
        }

        
    }

    /// <summary>
    /// EMA
    /// </summary>
    public class SMA : MGuide
    {
        int _N;
        int _M;
        public SMA(double[] data,int N,int M)
            : base(data)
        {
            _N = N;
            _M = M;
        }

        public override double[][] getData()
        {
            int N = _N;
            int M = _M;
            List<double> ret = new List<double>();
            double alpha = (double)M / (N + 1);
            for (int i = 0; i < OrgData.Length; i++)
            {
                if (i == 0)
                {
                    ret.Add(OrgData[i]);
                    continue;
                }
                ret.Add(alpha * OrgData[i] + (1 - alpha) * OrgData[i]);
            }
            double[][] r = new double[1][];
            r[0] = ret.ToArray();
            _LastResult = r;
            return r;
        }

        public double getEMAVal(List<double> vals)
        {
            int N = _N;
            int M = _M;
            double ret = 0;
            if (vals.Count <= 5)
            {
                return vals.Sum()/vals.Count;
            }
            int len = vals.Count;
            List<double> lastArr = vals.GetRange(0,len-1) ;
            double alpha = (double)M/(N+1);
            ret =alpha * vals[len-1] + (1 - alpha) * this.getEMAVal(lastArr);
            return ret;
        }

        public override double[] CurrValues
        {
            get {
                if (this._LastResult.Length != 0)
                    return new double[] { double.NaN };
                if (this._LastResult[0].Length == 0)
                {
                    return new double[] { double.NaN };
                }
                return new double[] { this._LastResult[0][this._LastResult[0].Length - 1] };
            }
        }

        public override double[][] getLastData()
        {
            throw new NotImplementedException();
        }

        public override double[][] Ref(int Cycs)
        {
            throw new NotImplementedException();
        }

    }

    public class EMA : SMA
    {
        public EMA(double[] data, int N)
            : base(data, N, 2)
        {
        }
    }

    /// <summary>
    /// KDJ
    /// </summary>
    public class KDJ : MGuide
    {
        int _K;
        int _D;
        int _J;
        public KDJ(double[] data,int K,int D,int J):base(data)
        {
            _K = K;
            _D = D;
            _J = J;
        }

        public override double[][] getData()
        {
            throw new NotImplementedException();
        }

        public override double[] CurrValues
        {
            get { throw new NotImplementedException(); }
        }

        public override double[][] getLastData()
        {
            throw new NotImplementedException();
        }

        public override double[][] Ref(int Cycs)
        {
            throw new NotImplementedException();
        }

       
    }

    /// <summary>
    /// 布林线
    /// </summary>
    public class Bull : MGuide
    {
        int _M;
        public Bull(double[] data,int M)
            : base(data)
        {
            _M = M;
        }

        public override double[][] getData()
        {
        //////BOLL: MA(CLOSE, M);
        //////UB: BOLL + 2 * STD(CLOSE, M);
        //////LB: BOLL - 2 * STD(CLOSE, M);
            List<double> boll = new List<double>();
            List<double> ub = new List<double>();
            List<double> lb = new List<double>();
            for (int i = 0; i < OrgData.Length; i++)
            {
                int M = _M;
                List<double> PastVal = null;
                if (i < M)
                {
                    PastVal = OrgData.ToList<double>();
                }
                else
                {
                    PastVal = OrgData.ToList<double>().GetRange(OrgData.Length-M,M);
                }
                double std = ProbMath.CalculateStdDev(PastVal);
                double ma = PastVal.Average();
                boll.Add(ma);
                ub.Add(ma + 2 * std);
                lb.Add(ma - 2 * std);
            }

            double[][] r = new double[3][];
            r[0] = boll.ToArray();
            r[1] = ub.ToArray();
            r[2] = lb.ToArray();
            _LastResult = r;//所有子类此方法内均需执行该语句
            return r;
        }

        public override double[] CurrValues
        {
            get {
                if (this._LastResult.Length !=3)
                    return new double[] { double.NaN };
                if (this._LastResult[0].Length == 0)
                {
                    return new double[] { double.NaN };
                }
                return new double[] { 
                    this._LastResult[0][this._LastResult[0].Length - 1],
                    this._LastResult[1][this._LastResult[1].Length - 1],
                    this._LastResult[2][this._LastResult[2].Length - 1]};
            }
        }
        public override double[][] getLastData()
        {
            return getLastData(0);
        }
        double[][] getLastData(int cycs)
        {
            double[] currData = OrgData;
            List<double> boll = new List<double>();
            List<double> ub = new List<double>();
            List<double> lb = new List<double>();
            int M = _M;
            int i = currData.Length - cycs;
            List<double> PastVal = null;
            if (i <= M)
            {
                PastVal = currData.ToList<double>();
            }
            else
            {
                PastVal = currData.ToList<double>().GetRange(i - M, M);
            }
            double std = ProbMath.CalculateStdDev(PastVal);
            double ma = PastVal.Average();
            boll.Add(ma);
            ub.Add(ma + 2 * std);
            lb.Add(ma - 2 * std);
            

            double[][] r = new double[3][];
            r[0] = boll.ToArray();
            r[1] = ub.ToArray();
            r[2] = lb.ToArray();
            _LastResult = r;//所有子类此方法内均需执行该语句
            return r;
        }

        public override double[][] Ref(int Cycs)
        {
            return getLastData(Cycs);
        }

    }

    /// <summary>
    /// MACD
    /// </summary>
    public class MACD : MGuide
    {
        public MACD(double[] data, int M, int L, int N)
            : base(data)
        {
        }
        public override double[][] getData()
        {
            throw new NotImplementedException();
        }

        public override double[] CurrValues
        {
            get { throw new NotImplementedException(); }
        }

        public override double[][] getLastData()
        {
            throw new NotImplementedException();
        }



        public override double[][] Ref(int Cycs)
        {
            throw new NotImplementedException();
        }
    }
}
