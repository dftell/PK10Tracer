using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WAPIWrapperCSharp;
using CFZQ_LHProcess;
using System.Data;
using System.Reflection;
namespace StrategyLibForWD
{
    public class CommWDToolClass
    {
        public static bool IsSecIndex(WindAPI w,string strName,DateTime EndT, out string[] seclist,out string[] secnames)
        {
            seclist = new string[0];
            secnames = new string[0];
            if (strName == null || strName.Trim().Length == 0) return false;
            
            SecIndexClass sic = new SecIndexClass(strName);
            secIndexBuilder sib = new secIndexBuilder(w,sic);
            MTable mtb = sib.getBkList(EndT);
            if (mtb.Count == 0)
            {
                return false;
            }

            //throw (new Exception(mtb.ToRowData(0)));
            seclist = mtb.ToList<string>("wind_code").ToArray();
            secnames = mtb.ToList<string>("sec_name").ToArray();
            return true;
        }
        /// <summary>
        /// 根据指数获得未停牌股票
        /// </summary>
        /// <param name="indexName">板块/指数组合</param>
        /// <param name="EndT"></param>
        /// <param name="CheckDays"></param>
        /// <param name="NextDay"></param>
        /// <param name="ExcludeSt"></param>
        /// <param name="MAFilter"></param>
        /// <param name="ExcludeSecList"></param>
        public static BaseDataTable GetMarketsStocks(WindAPI w, string indexName, DateTime EndT, Int64 CheckDays, bool NextDay, bool ExcludeSt, bool MAFilter, List<string> ExcludeSecList)
        {
            BaseDataTable ret = new BaseDataTable();
            string[] bkArr = indexName.Split(';');
            string[] seclist = new string[0];//默认股票列表为空
            string[] secnames = new string[0];//默认股票列表为空
            if (bkArr.Length > 1)//板块或指数组合
            {
                return ret;
            }
            else
            {
                if (!IsSecIndex(w, indexName, EndT, out seclist,out secnames))//不是集合
                {
                    if (indexName.IndexOf(".") > 0)//如果是0000xx.xx类型
                    {
                        seclist = new string[1] { indexName };
                    }
                    else
                    {
                        return ret;
                    }
                }
            }
            //ret.Result.AddColumnByArray<string>("Code", seclist);
            //ret.Result.AddColumnByArray<string>("Name", secnames);
            BaseDataTable dt = GetBaseData(w, seclist, EndT, Cycle.Day, PriceAdj.Beyond, new object[0] { });
            
            dt = dt.AvaliableData;
            BaseDataTable rdt = new BaseDataTable();
            rdt = new BaseDataTable(dt.GetTable().Clone());
            var Tarr = from dr in dt.ToFillableList<BaseDataItemClass>()
                       where dr.OnMarketDayCount >= CheckDays
                       select dr;
            rdt.FillByItems<BaseDataItemClass>(Tarr.ToArray<BaseDataItemClass>());
            ret = rdt;
            if (ExcludeSt)
            {
                var NoSTArr = from dr in rdt.ToFillableList<BaseDataItemClass>()
                              where dr.IsST == false
                              select dr;

                ret = new BaseDataTable(rdt.GetTable());
                ret.FillByItems<BaseDataItemClass>(NoSTArr.ToArray<BaseDataItemClass>());
            }
            return ret;
        }
       
        public static BaseDataTable GetBaseSerialData(WindAPI w, string code, DateTime begt, DateTime endt, Cycle cyc, PriceAdj prcAdj,params object[] args)
        {
            RunNoticeClass ret = new RunNoticeClass();
            //MACDGuidProcess mp = new MACDGuidProcess(gb.w);
            //RunResultClass ret = mp.getDateSerialResult("000100.SZ",Convert.ToDateTime("2017/7/7"), DateTime.Today);
            BaseDataProcess bp = new BaseDataProcess(w,cyc,prcAdj);
            RunResultClass bret = bp.getDateSerialResult(code,begt,endt,args);
            if (!bret.Notice.Success)
            {
                return new BaseDataTable();
            }
            return new BaseDataTable(bret.Result);
        }

        public static BaseDataTable GetBaseSerialData(WindAPI w, string code, DateTime begt, DateTime endt, Cycle cyc, PriceAdj prcAdj)
        {
            return GetBaseSerialData(w, code, begt, endt, cyc, prcAdj, new object[0]{});
        }

