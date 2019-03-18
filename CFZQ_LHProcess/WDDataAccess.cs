using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WAPIWrapperCSharp;
using System.Data;
using BaseObjectsLib;
namespace CFZQ_LHProcess
{
    public class LogException : Exception, LogLib.iLog
    {
        public string LogName => throw new NotImplementedException();

        public void Log(string logname, string Topic, string msg)
        {
            LogLib.LogableClass.ToLog(logname, Topic, msg);
        }

        public void Log(string Topic, string Msg)
        {
            LogLib.LogableClass.ToLog( Topic, Msg);
        }

        public void Log(string msg)
        {
            LogLib.LogableClass.ToLog( msg);
        }
    }
    public class WDErrorException : LogException
    {
        string _Message;
        public string Message
        {
            get
            {
                return _Message;
            }
        }
        public int WDErrorCode;
        static Dictionary<int,string> ErrList;
        public WDErrorException(WindAPI _w, int ErrorCode)
        {
            this.WDErrorCode = ErrorCode;
            if (ErrList == null) ErrList = new Dictionary<int, string>();
            if (ErrList.ContainsKey(ErrorCode))
            {
                this._Message = ErrList[ErrorCode];
            }
            else
            {
                this._Message= _w.getErrorMsg(ErrorCode);
                ErrList.Add(ErrorCode, this._Message);
            }
        }
     
    }
  
    public class WSSProcess
    {
        public DataTable getRecods(WindAPI w, string SecCodes, string Fields)
        {
            WSSClass wss = new WSSClass(w,SecCodes, Fields);
            return WDDataAdapter.getRecords(wss.getDataSet());
        }
    }
    /// <summary>
    /// 所有运行万得数据类的基类
    /// </summary>
    public class WDBuilder
    {
        public  WindAPI w;
        public WDBuilder(WindAPI _w)
        {
            if (_w == null)
            {
                throw (new Exception("万得接口突然崩溃！"));
                ////_w = new WindAPI();
                ////_w.start();
            }
            w = _w;
        }
    }

    

    public class WDReadClass : WDBuilder
    {

        public WDReadClass(WindAPI _w):base(_w)
        {
            
        }
        protected void InitConnect()
        {
            ////w = new WindAPI();
            ////w.start();
        }
    }
    
    public class WDDateReadClass:WDReadClass
    {
        
        protected string strType;
        protected string strInfo;
        public WDDateReadClass(WindAPI w):base(w)
        {
        }
    }

    public class WSSClass : WDReadClass,iWDReader
    {
        public string _SecCodes;
        public string _Fields;
        public string _Params ;
        string fs
        {
            get
            {
                if (_SelctFields != null && _SelctFields.Count > 0)
                {
                    return string.Join(",", _SelctFields.ToArray<string>());
                }
                return null;
            }
        }

        HashSet<string> _SelctFields;
        public WSSClass(WindAPI _w) : base(_w) { }
        public WSSClass(WindAPI _w,string SecCodes, string Fields):base(_w)
        {
            _Fields = Fields;
            _SecCodes = SecCodes;
            _Params = "";
        }

        public WSSClass(WindAPI _w, string SecCodes, HashSet<string> Fields)
            : base(_w)
        {
            _SelctFields = Fields;
            _SecCodes = SecCodes;
            _Params = "";
        }

        public WSSClass(WindAPI _w, string SecCodes, string Fields, string Params)
            : base(_w)
        {
            _Fields = Fields;
            _SecCodes = SecCodes;
            _Params = Params;
        }

        public WSSClass(WindAPI w, string SecCodes, HashSet<string> Fields, string Params)
            : base(w)
        {
            _SelctFields = Fields;
            _SecCodes = SecCodes;
            _Params = Params;
        }

        #region iWDReader 成员

        public WindData getDataSet()
        {
            WindData wd = w.wss(_SecCodes, fs ?? _Fields, _Params);
            if (wd.errorCode != 0) throw (new WDErrorException(w, wd.errorCode));
            return wd;
        }

