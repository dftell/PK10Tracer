using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;
using System.Threading;
using WolfInv.com.ProbMathLib;

namespace WolfInv.com.MachineLearnLib.Markov
{
    public class MarkovClass : MachineLearnClass<int, int>
    {
        public override int Classify(MLInstance<int, int> instances)
        {
            return 0;
        }

        public override void FillStructBySummary(int n)
        {
            return;
        }

        public override void InitFunctions()
        {
            allCols = new int[N];

                for(int r=0;r<instances.Count;r++)
                {
                    allCols[r] = instances[r].Feature[0];
                }
        
            return;
        }
        InstanceList instances = new InstanceList();
        int[] allCols;
        int N;
        int fN;
        public override void InitTrain()
        {
            instances.AddRange(TrainData);
            N = instances.Count;
            if(instances.Count>0)
                fN = instances[0].Feature.Count;
            InitFunctions();
        }

        public override void Train(int IteratCnt)
        {
            if (PredictResult == null)
                PredictResult = new MachineLearnClass<int, int>.PredictResultClass();
            if (FeatureSummary == null)
                FeatureSummary = new MLFeatureFunctionsSummary<int, int>();
            int len = N;

            runningSubClass rsc = new runningSubClass();
            rsc.mk = this;
            rsc.reviewCnt = this.LearnDeep;
            rsc.len = len;
            rsc.TopN = TopN;
            rsc.selectCnt = this.SelectCnt;
            rsc.Data = allCols;
            //new Task(rsc.run).Start();
            rsc.run();


            this.OnTrainFinished();
        }

        public override List<KeyValuePair<int,double>> getPredictResult(int key)
        {
            List<KeyValuePair<int, double>> ret = new List<KeyValuePair<int, double>>();
            DiscreteMarkov dm = new DiscreteMarkov(allCols.ToList(), this.SelectCnt, 5,TopN);
            if (dm.predictResult.Count >= 0)
            {
                for (int i = 0; i < dm.predictResult.Count; i++)
                {
                    KeyValuePair<int, double> kv = new KeyValuePair<int, double>(dm.predictResult[i].Value, dm.PredictValue[i]);
                    ret.Add(kv);
                }
            }
            return ret;
        }


        class runningSubClass
        {
            public int reviewCnt =100;
            public int len = 0;
            public int selectCnt = 10;
            public int TopN;
            public int[] Data;
            public MarkovClass mk;
            public void run()
            {
                for (int j = 0; j < len - 1 - reviewCnt-1; j++)
                {
                    int[] useList = new int[reviewCnt];
                    Array.Copy(Data, j, useList, 0, reviewCnt);
                    DiscreteMarkov dm = new DiscreteMarkov(useList.ToList(),selectCnt, 5,TopN);
                    List<int> res = dm.predictResult.Select(a=>a.Value).ToList();
                    mk.PredictResult.TrainCnt++;

                    if (res.Count <= 0)
                    {
                        notice();
                        continue;
                    }
                    mk.PredictResult.PreDictCnt++;//预测数量加1                    
                    int realRes = Data[j + reviewCnt ];//实际值，对比
                    string[] strRev = res.Select(a => a.ToString()).ToArray();// CombinClass.getReconvertString(CombinClass.CreateNumArr(selectCnt), res.Select(a=>a.ToString()).ToArray());
                    mk.PredictResult.ChipCnt += strRev.Length;
                    if (!strRev.Contains(realRes.ToString()))
                    {
                        notice();
                        continue;
                    }
                    mk.PredictResult.CorrectCnt++;
                    mk.PredictResult.MatchCnt++;
                    notice();
                    //mk.OnPeriodEvent(mk._GrpId, 0, mk.PredictResult.TrainCnt,new string[] { "胜率","训练数","判断数","成功数" }, new double[] { (double)(100.00*mk.PredictResult.MatchCnt/mk.PredictResult.PreDictCnt), mk.PredictResult.TrainCnt,mk.PredictResult.PreDictCnt, mk.PredictResult.CorrectCnt});
                }
            }

            void notice()
            {
                double succRate = 0;
                if (mk.PredictResult.PreDictCnt > 0)
                {
                    succRate = (double)(100.00 * mk.PredictResult.MatchCnt / mk.PredictResult.ChipCnt);
                }
                mk.OnPeriodEvent?.Invoke(mk._GrpId, 0, mk.PredictResult.TrainCnt, new string[] { "胜率", "训练数", "判断数", "成功数" }, new double[] { succRate, mk.PredictResult.TrainCnt, mk.PredictResult.PreDictCnt, mk.PredictResult.CorrectCnt });

            }
        }
    }

