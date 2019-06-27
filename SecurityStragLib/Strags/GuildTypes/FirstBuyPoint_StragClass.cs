using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
using WolfInv.com.SecurityLib;
using WolfInv.com.Strags;
using WolfInv.com.SecurityLib;
using WolfInv.com.SecurityLib.Strategies.Bussyniess;

namespace WolfInv.com.SecurityStragLib.Strags.GuildTypes
{
    /// <summary>
    /// 第一买点选股策略
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class FirstBuyPoint_StragClass<T> : BaseSecurityStragClass<T> , ITraceChance where T : TimeSerialData
    {
        
        public FirstBuyPoint_StragClass()
        {
            this._StragClassName = "第一买点选股策略";
            
            //LogicFilter = new FirstPointFilter();
        }
        bool _IsTracing;
        public bool IsTracing {
            get { return _IsTracing; }
            set { _IsTracing = value; }
        }

        public bool CheckNeedEndTheChance<T>(ChanceClass<T> cc, bool LastExpectMatched) where T : TimeSerialData
        {
            return false;
            //throw new NotImplementedException();
        }

        public override List<ChanceClass<T>> getChances(BaseCollection<T> sc, ExpectData<T> ed)
        {
            List<ChanceClass<T>> ret = new List<ChanceClass<T>>();
            FirstPointFilter_Logic_Strategy fpf = new FirstPointFilter_Logic_Strategy(new CommDataIntface());
            fpf.InParam = new CommStrategyInClass();
            
            return ret;
         }

        public long getChipAmount<T>(double RestCash, ChanceClass<T> cc, AmoutSerials amts) where T : TimeSerialData
        {
            throw new NotImplementedException();
        }

        public override StagConfigSetting getInitStagSetting()
        {
            return new StagConfigSetting();
        }

        
    }
}