        public Int64 getDataSetCount()
        {
            WindData wd = w.wss(_SecCodes,fs ?? _Fields, _Params);
            if (wd.errorCode!=0) throw (new WDErrorException(w, wd.errorCode));
            return wd.codeList.Length;
        }

        #endregion

    }

    public class WTDaysClass : WDBuilder,iWDReader
    {
        DateTime _From;
        DateTime _To;
        string _params;
        public WTDaysClass(WindAPI _w,DateTime From,DateTime To,string strParams):base(_w)
        {
            if (From.CompareTo(To) > 0)
            {
                _From = To;
                _To = From;
            }
            else
            {
                _From = From;
                _To = To;
            }
            _params = strParams;
        }



        public WindData getDataSet()
        {
            WindData wd = w.tdays(_From, _To, _params);
            if (wd.errorCode != 0) throw (new WDErrorException(w, wd.errorCode));
            return wd;
        }

        public long getDataSetCount()
        {
            return 1;
        }
    }

    public class WTDaysCountClass : WDBuilder, iWDReader
    {
        DateTime _From;
        DateTime _To;
        public WTDaysCountClass(WindAPI _w, DateTime From, DateTime To)
            : base(_w)
        {
            if (From.CompareTo(To) > 0)
            {
                _From = To;
                _To = From;
            }
            else
            {
                _From = From;
                _To = To;
            }
        }



        public WindData getDataSet()
        {
            WindData wd = w.tdayscount(_From, _To,"");
            if (wd.errorCode != 0) throw (new WDErrorException(w, wd.errorCode));
            return wd;
        }

        public long getDataSetCount()
        {
            return 1;
        }
    }

    public class WTDaysOffsetClass : WDBuilder, iWDReader
    {
        DateTime _From;
        int _Days;
        string _params;
        public WTDaysOffsetClass(WindAPI _w, DateTime From, int days, string strParams)
            : base(_w)
        {
            _From = From;
            _Days = days;
            _params = strParams;
        }

        public WindData getDataSet()
        {
            //WindData wd = w.tdaysoffset("2018-03-11", 2, "Period=W")
            WindData wd = w.tdaysoffset(_From, _Days, _params);
            if (wd.errorCode != 0) throw (new WDErrorException(w, wd.errorCode));
            return wd;
        }

        public long getDataSetCount()
        {
            return 1;
        }
    }


    /// <summary>
    /// 万得日期序列数据类
    /// </summary>
    public class WSDClass : WDReadClass,iWDReader
    {
        string _SecCodes;
        string _Fields;
        DateTime _BegT, _EndT;
        string _Params;
        static WSDClass()
        {
            //WindAPI w = new WindAPI();
            //w.start();
        }

        public WSDClass(WindAPI w, string SecCodes, string Fields, DateTime BegT, DateTime EndT, string strParams)
            : base(w)
        {
            _Fields = Fields;
            _SecCodes = SecCodes;
            _BegT = BegT;
            _EndT = EndT;
            _Params = strParams;
        }

        #region iWDReader 成员

        public WindData getDataSet()
        {
            InitConnect();
            WindData wd = w.wsd(_SecCodes, _Fields, _BegT.ToShortDateString(), _EndT.ToShortDateString(),_Params);
            if (wd.errorCode != 0) throw (new WDErrorException(w, wd.errorCode));
            return wd;
        }

        public Int64 getDataSetCount()
        {
            InitConnect();
            WindData wd = w.wsd(_SecCodes, _Fields, _BegT.ToShortDateString(), _EndT.ToShortDateString(), _Params);
            if (wd.errorCode != 0) throw (new WDErrorException(w, wd.errorCode));
            return wd.codeList.Length;
        }
        #endregion

    }

    public class WSETClass : WDDateReadClass, iWDReader
    {
        public string SecCode;
        public DateTime Date;
        string sType = "sectorconstituent";//sectorconstituent
        
