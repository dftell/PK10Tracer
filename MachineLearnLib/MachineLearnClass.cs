using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MachineLearnLib
{
    public abstract class MachineLearnClass<LabelT, FeatureT> :IMachineLearn<LabelT, FeatureT>
    {
        public abstract void Train();

        public double CheckInstances(List<MLInstance<LabelT, FeatureT>> TestList,long TrainCnt)
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
            return (double)1.0 * pass / TrainCnt;
        }

        public abstract LabelT Classify(MLInstance<LabelT, FeatureT> instances);

        public abstract double[] GetKeyResult();
    }

    public interface IMachineLearn<LabelT,FeatureT>
    {
        void Train();
        double[] GetKeyResult();

        double CheckInstances(List<MLInstance<LabelT, FeatureT>> intances, long TrainCnt);

        LabelT Classify(MLInstance<LabelT, FeatureT> instances);
    }

    public class MLInstance<LabelT,FeatureT>
    {
        public LabelT Label;
        public Type FeatureType;
        public MLFeature<FeatureT> Feature;
        
    }

    public class MLFeature<T> : IList<T>
    {
        public T this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class MLDoubleFeature : MLFeature<double>
    {
    }

    public class MLIntFeature : MLFeature<int>
    {
    }
}