    /// <summary>离散型马尔可夫链预测模型</summary>
    public class DiscreteMarkov
    {
        #region 属性
        /// <summary>样本点状态时间序列,按照时间升序</summary>
        public List<int> StateList { get; set; }
        /// <summary>状态总数,对应模型的m</summary>
        public int Count { get; set; }
        /// <summary>概率转移矩阵Pij</summary>
        public List<DenseMatrix> ProbMatrix { get; set; }
        /// <summary>
        /// 实际N步转移概率矩阵
        /// </summary>
        public List<DenseMatrix> ProbStepsStatus { get; set; }
        /// <summary>各阶的自相关系数</summary>
        public double[] Rk { get; set; }
        public double[] Rk_miss { get; set; }
        /// <summary>各阶的权重/summary>
        public double[] Wk { get; set; }
        public double[] wk_miss { get; set; }
        /// <summary>频数矩阵/summary>
        public int[][] CountStatic { get; set; }
        /// <summary>目标序列是否满足"马氏性"/summary>
        public Boolean IsMarkov { get; set; }
        /// <summary>滞时期，K/summary>
        public int LagPeriod { get; set; }
        static int NeedShift = 0;

        /// <summary>预测概率</summary>
        public Dictionary<int,double> PredictValue { get; set; }
        public Dictionary<int,double> PredictValue_miss { get; set; }
        #endregion

        #region 构造函数
        public DiscreteMarkov(List<int> data, int count, int K = 5,int TopN =1)
        {
            if(count>10)
            {
                NeedShift = 1;
            }
            this.StateList = data;
            this.LagPeriod = K;
            this.Count = count;
            this.CountStatic = StaticCount(data, count);//一步状态转移数
            this.ProbMatrix = new List<DenseMatrix>();
            this.ProbStepsStatus = new List<DenseMatrix>();
            var t0 = DenseMatrix.OfArray(StaticProbability(this.CountStatic).ConvertToArray<double>());//一步转移
            ProbMatrix.Add(t0);//概率矩阵
            ProbStepsStatus.Add(t0);//实际概率矩阵
            for (int i = 1; i < K; i++) //根据CK方程，计算各步的状态转移矩阵
            {
                this.ProbStepsStatus.Add(DenseMatrix.OfArray(StaticProbability(StaticCount(data, count, i + 1)).ConvertToArray<double>()));//计算实际转移步数后概率
                var temp = ProbMatrix[i - 1] * t0;//ck方程计算后概率
                ProbMatrix.Add(temp);//n步转移后的矩阵,理论的
            }
            if (ValidateMarkov())
            {
                CorrCoefficient_miss();
                CorrCoefficient();
                TimeWeight();
                TimeWeight_miss();
                PredictProb(TopN);
            }
            else
            {
                Console.WriteLine("马氏性 检验失败,无法进行下一步预测");
            }
        }
        #endregion

        

        #region 验证
        /// <summary>验证是否满足马氏性,默认的显著性水平是0.05，自由度25</summary>
        /// <returns></returns>
        public Boolean ValidateMarkov()
        {
            //计算列和
            int[] cp1 = new int[Count];
            int allcount = CountStatic.Select(n => n.Sum()).Sum();//总数

            for (int i = 0; i < Count; i++)
            {
                for (int j = 0; j < Count; j++) cp1[i] += CountStatic[j][i];
            }
            double[] cp = cp1.Select(n => (double)n / (double)allcount).ToArray();

            //计算伽马平方统计量
            double gm = 0;
            for (int i = 0; i < Count; i++)
            {
                for (int j = 0; j < Count; j++)
                {
                    if (CountStatic[i][j] != 0)
                        gm += 2 * CountStatic[i][j] * Math.Abs(Math.Log(ProbMatrix[0][i, j] / cp[j], Math.E));
                }
            }
            //查表求a = 0.05时，伽马分布的临界值F(m-1)^2,如果实际的gm值大于差别求得的值，则满足
            //查表要自己做表，这里只演示0.05的情况  卡方分布
            //自由度设为100，对应0.05为124.34
            //return gm >= 37.65;
            return gm > 124.34;
        }