        #region 构造函数
        public WSETClass(WindAPI w):base(w)
        {
            strType = sType;
        }

        public WSETClass(WindAPI w,string strCode):base(w)
        {
            strType = sType;
            SecCode = strCode;
            Date = DateTime.Today;
        }
        public WSETClass(WindAPI w, string strCode, DateTime dt)
            : base(w)
        {
            strType = sType;
            SecCode = strCode;
            Date = dt;
        }

        public WSETClass(WindAPI w, string _Type, string strCode, DateTime dt)
            : base(w)
        {
            strType = _Type;
            SecCode = strCode;
            Date = dt;
        }
        #endregion

        #region iWDReader 成员

        public WindData getDataSet()
        {
            InitConnect();
            string strExecInfo = string.Format(strInfo, SecCode, Date.ToShortDateString());
            WindData wd =  w.wset(strType, strExecInfo);
            if (wd.errorCode != 0) throw (new WDErrorException(w, wd.errorCode));
            return wd;
        }

        public Int64 getDataSetCount()
        {
            //WindAPI w = new WindAPI();
            //w.start();
            string strExecInfo = string.Format(strInfo, SecCode, Date.ToShortDateString());
            WindData wd = w.wset(strType, strExecInfo);
            if (wd.errorCode != 0) throw (new WDErrorException(w, wd.errorCode));
            if (wd.codeList == null) return 0;
            return wd.codeList.Length;
        }

        #endregion

    }
    /// <summary>
    /// 板块/指数集合类
    /// </summary>
    public class WSETCommIndexClass : WSETClass 
    {
        string _strInfo = "date={1};sectorid={0}";
        public WSETCommIndexClass(WindAPI w)
            : base(w)
        {
            strInfo = _strInfo;
        }

        public WSETCommIndexClass(WindAPI w,string code)
            : base(w,code)
        {
            strInfo = _strInfo;
        }

        public WSETCommIndexClass(WindAPI w,string code, DateTime dt)
            : base(w,code, dt)
        {
            strInfo = _strInfo;
        }
    }
    /// <summary>
    /// 万得指数集合类（可查权重)
    /// </summary>
    public class WSETIndexClass : WSETClass
    {
        string _strInfo = "date={1};windcode={0};field=wind_code,sec_name,i_weight";
        public WSETIndexClass(WindAPI w)
            : base(w)
        {
            strInfo = _strInfo;
        }

        public WSETIndexClass(WindAPI w,string code)
            : base(w,code)
        {
            strInfo = _strInfo;
        }

        public WSETIndexClass(WindAPI w,string code, DateTime dt)
            : base(w,code, dt)
        {
            strInfo = _strInfo;
        }

        public WSETIndexClass(WindAPI w,string _type,string code, DateTime dt)
            : base(w,_type,code, dt)
        {
            strInfo = _strInfo;
        }
    }

    /// <summary>
    /// 万得板块集合类
    /// </summary>
    public class WSETMarketClass : WSETClass
    {
        string _strInfo = "date={1};sectorid={0};field=wind_code,sec_name";
        public WSETMarketClass(WindAPI w)
            : base(w)
        {
            strInfo = _strInfo;
        }

        public WSETMarketClass(WindAPI w,string code)
            : base(w,code)
        {
            strInfo = _strInfo;
        }

        public WSETMarketClass(WindAPI w,string code, DateTime dt)
            : base(w,code, dt)
        {
            strInfo = _strInfo;
        }
    }

    interface iWDReader
    {
        WindData getDataSet();
        Int64 getDataSetCount();
    }

