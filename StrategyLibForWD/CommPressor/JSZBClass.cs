using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CFZQ_LHProcess;
using WAPIWrapperCSharp;
using System.Reflection;
using Microsoft.VisualBasic;
namespace StrategyLibForWD
{
    public enum BaseDataPoint
    {
        windcode, sec_name, riskadmonition_date, ipo_date, open, high, low, close, volume, trade_status, susp_days, maxupordown, sec_type, pct_chg
    }

    public enum SecType 
    {
        Equit,Index,Fund,Bond,Other
    }

    /// <summary>
    /// 技术指标基类
    /// </summary>
    public abstract class GuidBaseClass
    {
        public HashSet<string> Fields;
        protected string strParamStyle;
        public DateTime tradeDate;
        string _strFiled;
        public String GuidName
        {
            get
            {
                if (Fields == null)
                    return _strFiled;
                return _strFiled ?? string.Join(",", Fields.ToArray<string>());
            }
            set
            {
                _strFiled = value;
            }
        }
        public Cycle cycle;
        public PriceAdj priceAdj;
        protected string _strParam;
        public string strParam{get{return getParamString();}}
        public abstract string getParamString();
        public GuidBaseClass()
        {
        }
    }

    public class CommDataBuilder : WDBuilder
    {
        ////public PriceAdj prcAdj;
        ////public Cycle cycle;
        protected GuidBaseClass gbc;
        protected string strParamsStyle;
        public CommDataBuilder(WindAPI w) : base(w) { }
        public CommDataBuilder(WindAPI _w, GuidBaseClass guidClass)
            : base(_w)
        {
            gbc = guidClass;
        }
    }

    public class TDayGuildBuilder : CommDataBuilder
    {
        public TDayGuildBuilder(WindAPI _w, GuidBaseClass guidClass)
            : base(_w, guidClass)
        {
            strParamsStyle = "Period={0};";//Days=Weekdays
        }

        public MTable getRecords(DateTime begdt, DateTime enddt)
        {
            if (gbc == null) return null;
            WTDaysClass wsetobj;
            wsetobj = new WTDaysClass(w, begdt, enddt, string.Format(strParamsStyle, gbc.cycle.ToString().Substring(0,1)));
            return WDDataAdapter.getTable(wsetobj.getDataSet());
        }

        public MTable getRecords(DateTime endt, int N)
        {
            if (gbc == null) return null;
            WTDaysOffsetClass wsetobj;
            wsetobj = new WTDaysOffsetClass(w, endt, N, string.Format(strParamsStyle, gbc.cycle.ToString().Substring(0, 1)));
            return WDDataAdapter.getTable(wsetobj.getDataSet());
        }

        public MTable getRecordsCount(DateTime begdt, DateTime enddt)
        {
            if (gbc == null) return null;
            WTDaysCountClass wsetobj;
            wsetobj = new WTDaysCountClass(w, begdt, enddt);
            return WDDataAdapter.getTable(wsetobj.getDataSet());
        }
    }

    /// <summary>
    /// 日期序列指标工厂类
    /// </summary>
    public class DateSerialGuidBuilder : CommDataBuilder
    {
        //WindData wd = w.wsd("600011.SH", "MACD", "2017-02-05", "2018-03-06", "MACD_L=26;MACD_S=12;MACD_N=9;MACD_IO=1;Fill=Previous");
        public DateSerialGuidBuilder(WindAPI _w, GuidBaseClass guidClass):base(_w,guidClass)
        {
            strParamsStyle = "priceAdj={1};Period={2};Fill=Previous;{0}";
         }
        public MTable getRecords(string Sector, DateTime begdt, DateTime enddt)
        {
            if (gbc == null) return null;
            WSDClass wsetobj;
            wsetobj = new WSDClass(w,Sector,gbc.GuidName,begdt,enddt,string.Format(strParamsStyle,gbc.strParam,gbc.priceAdj.ToString().Substring(0,1),gbc.cycle.ToString().Substring(0,1)));
            return WDDataAdapter.getTable(wsetobj.getDataSet(),gbc.GuidName,typeof(decimal));
        }
    }

