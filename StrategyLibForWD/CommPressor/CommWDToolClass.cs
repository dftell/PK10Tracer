using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WAPIWrapperCSharp;
using WolfInv.com.CFZQ_LHProcess;
using System.Data;
using System.Reflection;
using WolfInv.com.SecurityLib;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;

namespace WolfInv.com.StrategyLibForWD
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
            BaseDataProcess_ForWD bp = new BaseDataProcess_ForWD(w,cyc,prcAdj);
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
            BaseDataProcess bp = new BaseDataProcess_ForWD(w,cyc, prcAdj);
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
                BaseDataProcess bp = new BaseDataProcess_ForWD(w, cyc, prcAdj);
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
                MutliValueGuidProcess_ForWD cgp = new MutliValueGuidProcess_ForWD(w,key,guids[key].ToArray<string>());
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
                BaseDataProcess bp = new BaseDataProcess_ForWD(w, cyc, prcAdj);
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
                MutliValueGuidProcess_ForWD cgp = new MutliValueGuidProcess_ForWD(w, key, guids[key].ToArray<string>());
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
            if (mt == null || mt.Count == 0)
                return mt;
            BaseDataProcess_ForWD bdp = new BaseDataProcess_ForWD(w);
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
}
