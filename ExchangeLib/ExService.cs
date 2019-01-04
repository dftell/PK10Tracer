using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Data;
using PK10CorePress;
using Strags;
using LogLib;
using System.ComponentModel;
namespace ExchangeLib
{
    [Serializable]
    public class ExchangeService
    {
        public ExchangeService()
        {
        }
        //Dictionary<DateTime, ExchangeChance> EQ;
        Timer time_ForExec = new Timer();
        double _InitCash = 0;
        double CurrMoney = 0;
        ExchangeDataTable ed;
        List<double> MoneyLine;
        int _ExpectCnt;
        DataTable MoneyChangeTable;
        public int ExpectCnt
        {
            get
            {
                return _ExpectCnt;
            }
        }

        public void UpdateExpectCnt(int val)
        {
            _ExpectCnt = val;
        }

        public ExchangeService(double InitCash, double odds)
        {
            _InitCash = InitCash;
            ed = new ExchangeDataTable(odds);
            CurrMoney = InitCash;
            MoneyLine = new List<double>();
            MoneyLine.Add(CurrMoney);
            //EQ = new Dictionary<DateTime, ExchangeChance>();
            ////time_ForExec.Interval = 1000;
            ////time_ForExec.AutoReset = true;
            ////time_ForExec.Elapsed += new ElapsedEventHandler(ExchangeTheChance);
        }

        public void Reset()
        {
            if (ed != null)
                ed.Rows.Clear();
            if (MoneyChangeTable != null)
                MoneyChangeTable.Rows.Clear();
            CurrMoney = _InitCash;
            MoneyLine = new List<double>();
            _ExpectCnt = 0;
        }

        //////void ExchangeTheChance(object sender, ElapsedEventArgs e)
        //////{
        //////    ////ExchangeLock = true;
        //////    //////ToAdd 交易内容
        //////    ////ExchangeLock = false;
        //////}


        public bool Push(ref ExchangeChance ec)
        {
            lock (ed)
            {
                if (ec.OwnerChance == null) return false;
                if (ec.OccurStrag == null) return false;
                #region 单位金额由外面传入
                ////////////if (ec.OwnerChance.IncrementType == InterestType.CompoundInterest)//如果是复利
                ////////////{
                ////////////    if (ec.OccurStrag is ProbWaveSelectStragClass)//由外面传入
                ////////////    {
                ////////////    }
                ////////////    else
                ////////////    {
                ////////////        if (ec.OwnerChance.Tracerable)
                ////////////        {
                ////////////            ec.ExchangeAmount = (ec.OwnerChance as ISpecAmount).getChipAmount(CurrMoney, ec.OwnerChance, ec.OccurStrag.CommSetting.GetGlobalSetting().DefaultHoldAmtSerials);
                ////////////        }
                ////////////        else
                ////////////        {
                ////////////            ec.ExchangeAmount = (ec.OccurStrag as ChanceTraceStragClass).getChipAmount(CurrMoney, ec.OwnerChance, ec.OccurStrag.CommSetting.GetGlobalSetting().DefaultHoldAmtSerials);
                ////////////            ec.ExchangeRate = ec.ExchangeAmount * ec.OwnerChance.ChipCount / CurrMoney;
                ////////////        }
                ////////////    }
                ////////////    //ec.ExchangeAmount = (int)Math.Floor(CurrMoney * ec.OccurStrag.StagSetting.BaseType.ChipRate);
                ////////////}
                ////////////else
                ////////////{
                ////////////    if (ec.OwnerChance.Tracerable)
                ////////////    {
                ////////////        ec.ExchangeAmount = (ec.OwnerChance as ISpecAmount).getChipAmount(CurrMoney, ec.OwnerChance, ec.OccurStrag.CommSetting.GetGlobalSetting().DefaultHoldAmtSerials);
                ////////////    }
                ////////////    else
                ////////////    {
                ////////////        ec.ExchangeAmount = (ec.OccurStrag as ChanceTraceStragClass).getChipAmount(CurrMoney, ec.OwnerChance, ec.OccurStrag.CommSetting.GetGlobalSetting().DefaultHoldAmtSerials);
                ////////////    }
                ////////////}
                #endregion
                if (CurrMoney < ec.ExchangeAmount * ec.OwnerChance.ChipCount)
                {
                    LogableClass.ToLog("风险","剩余金额不足", ec.OwnerChance.ToDetailString());
                    return false;
                }
                if (ec.ExchangeAmount * ec.OwnerChance.ChipCount * 10 > CurrMoney)//记录投入资金超出10%的记录
                {
                    LogableClass.ToLog("风险",string.Format("大投入组合[{0}]", (double)100 * ec.ExchangeAmount * ec.OccurStrag.ChipCount / CurrMoney), ec.OwnerChance.ToDetailString());
                }
                ec = ed.AddAChance(ec);
                CurrMoney -= ec.ExchangeAmount * ec.OwnerChance.ChipCount;
                //MoneyLine.Add(CurrMoney);
                return true;
            }
        }

        public bool Update(ExchangeChance ec)
        {
            lock (ed)
            {
                double Gained = 0.0;
                bool suc = ed.UpdateChance(ec, out Gained);
                CurrMoney += Gained;
                MoneyLine.Add(CurrMoney);
                return suc;
            }
        }

        /// <summary>
        /// 当前余额
        /// </summary>
        public double summary
        {
            get { return CurrMoney; }
        }

        public double InitCash
        {
            get
            {
                return _InitCash;
            }
        }

        public DataTable ExchangeDetail
        {
            get
            {
                return ed;
            }
        }

        public double GainedRate
        {
            get
            {
                try
                {
                    return Math.Round(100 * (CurrMoney - _InitCash) / _InitCash, 2);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public double MaxRate
        {
            get
            {
                try
                {
                    return Math.Round(100 * (MoneyLine.Max() - _InitCash) / _InitCash, 2);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public double MinRate
        {
            get
            {
                try
                {
                    return Math.Round(100 * (MoneyLine.Min() - _InitCash) / _InitCash, 2);
                }
                catch
                {
                    return 0;
                }
            }
        }

        

        public DataTable MoneyIncreamLine
        {

            get
            {
                lock (MoneyLine)
                {
                    if (MoneyChangeTable == null || MoneyChangeTable.Rows.Count < MoneyLine.Count)
                    {
                        MoneyChangeTable = new DataTable();
                        MoneyChangeTable.Columns.Add("id", typeof(int));
                        MoneyChangeTable.Columns.Add("val", typeof(double));
                        for (int i = 0; i < MoneyLine.Count; i++)
                        {
                            DataRow dr = MoneyChangeTable.NewRow();
                            dr["id"] = i;
                            dr["val"] = Math.Round(100 * (MoneyLine[i] - _InitCash) / _InitCash, 2);
                            MoneyChangeTable.Rows.Add(dr);
                        }
                    }
                    return MoneyChangeTable;
                }
            }
        }

        public int CurrIndex
        {
            get
            {
                return ed.Rows.Count;
            }
        }
    }

}