    /// <summary>
    /// 多维指标工厂类
    /// </summary>
    public class GuidBuilder : CommDataBuilder
    {
        public GuidBuilder(WindAPI _w, GuidBaseClass guidClass):base(_w,guidClass)
        {
            strParamsStyle = "tradeDate={0};priceAdj={2};cycle={3};{1}";
        }
        public MTable getRecords(string[] Sectors, DateTime dt)
        {
            if (gbc == null) return null;
            gbc.tradeDate = dt;
            WSSClass wsetobj;
            wsetobj = new WSSClass(w,string.Join(",", Sectors),gbc.GuidName,string.Format(strParamsStyle,dt.ToShortDateString().Replace("-",""),gbc.strParam,gbc.priceAdj.ToString().Substring(0,1),gbc.cycle.ToString().Substring(0,1)));
            return WDDataAdapter.getTable(wsetobj.getDataSet(), gbc.GuidName, typeof(decimal));
        }
    }

    /// <summary>
    /// 多返回值指标类
    /// </summary>
    public abstract class MutliReturnValueGuidClass : GuidBaseClass
    {
        int IOType;
        public string ReturnValueName;
        protected abstract int getIOValue();
        protected abstract void InitClass();
    }

    /// <summary>
    /// KDJ指标类
    /// </summary>
    public class KDJGuidClass : MutliReturnValueGuidClass
    {
        int iN=9, iM1=3, iM2=3, iIO=3;
        protected override void InitClass()
        {
            GuidName = "KDJ";
            strParamStyle = "KDJ_N={0};KDJ_M1={1};KDJ_M2={2};KDJ_IO={3}";
        }
        public KDJGuidClass()
        {
            InitClass();
        }
        public KDJGuidClass(int N, int M1, int M2)
        {
            InitClass();
            iN = N;
            iM1 = M1;
            iM2 = M2;

        }
        public override string getParamString()
        {
            return string.Format(strParamStyle,iN,iM1,iM2,getIOValue());
        }

        protected override int getIOValue()
        {
            return this.ReturnValueName == "K" ? 1 : this.ReturnValueName == "D" ? 2 : 3;
        }
    }

    public class TDaysGuidClas : GuidBaseClass
    {

        public override string getParamString()
        {
            return "";
        }
    }

    public enum MACDType{DIFF,DEA,MACD}

    /// <summary>
    /// MACD指标类
    /// </summary>
    public class MACDGuidClass : MutliReturnValueGuidClass
    {
        int iL=26, iS=12, iN=9, iIO=3;
        protected override void InitClass()
        {
            GuidName = "MACD";
            strParamStyle = "MACD_L={0};MACD_S={1};MACD_N={2};MACD_IO={3};";
        }
        public MACDGuidClass()
        {
            InitClass();
        }
        public MACDGuidClass(int N, int M1, int M2)
        {
            InitClass();
            iL = N;
            iS = M1;
            iN = M2;
        }
        
        public override string getParamString()
        {
            string sCycle = cycle.ToString().Substring(0,1);
            string sPriceAdj = priceAdj.ToString().Substring(0, 1);
            return string.Format(strParamStyle, iL, iS, iN, getIOValue());
        }

        protected override int getIOValue()
        {
            return this.ReturnValueName == "DIFF" ? 1 : this.ReturnValueName == "DEA" ? 2 : 3;
        }
    }

    public class BaseDataPointGuidClass : GuidBaseClass
    {
        //无指标名称
        
        public BaseDataPointGuidClass(params BaseDataPoint[] args)
        {
            this.Fields = new HashSet<string>();
            for (int i = 0; i < args.Length; i++)
                this.Fields.Add(args[i].ToString());
        }

        public BaseDataPointGuidClass(string strNames)
        {
            this.Fields = new HashSet<string>();
            if (strNames.IndexOf(",") > 0)
            {
                string[] args = strNames.Split(',');
                
                for (int i = 0; i < args.Length; i++)
                {
                    string field = args[i];
                    if (!this.Fields.Contains(field))
                        this.Fields.Add(field);
                }
            }
            else
            {
                this.Fields.Add(strNames);
            }
        }

