using System;
using System.Collections.Generic;
using WAPIWrapperCSharp;
using System.Data;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.CFZQ_LHProcess
{
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
                        if (!double.IsInfinity((double)darr[i]) && !double.IsNaN((double)darr[i]))
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