        public static BaseDataTable GetBaseSerialData(WindAPI w, string code, DateTime begt, DateTime endt)
        {
            return GetBaseSerialData(w, code, begt, endt, Cycle.Day, PriceAdj.Beyond);
        }


        public static BaseDataTable GetBaseData(WindAPI w, string[] codes,  DateTime endt, Cycle cyc, PriceAdj prcAdj,params object[] args)
        {
            RunNoticeClass ret = new RunNoticeClass();
            //MACDGuidProcess mp = new MACDGuidProcess(gb.w);
            //RunResultClass ret = mp.getDateSerialResult("000100.SZ",Convert.ToDateTime("2017/7/7"), DateTime.Today);
            BaseDataProcess bp = new BaseDataProcess(w, cyc, prcAdj);
            RunResultClass bret = bp.getSetDataResult(codes, endt,args);
            if (!bret.Notice.Success)
            {
                return new BaseDataTable();
            }
            return new BaseDataTable(bret.Result);
        }
        
        public static BaseDataTable GetBaseData(WindAPI w, string codes, DateTime endt, Cycle cyc, PriceAdj prcAdj, params object[] args)
        {
            return GetBaseData(w, codes.Split(','), endt, cyc,prcAdj, args);
        }

        public static MTable GetMutliSerialData(WindAPI w, string code, DateTime begt, DateTime endt, Cycle cyc, PriceAdj prcAdj,bool IncludeBaseData, string args)
        {
            
            RunNoticeClass ret = new RunNoticeClass();
            MTable mtab = new MTable();
            if (IncludeBaseData)
            {
                BaseDataProcess bp = new BaseDataProcess(w, cyc, prcAdj);
                RunResultClass bret = bp.getDateSerialResult(code, begt, endt, new object[0] { });
                if (!bret.Notice.Success)
                {
                    mtab.Union(bret.Result);
                    //return new BaseDataTable();
                }
            }
            Dictionary<string, HashSet<string>> guids = getMutliValueGuid(args.Split(','));
            foreach (string key in guids.Keys)
            {
                MutliValueGuidProcess cgp = new MutliValueGuidProcess(w,key,guids[key].ToArray<string>());
                RunResultClass cret = cgp.getDateSerialResult(code, begt, endt, new object[0] { });
               
            }
            return mtab;
        }

        public static MTable GetMutliSetData(WindAPI w, string[] code, DateTime endt, Cycle cyc, PriceAdj prcAdj, bool IncludeBaseData, string args)
        {

            RunNoticeClass ret = new RunNoticeClass();
            MTable mtab = new MTable();
            if (IncludeBaseData)
            {
                BaseDataProcess bp = new BaseDataProcess(w, cyc, prcAdj);
                RunResultClass bret = bp.getSetDataResult(code,  endt, new object[0] { });
                if (!bret.Notice.Success)
                {
                    mtab.Union(bret.Result);
                    //return new BaseDataTable();
                }
            }
            Dictionary<string, HashSet<string>> guids = getMutliValueGuid(args.Split(','));
            foreach (string key in guids.Keys)
            {
                MutliValueGuidProcess cgp = new MutliValueGuidProcess(w, key, guids[key].ToArray<string>());
                cgp.cycle = cyc;
                cgp.prcAdj = prcAdj;
                RunResultClass cret = cgp.getSetDataResult(code,  endt, new object[0] { });
                mtab.Union(cret.Result);
            }
            return mtab;
        }

        public static MTable getBkList(WindAPI w, string sec, DateTime dt)
        {
            return getBkList(w, sec, dt, true);
        }
        public static MTable getBkList(WindAPI w, string sec, DateTime dt,bool FilterTheNoPrice)
        {

            SecIndexClass sic = new SecIndexClass(sec);
            secIndexBuilder sib = new secIndexBuilder(w, sic);
            MTable mt = sib.getBkList(dt);
            BaseDataProcess bdp = new BaseDataProcess(w);
            string[] seccodes = mt["wind_code"].ToList<string>().ToArray();
            RunResultClass rc = bdp.getSetDataResult(seccodes, "close", dt);
            MTable ret = new MTable();
            List<DataRow> NewDrs = new List<DataRow>();
            for (int i = 0; i < rc.Result.Count; i++)
            {
                DataRow retdr = mt.GetTable().Rows[i];
                DataRow dr = rc.Result.GetTable().Rows[i];
                if (dr.IsNull("CLOSE") && FilterTheNoPrice) 
                    continue;
                NewDrs.Add(retdr);
            }
            ret.FillList(NewDrs.ToArray());
            return ret;
        }

