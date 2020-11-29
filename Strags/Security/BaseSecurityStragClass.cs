using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.SecurityLib;
using WolfInv.com.GuideLib.LinkGuid;
using WolfInv.com.GuideLib;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.LogLib;
using WolfInv.com.Strags;
using WolfInv.com.SecurityLib;
namespace WolfInv.com.Strags.Security
{
    public abstract class BaseSecurityStragClass<T>:BaseStragClass<T> where T:TimeSerialData
    {
        protected CommFilterLogicBaseClass<T> LogicFilter;
        protected MongoDataDictionary<T> CurrSecDic;
        protected BaseSecurityStragClass()
        {

        }

        //public override List<ChanceClass<T>> getChances(BaseCollection<T> sc, ExpectData<T> ed);

        //public override StagConfigSetting getInitStagSetting()
        //{

        //}

        public override Type getTheChanceType()
        {
            return this.GetType();
        }

        
    }
}
