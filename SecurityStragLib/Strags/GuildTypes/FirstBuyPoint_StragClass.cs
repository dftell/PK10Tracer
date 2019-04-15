using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
using WolfInv.com.SecurityLib;
using WolfInv.com.Strags;

namespace WolfInv.com.SecurityStragLib.Strags.GuildTypes
{
    public class FirstBuyPoint_StragClass<T> : BaseSecurityStragClass<T> where T : TimeSerialData
    {
        public override List<ChanceClass<T>> getChances(BaseCollection<T> sc, ExpectData<T> ed)
        {
            List<ChanceClass<T>> ret = new List<ChanceClass<T>>();
            return ret;
         }

        public override StagConfigSetting getInitStagSetting()
        {
            return new StagConfigSetting();
        }

        
    }
}