        public BaseDataPointGuidClass(bool AllBasePoint)
        {
            Type t = typeof(BaseDataPoint);
            FieldInfo[] flds = t.GetFields(BindingFlags.Public | BindingFlags.Static);
            this.Fields = new HashSet<string>();
            for (int i = 0; i < flds.Length; i++)
            {
                if(!this.Fields.Contains(flds[i].Name))
                    this.Fields.Add(flds[i].Name);
            }
        }

        public BaseDataPointGuidClass(params object[] args)
        {
            this.Fields = new HashSet<string>();
            for (int i = 0; i < args.Length; i++)
            {
                string field = args[i].ToString();
                if (args[i].GetType() == typeof(BaseDataPoint))
                {
                    field = args[i].ToString();
                }
                if (!this.Fields.Contains(field))
                    this.Fields.Add(field);
            }
        }

        public override string getParamString()
        {
            return "";
            //throw new NotImplementedException();
        }
    }

    public abstract class CommGuidProcess:WDBuilder
    {
        public PriceAdj prcAdj;
        public Cycle cycle;
        protected CommDataBuilder gbc;
        public CommGuidProcess(WindAPI _w):base(_w)
        {

        }
        public CommGuidProcess(WindAPI _w,CommDataBuilder _gbc, Cycle cyc, PriceAdj rate):base(_w)
        {
            gbc = _gbc;
            cycle = cyc;
            prcAdj = rate;
        }
        /// <summary>
        /// 获得多维数据
        /// </summary>
        /// <param name="secCodes"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public abstract RunResultClass getSetDataResult(string[] secCodes, DateTime dt);
        public abstract RunResultClass getSetDataResult(string[] secCodes, string DataPoints, DateTime dt);
        public abstract RunResultClass getSetDataResult(string secCodes, string DataPoints, DateTime dt);
        public abstract RunResultClass getSetDataResult(string[] secCodes, DateTime dt,params object[] DataPoints);
        public abstract RunResultClass getDateSerialResult(string secCode, DateTime begt,DateTime endt, params object[] DataPoints);
    }

    public class BaseDataProcess : CommGuidProcess
    {
        public BaseDataProcess(WindAPI _w)
            : base(_w)
        { }
        public BaseDataProcess(WindAPI _w,Cycle cyc, PriceAdj rate)
            : base(_w)
        {
            this.cycle = cyc;
            this.prcAdj = rate;
        }

         RunResultClass GetSetBaseData(string[] secCodes, DateTime EndT, params object[] datapointnames)
        {
            RunResultClass ret = new RunResultClass();
            BaseDataPointGuidClass gd = null;
            if (datapointnames.Length == 0)
                gd = new BaseDataPointGuidClass(true);
            else
            {
                gd = new BaseDataPointGuidClass(datapointnames);
            }
            GuidBuilder gb = new GuidBuilder(w, gd);
            gd.cycle = this.cycle;
            gd.priceAdj = this.prcAdj;
            ret.Result = gb.getRecords(secCodes, EndT);
            ret.Result.AddColumnByArray<DateTime>("DateTime",EndT);
            ret.Notice.Success = true;
            return ret;
        }

         RunResultClass GetSetBaseData( string[] secCodes, DateTime EndT)
        {
            return GetSetBaseData(secCodes, EndT, new object[0] { });
        }

        RunResultClass GetSetBaseData(string secCodes, string strFields, DateTime EndT)
        {
            return GetSetBaseData(secCodes.Split(','), EndT, strFields);
        }

        RunResultClass GetSetBaseData( string secCodes, DateTime EndT)
        {
            return GetSetBaseData(secCodes.Split(','), EndT, new object[0] { });
        }

        public override RunResultClass getSetDataResult(string[] secCodes, DateTime dt)
        {
            return this.GetSetBaseData(secCodes, dt);
        }

        public override RunResultClass getSetDataResult(string[] secCodes, string DataPoints, DateTime dt)
        {
            return this.GetSetBaseData(secCodes, dt, DataPoints);
        }

        public override RunResultClass getSetDataResult(string secCodes, string DataPoints, DateTime dt)
        {
            return this.GetSetBaseData(secCodes.Split(','), dt, DataPoints);
        }

