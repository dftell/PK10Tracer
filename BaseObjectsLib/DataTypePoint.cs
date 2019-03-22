using System;
using System.Collections.Generic;
using System.Reflection;
namespace WolfInv.com.BaseObjectsLib
{
    public class DataTypePoint
    {
        public string DbHost;
        public string DbUser;
        public string DbPassword;
        public string DbName;
        public string DataType;
        public string DBType;
        public string NewestTable;
        public string BlackTable;//板块表
        public string IndexDayTable;//指数日线表
        public string IndexTable;//指数列表
        public string StockListTable;//股票清单表
        public string XDXRTable;//除权除息表
        public string HistoryTable;
        public string MissHistoryTable;
        public string MissNewestTable;
        public string ChanceTable;
        public string ResultTable;
        public int CheckNewestDataDays;
        public long ReceiveSeconds; //刷新间隔秒数
        public DateTime ReceiveStartTime;//数据接收开始时间
        public DateTime ReceiveEndTime; //数据接收停止时间
        public int SubScriptModel = 0;//使用订阅模式
        public string SubScriptSrc = "";//订阅板块
        public string SubScriptSector;//订阅板块
        public string SubScriptFields;//订阅字段集
        public string SubScriptOptions;//订阅其他参数
        public int SubScriptUpdateAll;//是否更新全部
        public long SaveInterVal=0;//存储间隔（毫秒）随机存储
        public int SubScriptGrpCnt = -1;//默认全部分组

        public Dictionary<Cycle,List<string>> AllTypeTimes;//所有的期数/时间戳

        public DataTypePoint(string name,Dictionary<string,string> list)
        {
            DataType = name;
            Type t = this.GetType();
            FieldInfo[] fs = t.GetFields();
            for(int i=0;i<fs.Length;i++)
            {
                if(list.ContainsKey(fs[i].Name))
                {
                    fs[i].SetValue(this, Convert.ChangeType(list[fs[i].Name],fs[i].FieldType));
                }
            }
        }
    }
}