        /// <summary>计算相关系数</summary>
        public void CorrCoefficient_miss()
        {
            double mean = (double)StateList.Sum() / (double)StateList.Count;//均值
            double p = StateList.Select(n => (n - mean) * (n - mean)).Sum();//所有方差和

            Rk_miss = new double[LagPeriod];//步长权重数组

            for (int i = 0; i < LagPeriod; i++)
            {
                double s1 = 0;
                for (int L = 0; L < StateList.Count - LagPeriod; L++)
                {
                    s1 += (StateList[L] - mean) * (StateList[L + i] - mean);//差和步差之积
                }
                Rk_miss[i] = s1 / p;
            }
        }

        public void CorrCoefficient()
        {
            Rk = new double[LagPeriod];//步长权重数组;
            Rk[0]=1.0;
            //比较实际概率矩阵和ck方程计算结果后概率矩阵的相关系数
            for (int i=1;i<LagPeriod;i++)
            {
                double[] m1 = ProbMatrix[i].Values;
                double[] m2 = ProbStepsStatus[i].Values;
                Rk[i] = Correlation.Pearson(m1,m2);
            }
        }

        /// <summary>计算滞时的步长</summary>
        public void TimeWeight()
        {
            double sum = Rk.Select(n => Math.Abs(n)).Sum();
            Wk = Rk.Select(n => Math.Abs(n) / sum).ToArray();
        }

        public void TimeWeight_miss()
        {
            double sum_miss = Rk_miss.Select(n => Math.Abs(n)).Sum();
            wk_miss = Rk_miss.Select(n => Math.Abs(n) / sum_miss).ToArray();
        }
        public List<int?> predictResult;
        public List<double> predValue;
        /// <summary>预测状态概率</summary>
        public void PredictProb(int firstN)
        {
            PredictValue = new Dictionary<int, double>();
            PredictValue_miss = new Dictionary<int, double>();
            //这里很关键，权重和滞时的关系要颠倒，循环计算的时候要注意
            //另外，要根据最近几期的出现数，确定概率的状态，必须取出最后几期的数据

            //1.先取最后K期数据
            var last = StateList.GetRange(StateList.Count - LagPeriod, LagPeriod);
            //2.注意last数据是升序,最后一位对于的滞时期 是k =1
            for (int i = 0; i < Count; i++)
            {
                PredictValue.Add(i, 0);
                PredictValue_miss.Add(i, 0);
                for (int j = 0; j < LagPeriod; j++)
                {
                    //滞时期j的数据状态
                   var state = last[last.Count - 1 - j]-NeedShift ; 
                    PredictValue[i] += Wk[j] * ProbMatrix[j][state, i];
                    PredictValue_miss[i] += wk_miss[j] * ProbMatrix[j][state, i];
                }
            }
            var res = PredictValue.OrderByDescending(a => a.Value).Take(firstN);
            var res_miss = PredictValue_miss.OrderByDescending(a => a.Value).Take(firstN);
            if (predictResult == null)
            {
                predictResult = new List<int?>();
                predValue = new List<double>();
            }
            foreach (var key in res)
            {
                if (res_miss.Select(b => b.Key).Contains(key.Key))
                {
                    
                    predictResult.Add(key.Key+NeedShift);
                    predValue.Add(key.Value);
                }
            }
        }
        #endregion

        #region 静态 辅助方法
        /// <summary>统计频数矩阵</summary>
        /// <param name="data">升序数据</param>
        public static int[][] StaticCount(List<int> data, int statusCount,int stepCnt=1)
        {
            int[][] res = new int[statusCount][];

            for (int i = 0; i < statusCount; i++) res[i] = new int[statusCount];

            //for (int i = 0; i < data.Count - 1; i++) res[data[i] - 1][data[i + 1] - 1]++;　１０，１，２，３．．．
            for (int i = 0; i < data.Count - stepCnt; i++) res[data[i]- NeedShift][data[i + stepCnt]- NeedShift]++;//支持多步,我们的标签从0开始，所以不要减1
            return res;
        }
        /// <summary>根据频数，计算转移概率矩阵</summary>
        /// <param name="data">频率矩阵</param>
        public static double[][] StaticProbability(int[][] data)
        {
            double[][] res = new double[data.Length][];
            for (int i = 0; i < data.Length; i++)
            {
                int sum = data[i].Sum();
                res[i] = data[i].Select(n => (double)n / (double)sum).ToArray();
            }
            return res;
        }
        #endregion
    }

     public static class Extends
    {
        public static T[,] ConvertToArray<T>(this T[][] data)
        {
            T[,] res = new T[data.Length, data[0].Length];
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[i].Length; j++) res[i, j] = data[i][j];
            }
            return res;
        }
    }
}
