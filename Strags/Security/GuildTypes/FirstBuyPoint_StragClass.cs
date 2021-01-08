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
            MongoDataDictionary<T> useAllDic = CurrSecDic;
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
                CommSecurityProcessClass<T> secprs = new CommSecurityProcessClass<T>(checkUseSec.SecInfo, checkUseSec);
                CommFilterLogicBaseClass<T> checkSec = new MACDTopRevFilter<T>(useExpect, secprs);
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
                RunResultClass rrc = fpf.Execute();
                for (int i = 0; i < rrc.Result.Count; i++)
                {
                    string code = rrc.Result[i].Key;
                    SecurityChance<T> cc = new SecurityChance<T>();
                    if (!CurrSecDic.ContainsKey(code))
                        continue;
                    //允许交易
                    if (TryOpenExchange(fpf.InParam, cc, code, CurrSecDic[code], ed, useData.LastData, review))
                    {
                        cc.inputStatus = rrc.Result[i].Status;
                        cc.ReferValues = rrc.Result[i].ReferValues;
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
