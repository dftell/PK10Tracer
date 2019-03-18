using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WAPIWrapperCSharp;
using System.Data;
namespace StrategyLibForWD
{

    public class WSSProcess
    {
        public DataTable getRecods(string SecCodes, string Fields)
        {
            WSSClass wss = new WSSClass(SecCodes, Fields);
            return WDDataAdapter.getRecords(wss.getDataSet());
        }
    }

    public class WDDateReadClass
    {
        protected string strType;
        protected string strInfo;
    }

    public class WSSClass : iWDReader
    {
        string _SecCodes;
        string _Fields;
        string _Params ;
        public WSSClass(string SecCodes, string Fields)
        {
            _Fields = Fields;
            _SecCodes = SecCodes;
            _Params = "";
        }

        public WSSClass(string SecCodes, string Fields,string Params)
        {
            _Fields = Fields;
            _SecCodes = SecCodes;
            _Params = Params;
        }

        #region iWDReader 成员

        public WindData getDataSet()
        {
            WindAPI w = new WindAPI();
            w.start();
            WindData wd = w.wss(_SecCodes, _Fields, _Params);
            return wd;
        }

        public Int64 getDataSetCount()
        {
            WindAPI w = new WindAPI();
            w.start();
            WindData wd = w.wss(_SecCodes, _Fields, _Params);
            if (wd == null) return -1;
            return wd.codeList.Length;
        }

        #endregion

    }

    public class WSDClass : iWDReader
    {
        string _SecCodes;
        string _Fields;
        DateTime _BegT, _EndT;
        static WindAPI w;
        static WSDClass()
        {
            w = new WindAPI();
            w.start();
        }

        public WSDClass(string SecCodes, string Fields, DateTime BegT, DateTime EndT)
        {
            _Fields = Fields;
            _SecCodes = SecCodes;
            _BegT = BegT;
            _EndT = EndT;
        }

        #region iWDReader 成员

        public WindData getDataSet()
        {

            WindData wd = w.wsd(_SecCodes, _Fields, _BegT.ToShortDateString(), _EndT.ToShortDateString(), "");
            return wd;
        }

        public Int64 getDataSetCount()
        {

            WindData wd = w.wsd(_SecCodes, _Fields, _BegT.ToShortDateString(), _EndT.ToShortDateString(), "");
            if (wd == null) return -1;
            return wd.codeList.Length;
        }
        #endregion

    }

    public class WSETClass : WDDateReadClass, iWDReader
    {
        public string SecCode;
        public DateTime Date;
        string sType = "sectorconstituent";
        protected static WindAPI w;
        static WSETClass()
        {
            w = new WindAPI();
            w.start();
        }
        public WSETClass()
        {
            strType = sType;
        }

        public WSETClass(string strCode)
        {
            strType = sType;
            SecCode = strCode;
            Date = DateTime.Today;
        }
        public WSETClass(string strCode, DateTime dt)
        {
            strType = sType;
            SecCode = strCode;
            Date = dt;
        }
        #region iWDReader 成员

        public WindData getDataSet()
        {
            //WindAPI w = new WindAPI();
            //w.start();
            string strExecInfo = string.Format(strInfo, SecCode, Date.ToShortDateString());
            WindData wd = w.wset(strType, strExecInfo);
            return wd;
        }

        public Int64 getDataSetCount()
        {
            //WindAPI w = new WindAPI();
            //w.start();
            string strExecInfo = string.Format(strInfo, SecCode, Date.ToShortDateString());
            WindData wd = w.wset(strType, strExecInfo);
            if (wd == null) return 0;
            if (wd.codeList == null) return 0;
            return wd.codeList.Length;
        }

        #endregion

    }
    /// <summary>
    /// 通用指数集合类
    /// </summary>
    public class WSETCommIndexClass : WSETClass 
    {
        string _strInfo = "date={1};sectorid={0}";
        public WSETCommIndexClass()
            : base()
        {
            strInfo = _strInfo;
        }

        public WSETCommIndexClass(string code)
            : base(code)
        {
            strInfo = _strInfo;
        }

        public WSETCommIndexClass(string code, DateTime dt)
            : base(code, dt)
        {
            strInfo = _strInfo;
        }
    }
    /// <summary>
    /// 万得专用指数集合类
    /// </summary>
    public class WSETIndexClass : WSETClass
    {
        string _strInfo = "date={1};windcode={0}";
        public WSETIndexClass()
            : base()
        {
            strInfo = _strInfo;
        }

        public WSETIndexClass(string code)
            : base(code)
        {
            strInfo = _strInfo;
        }

        public WSETIndexClass(string code, DateTime dt)
            : base(code, dt)
        {
            strInfo = _strInfo;
        }
    }

    public class WSETMarketClass : WSETClass
    {
        string _strInfo = "date={1};sectorid={0}";
        public WSETMarketClass()
            : base()
        {
            strInfo = _strInfo;
        }

        public WSETMarketClass(string code)
            : base(code)
        {
            strInfo = _strInfo;
        }

        public WSETMarketClass(string code, DateTime dt)
            : base(code, dt)
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
        public static DataTable getRecords(WindData wd)
        {
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
                dt.Columns.Add(new DataColumn(wd.fieldList[i]));
            }
            for (int i = 0; i < DataCnt; i++)
            {
                DataRow dr = dt.NewRow();
                for (int f = 0; f < wd.fieldList.Length; f++)
                {
                    dr[wd.fieldList[f]] = wddata[wd.fieldList.Length * i + f];
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

        public static MTable getTable(WindData wd)
        {
            MTable ret = new MTable();
            ret.FillTable(getRecords(wd));
  
            return ret;
        }
    }

}
