using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strags.MLStragClass
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
