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
using WolfInv.com.GuideLib;
using WolfInv.com.SecurityLib.Filters.StrategyFilters;

namespace WolfInv.com.Strags.Security
{
    /// <summary>
    /// 第一买点选股策略
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DescriptionAttribute("第一买点选股策略"),
        DisplayName("第一买点选股策略")]
    [Serializable]
    public class FirstBuyPoint_StragClass<T> : BaseSecurityStragClass<T> , ITraceChance<T> where T : TimeSerialData
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

        public bool CheckNeedEndTheChance(ChanceClass<T> cc, bool LastExpectMatched)
        {
            //CommIndustryStrategyInParams csi = new CommIndustryStrategyInParams();
            CurrSecDic = new MongoDataDictionary<T>(LastUseData());
            if(CurrSecDic==null)
            {
                return false; 
            }
            if(!CurrSecDic.ContainsKey(cc.ChanceCode))
            {
                return false;
            }
            MongoReturnDataList<T> sec = CurrSecDic[cc.ChanceCode];
            (cc as SecurityChance<T>).currUnitPrice = (sec.Last() as StockMongoData).close;//每次都变更当前价
            CommStrategyInClass fpf = new CommStrategyInClass();
            fpf.MinDays = this.InputMaxTimes;
            fpf.ReferDays = this.InputMinTimes;
            fpf.BuffDays = this.ChipCount;
            fpf.TopN = this.MaxHoldCnt;
            fpf.StartDate = cc.ExpectCode;            
            CommSecurityProcessClass<T> secprs = new CommSecurityProcessClass<T>(sec.SecInfo, sec);
            CommFilterLogicBaseClass<T> checkSec = new StopLossFilter<T>(secprs);
            secprs = checkSec.ExecFilter(fpf);
            if (secprs.Enable)
                return true;
            checkSec = new MACDTopRevFilter<T>(secprs);
            
            
            return checkSec.ExecFilter(fpf).Enable;
            //throw new NotImplementedException();
        }
        public override List<ChanceClass<T>> getChances(BaseCollection<T> sc, ExpectData<T> ed)
        {
            List<ChanceClass<T>> ret = new List<ChanceClass<T>>();
            FirstPointFilter_Logic_Strategy<T> fpf = new FirstPointFilter_Logic_Strategy<T>(new SecurityDataInterface<T>(this.LastUseData()));
            CurrSecDic = fpf.DataInterFace.getData();
            fpf.InParam = new CommStrategyInClass();
            fpf.InParam.MinDays = this.InputMaxTimes;
            fpf.InParam.ReferDays = this.InputMinTimes;
            fpf.InParam.BuffDays = this.ChipCount;
            fpf.InParam.TopN = this.MaxHoldCnt;
            RunResultClass rrc = fpf.ExecSelect();
            rrc.Result.ForEach(a => {
                SecurityChance<T> cc = new SecurityChance<T>();
                cc.ChanceCode = a;
                cc.ExpectCode = ed.Expect;
                cc.UnitCost = (CurrSecDic[a].Last() as StockMongoData).close;
                cc.currUnitPrice = cc.UnitCost;
                cc.ChipCount = 0 ;//必须不要指定
                ret.Add(cc);
            });
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

        public double getChipAmount(double RestCash, ChanceClass<T> cc, AmoutSerials amts)
        {
            if(cc.FixAmt!=null && cc.FixAmt.Value> 0)
            {
                if (CurrSecDic.ContainsKey(cc.ChanceCode))
                {
                    double amt =  (CurrSecDic[cc.ChanceCode].Last() as StockMongoData).close;
                    if(cc.ChipCount == 0)
                        cc.ChipCount = Math.Max(1, (int)Math.Floor(cc.FixAmt.Value / 100 / amt)) * 100;
                    return amt;
                }
            }
            return 100;
        }

        public override StagConfigSetting getInitStagSetting()
        {
            return new StagConfigSetting();
        }        
    }
}
