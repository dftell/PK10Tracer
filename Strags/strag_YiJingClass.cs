﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using WolfInv.com.PK10CorePress;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.Strags
{
    [DescriptionAttribute("易经选号策略"),
        DisplayName("易经选号策略")]
    [Serializable]
    public class strag_YiJingClass:StragClass
    {
        public strag_YiJingClass()
            : base()
        {
            this._StragClassName = "易经选号策略";
        }

        public override List<ChanceClass> getChances(CommCollection sc, ExpectData ed)
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
