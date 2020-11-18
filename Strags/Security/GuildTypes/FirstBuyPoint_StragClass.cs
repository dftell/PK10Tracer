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
using System.ComponentModel;
using WolfInv.com.GuideLib.Filter;
using WolfInv.com.GuideLib;
namespace WolfInv.com.Strags.Security
{
    /// <summary>
    /// 第一买点选股策略
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DescriptionAttribute("第一买点选股策略"),
        DisplayName("第一买点选股策略")]
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
            FirstPointFilter_Logic_Strategy<T> fpf = new FirstPointFilter_Logic_Strategy<T>(new CommDataIntface());

            fpf.InParam = new CommStrategyInClass();
            RunResultClass rrc = fpf.ExecSelect();

            ////MongoDataDictionary<T> useData = new MongoDataDictionary<T>(this.LastUseData());

            ////foreach(string key in useData.Keys)
            ////{
            ////    MongoReturnDataList<T> stockData = useData[key];
            ////    FirstPointClass<T> fc = new FirstPointClass<T>(stockData);
            ////    if(!fc.matched())
            ////    {
            ////        continue;
            ////    }
            ////}
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