    public class WDDataAdapter
    {
        public static Dictionary<string,Type> getColumnTypes(params object[] specColumnTypes)
        {
            Dictionary<string, Type> SpecColTypes = new Dictionary<string, Type>();
            int i = 0;
            while (i < specColumnTypes.Length)
            {
                if (specColumnTypes[i].GetType() != typeof(string)) break;//如果i参数是非字符串，终止
                string colname = specColumnTypes[i].ToString();
                if (SpecColTypes.ContainsKey(colname)) break;//如果存在重复的column,终止
                if (i + 1 >= specColumnTypes.Length) break;//如果i+1参数不存在，终止
                if (specColumnTypes[i+1] == typeof(Type)) break;//如果i+1不是Type，终止
                SpecColTypes.Add(colname, (Type)specColumnTypes[i + 1]);
                i = i + 2;
            }
            return SpecColTypes;
        }

        public static DataTable getRecords(WindData wd, params object[] specColumnTypes)
        {
            Dictionary<string, Type> SpecColTypes = getColumnTypes(specColumnTypes);
            if (wd == null || wd.errorCode != 0 || wd.data == null) return null;
            DataTable dt = new DataTable();
            bool TimeSer = false;
            if (wd == null || wd.data == null) return dt;
            object[] wddata ;
            switch (wd.data.GetType().ToString())
            {
                case "System.Int32[]":
                case "System.Double[]":
                    double[] darr = wd.data as double[];
                    wddata = new object[darr.Length];
                    for (int i = 0; i < darr.Length; i++)
                    {
                        if (darr[i] != null && !double.IsInfinity((double)darr[i]) && !double.IsNaN((double)darr[i]))
                            wddata[i] = (decimal)darr[i];
                    }
                    darr.CopyTo(wddata, 0);
                    break;
                case "System.String[]":
                    wddata = wd.data as string[];
                    break;
                case "Object[]":
                default:
                    wddata = wd.data as object[];
                    break;
               
            }
            Int64 DataCnt = wddata.Length / wd.fieldList.Length;
            if (wd.timeList != null && wd.timeList.Length == wddata.Length / wd.fieldList.Length)
            {
                TimeSer = true;
            }
            for (int i = 0; i < wd.fieldList.Length; i++)
            {
                string fldname = wd.fieldList[i].Trim();
                if (fldname.Length == 0)
                {
                    fldname = "UnDefined";
                }
                if (SpecColTypes.ContainsKey(wd.fieldList[i]))//如果指定了列数据类型
                {
                    dt.Columns.Add(new DataColumn(fldname, SpecColTypes[wd.fieldList[i]]));
                }
                else
                {
                    dt.Columns.Add(new DataColumn(fldname));
                }
            }
            for (int i = 0; i < DataCnt; i++)
            {
                DataRow dr = dt.NewRow();
                for (int f = 0; f < wd.fieldList.Length; f++)
                {
                    if (wddata[wd.fieldList.Length * i + f] == null)
                        continue;
                    if (wddata[wd.fieldList.Length * i + f].GetType() == typeof(double))
                    {
                        double ft = (double)wddata[wd.fieldList.Length * i + f];
                        if (double.IsNaN(ft))
                        {
                            continue;
                        }
                        if (double.IsInfinity(ft))
                        {
                            continue;
                        }
                     }
                     dr[wd.fieldList[f] == "" ? "UnDefined" : wd.fieldList[f]] = wddata[wd.fieldList.Length * i + f];
                }
                dt.Rows.Add(dr);
            }
            if (TimeSer)
            {
                dt.Columns.Add(new DataColumn("DateTime", typeof(DateTime)));
                for (int i = 0; i < DataCnt; i++) { dt.Rows[i]["DateTime"] = wd.timeList[i]; }
            }
            return dt;
        }

        public static MTable getTable(WindData wd,params object[] colTypes)
        {
            MTable ret = new MTable();
            ret.FillTable(getRecords(wd, colTypes));
            return ret;
        }

        public static List<DetailStringClass> GetList(DataTable dt)
        {
            List<DetailStringClass> ret = new List<DetailStringClass>();
            DataFrame<DetailStringClass> df = new DataFrame<DetailStringClass>();
            ret = df.GetData(dt);
            return ret;
        }
    }

}
