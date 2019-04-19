using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using WolfInv.com.PK10CorePress;
using WolfInv.com.ProbMathLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.Strags.MLStragClass
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

        public override List<ChanceClass> getChances(BaseCollection sc, ExpectData ed)
        {
            throw new NotImplementedException();
        }

        public override StagConfigSetting getInitStagSetting()
        {
            throw new NotImplementedException();
        }

        public override Type getTheChanceType()
        {
            return this.GetType();
        }

        public override long getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts)
        {
            return getDefaultChipAmount(RestCash, cc, amts);
        }
    }
}