        static Dictionary<string, HashSet<string>> getMutliValueGuid(params string[] args)
        {
            Dictionary<string, HashSet<string>> guides = new Dictionary<string, HashSet<string>>();
            var vals = from obj in args
                      let GuidName= obj.Split('.')[0]
                      let SubGuid = obj.Split('.')[1]
                      select new{GuidName,SubGuid};
            foreach (var key in vals)
            {
                HashSet<string> subGuids = null;
                if (!guides.ContainsKey(key.GuidName))
                {
                    subGuids = new HashSet<string>();
                    guides.Add(key.GuidName, subGuids);
                }
                subGuids = guides[key.GuidName];
                if (!subGuids.Contains(key.SubGuid))
                {
                    subGuids.Add(key.SubGuid);
                }
            }
            return guides;
        }
    }

    public class PrintClass
    {
        public string ToClassInfo()
        {
            Type myinfo = this.GetType();//BindingFlags.Public|BindingFlags.GetField|BindingFlags.Instance
            var v = from n in myinfo.GetFields()
                    let strItem=string.Format("{0}={1}",n.Name,n.GetValue(this))
                    select strItem;
            return string.Join(";", v.ToArray<string>());
        }
    }
    
    public class BaseDataItemClass : PrintClass, iFillable
    {
        WindAPI w;
        /// <summary>
        /// 万得内部名称，专供指数顶类
        /// </summary>
        public string SecCode;
        public void AllowBuild(WindAPI _w)
        {
            w = _w;
        }
        public DateTime DateTime;
        public string WindCode,Sec_Name;
        public string riskadmonition_date;//摘帽时间

        public bool IsST
        {
            get
            {
                return IsSTInDay(DateTime) > 0;
            }
        }

        public int IsSTInDay(DateTime endt)
        {
            if (this.riskadmonition_date == null) //未被初始化
            {
                return -1;//不知道
            }
            else
            {
                if (this.riskadmonition_date.Length == 0)//从未被ST过
                {
                    return 1;
                }
                else
                {
                    string[] res = this.riskadmonition_date.Split(',');
                    var vv = from n in res
                             let ST = n.Split('：')[0].Trim()
                             let Date = n.Split('：')[1].Trim()
                            select new { ST, Date };
                    foreach (var key in vv)
                    {
                        string st = key.ST + key.Date;
                    }
                    var ss = from n in vv
                             where str2Date(n.Date) < endt
                             orderby n.Date descending
                             select n.ST; //比比较日期小的戴帽摘帽信息按日期倒序
                    if (ss.Count<string>() ==0 ) //如果不存在
                    {
                        return 0;
                    }
                    else //如果是去，就是摘帽，非ST
                    {
                        string str = ss.First<string>();
                        if (str.Substring(0, 1) == "去")//刚去
                        {
                            return 0;
                        }

                        return 1;
                    }
                }
            }  
        }

