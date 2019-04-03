using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;

namespace WolfInv.com.PK10CorePress
{
    public class ChanceClass : BaseObjectsLib.ChanceClass<TimeSerialData>
    {
        public EventCheckNeedEndTheChance OnCheckTheChance;
    }
    public abstract class TraceChance : ChanceClass, ITraceChance, ISpecAmount
    {
        public bool CheckNeedEndTheChance(ChanceClass cc, bool LastExpectMatched)
        {
            if (this.MatchChips > 0)//如果命中，即关闭
            {
                return true;
            }
            return false;
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

        long getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts)
        {
            throw new NotImplementedException();
        }

        long ISpecAmount.getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts)
        {
            throw new NotImplementedException();
        }

        public abstract bool CheckNeedEndTheChance<T>(ChanceClass<T> cc, bool LastExpectMatched) where T : TimeSerialData;
        public abstract long getChipAmount<T>(double RestCash, ChanceClass<T> cc, AmoutSerials amts) where T : TimeSerialData;
    }
    public class NolimitTraceChance : TraceChance
    {
        public override bool CheckNeedEndTheChance<T>(ChanceClass<T> cc, bool LastExpectMatched)
        {
            return CheckNeedEndTheChance(cc, LastExpectMatched);
        }
        

        public bool CheckNeedEndTheChance(ChanceClass cc, bool LastExpectMatched)
        {
            return true;
        }

        

        public override long getChipAmount<T>(double RestCash, ChanceClass<T> cc, AmoutSerials amts) 
        {
            return getChipAmount(RestCash, cc, amts);
        }

        public long getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts)
        {
            int chips = 0;
            int maxcnt = amts.MaxHoldCnts[chips];
            int bShift = 0;
            int eShift = 0;
            int bHold = cc.HoldTimeCnt;// HoldCnt - CurrChancesCnt + 1;
            if (cc.IncrementType == InterestType.CompoundInterest)
            {
                if (cc.AllowMaxHoldTimeCnt > 0 && cc.HoldTimeCnt > cc.AllowMaxHoldTimeCnt)
                {
                    return 0;
                }
                return (long)Math.Floor(cc.FixRate.Value * RestCash / cc.ChipCount);
            }
            //Log("获取机会金额处理", string.Format("当前持有次数：{0}", HoldCnt));

            if (bHold > maxcnt)
            {
                Log("风险", "通用重复策略开始次数达到最大上限", string.Format("机会{0}持有次数达到{1}次总投入金额已为{2}", cc.ChanceCode, bHold, "未知"));
                bShift = (int)maxcnt * 2 / 3;
            }
            int HoldCnt = bHold;
            int bRCnt = (bHold % (maxcnt + 1)) + bShift - 1;
            int eRCnt = (HoldCnt % (maxcnt + 1)) + eShift - 1;
            if (cc.ChipCount < 4)//如果是4码以下取平均值
            {
                return (amts.Serials[chips][bRCnt] + amts.Serials[chips][eRCnt]) / 2;
            }
            //四码以上取最大值，防止投入不够导致亏损
            return amts.Serials[chips][eRCnt];

        }

    }


    public class OnceChance : ChanceClass
    {
        public OnceChance()
        {
            AllowMaxHoldTimeCnt = 1;
        }
    }

    public delegate bool EventCheckNeedEndTheChance(ChanceClass CheckCc, bool LastExpectMatched);


    public class DbChanceList : DbChanceList<TimeSerialData>
    {
    }

        public interface ISpecAmount
    {
        Int64 getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts);
    }
    public interface ITraceChance_Del
    {
        bool IsTracing { get; set; }
        bool CheckNeedEndTheChance(ChanceClass cc, bool LastExpectMatched);
    }
}
