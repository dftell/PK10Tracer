using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.PK10CorePress;
namespace WolfInv.com.Strags.MLStragClass
{
    ///机器学习基类
    public abstract class MachineLearnStragClass:StragClass,iMachineLearnable
    {
        int _LearnCnt;
        public int LearnCnt
        {
            get
            {
                return _LearnCnt;
            }
            set
            {
                _LearnCnt = value;
            }
        }

        public abstract void Train();
        public override bool CheckNeedEndTheChance(ChanceClass cc1, bool LastExpectMatched1)
        {
            return LastExpectMatched1;
        }
    }

    /// <summary>
    /// 机器学习接口
    /// </summary>
    public interface iMachineLearnable
    {
        int LearnCnt { get; set; }

        void Train();
    }

    /// <summary>
    /// 叶贝斯网络策略基类
    /// </summary>
    public abstract class BayesLearnStragClass : MachineLearnStragClass
    {
        
    }

}
