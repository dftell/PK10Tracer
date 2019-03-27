using System;
using System.Collections.Generic;
using System.Reflection;
namespace WolfInv.com.BaseObjectsLib
{
    public class DataTypePoint:DetailStringClass
    {
        public int IsSecurityData = 0;//是否是证券数据
        public string MainDataUrl = "";
        public string SubDataUrl = "";
        public int SrcUseXml = 1;


        public string DataDecode = "utf-8";
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

        public int NeedLoadAllCodes = 0;
        public int NeedLoadAllXDXR = 0;
        public string DateIndex = "000001";
        public int CodeGrpCnt = 100;
        public Dictionary<Cycle,List<string>> AllTypeTimes;//所有的期数/时间戳


        public DataPointBuff RuntimeInfo;
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

    public class DataPointBuff
    {
        public DataPointBuff()//为detailstringclass准备一个0参数的构造函数
        {

        }
        public DataPointBuff(DataTypePoint obj)
        {
            parent = obj;
        }
        DataTypePoint parent;
        public string[] SecurityCodes { get; set; }
        public Dictionary<string,StockInfoMongoData> SecurityInfoList { get; set; }
        public MongoDataDictionary<XDXRData> XDXRList { get; set; }

        public List<string> HistoryDateList { get; set; }

        List<string[]> CodeGrp;

        public List<string[]> getGrpCodes
        {
            get
            {
                if (SecurityCodes == null)
                    return new List<string[]>();
                if (CodeGrp == null)
                {
                    CodeGrp = GroupBuilder.ToGroup<string>(SecurityCodes, parent.CodeGrpCnt);
                }
                return CodeGrp;
            }
        }

        //默认的
        public string DefaultDataUrl;
        public int DefaultUseXmlModel;
        public string DefaultDataDecode;
    }
}
