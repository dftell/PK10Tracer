using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.MachineLearnLib
{
    public delegate void EventTrainFinished();
    public delegate void PeriodEvent(params object[] objects);
    public delegate void SaveFileEvent(string text);
    public delegate string LoadFileEvent();

    public interface IMachineLearn<LabelT, FeatureT>
    {
        long TrainCount { get; }
        void Train();
        void Train(int IteratCnt);

        double CheckInstances(List<MLInstance<LabelT, FeatureT>> intances);

        LabelT Classify(MLInstance<LabelT, FeatureT> instances);

        bool FillTrainData(List<List<FeatureT>> FeatureData, List<LabelT> LabelData);

        bool FillTrainData(MLInstances<LabelT, FeatureT> instances);

        void InitFunctions();

        void InitTrain();
        void SaveSummary();
        void LoadSummary();
        void FillStructBySummary(int n);
    }

    public abstract class MachineLearnClass<LabelT, FeatureT> : IMachineLearn<LabelT, FeatureT>
    {
        protected int _GrpId = -1;
        public int GroupId
        {
            set { _GrpId = value; }
        }
        public int LearnDeep;
        public int TrainIterorCnt ;
        public  PredictResultClass PredictResult;
        public static MLFeatureFunctionsSummary<LabelT, FeatureT> FeatureSummary;//= new MLFeatureFunctionsSummary<LabelT, FeatureT>();
        protected MLInstances<LabelT, FeatureT> TrainData;
        protected Dictionary<string, MLFeatureFunctionsClass<LabelT, FeatureT>> functions = new Dictionary<string, MLFeatureFunctionsClass<LabelT, FeatureT>>();
        public void Train()
        {
            Train(TrainIterorCnt);
        }

        public abstract void InitFunctions();
        public abstract void Train(int IteratCnt);
        public EventTrainFinished OnTrainFinished;
        public PeriodEvent OnPeriodEvent;
        public SaveFileEvent OnSaveEvent;
        public LoadFileEvent OnLoadLocalFile;
        protected long _TrainCnt;
        public int ThreadCnt=10;
        public long TrainCount
        {
            get
            {
                if (TrainData!= null)
                    _TrainCnt = TrainData.Count;
                return _TrainCnt;
            }
            set
            {
                _TrainCnt = value;
            }
        }
        List<MLInstance<LabelT, FeatureT>> _TestList;

        public void SetTestInstances(List<MLInstance<LabelT, FeatureT>> TestList)
        {
            _TestList  = TestList;

        }

        public double CheckInstances()
        {
            return CheckInstances(_TestList);
        }

        public double CheckInstances(List<MLInstance<LabelT, FeatureT>> TestList)
        {
            //该函数默认已经训练完毕
            List<MLInstance<LabelT, FeatureT>> trainInstances = TestList;// DataSet.readDataSet("examples/zoo.test");
            int pass = 0;
            foreach (MLInstance<LabelT, FeatureT> instance in trainInstances)
            {
                LabelT predict = Classify(instance);
                if (predict.Equals(instance.Label))
                {
                    pass += 1;
                }
            }
            return (double)1.0 * pass / TestList.Count;
        }

        ////public double CheckInstances(MLInstances<LabelT,FeatureT> TestList)
        ////{
        ////    List<MLInstance<LabelT, FeatureT>> list = new List<MLInstance<LabelT, FeatureT>>();
        ////    list.AddRange(TestList);
        ////    return CheckInstances(list);
        ////}

        public abstract LabelT Classify(MLInstance<LabelT, FeatureT> instances);

        public bool FillTrainData(List<List<FeatureT>> FeatureData, List<LabelT> LabelData)
        {
            if (TrainData == null)
                TrainData = new MLInstances<LabelT, FeatureT>();
            try
            {
                if (FeatureData.Count != LabelData.Count)
                    return false;
                if (FeatureData.Count == 0 || LabelData.Count == 0)
                    return false;
                MLInstances<LabelT, FeatureT> _TrainData = new MLInstances<LabelT, FeatureT>();
                for(int i=0;i<FeatureData.Count;i++)
                {
                    _TrainData.Add(new MLInstance<LabelT, FeatureT>(LabelData[i], FeatureData[i]));
                }
                FillTrainData(_TrainData); 
            }
            catch (Exception ce)
            {
                return false;
            }
            return true;
        }

        public bool FillTrainData(MLInstances<LabelT, FeatureT> _TrainData)
        {
            TrainData = _TrainData;
            return true;
        }


        public abstract void InitTrain();
        
        public void SaveSummary()
        {
            List<MLFeatureFunctionsSummary<LabelT, FeatureT>> ret = new List<MLFeatureFunctionsSummary<LabelT, FeatureT>>();
            ret.Add(FeatureSummary);
            string strText = DetailStringClass.getXmlByObjectList<MLFeatureFunctionsSummary<LabelT, FeatureT>>(ret);
            OnSaveEvent(strText);

        }

        /// <summary>
        /// 直接预测结果类
        /// </summary>
        public class PredictResultClass
        {
            /// <summary>
            /// 训练个数
            /// </summary>
            public int TrainCnt;
            /// <summary>
            /// 做出过预测的个数
            /// </summary>
            public int PreDictCnt;
            /// <summary>
            /// 成功次数
            /// </summary>
            public int CorrectCnt;
            /// <summary>
            /// 成功匹配个数
            /// </summary>
            public long MatchCnt;
        }

        public void LoadSummary()
        {
            string txt = OnLoadLocalFile();
            List<MLFeatureFunctionsSummary<LabelT, FeatureT>> ret  = DetailStringClass.getObjectListByXml<MLFeatureFunctionsSummary<LabelT, FeatureT>>(txt);
            if(ret != null && ret.Count>0)
            {
                FeatureSummary = ret[0];
            }
            
        }

        public abstract void FillStructBySummary(int n);
    }

    
    public class MLInstance<LabelT,FeatureT>
    {
        public LabelT Label;
        public Type FeatureType;
        public MLFeature<FeatureT> Feature;
        public MLInstance(List<FeatureT> flist)
        {
            Feature = new MLFeature<FeatureT>(flist);
        }

        public MLInstance(LabelT label, List<FeatureT> flist)
        {
            Feature = new MLFeature<FeatureT>();
            Label = label;
        }

        public MLInstance()
        {

        }
        public MLInstance(MLFeature<FeatureT> ft)
        {
            Feature = ft;
        }
    }

    public class MLFeature<T> :List<T>
    {
        public MLFeature()
        {

        }

        public MLFeature(List<T> list)
        {
            this.Clear();
            this.AddRange(list);
        }


    }

    public class MLDoubleFeature : MLFeature<double>
    {
    }

    public class MLIntFeature : MLFeature<int>
    {
    }

    [Serializable]
    public class MLFeatureFunctionsSummary<LabelT,FeatureT>:DetailStringClass
    {
        public long TrainCnt;
        public int FeatureCnt;
        public int LabelCnt;
        public List<List<FeatureT>> FeatureList;
        public List<LabelT> LabelList;
        public List<double[]> Keys;
        public List<MLFeatureFunctionsClass<LabelT, FeatureT>> FuncList;
        //Dictionary<string, MLFeatureFunctionsClass<LabelT, FeatureT>> Functions;
        

        public MLFeatureFunctionsSummary()
        {
            FeatureList = new List<List<FeatureT>>();
            LabelList = new List<LabelT>();
            //Functions = new Dictionary<string,MLFeatureFunctionsClass<LabelT, FeatureT>>();
            FuncList = new List<MLFeatureFunctionsClass<LabelT, FeatureT>>();
            Keys = new List<double[]>();
        }
    }

    [Serializable]
    public class MLFeatureFunctionsClass<LabelT, FeatureT>:DetailStringClass
    {
        public int index;
        public FeatureT value;
        public LabelT label;
        public MLFeatureFunctionsClass()
        {

        }
        protected static Dictionary<string, MLFeatureFunctionsClass<LabelT,FeatureT>> AllFeatureFunctions = new  Dictionary<string, MLFeatureFunctionsClass<LabelT, FeatureT>>();

        public MLFeatureFunctionsClass(int index, FeatureT value, LabelT label)
        {
            this.index = index;
            this.value = value;
            this.label = label;
            string strKey = "{0}_{1}_{2}";
            string key = string.Format(strKey, index, value, label);
            if (!AllFeatureFunctions.ContainsKey(key))
                AllFeatureFunctions.Add(key,this);

        }
               
        /**
             * 代入函数
             * @param feature 特征X（维度由构造时的index指定）
             * @param label Y
             * @return
             */
        public int Apply(MLFeature<FeatureT> feature, LabelT label)
        {
            if (feature[index].Equals(value) && label.Equals(this.label))
                return 1;
            return 0;
        }
    }


    public class MLInstances<LabelT,FeatureT>:List<MLInstance<LabelT, FeatureT>>
    {

    }

    
}

