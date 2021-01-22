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
using WolfInv.com.SecurityLib;
using WolfInv.com.GuideLib;
using System.Threading;
using System.Collections.Concurrent;

namespace WolfInv.com.ExchangeLib
{
    [Serializable]
    public class ExchangeService<T>: DisplayAsTableClass,ISerializable where T:TimeSerialData
    {
        public ExchangeService(int test)
        {
            //ed = null;
        }

        public ExchangeService(DataTypePoint _dtp)
        {
            dtp = _dtp;
        }
        //实现了ISerializable接口的类必须包含有序列化构造函数，否则会出错。        
        protected ExchangeService(SerializationInfo info, StreamingContext context)
        {
            //Value = info.GetBoolean("Test_Value");
        }
        public DataTypePoint dtp;
        //Dictionary<DateTime, ExchangeChance> EQ;
        AssetUnitClass<T> CurrUnit;
        System.Timers.Timer time_ForExec = new System.Timers.Timer();
        double _InitCash = 0;
        double CurrMoney = 0;
        double CurrAssetValue = 0;
        int AssetCount = 0;
        double benchMarkVal;
        double benchMarkInitVal;
        public void updateBenchVal(double val)
        {
            benchMarkVal = val;
        }

        ExchangeDataTable ed;
        //List<int> MoneyLine;
        ConcurrentDictionary<string, ExchangeData> ExpectMoneyLine;
        int _ExpectCnt;
        //Dictionary<string,ExchangeChance<T>> allChances = new Dictionary<string, ExchangeChance<T>>();
        DataTable MoneyChangeTable;
        public int ExpectCnt
        {
            get
            {
                return _ExpectCnt;
            }
        }


        public AssetUnitClass<T> getCurrAsset()
        {
            return CurrUnit;
        }

        public void setCurrAsset(AssetUnitClass<T> unit)
        {
            CurrUnit = unit;
        }

        public void UpdateExpectCnt(int val)
        {
            _ExpectCnt = val;
        }

        

        public ExchangeService(DataTypePoint _dtp, double InitCash, double odds)
        {
            _InitCash = InitCash;
            dtp = _dtp;
            ed = new ExchangeDataTable(odds);
            CurrMoney = InitCash;
            //MoneyLine = new List<double>();
            ExpectMoneyLine = new ConcurrentDictionary<string, ExchangeData>();
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
                ExchangeData ed = new ExchangeData();
                ed.currCash = this.InitCash * (100 + rate) / 100;
                ed.currAsset = CurrAssetValue;
                //ed.currTotal = ed.currCash + ed.currAsset;
                ExpectMoneyLine.TryAdd(id,ed);
            }
            if (ExpectMoneyLine.Count > 0)
                CurrMoney = ExpectMoneyLine.Values.Last().currCash;
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
            ExpectMoneyLine = new ConcurrentDictionary<string, ExchangeData>();
            
            //MoneyLine = new List<double>();
            _ExpectCnt = 0;
        }

        //////void ExchangeTheChance(object sender, ElapsedEventArgs e)
        //////{
        //////    ////ExchangeLock = true;
        //////    //////ToAdd 交易内容
        //////    ////ExchangeLock = false;
        //////}

        public double getAsset()
        {
            double ret = 0;
            if (ExchangeDetail != null)
            {
                string val = ExchangeDetail.Compute("Sum(Cost)+Sum(Profit)", "Closed=0").ToString();
                double.TryParse(val, out ret);
            }
            return ret;
        }

        public int getAssetCount()
        {
            int ret = 0;
            if (ExchangeDetail != null)
            {
                string val = ExchangeDetail.Compute("Count(Closed)", "Closed=0").ToString();
                int.TryParse(val, out ret);
            }
            return ret;
        }

