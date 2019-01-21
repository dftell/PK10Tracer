using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MachineLearnLib
{
    public delegate void EventTrainFinished();
    public delegate void PeriodEvent(params object[] objects);
    public abstract class MachineLearnClass<LabelT, FeatureT> : IMachineLearn<LabelT, FeatureT>
    {
        protected MLInstances<LabelT, FeatureT> TrainData;
        public abstract void Train();
        public EventTrainFinished OnTrainFinished;
        public PeriodEvent OnPeriodEvent;
        public long TrainCount { get { return TrainData.Count; } }

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
            return (double)1.0 * pass / TrainCount;
        }

        public abstract LabelT Classify(MLInstance<LabelT, FeatureT> instances);

        public abstract double[] GetKeyResult();


        public bool FillTrainData(List<List<FeatureT>> FeatureData, List<LabelT> LabelData)
        {
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
    }

    public interface IMachineLearn<LabelT,FeatureT>
    {
        long TrainCount { get; }
        void Train();
        double[] GetKeyResult();

        double CheckInstances(List<MLInstance<LabelT, FeatureT>> intances);

        LabelT Classify(MLInstance<LabelT, FeatureT> instances);

        bool FillTrainData(List<List<FeatureT>> FeatureData, List<LabelT> LabelData);

        bool FillTrainData(MLInstances<LabelT,FeatureT> instances);

        void InitTrain();
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
    
    public class MLInstances<LabelT,FeatureT>:List<MLInstance<LabelT, FeatureT>>
    {

    }

    
}

