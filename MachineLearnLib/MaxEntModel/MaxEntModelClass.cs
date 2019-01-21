using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MachineLearnLib
{
    /// <summary>
    /// 最大熵模型类
    /// </summary>


    /**
 * 最大熵的IIS（Improved Iterative Scaling）训练算法
 * User: tpeng  <pengtaoo@gmail.com>
 */
    public class MaxEnt : MachineLearnClass<int, int>
    {

        private static bool DEBUG = false;

        /**
         * 迭代次数
         */
        private int ITERATIONS = 200;

        /**
         * 浮点数精度
         */
        private static double EPSILON = 0.001;

        // the number of training instances
        /**
         * 训练实例数
         */
        private int N;

        // the minimal of Y
        /**
         * Y的最小值
         */
        private int minY;

        // the maximum of Y
        /**
         * Y的最大值
         */
        private int maxY;

        // the empirical expectation value of f(x, y)
        /**
         * 联合分布的期望
         */
        private double[] empirical_expects;

        // the weight to learn.
        /**
         * 模型参数
         */
        private double[] w;
        string[] wNames;
        /**
         * 实例列表
         */
        private InstanceList instances = new InstanceList();
        int FeatureCnt = 0;
        /**
         * 特征函数列表
         */
        private Dictionary<string,FeatureFunction> functions = new Dictionary<string,FeatureFunction>();

        /**
         * 特征列表
         */
        private List<MLFeature<int>> features = new List<MLFeature<int>>();

        public MaxEnt(List<Instance> trainInstance)
        {

            instances.AddRange(trainInstance);
            N = instances.Count;
            createFeatFunctions(instances);
            w = new double[functions.Count];
            wNames = new string[functions.Count];
            empirical_expects = new double[functions.Count];
            calc_empirical_expects();
        }

        public override void InitTrain()
        {

            instances.AddRange(TrainData);
            N = instances.Count;
            createFeatFunctions(instances);
            w = new double[functions.Count];
            wNames = new string[functions.Count];
            empirical_expects = new double[functions.Count];
            calc_empirical_expects();
        }

        public MaxEnt()
        {
        }

        public static double Run(List<Instance> TrainList, List<Instance> TestList)
        {
            List<Instance> instances = TrainList;// DataSet.readDataSet("examples/zoo.train");
            MaxEnt me = new MaxEnt(instances);
            me.Train();
            List<Instance> trainInstances = TestList;// DataSet.readDataSet("examples/zoo.test");
            int pass = 0;
            foreach (Instance instance in trainInstances)
            {
                int predict = me.Classify(instance);
                if (predict == instance.Label)
                {
                    pass += 1;
                }
            }
            return (double)1.0 * pass / trainInstances.Count;
            //System.out.println("accuracy: " + 1.0 * pass / trainInstances.size());
        }


        public static Dictionary<int, double> getLabels(List<Instance> TrainList, List<Instance> TestList)
        {
            Dictionary<int, double> ret = new Dictionary<int, double>();
            List<int> testLabels = new List<int>();
            for (int i = 0; i < 10; i++)
                testLabels.Add((i + 1) % 10);
            List<Instance> instances = TrainList;// DataSet.readDataSet("examples/zoo.train");
            MaxEnt me = new MaxEnt(instances);
            me.Train();
            for (int i = 0; i < testLabels.Count; i++)
            {
                TestList[TestList.Count - 1].Label = testLabels[i];//最后一条记录的label更换为测试的label
                List<Instance> trainInstances = TestList;// DataSet.readDataSet("examples/zoo.test");
                int pass = 0;
                foreach (Instance instance in trainInstances)
                {
                    int predict = me.Classify(instance);
                    if (predict == instance.Label)
                    {
                        pass += 1;
                    }
                }
                ret.Add(testLabels[i], (double)1.0 * pass / trainInstances.Count);
            }
            return ret;
            //System.out.println("accuracy: " + 1.0 * pass / trainInstances.size());
        }



        /**
         * 创建特征函数
         * @param instances 实例
         */
        private void createFeatFunctions(InstanceList instances)
        {

            int maxLabel = 0;
            int minLabel = int.MaxValue;
            int[] maxFeatures = new int[instances[0].Feature.Count];

            List<MLFeature<int>> featureSet = new List<MLFeature<int>>();

            foreach (MLInstance<int,int> instance in instances)
            {

                if (instance.Label > maxLabel)
                {
                    maxLabel = instance.Label;
                }
                if (instance.Label < minLabel)
                {
                    minLabel = instance.Label;
                }


                for (int i = 0; i < instance.Feature.Count; i++)
                {
                    if (instance.Feature[i] > maxFeatures[i])
                    {
                        maxFeatures[i] = instance.Feature[i];
                    }
                }

                featureSet.Add(instance.Feature);
            }

            features = new List<MLFeature<int>>();
            List<List<int>> flist = Feature.getNextFeature("1234567890", maxFeatures.Length);
            flist.ForEach(p => features.Add(new Feature(p)));
            //featureSet
            maxY = maxLabel;
            minY = minLabel;
            for (int i = 0; i < maxFeatures.Length; i++)
            {
                for (int x = 0; x <= maxFeatures[i]; x++)
                {
                    for (int y = minY; y <= maxLabel; y++)
                    {
                        functions.Add(string.Format("{0}_{1}_{2}",i,x,y),new FeatureFunction(i, x, y));
                    }
                }
            }

            if (DEBUG)
            {
                //System.out.println("# features = " + features.size());
                //System.out.println("# functions = " + functions.size());
            }
        }

        // calculates the p(y|x)
        /**
         * 计算条件概率 p(y|x)
         * @return p(y|x)
         */
        private double[,] calc_prob_y_given_x()
        {

            double[,] cond_prob = new double[features.Count, maxY + 1];

            for (int y = minY; y <= maxY; y++)
            {
                for (int i = 0; i < features.Count; i++)
                {
                    double z = 0;
                    //for (int j = 0; j < functions.Values.Count ; j++)
                    int j = 0;
                    foreach(string key in functions.Keys)
                    {
                            //z += w[j] * functions[j].apply(features[i], y);
                            z += w[j] * functions[key].apply(features[i], y);
                        j++;
                    }
                    cond_prob[i, y] = Math.Exp(z);
                }
            }

            for (int i = 0; i < features.Count; i++)
            {
                double normalize = 0;
                for (int y = minY; y <= maxY; y++)
                {
                    normalize += cond_prob[i, y];
                }
                for (int y = minY; y <= maxY; y++)
                {
                    cond_prob[i, y] /= normalize;
                }
            }

            return cond_prob;
        }

        /**
         * 训练
         */
        public override void Train()
        {
            for (int k = 0; k < ITERATIONS; k++)
            {
                int i = 0;
                //for (int i = 0; i < functions.Count; i++)
                foreach(string key in functions.Keys)
                {
                    double delta = iis_solve_delta(empirical_expects[i], key);
                    if(k==0)
                    {
                        wNames[i] =  key;
                    }
                    w[i] += delta;
                    OnPeriodEvent(k, i, wNames,w);
                    i++;
                }
                //if (DEBUG)  System.out.println("ITERATIONS: " + k + " " + Arrays.toString(w));
            }
            this.OnTrainFinished();
        }

        /**
         * 分类
         * @param instance
         * @return
         */
        public override int Classify(MLInstance<int, int> instance)
        {

            double max = 0;
            int label = 0;

            for (int y = minY; y <= maxY; y++)
            {
                double sum = 0;
                //for (int i = 0; i < functions.Count; i++)
                int i = 0;
                foreach(string key in functions.Keys)
                {
                    sum += Math.Exp(w[i] * functions[key].apply((Feature)instance.Feature, y));
                    i++;
                }
                if (sum > max)
                {
                    max = sum;
                    label = y;
                }
            }
            return label;
        }




        /// <summary>
        /// 计算经验期望
        /// </summary>
        private void calc_empirical_expects()
        {

            foreach (MLInstance<int,int> instance in instances)
            {
                int y = instance.Label;
                MLFeature<int> feature = instance.Feature;
                //for (int i = 0; i < functions.Count; i++)
                int i = 0;
                foreach(string key in functions.Keys)
                {
                    empirical_expects[i] += functions[key].apply(feature, y);
                    i++;
                }
            }
            for (int i = 0; i < functions.Count; i++)
            {
                empirical_expects[i] /= 1.0 * N;
            }
            //if (DEBUG)  System.out.println(Arrays.toString(empirical_expects));
        }

        /**
         * 命中的所有特征函数输出之和
         * @param feature
         * @param y
         * @return
         */
        private int apply_f_sharp(Feature feature, int y)
        {

            int sum = 0;
            //for (int i = 0; i < functions.Count; i++)
            foreach(string i in functions.Keys)
            {
                FeatureFunction function = functions[i];
                sum += function.apply(feature, y);
            }
            return sum;
        }


        ////
        /// <summary>
        /// 求delta_i
        /// </summary>
        /// <param name="empirical_e">@param empirical_e fi的期望</param>
        /// <param name="fi">@param fi fi的下标</param>
        /// <returns></returns>
        //private double iis_solve_delta(double empirical_e, int fi)
        private double iis_solve_delta(double empirical_e, string fi)
        {

            double delta = 0;
            double f_newton, df_newton;
            double[,] p_yx = calc_prob_y_given_x();

            int iters = 0;

            while (iters < 50)                                  // 牛顿法
            {
                f_newton = df_newton = 0;
                for (int i = 0; i < instances.Count; i++)
                {
                    MLInstance<int,int> instance = instances[i];
                    MLFeature<int> tfeature = instance.Feature;
                    Feature feature = new Feature(tfeature);
                    int index = features.IndexOf(feature);
                    if (index == -1)
                    {
                        index = feature.getIndex(instance.Feature.Count);
                        if (string.Join("", features[index]) == string.Join("",feature))
                        {
                            features[index] = feature;
                        }
                    }
                    for (int y = minY; y <= maxY; y++)
                    {
                        int f_sharp = apply_f_sharp(feature, y);
                        double prod = p_yx[index, y] * functions[fi].apply(feature, y) * Math.Exp(delta * f_sharp);
                        f_newton += prod;
                        df_newton += prod * f_sharp;
                    }
                }
                f_newton = empirical_e - f_newton / N;      // g
                df_newton = -df_newton / N;                 // g的导数

                if (Math.Abs(f_newton) < 0.0000001)
                    return delta;

                double ratio = f_newton / df_newton;

                delta -= ratio;
                if (Math.Abs(ratio) < EPSILON)
                {
                    return delta;
                }
                iters++;
            }
            return double.NaN; //如果不收敛，返回NaN
            throw new Exception("IIS did not converge"); // w_i不收敛
        }

        public override double[] GetKeyResult()
        {
            return w;
        }




        /**
         * 特征函数
         * 应该是按照具体的逻辑定义的类
         */
        class FeatureFunction
        {

            private int index;
            private int value;
            private int label;

            public FeatureFunction(int index, int value, int label)
            {
                this.index = index;
                this.value = value;
                this.label = label;
            }

            /**
             * 代入函数
             * @param feature 特征X（维度由构造时的index指定）
             * @param label Y
             * @return
             */
            public int apply(MLFeature<int> feature, int label)
            {
                if (feature[index] == value && label == this.label)
                    return 1;
                return 0;
            }
        }

    }

    /// <summary>
    /// 特征
    /// </summary>
    public class Feature : MLIntFeature
    {

        /**
         * 特征的具体值
         */
        public Feature(List<int> list)
        {
            this.Clear();
            list.ForEach(p => this.Add(p));
        }

        public int getIndex(int vcnt)
        {
            List<int> list = new List<int>();
            list.AddRange(this);
            list.Reverse();
            string strInt = string.Join("", list);
            string strDiff = "";
            for (int i = 0; i < vcnt; i++)
            {
                strDiff = strDiff + "1";
            }
            int IntRes = int.Parse(strInt);
            int IntDiff = int.Parse(strDiff);
            if (IntRes < IntDiff)
            {
                IntRes = (int)Math.Pow(10, vcnt) + IntRes;
            }
            int ret = IntRes - IntDiff;

            return ret;
        }

        public new String ToString()
        {
            return string.Join(";", this.ToArray());
        }

        public static List<List<int>> getNextFeature(int Vcnt, int Index, int XMin, int XMax, List<int> orgList)
        {
            List<List<int>> retList = new List<List<int>>();
            List<int> ret = new List<int>();
            if (Index >= Vcnt)
            {
                return retList;
            }
            ret.AddRange(orgList);
            if (orgList.Count < Index + 1)
            {
                //orgList = new List<int>();
                //for (int i = Index; i < Index; i++)
                ret.Add(0);
            }
            if (XMin <= XMax)//获得本级本次及以下
            {
                if (XMin > 0)
                {
                    ret[Index] = XMin;

                    //retList.Add(ret);
                }
                if (ret.Count == Vcnt)
                    retList.Add(ret);
                List<List<int>> resNext = getNextFeature(Vcnt, Index + 1, XMin, XMax, ret);
                retList.AddRange(resNext);
                for (int i = XMin + 1; i <= XMax; i++)//获得本级后面所有次及以下
                {
                    List<List<int>> res = getNextFeature(Vcnt, Index, i, XMax, ret);
                    retList.AddRange(res);
                }
            }

            return retList;
        }

        static IEnumerable<string> foo(IEnumerable<string> src, string meta, int n)
        {
            if (src.First().Length == n)
                return src;
            else
                return foo(meta.SelectMany(x => src.Select(y => y + x)), meta, n);
        }

        public static List<List<int>> getNextFeature(string AllString, int vcnt)
        {
            List<string> ret = new List<string>();
            foreach (string s in foo(AllString.Select(x => x.ToString()), AllString, vcnt))
                ret.Add(s);
            List<List<int>> retInt = new List<List<int>>();
            for (int i = 0; i < ret.Count; i++)
            {
                char[] strArr = ret[i].ToCharArray();
                List<int> ilist = new List<int>();
                for (int j = 0; j < strArr.Length; j++)
                    ilist.Add(int.Parse(strArr[j].ToString()));
                retInt.Add(ilist);
            }
            return retInt;
        }
    }


    /// <summary>
    /// 实例
    /// </summary>
    public class Instance : MLInstance<int, int>
    {
        /**
         * 标签
         */
        int label;
        /**
         * 特征
         */
        Feature feature;

        public Instance(int label, int[] xs)
        {
            this.label = label;
            this.feature = new Feature(xs.ToList());
        }

        public int Label
        {
            get
            {
                return label;
            }
            set
            {
                label = value;
            }
        }

        public Feature Feature
        {
            get
            {
                return feature;
            }
        }

        public new String ToString()
        {
            return string.Format("Instance{label={0},feature={1}}", label, feature.ToString());
        }
    }

    public class InstanceList: MLInstances<int, int>
    {

    }
}