        public bool ExchangeNormal
        {
            get
            {

                if (this.SecType == SecType.Equit)
                {
                    if (this.trade_status == null)
                    {
                        return false;
                    }
                    if (this.trade_status.Trim() == "交易")
                        return true;
                    else
                        return false;
                }
                else if(this.SecType == StrategyLibForWD.SecType.Index)
                {
                    if (float.IsNaN((float)this.Close) || float.IsInfinity((float)this.Close))//指数的收盘价为空，返回状态为非正常
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// 自然日数，非交易日
        /// </summary>
        

        public DateTime str2Date(string str)
        {
            return Convert.ToDateTime(string.Format("{0}/{1}/{2}", str.Substring(0, 4), str.Substring(4, 2), str.Substring(6, 2)));
        }

        public DateTime Ipo_date;
        public decimal Open, High, Low, Close, Volume, pct_chg;
        public string trade_status;//交易
        public int susp_days, maxupordown;
        public string sec_type;
        public SecType SecType
        {
            get
            {
                if (sec_type == "普通股")
                {
                    return SecType.Equit;
                }
                else
                {
                    if (sec_type.IndexOf("指数") > 0)
                    {
                        return SecType.Index;
                    }
                }
                return SecType.Other;
            }
        }
        public BaseDataItemClass()
        {
            

        }

        public BaseDataItemClass(DataRow dr)
        {

            FillByDataRow(dr);
        }
        
        public void FillByDataRow(DataRow dr)
        {
            Type MyInfo = this.GetType();
            foreach (FieldInfo n in MyInfo.GetFields())
            {
                if (dr.Table.Columns.Contains(n.Name.ToUpper()))
                {
                    if (dr[n.Name.ToUpper()] != null && dr[n.Name.ToUpper()] != DBNull.Value)
                        n.SetValue(this, Convert.ChangeType(dr[n.Name.ToUpper()], n.FieldType));
                }
            }
        }

        public DataRow FillRow(DataRow dr)
        {
            Type MyInfo = this.GetType();
            foreach (FieldInfo n in MyInfo.GetFields())
            {
                if (dr.Table.Columns.Contains(n.Name.ToUpper()))
                    dr[n.Name.ToUpper()] = Convert.ChangeType(n.GetValue(this),n.FieldType);
            }
            return dr;
        }

        DateTime[] OnMarketDays=new DateTime[0]{};
        
        /// <summary>
        /// 上市交易日数，不包括停牌，且是大约数。
        /// </summary>
        public int OnMarketDayCount
        {
            get
            {
                if (OnMarketDays.Length == 0)
                {
                    if(this.SecType == SecType.Equit)
                    {
                        //OnMarketDays =  WDDayClass.getTradeDates(w,this.Ipo_date,this.DateTime);
                        return this.DateTime.Subtract(this.Ipo_date).Days * 5 / 7;//OnMarketDays.Length;
                    }
                }
                return 99999;
            }
        }
    }

    public class BaseDataTable:MTable
    {
        public BaseDataTable()
        {
        }

        public BaseDataTable(DataTable dt):base(dt)
        {
        }

        public BaseDataTable(MTable tb)
        {
            if(tb==null||tb.GetTable() == null) throw(new Exception("数据异常！"));
            this.Table = tb.GetTable().Copy();
        }

        public override iFillable this[int id]
        {
            get
            {
                if (id >= this.Table.Rows.Count) return new BaseDataItemClass();
                return new BaseDataItemClass(this.Table.Rows[id]);
            }
        }

        

        protected BaseDataTable _AvaliableData;//剔除非交易日期
        /// <summary>
        /// 交易日数据
        /// </summary>
        public BaseDataTable AvaliableData
        {
            get
            {
                if (_AvaliableData == null)
                {
                    if (this.Count > 0)
                    {
                        _AvaliableData = new BaseDataTable(this.tTable.Clone()); //必须要复制结构
                        var vv = from item in this.ToFillableList<BaseDataItemClass>()
                                 where item.ExchangeNormal
                                 select item;
                        BaseDataItemClass[] bics = vv.ToArray<BaseDataItemClass>();
                        _AvaliableData.FillByItems<BaseDataItemClass>(bics);
                    }
                }
                return _AvaliableData;
            }
        }

    }

    public class WDDayClass
    {
        public static DateTime[] getTradeDates(WindAPI w, DateTime begt, DateTime endt, Cycle cyc)
        {
            TDaysGuidClas tgc = new TDaysGuidClas();
            tgc.cycle = cyc;
            TDayGuildBuilder tgb = new TDayGuildBuilder(w,tgc);
            MTable ret = tgb.getRecords(begt, endt);
            return ret.ToList<DateTime>().ToArray();
        }

        public static DateTime[] getTradeDates(WindAPI w, string SecCode, DateTime begt, DateTime endt, Cycle cyc)
        {
            BaseDataTable tb = CommWDToolClass.GetBaseSerialData(w, SecCode, begt, endt, cyc, PriceAdj.UnDo, BaseDataPoint.trade_status, BaseDataPoint.sec_type, BaseDataPoint.close);
            BaseDataTable ttb = tb.AvaliableData;
            return ttb["DateTime"].ToList<DateTime>().ToArray();
        }

        public static DateTime[] getTradeDates(WindAPI w, DateTime begt, DateTime endt)
        {
            return getTradeDates(w, begt, endt, Cycle.Day);
        }

        public static int getTradeDays(WindAPI w, DateTime begt, DateTime endt)
        {
            TDaysGuidClas tgc = new TDaysGuidClas();
            TDayGuildBuilder tgb = new TDayGuildBuilder(w, tgc);
            MTable ret = tgb.getRecordsCount(begt, endt);
            return (int)ret.GetTable().Rows[0][0];
        }

        public static DateTime Offset(WindAPI w, DateTime  endt,int N,Cycle cyc)
        {
            TDaysGuidClas tgc = new TDaysGuidClas();
            tgc.cycle = cyc;
            TDayGuildBuilder tgb = new TDayGuildBuilder(w, tgc);
            MTable ret = tgb.getRecords(endt,N);
            DateTime lastdate = Convert.ToDateTime(ret.GetTable().Rows[0][0]);
            if(cyc == Cycle.Day) return lastdate;
            DateTime[] dates = getTradeDates(w, lastdate, endt);
            for (int i = 0; i < dates.Length-1; i++)
            {
                if (dates[i].AddDays(1).CompareTo(dates[i + 1]) < 0)
                {
                    return dates[i];
                }
            }
            return lastdate;

        }

        public static DateTime Offset(WindAPI w, BaseDataItemClass SecItem, DateTime dt, int N, Cycle cyc)
        {
            DateTime begt = SecItem.Ipo_date;
            if(SecItem.SecType == SecType.Index)
            {
                //指数不停牌，自然日按交易日*7/5，放大到10/2=2倍指定开始日
                switch(cyc)
                {
                    case Cycle.Day:
                        {
                            begt =  dt.AddDays(-2*N);
                            break;
                        }
                    case Cycle.Week:
                        {
                            begt = dt.AddDays(-7*N);
                            break;
                        }
                    case Cycle.Month:
                        {
                            begt = dt.AddMonths(N);
                            break;
                        }
                    case Cycle.Year:
                        {
                            begt = dt.AddYears(N);
                            break;
                        }
                }
            
            }
            if(begt.CompareTo(SecItem.Ipo_date)<0)//任何情况（index的ipo日期为1899-12-31）如果开始日期小于ipo日期，开始日期设置为Ipo日期。
            {
                begt = SecItem.Ipo_date;
            }

            
            BaseDataTable tb = CommWDToolClass.GetBaseSerialData(
                w, 
                SecItem.WindCode,
                begt,
                dt, 
                Cycle.Day, 
                PriceAdj.UnDo, 
                BaseDataPoint.trade_status,BaseDataPoint.sec_type);
            BaseDataTable ttb = tb.AvaliableData;
            if(ttb.Count == 0) return dt;//如果数据为空，返回回览日
            if (ttb.Count >N) return ((BaseDataItemClass)ttb[ttb.Count -N]).DateTime;
            return ((BaseDataItemClass)ttb[0]).DateTime;
        }
        /// <summary>
        /// 市场最后交易日
        /// </summary>
        /// <param name="w"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime LastTradeDay(WindAPI w, DateTime dt, Cycle cy)
        {
            DateTime[] ret = getTradeDates(w, dt.AddDays(-20), dt,cy);
            if (ret.Length > 0)
            {
                return ret[ret.Length - 1];
            }
            return DateTime.MinValue;
        }

        public static DateTime LastTradeDay(WindAPI w, DateTime dt)
        {
            return LastTradeDay(w, dt, Cycle.Day);
        }

        public static DateTime LastTradeDay(WindAPI w, string SecCode, DateTime dt)
        {
            BaseDataTable tb =  CommWDToolClass.GetBaseSerialData(w, SecCode, dt.AddDays(-1000), dt, Cycle.Day, PriceAdj.UnDo,BaseDataPoint.trade_status,BaseDataPoint.sec_type,BaseDataPoint.close);
            BaseDataTable ttb = tb.AvaliableData;
            if (ttb.Count == 0) return DateTime.MinValue;
            return ((BaseDataItemClass)ttb[ttb.Count - 1]).DateTime;
        }
    }
}
