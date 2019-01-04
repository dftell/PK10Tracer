using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MachineLearnLib
{
    /// <summary>
    /// 最大熵模型类
    /// </summary>
    public class MaxEntModelClass
    {
    }

    /**
 * 最大熵的IIS（Improved Iterative Scaling）训练算法
 * User: tpeng  <pengtaoo@gmail.com>
 */
    public class MaxEnt
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

        /**
         * 实例列表
         */
        private List<Instance> instances = new List<Instance>();

        /**
         * 特征函数列表
         */
        private List<FeatureFunction> functions = new List<FeatureFunction>();

        /**
         * 特征列表
         */
        private List<Feature> features = new List<Feature>();

        public double Run(List<Instance> TrainList, List<Instance> TestList)
        {
            List<Instance> instances = TrainList;// DataSet.readDataSet("examples/zoo.train");
            MaxEnt me = new MaxEnt(instances);
            me.train();
            List<Instance> trainInstances = TestList;// DataSet.readDataSet("examples/zoo.test");
            int pass = 0;
            foreach (Instance instance in trainInstances)
            {
                int predict = me.classify(instance);
                if (predict == instance.Label)
                {
                    pass += 1;
                }
            }
            return (double)1.0 * pass / trainInstances.Count;
            //System.out.println("accuracy: " + 1.0 * pass / trainInstances.size());
        }

        public MaxEnt(List<Instance> trainInstance)
        {

            instances.AddRange(trainInstance);
            N = instances.Count;
            createFeatFunctions(instances);
            w = new double[functions.Count];
            empirical_expects = new double[functions.Count];
            calc_empirical_expects();
        }

        /**
         * 创建特征函数
         * @param instances 实例
         */
        private void createFeatFunctions(List<Instance> instances)
        {

            int maxLabel = 0;
            int minLabel = int.MaxValue;
            int[] maxFeatures = new int[instances[0].Feature.Count];
            List<Feature> featureSet = new List<Feature>();

            foreach (Instance instance in instances)
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

            features = new List<Feature>();
            //featureSet
            maxY = maxLabel;
            minY = minLabel;

            for (int i = 0; i < maxFeatures.Length; i++)
            {
                for (int x = 0; x <= maxFeatures[i]; x++)
                {
                    for (int y = minY; y <= maxLabel; y++)
                    {
                        functions.Add(new FeatureFunction(i, x, y));
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
                    for (int j = 0; j < functions.Count; j++)
                    {
                        z += w[j] * functions[j].apply(features[i], y);
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
        public void train()
        {
            for (int k = 0; k < ITERATIONS; k++)
            {
                for (int i = 0; i < functions.Count; i++)
                {
                    double delta = iis_solve_delta(empirical_expects[i], i);
                    w[i] += delta;
                }
                //if (DEBUG)  System.out.println("ITERATIONS: " + k + " " + Arrays.toString(w));
            }
        }

        /**
         * 分类
         * @param instance
         * @return
         */
        public int classify(Instance instance)
        {

            double max = 0;
            int label = 0;

            for (int y = minY; y <= maxY; y++)
            {
                double sum = 0;
                for (int i = 0; i < functions.Count; i++)
                {
                    sum += Math.Exp(w[i] * functions[i].apply(instance.Feature, y));
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

            foreach (Instance instance in instances)
            {
                int y = instance.Label;
                Feature feature = instance.Feature;
                for (int i = 0; i < functions.Count; i++)
                {
                    empirical_expects[i] += functions[i].apply(feature, y);
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
            for (int i = 0; i < functions.Count; i++)
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
        private double iis_solve_delta(double empirical_e, int fi)
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
                    Instance instance = instances[i];
                    Feature feature = instance.Feature;
                    int index = features.IndexOf(feature);
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
            public int apply(Feature feature, int label)
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
    public class Feature : List<int>
    {

        /**
         * 特征的具体值
         */
        public Feature(List<int> list)
        {
            this.Clear();
            list.ForEach(p => this.Add(p));
        }

        public new String ToString()
        {
            return string.Join(";", this.ToArray());
        }
    }


    /// <summary>
    /// 实例
    /// </summary>
    public class Instance
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


}
