using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.PK10CorePress;

namespace WolfInv.com.Strags.KLXxY
{
    [DescriptionAttribute("X选Y小敞口暴露组合投注策略"),
        DisplayName("X选Y小敞口暴露组合投注策略")]
    public class Strag_KLXxY_Combin_All8_All3 : StragClass
    {
        public Strag_KLXxY_Combin_All8_All3()
        {
            this._StragClassName = "X选Y小敞口暴露组合投注策略";
        }

        public override bool CheckNeedEndTheChance(ChanceClass cc1, bool LastExpectMatched1)
        {
            return true;
        }

        public override List<ChanceClass> getChances(BaseCollection sc, ExpectData ed)
        {
            //使用投注方式
            //A8
            //11选5使用A8+3个A2覆盖，暴露A1
            List<ChanceClass> ret = new List<ChanceClass>();
            return ret;
        }

        public override long getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts)
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
