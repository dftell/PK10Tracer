using System;
using System.Collections.Generic;
using System.ComponentModel;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.PK10CorePress;
namespace WolfInv.com.Strags
{
    [DescriptionAttribute("泊松分布选号策略"),
        DisplayName("泊松分布选号策略")]
    public class strag_PoissonRandomClass:StragClass
    {
        public override List<ChanceClass> getChances(BaseCollection sc, ExpectData ed)
        {
            List<ChanceClass> ret = new List<ChanceClass>();
            
            return ret;
        }

        public override StagConfigSetting getInitStagSetting()
        {
            StagConfigSetting ret = new StagConfigSetting();
            return ret;
        }

        public override Type getTheChanceType()
        {
            return typeof(ChanceClass);
        }
        bool _IsTracing;
        public bool IsTracing
        {
            get
            {
                return _IsTracing;
            }
            set
            {
                _IsTracing = value;
            }
        }

        public override bool CheckNeedEndTheChance(ChanceClass cc, bool LastExpectMatched)
        {
            return true;
        }

        public override long getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts)
        {
            return (Int64)Math.Floor(RestCash*0.001);//ProbMath.GetFactorial
        }
    }
}