        public bool Push(ref ExchangeChance<T> ec,bool noNeedProcessTable,bool isSecurity,bool debugOrTest)
        {
            lock (ed)
            {
                if (dtp.IsSecurityData == 0)
                {
                    if (ec.OwnerChance == null) return false;
                    if (ec.OccurStrag == null) return false;
                }
                //allChances.Add(ec.OwnerChance.GUID,ec);
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
                    //LogableClass.ToLog("风险","剩余金额不足", ec.OwnerChance.ToDetailString());
                    //CurrMoney += InitCash;
                    if (dtp.IsSecurityData == 1)
                    {
                        return false;
                    }
                }
                if (ec.ExchangeAmount * ec.OwnerChance.ChipCount * 10 > CurrMoney)//记录投入资金超出10%的记录
                {
                    if(!debugOrTest)
                        LogableClass.ToLog("风险",string.Format("大投入组合[{0}]", (double)100 * ec.ExchangeAmount * ec.OccurStrag.ChipCount / CurrMoney), ec.OwnerChance.ToDetailString());
                }
                if (!noNeedProcessTable)
                {
                    if (isSecurity)
                        ec = ed.AddSecurityChance(ec);
                    else
                        ec = ed.AddAChance(ec);
                }
                CurrMoney -= ec.ExchangeAmount * ec.OwnerChance.ChipCount;
                if(dtp.IsSecurityData==1)//如果是证券类型，资产价值以当前价格表示
                {
                    CurrAssetValue = getAsset();//+= ec.ExchangeAmount * ec.OwnerChance.ChipCount;
                    AssetCount = getAssetCount();
                }
                //MoneyLine.Add(CurrMoney);
                ExchangeData exd = new ExchangeData();
                exd.currCash = CurrMoney;
                exd.currAsset = CurrAssetValue;
                exd.benchMark = benchMarkVal;
                //exd.currTotal = exd.currAsset + exd.currCash;
                if (ExpectMoneyLine.ContainsKey(ec.ExExpectNo))
                {
                    
                    ExpectMoneyLine[ec.ExExpectNo] = exd;
                }
                else
                {
                    ExpectMoneyLine.TryAdd(ec.ExExpectNo, exd);
                }
                return true;
            }
        }

        public bool UpdateSecurity(ExchangeChance<T> ec,bool noNeedUpdateTable = false,string status=null, double Gained = 0.0)
        {
            lock(ed)
            {
                SecurityChance<T> cc = ec.OwnerChance as SecurityChance<T>;
                
                if (!noNeedUpdateTable)
                {
                    double tGained = 0.0;
                    bool suc = ed.UpdateSecurityChance(ec, out tGained,status);
                    if (suc == false)
                    {
                        return suc;
                    }
                    CurrMoney += tGained;
                    CurrAssetValue = tGained;
                    //exd.benchMark = benchMarkVal;
                }
                else //如果不变更表，不变动现金，只计算资产的变动
                {
                    double tGained = 0;
                    ed.UpdateSecurityChance(ec,out tGained, status);
                    CurrAssetValue = getAsset(); ;
                    AssetCount = getAssetCount();
                }
                //allChances.Remove(ec.OwnerChance.GUID);
                ExchangeData exd = new ExchangeData();
                exd.currCash = CurrMoney;
                exd.currAsset = CurrAssetValue;
                exd.benchMark = benchMarkVal;
                //exd.currTotal = exd.currAsset + exd.currCash;
                //MoneyLine.Add(CurrMoney);
                if (ExpectMoneyLine.ContainsKey(ec.ExExpectNo))
                {
                    ExpectMoneyLine[ec.ExExpectNo] = exd;
                }
                else
                {
                    ExpectMoneyLine.TryAdd(ec.ExExpectNo, exd);
                }
                return true;
            }
            return true;
        }

        public bool Update(ExchangeChance<T> ec, bool onlyModify=false)
        {
            lock (ed)
            {
                double Gained = 0.0;
                bool suc = ed.UpdateChance(ec, out Gained);
                if(suc == false)
                {
                    return suc;
                }
                //allChances.Remove(ec.OwnerChance.GUID);
                CurrMoney += Gained;
                //MoneyLine.Add(CurrMoney);
                ExchangeData exd = new ExchangeData();
                exd.currCash = CurrMoney;
                exd.currAsset = CurrAssetValue;
                exd.benchMark = benchMarkVal;
                //exd.currTotal = exd.currAsset + exd.currCash;
                if (ExpectMoneyLine.ContainsKey(ec.ExExpectNo))
                {
                    ExpectMoneyLine[ec.ExExpectNo] = exd;
                }
                else
                {
                    ExpectMoneyLine.TryAdd(ec.ExExpectNo , exd);
                }
                return suc;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            
        }

        /// <summary>
        /// 当前汇总=当前现金+当前资产
        /// </summary>
        public ExchangeData summary
        {
            get
            {
                ExchangeData exd = new ExchangeData();
                exd.currAsset = CurrAssetValue;
                exd.currCash = CurrMoney;
                exd.benchMark = benchMarkVal;
                //exd.currTotal = CurrAssetValue + CurrMoney;
                return exd;
            }
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
                    return (100.00 * (getAsset()+CurrMoney - _InitCash) / _InitCash).ToEquitPrice(2);
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
                    return Math.Round(100 * (ExpectMoneyLine.Values.Max(a=>a.currTotal) - _InitCash) / _InitCash, 2);
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
                    return Math.Round(100 * (ExpectMoneyLine.Values.Min(a=>a.currTotal) - _InitCash) / _InitCash, 2);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public ConcurrentDictionary<string,ExchangeData> getMoneys()
        {
            return ExpectMoneyLine;
        }
        ReaderWriterLockSlim LockSlim = new ReaderWriterLockSlim();
        public DataTable MoneyIncreamLine
        {

            get
            {
                if(ExpectMoneyLine == null)
                {
                    ExpectMoneyLine = new ConcurrentDictionary<string, ExchangeData>();
                }
                
                try
                {
                    //LockSlim.EnterWriteLock();
                    if (MoneyChangeTable == null || MoneyChangeTable.Rows.Count < ExpectMoneyLine.Count)
                    {
                        double assetVal = 0;
                        if (MoneyChangeTable==null)
                        {
                            MoneyChangeTable = new DataTable();
                            if(dtp.IsSecurityData==0)
                                MoneyChangeTable.Columns.Add("id",typeof(long));
                            else
                                MoneyChangeTable.Columns.Add("id", typeof(string));
                            MoneyChangeTable.Columns.Add("total", typeof(double));
                            MoneyChangeTable.Columns.Add("val", typeof(double));
                            MoneyChangeTable.Columns.Add("cash", typeof(double));
                            MoneyChangeTable.Columns.Add("asset", typeof(double));
                            MoneyChangeTable.Columns.Add("BenchMarkVal", typeof(double));
                            MoneyChangeTable.Columns.Add("BenchMark", typeof(double));
                        }
                        else
                        {
                            MoneyChangeTable.Rows.Clear();
                        }
                        var items = ExpectMoneyLine.OrderBy(a => a.Key);
                        double firstVal = items.First().Value.benchMark;
                        foreach (var item in items)
                        {
                            DataRow dr = MoneyChangeTable.NewRow();
                            string strkey = item.Key;
                            if (dtp.IsSecurityData == 0)
                                dr["id"] = long.Parse(strkey);
                            else
                                dr["id"] = strkey;
                            dr["total"] = ExpectMoneyLine[strkey].currTotal;
                            dr["val"] = (100 * (ExpectMoneyLine[strkey].currTotal - _InitCash) / _InitCash).ToEquitPrice(2);
                            dr["cash"] = (100 * (ExpectMoneyLine[strkey].currCash ) / _InitCash).ToEquitPrice(2);
                            dr["asset"] = (100 * (ExpectMoneyLine[strkey].currAsset) / _InitCash).ToEquitPrice(2);
                            dr["BenchMarkVal"] = ExpectMoneyLine[strkey].benchMark;
                            dr["BenchMark"] = 100 * (ExpectMoneyLine[strkey].benchMark - firstVal) / firstVal;
                            MoneyChangeTable.Rows.Add(dr);
                        }
                    }
                    
                }
                catch(Exception ce)
                {

                }
                finally
                {
                    
                    //LockSlim.ExitWriteLock();
                }
                return MoneyChangeTable;
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

    public struct ExchangeData
    {
        public double benchMark;
        public double currCash;
        public double currAsset;
        public double currTotal
        {
            get
            {
                return currCash + currAsset;
            }
        }
    }
}
