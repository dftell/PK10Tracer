using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using PK10CorePress;
using ProbMathLib;

namespace Strags.MLStragClass
{
    /// <summary>
    /// 奇偶大小随机游走机器学习策略
    /// </summary>
    [DescriptionAttribute("奇偶大小随机游走机器学习策略"),
        DisplayName("奇偶大小随机游走机器学习策略")]
    [Serializable]
    public class OddEvenAndBigSmall_AutoLearnStragClass:BayesLearnStragClass
    {
        public OddEvenAndBigSmall_AutoLearnStragClass()
            : base()
        {
            _StragClassName = "奇偶大小随机游走机器学习策略";
        }

        public override void Train()
        {
            throw new NotImplementedException();
        }

        public override List<PK10CorePress.ChanceClass> getChances(PK10CorePress.CommCollection sc, PK10CorePress.ExpectData ed)
        {
            throw new NotImplementedException();
        }

        public override StagConfigSetting getInitStagSetting()
        {
            throw new NotImplementedException();
        }

        public override Type getTheChanceType()
        {
            throw new NotImplementedException();
        }
    }
}
