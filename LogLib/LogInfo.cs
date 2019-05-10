using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
namespace WolfInv.com.LogLib
{
    public class LogInfo : LogClass
    {
        public Dictionary<DateTime, string> GetLogAfterDate(DateTime dt)
        {
            //LogableClass.ToLog("获取指定时间以后的日志列表", dt.ToString());
            if (LogBuffs == null) 
                LogBuffs = new Dictionary<DateTime, string>();
            Dictionary<DateTime, string> tmp = new Dictionary<DateTime, string>();
            foreach (DateTime t in LogBuffs.Keys)
            {
                if (t.CompareTo(dt)>0)
                {
                    tmp.Add(t,LogBuffs[t]);
                }
            }
            //tmp = LogBuffs.Where(t => (t.Key.CompareTo(dt) > 0)) as Dictionary<DateTime, string>;
            if (tmp == null)
            {
                LogableClass.ToLog("原始日志数量", LogBuffs.Count.ToString());
                return new Dictionary<DateTime, string>();
            }
            //LogableClass.ToLog("获取到日志数量", tmp.Count.ToString());
            return tmp;
        }

        public void ClearBuff(DateTime befDate)
        {
            LogableClass.ToLog("清空指定时间以后的日志列表", befDate.ToString());
            if (LogBuffs == null)
                LogBuffs = new Dictionary<DateTime, string>();
            Dictionary<DateTime, string> tmp = LogBuffs.Where(t => t.Key > befDate) as Dictionary<DateTime, string>;
            LogBuffs = tmp;
        }

        public void ClearBuff()
        {
            LogBuffs.Clear();// 
        }

        public DataTable GetLogTableAfterDate(DateTime dt)
        {
            DataTable tab = new DataTable();
            tab.Columns.Add("LogTime", typeof(DateTime));
            tab.Columns.Add("Log", typeof(string));
            Dictionary<DateTime, string> ret = GetLogAfterDate(dt);
            if (ret == null)
            {
                return tab;
            }
            foreach (DateTime key in ret.Keys)
            {
                DataRow dr = tab.NewRow();
                dr[0] = key;
                dr[1] = ret[key];
                tab.Rows.Add(dr);
            }
            return tab;

        }
        
        public void WriteFile(string txt, string specPath, string filename, string filetype,bool NoTime,bool writeover)
        {
            threadWrite(txt, specPath, filename, filetype, NoTime, writeover);
        }
    }
}
