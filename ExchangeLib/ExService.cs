using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Data;
using WolfInv.com.PK10CorePress;
using WolfInv.com.Strags;
using WolfInv.com.LogLib;
using System.ComponentModel;
using System.Reflection;
using WolfInv.com.BaseObjectsLib;
using System.Runtime.Serialization;
namespace WolfInv.com.ExchangeLib
{
    [Serializable]
    public class ExchangeService: DisplayAsTableClass,ISerializable
    {
        public ExchangeService()
        {
            //ed = null;
        }
        //实现了ISerializable接口的类必须包含有序列化构造函数，否则会出错。        
        protected ExchangeService(SerializationInfo info, StreamingContext context)
        {
            //Value = info.GetBoolean("Test_Value");
        }

        //Dictionary<DateTime, ExchangeChance> EQ;
        AssetUnitClass CurrUnit;
        Timer time_ForExec = new Timer();
        double _InitCash = 0;
        double CurrMoney = 0;
        ExchangeDataTable ed;
        //List<int> MoneyLine;
        Dictionary<string, double> ExpectMoneyLine;
        int _ExpectCnt;
        DataTable MoneyChangeTable;
        public int ExpectCnt
        {
            get
            {
                return _ExpectCnt;
            }
        }


        public AssetUnitClass getCurrAsset()
        {
            return CurrUnit;
        }

        public void setCurrAsset(AssetUnitClass unit)
        {
            CurrUnit = unit;
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
            //MoneyLine = new List<double>();
            ExpectMoneyLine = new Dictionary<string, double>();
            //MoneyLine.Add(CurrMoney);
            //EQ = new Dictionary<DateTime, ExchangeChance>();
            ////time_ForExec.Interval = 1000;
            ////time_ForExec.AutoReset = true;
            ////time_ForExec.Elapsed += new ElapsedEventHandler(ExchangeTheChance);
        }

        public void LoadTheLastRecords(DataTable dt)
        {
            if (dt == null) return;
            MoneyChangeTable = null;
            //MoneyLine.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                double rate = double.Parse(dt.Rows[i]["val"].ToString());
                string id = dt.Rows[i]["id"].ToString();

                ExpectMoneyLine.Add(id,this.InitCash*(100+rate)/100);
            }
            if (ExpectMoneyLine.Count > 0)
                CurrMoney = ExpectMoneyLine.Values.Last();
            else
                CurrMoney = _InitCash;
        }

        public void Reset()
        {
            if (ed != null)
                ed.Rows.Clear();
            if (MoneyChangeTable != null)
                MoneyChangeTable.Rows.Clear();
            CurrMoney = _InitCash;
            ExpectMoneyLine = new Dictionary<string, double>();
            //MoneyLine = new List<double>();
            _ExpectCnt = 0;
        }

        //////void ExchangeTheChance(object sender, ElapsedEventArgs e)
        //////{
        //////    ////ExchangeLock = true;
        //////    //////ToAdd 交易内容
        //////    ////ExchangeLock = false;
        //////}


        public bool Push<T>(ref ExchangeChance<T> ec) where T : TimeSerialData
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

        public bool Update<T>(ExchangeChance<T> ec) where T : TimeSerialData
        {
            lock (ed)
            {
                double Gained = 0.0;
                bool suc = ed.UpdateChance(ec, out Gained);
                CurrMoney += Gained;
                //MoneyLine.Add(CurrMoney);
                if(ExpectMoneyLine.ContainsKey(ec.ExExpectNo))
                {
                    ExpectMoneyLine[ec.ExExpectNo] = CurrMoney;
                }
                else
                {
                    ExpectMoneyLine.Add(ec.ExExpectNo , CurrMoney);
                }
                return suc;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            
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
                //ToLog("资产单元", this.CurrUnit.UnitName, string.Format("记录条数:{0};", ed.Rows.Count));
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
                    return Math.Round(100 * (ExpectMoneyLine.Values.Max() - _InitCash) / _InitCash, 2);
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
                    return Math.Round(100 * (ExpectMoneyLine.Values.Min() - _InitCash) / _InitCash, 2);
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
                lock (ExpectMoneyLine)
                {
                    if (MoneyChangeTable == null || MoneyChangeTable.Rows.Count < ExpectMoneyLine.Count)
                    {
                        if (MoneyChangeTable==null)
                        {
                            MoneyChangeTable = new DataTable();
                            MoneyChangeTable.Columns.Add("id");
                            MoneyChangeTable.Columns.Add("val", typeof(double));
                        }
                        else
                        {
                            MoneyChangeTable.Rows.Clear();
                        }
                        foreach (string strkey in ExpectMoneyLine.Keys)
                        {
                            DataRow dr = MoneyChangeTable.NewRow();
                            dr["id"] = strkey;
                            dr["val"] = Math.Round(100 * (ExpectMoneyLine[strkey] - _InitCash) / _InitCash, 2);
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
