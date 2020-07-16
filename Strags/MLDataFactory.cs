using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.PK10CorePress;
using WolfInv.com.MachineLearnLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.Strags
{
    //机器数据分类工厂基类
    public abstract class MLDataCategoryFactoryClass
    {
        protected ExpectList Data;
        
        public abstract MLInstances<int, int> getCategoryData(int col, int Deep, int AllowUseShift);


        public void Init(ExpectList el)
        {
            Data = el;
        }
    }
    /// <summary>
    /// 环绕立体分类
    /// </summary>
    public class MLDataFactory: MLDataCategoryFactoryClass
    {
        
        public MLDataFactory()
        {
            
        }        


        public override MLInstances<int, int> getCategoryData(int col, int Deep, int AllowUseShift)
        {
            if (Data == null)
                return null;
            return getAllSpecColRoundLabelAndFeatures(col, Deep, AllowUseShift);
        }

        BayesDicClass OccurrDir(int col, int shiftCol, int TestLength, int LastTimes, bool LastisSerial)//add by zhouys 2019/1/15
        {
            BayesDicClass ret = new BayesDicClass();
            int iShift = Data.Count - TestLength;
            if (iShift <= LastTimes) //Data length must more than TestLength+LastTimes+1
                return ret;
            Dictionary<string, int> defaultDic = PKProbVector.getDefaultCombDic();
            Dictionary<int, int> PreA = PKProbVector.InitPriorProbabilityDic();
            Dictionary<int, int> PreB = PKProbVector.InitPriorProbabilityDic();
            //for (int col=0;col<10;col++)
            //{
            Dictionary<string, int> combDic = defaultDic;
            int BColIndex = (col + shiftCol) % 10;//对于大于10的取模
            if (BColIndex < 0)//对于小于0的，+10 如：0 + （-1） = 9
            {
                BColIndex = BColIndex + 10;
            }
            for (int i = iShift - 1; i < Data.Count; i++)
            {
                int CurrA = int.Parse(Data[i].ValueList[col]);
                int CurrB = int.Parse(Data[i - LastTimes].ValueList[BColIndex]);
                string key = string.Format("{0}_{1}", CurrA, CurrB);
                int cnt = combDic[key];
                combDic[key] = cnt + 1;
                PreA[CurrA] = PreA[CurrA] + 1;
                PreB[CurrB] = PreB[CurrB] + 1;
            }
            ret.PosteriorProbDic = combDic;
            ret.PriorProbDicA = PreA;
            ret.PriorProbDicB = PreB;
            ret.TestLength = TestLength;
            //}
            return ret;
        }

        public List<Instance> OccurrTestInstances(int col, int shiftCol, int TestLength, int LastTimes, bool LastisSerial)
        {
            List<Instance> ret = new List<Instance>();
            int iShift = Data.Count - TestLength;
            if (iShift <= LastTimes) //Data length must more than TestLength+LastTimes+1
                return ret;
            int BColIndex = (col + shiftCol) % 10;//对于大于10的取模
            if (BColIndex < 0)//对于小于0的，+10 如：0 + （-1） = 9
            {
                BColIndex = BColIndex + 10;
            }

            for (int i = iShift - 1; i < Data.Count; i++)
            {

                Dictionary<int, List<int>> combDic = new Dictionary<int, List<int>>();
                int CurrA = int.Parse(Data[i].ValueList[col]);
                List<int> list = getFeatures(col, shiftCol, i - 1, LastTimes, LastisSerial);
                combDic.Add(CurrA, list);
                Instance it = new Instance(CurrA, list.ToArray());
                ret.Add(it);
            }
            return ret;
        }

        List<int> getFeatures(int col, int shiftCol, int index, int LastTimes, bool LastisSerial)
        {
            List<int> list = new List<int>();
            int i = index;
            int BColIndex = (col + shiftCol) % 10;//对于大于10的取模
            if (BColIndex < 0)//对于小于0的，+10 如：0 + （-1） = 9
            {
                BColIndex = BColIndex + 10;
            }
            for (int bi = i - LastTimes + 1; bi <= i; bi++)//如果连续取前N期
            {
                int CurrB = int.Parse(Data[bi].ValueList[BColIndex]);
                list.Add(CurrB);
                if (!LastisSerial)
                    break;
            }
            return list;
        }

        public List<Instance> getLastFeatures(int col, int shiftCol, int index, int LastTimes, bool LastisSerial)
        {
            List<Instance> ret = new List<Instance>();
            List<int> testList = getFeatures(col, shiftCol, index, LastTimes, LastisSerial);
            //testList.Add(TestVal);
            ret.Add(new Instance(0, testList.ToArray()));
            return ret;
        }

        public Dictionary<int, double> getMaxProb(int col, int shiftCol, int TestLength, int LastTimes, bool LastisSerial)
        {
            Dictionary<int, double> ret = new Dictionary<int, double>();
            List<Instance> TrainSet = OccurrTestInstances(col, shiftCol, TestLength, LastTimes, LastisSerial);
            List<Instance> TestSet = getLastFeatures(col, shiftCol, Data.Count - 1, LastTimes, LastisSerial);
            ret = MaxEnt.getLabels(TrainSet, TestSet);


            return ret;
        }

        public Dictionary<int, Dictionary<int, int>> getAllShiftAndColMaxProbList(int TestLength, int LastTimes, bool LastisSerial)
        {
            Dictionary<int, Dictionary<int, int>> ret = new Dictionary<int, Dictionary<int, int>>();
            for (int sft = 0; sft < 10; sft++)
            {
                Dictionary<int, int> shiftRs = new Dictionary<int, int>();
                for (int col = 0; col < 10; col++)
                {
                    Dictionary<int, double> res = getMaxProb(col, sft, TestLength, LastTimes, LastisSerial);
                    int MaxKey = res.OrderByDescending(p => p.Value).First().Key;
                    shiftRs.Add((col + 1) % 10, MaxKey);
                    if (col > 1)
                        break;
                }
                ret.Add(sft, shiftRs);
                if (sft > 1)
                    break;
            }
            return ret;
        }


        public MLFeature<int> getSpecRowRoundFeatures(long rowid, int col, int Deep)
        {
            return getSpecRowRoundFeatures(rowid, col, Deep, 0);
        }
        /// <summary>
        /// 产生环绕Label的实例，深度为0是下一，基本配置，深度为1则是左1，下一，下二，右一，以此类推
        /// </summary>
        /// <returns></returns>
        public MLFeature<int> getSpecRowRoundFeatures(long rowid, int col, int Deep, int AllowLRShift)
        {
            MLFeature<int> feature = new MLFeature<int>();
            if ((rowid) - Deep < 0)//rowid 至少要大于等于Deep;
                return feature;
            long baseIndex = rowid;
            for (int i = -1 * Deep * AllowLRShift; i <= 1 * Deep * AllowLRShift; i++)//偏移
            {
                int BColIndex = (col + i) % 10;//对于大于10的取模
                if (BColIndex < 0)//对于小于0的，+10 如：0 + （-1） = 9
                {
                    BColIndex = BColIndex + 10;
                }
                for (int j = 0; j <= Deep; j++)
                {
                    if ((i * i + j * j) <= Deep * Deep)//深度内
                    {
                        int Fval = int.Parse(Data[(int)baseIndex - j].ValueList[BColIndex]);
                        feature.Add(Fval);
                    }
                }
            }
            return feature;
        }

        public List<MLFeature<int>> getAllSpecRowRoundFeatures(long rowid,int Deep, int AllowLRShift)
        {
            List<MLFeature<int>> ret = new List<MLFeature<int>>();
            for (int i = 0; i < 10; i++)
                ret.Add(getSpecRowRoundFeatures(rowid, i, Deep, AllowLRShift));
            return ret;
        }

        public MLInstance<int, int> getSpecRowRoundLabelFeatures(long rowid, int col, int Deep, int AllowUseShift)
        {
            MLInstance<int, int> ret = new MLInstance<int, int>();
            if ((rowid - 1) - Deep < 0)//rowid 至少要大于等于Deep;
                return ret;
            ret.Feature = getSpecRowRoundFeatures(rowid - 1, col, Deep, AllowUseShift);
            ret.Label = int.Parse(Data[(int)rowid].ValueList[col]);
            return ret;
        }

        public MLInstances<int, int> getAllSpecColRoundLabelAndFeatures(int col, int Deep, int AllowUseShift)
        {
            MLInstances<int, int> ret = new MLInstances<int, int>();
            for (int i = Deep + 1; i < Data.Count; i++)
            {
                ret.Add(getSpecRowRoundLabelFeatures(i, col, Deep, AllowUseShift));
            }
            return ret;
        }

        public List<MLInstances<int, int>> getAllSpecColRoundLabelAndFeatures(int Deep, int AllowUseShift)
        {
            List<MLInstances<int, int>> ret = new List<MLInstances<int, int>>();
            for (int i = 0; i < 10; i++)
            {
                ret.Add(getAllSpecColRoundLabelAndFeatures(i, Deep, AllowUseShift));
            }
            return ret;
        }
    }

    public class SpecLengthMatchTimesCategoryFactoryClass: MLDataCategoryFactoryClass
    {
        public SpecLengthMatchTimesCategoryFactoryClass()
        {

        }

        public override MLInstances<int, int> getCategoryData(int col, int Deep, int AllowUseShift)
        {
            return null;
        }

        MLInstance<int,int> getSpecLengthMatchTimesProb(int col, int Deep, int AllowUseShift)
        {
            MLInstance<int, int> ret = new MLInstance<int, int>();
            return ret;     
        }
    }

    /// <summary>
    /// 马尔科夫分类特征
    /// </summary>
    public class MarkovCategoryFactioryClass : MLDataCategoryFactoryClass
    {
        public override MLInstances<int, int> getCategoryData(int col, int Deep, int AllowUseShift)
        {
            MLInstances<int, int> ret = new MLInstances<int, int>();
            for(int i=0;i<this.Data.Count;i++)
            {
                ExpectData ed = AllowUseShift==0? Data[i]:new Combin_ExpectData(Data[i]);
                ret.Add(new MLInstance<int, int>(new int[] { int.Parse(ed.ValueList[col]) }.ToList()));
                
            }
            return ret;
        }
    }
}
