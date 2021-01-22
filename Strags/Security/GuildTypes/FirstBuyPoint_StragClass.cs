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
using System.Collections.Concurrent;

namespace WolfInv.com.Strags.Security
{
    /// <summary>
    /// 第一买点选股策略
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DescriptionAttribute("第一买点选股策略"),
        DisplayName("第一买点选股策略")]
    [Serializable]
    public class FirstBuyPoint_StragClass<T> : BaseSecurityStragClass<T>  where T : TimeSerialData
    {        
        public FirstBuyPoint_StragClass()
        {
            this._StragClassName = "第一买点选股策略";
            //LogicFilter = new FirstPointFilter();
        }
        bool _IsTracing;
        public override bool IsTracing {
            get { return _IsTracing; }
            set { _IsTracing = value; }
        }

        public override bool CheckNeedEndTheChance(ChanceClass<T> scc, bool LastExpectMatched,bool review)
        {
            SecurityChance<T> cc = scc as SecurityChance<T>;
            bool check = base.CheckNeedEndTheChance(cc, LastExpectMatched, review);
            if(check)
            {
                return check;
            }
            ExpectList<T> AllData = LastUseData();
            ExpectList<T> list = AllData;            
            ExpectData<T> useData = list.LastData;
            ConcurrentDictionary<string,MongoReturnDataList<T>> useAllDic = CurrSecDic;
            try
            {
                string useExpect = useData.Expect;
                if (review)
                {
                    list = AllData.ExcludeLastLong(1);
                    useData = list.LastData;
                    useExpect = useData.Expect;
                    useAllDic = new MongoDataDictionary<T>(list, true);
                }
                if (CurrSecDic == null)
                {
                    return false;
                }
                if (!CurrSecDic.ContainsKey(cc.ChanceCode))
                {
                    return false;
                }
                MongoReturnDataList<T> sec = CurrSecDic[cc.ChanceCode];
                string expectNo = LastUseData().LastData.Expect;//当期 
                MongoReturnDataList<T> checkUseSec = CurrSecDic[cc.ChanceCode];
                if (sec.Last().Expect != LastUseData().LastData.Expect)
                {
                    return false;
                }
                //KLineData<T> klines = new KLineData<T>(useExpect,sec,PriceAdj.Beyond);//必须用向后复权价
                cc.currCantEndChance = false;
                CommStrategyInClass fpf = new CommStrategyInClass();
                fpf.EndExpect = useExpect;
                fpf.SecIndex = cc.ChanceCode;
                fpf.SignDate = cc.SignExpectNo;
                fpf.MinDays = this.InputMaxTimes;
                fpf.ReferDays = this.InputMinTimes;
                fpf.BuffDays = this.ChipCount;
                fpf.TopN = this.SingeExpectSelectTopN;
                fpf.StartDate = cc.exchangeExpect;
                fpf.StopPrice = cc.stopPrice;
                fpf.StopPriceDate = cc.stopPriceDate;
                fpf.useMutliCycle = this.IsMutliCycle;
                fpf.useMaxCycle = this.MaxCycle;
                fpf.useMinCycle = this.MinCycle;
                CommSecurityProcessClass<T> secprs = new CommSecurityProcessClass<T>(checkUseSec.SecInfo, checkUseSec);
                Cycle useCyc = Cycle.Day;
                cc.HoldTimeCnt = Math.Max(1, sec.Where(a => a.Expect.ToDate() > cc.ExpectCode.ToDate()).Count()) - 1;
                if (cc.HoldTimeCnt>60 && cc.HoldTimeCnt<=250)
                {
                    useCyc = Cycle.Week;
                }
                else if(cc.HoldTimeCnt>250)
                {
                    useCyc = Cycle.Month;
                }
                CommFilterLogicBaseClass<T> checkSec = new MACDTopRevFilter<T>(useExpect, secprs, PriceAdj.Beyond, useCyc);
                checkSec.getSingleData = getSingleData;//需要用到穿透到外面取数据的地方一定要给赋值
                var res = checkSec.ExecFilter(fpf);
                if (res.Enable)//需要关闭
                {
                    cc.endStatus = res.Status;
                    //是否能交易必须使用最新日期的数据sec
                    cc.currCantEndChance = !TryCloseExchange(cc as SecurityChance<T>, sec, AllData.LastData, review);
                    return true;
                }
            }
            catch(Exception ce)
            {
                

            }
            finally
            {
                AllData = null;
                list = null;
            }
            return false;
            //throw new NotImplementedException();
        }
        public override List<ChanceClass<T>> getChances(BaseCollection<T> sc, ExpectData<T> ed, bool review = false)
        {
            List<ChanceClass<T>> ret = new List<ChanceClass<T>>();
            //CurrSecDic = new MongoDataDictionary<T>(this.LastUseData(), true);//最后的数据
            ExpectList<T> useData = review ? this.LastUseData().ExcludeLastLong(1) : this.LastUseData();
            FirstPointFilter_Logic_Strategy<T> fpf = new FirstPointFilter_Logic_Strategy<T>(new SecurityDataInterface<T>(useData));//去除本期
            fpf.afterSelectSingleSecurity = getInterProcessFinished();
            try
            {
                                                                                                                                       //CurrSecDic = fpf.DataInterFace.getData();
                fpf.InParam = new CommStrategyInClass();
                fpf.InParam.SecsPool = useData.LastData.Values.Select(a => a.Key).ToList();
                fpf.InParam.EndExpect = useData.LastData.Expect;//
                fpf.InParam.MinDays = this.InputMaxTimes;
                fpf.InParam.ReferDays = this.InputMinTimes;
                fpf.InParam.BuffDays = this.ChipCount;
                fpf.InParam.TopN = 2 * this.SingeExpectSelectTopN;//2倍范围
                fpf.InParam.allowMaxDownRate = -5;//开盘跌出5%不成交
                fpf.InParam.allowMaxRaiseRate = 5;//开盘涨过5%不成交
                fpf.InParam.useMutliCycle = this.IsMutliCycle;
                fpf.InParam.useMaxCycle = this.MaxCycle;
                fpf.InParam.useMinCycle = this.MinCycle;
                RunResultClass rrc = fpf.Execute();
                for (int i = 0; i < rrc.Result.Count; i++)
                {
                    string code = rrc.Result[i].Key;
                    SecurityChance<T> cc = new SecurityChance<T>();
                    
                    if (!CurrSecDic.ContainsKey(code))
                        continue;
                    if (string.IsNullOrEmpty(CurrSecDic[code].SecInfo.name))//如果是指数，暂时这样排除
                        continue;
                    //检验是否允许交易
                    ///先给必须要先赋值的属性赋值
                    cc.ChanceCode = code;
                    cc.chanceName = CurrSecDic[code]?.SecInfo?.name;
                    cc.ReferValues = rrc.Result[i].ReferValues;//必须要先指定参考值，在里面由检验过程按需要
                    ///默认参考数组
                    ///第一个元素是前期低点，止损值，没有为0
                    ///第二个元素是前期低点产生的日期，没有为空
                    if (TryOpenExchange(fpf.InParam, cc, CurrSecDic[code], ed, useData.LastData, review))//检验通过，则增加该机会
                    {
                        cc.inputStatus = rrc.Result[i].Status;                        
                        ret.Add(cc);
                    }
                    if (ret.Count >= this.SingeExpectSelectTopN)//实际成交不能超过TopN
                    {
                        break;
                    }
                }
            }
            catch(Exception ce)
            {

            }
            finally
            {
                useData = null;
                CurrSecDic = null;
                fpf = null;
                GC.Collect();
            }
            return ret;
         }

        
        public override StagConfigSetting getInitStagSetting()
        {
            return new StagConfigSetting();
        }        
    }
}