        public override RunResultClass getSetDataResult(string[] secCodes, DateTime dt, params object[] DataPoints)
        {
            return this.GetSetBaseData(secCodes, dt,DataPoints);
        }

        public override RunResultClass getDateSerialResult(string secCode, DateTime begt, DateTime endt, params object[] DataPoints)
        {
            RunResultClass ret = new RunResultClass();
            MTable tab = new MTable();
            BaseDataPointGuidClass gd;
            if (DataPoints.Length > 0)
                gd = new BaseDataPointGuidClass(DataPoints);
            else
                gd = new BaseDataPointGuidClass(true);
            gd.cycle = this.cycle;
            gd.priceAdj = this.prcAdj;
            DateSerialGuidBuilder gb = new DateSerialGuidBuilder(w, gd);
            tab = gb.getRecords(secCode, begt, endt);
            ret.Result = tab;
            ret.Notice.Success = true;
            return ret;
        }
    }

    /// <summary>
    /// 多值指标处理类
    /// </summary>
    public class MutliValueGuidProcess : CommGuidProcess
    {
        public string  GuildName;//指标名
        public string[] ValueNames;
        public MutliValueGuidProcess(WindAPI w, string guidName,params string[] args) : base(w) 
        { 
            GuildName = guidName;
            ValueNames = args;
        }

        public override RunResultClass getSetDataResult(string[] secCodes, DateTime dt)
        {
            RunResultClass ret = new RunResultClass();
            MTable tab = new MTable();
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            string ClassName = string.Format("{0}GuidClass", GuildName);
            var val = from t in assembly.GetTypes()
                      where Strings.Right(t.Name,ClassName.Length) == ClassName
                      select t;
            Type ct = null;
            if(val.Count<Type>() == 1)
            {
                ct = val.First<Type>();
            }
            MutliReturnValueGuidClass gd = assembly.CreateInstance(ct.FullName) as MutliReturnValueGuidClass;
            //MACDGuidClass gd = new MACDGuidClass(MACDType.MACD);
            gd.cycle = this.cycle;
            gd.priceAdj = this.prcAdj;
            GuidBuilder gb = null;
            for (int i = 0; i < ValueNames.Length; i++)
            {
                gd.ReturnValueName = ValueNames[i];
                gb = new GuidBuilder(w, gd);
                MTable tmp = gb.getRecords(secCodes, dt);
                tab.AddColumnByArray(ValueNames[i], tmp, GuildName);
            }
            ret.Notice.Success = true;
            ret.Result = tab;
            return ret;
        }

        public override RunResultClass getSetDataResult(string[] secCodes, string DataPoints, DateTime dt)
        {
            return getSetDataResult(secCodes, dt);
        }

        public override RunResultClass getSetDataResult(string secCodes, string DataPoints, DateTime dt)
        {
            return getSetDataResult(secCodes.Split(','), dt);
        }

        public override RunResultClass getSetDataResult(string[] secCodes, DateTime dt, params object[] DataPoints)
        {
            return getSetDataResult(secCodes, dt);
        }

        public override RunResultClass getDateSerialResult(string secCode, DateTime begt, DateTime endt, params object[] DataPoints)
        {
            RunResultClass ret = new RunResultClass();
            MTable tab = new MTable();
            Type t = Type.GetType(GuildName + "GuidClass");
            Assembly assembly = Assembly.GetExecutingAssembly();
            MutliReturnValueGuidClass gd = assembly.CreateInstance(t.Name) as MutliReturnValueGuidClass;
            //MACDGuidClass gd = new MACDGuidClass(MACDType.MACD);
            gd.cycle = this.cycle;
            gd.priceAdj = this.prcAdj;
            DateSerialGuidBuilder gb = null;
            for (int i = 0; i < ValueNames.Length; i++)
            {
                gd.ReturnValueName = ValueNames[i];
                gb = new DateSerialGuidBuilder(w, gd);
                MTable tmp = gb.getRecords(secCode, begt,endt);
                tab.AddColumnByArray(ValueNames[i], tmp, GuildName);
            }
            ret.Notice.Success = true;
            ret.Result = tab;
            return ret;
        }
    }

}

