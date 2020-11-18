using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Xml;

namespace WolfInv.com.BaseObjectsLib
{
    public class DataTypePoint:DetailStringClass
    {
        public int onlyDebug = 0;//只是测试，为1导致主界面切换至测试界面
        public int IsSecurityData = 0;//是否是证券数据
        public string MainDataUrl = "";
        public string MainDataType = "XML";//HTML,XML,JSON,TXT
        public string SubDataUrl = "";
        public string SubDataType = "HTML";//HTML,XML,JSON,TXT
        //public int SrcUseXml = 1;
        public int AutoSwitchHost = 0;//是否自动切换主机

        public int ExpectCodeDateLong = 8;
        public int ExpectCodeCounterMax = 180;
        public string ExpectCodeDateFormate = "yyyyMMdd";
        public int ExpectCodeCounterLen;//gd11x5突然改变长度，计数器长度42变为042三位数，为适应新的改变必须规范使用统一的长度做约束，不改变以前的数据

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
        
        public int DiffHours;//时差数
        public int DiffMinutes;//时差分钟数
        public int NewRecCount = 20;
        public int SubScriptModel = 0;//使用订阅模式
        public string SubScriptSrc = "";//订阅板块
        public string SubScriptSector;//订阅板块
        public string SubScriptFields;//订阅字段集
        public string SubScriptOptions;//订阅其他参数
        public int SubScriptUpdateAll;//是否更新全部
        public long SaveInterVal=0;//存储间隔（毫秒）随机存储
        public int SubScriptGrpCnt = -1;//默认全部分组

        #region X选Y
        public int IsXxY = 0;
        public bool TenToZero = false;
        public bool ClientNoNeedZero = false;
        public int AllNums;
        public int SelectNums;
        public string strAllTypeOdds;
        public string strCombinTypeOdds;
        public string strPermutTypeOdds;
        #endregion


        public int NeedLoadAllCodes = 0;
        public int NeedLoadAllXDXR = 0;
        public string DateIndex = "000001";
        public int CodeGrpCnt = 100;
        public int MaxThreadCnt = 50;
        public int ThreadUnitCnt = 20;
        public Dictionary<Cycle,List<string>> AllTypeTimes;//所有的期数/时间戳
        /// <summary>
        /// client use
        /// </summary>
        public ExDataConfigClass ExDataConfig;
       
        public string InstHost;
        /// <summary>
        /// 客户端请求期号截取位置，默认不截取，对于广东11选5需要截取2位
        /// </summary>
        public int ClientExpectCodeSubIndex;
        public int ClientSerLen;//对某些平台序号长度可能要填充0，需要指定长度
        public DataPointBuff RuntimeInfo;
        public DataTypePoint(string name,Dictionary<string,string> list)
        {
            DataType = name;
            ReadData(this.GetType(), this, list);
            this.RuntimeInfo = new DataPointBuff();
            this.RuntimeInfo.DefaultDataUrl = this.MainDataUrl;
            this.RuntimeInfo.DefaultUseDataType = this.MainDataType;
            this.RuntimeInfo.DefaultDataDecode = this.DataDecode;
        }

        public static bool ReadData(Type t,object obj, Dictionary<string, string> list)
        {
            try
            {
                //Type t = this.GetType();
                FieldInfo[] fs = t.GetFields();

                for (int i = 0; i < fs.Length; i++)
                {
                    if (list.ContainsKey(fs[i].Name))
                    {
                        //ToLog(fs[i].Name, list[fs[i].Name]);
                        if((fs[i].FieldType.IsSubclassOf(typeof(System.ValueType)) == false) && (fs[i].FieldType != typeof(string)))//如果是对象，并且不是字符串，要特别处理
                        {
                            object sobj = Activator.CreateInstance(fs[i].FieldType);
                            string strVal = list[fs[i].Name];
                            Dictionary<string, string> slist = new Dictionary<string, string>();
                            XmlDocument xmldoc = new XmlDocument();
                            try
                            {
                                xmldoc.LoadXml(strVal);
                                XmlNodeList nodes = xmldoc.SelectNodes("item/item");
                                
                                for(int d=0;d<nodes.Count;d++)
                                {
                                    string key=null, val=null;
                                    XmlAttribute att = nodes[d].Attributes["key"];
                                    if(att!= null)
                                    {
                                        key = att.Value;
                                    }
                                    if (string.IsNullOrEmpty(key))
                                        continue;
                                    att = nodes[d].Attributes["value"];
                                    if (att != null)
                                        val = att.Value;
                                    if(slist.ContainsKey(key) == false)
                                    {
                                        slist.Add(key, val);
                                    }
                                }
                            }
                            catch
                            {
                                continue;
                            }
                            ReadData(sobj.GetType(), sobj, slist);//再读取子节点进对象
                            try
                            {
                                fs[i].SetValue(obj, sobj);
                            }
                            catch
                            {

                            }
                            continue;
                        }
                        try
                        {
                            fs[i].SetValue(obj, Convert.ChangeType(list[fs[i].Name], fs[i].FieldType));
                        }
                        catch (Exception ce)
                        {
                            ToLog(fs[i].Name, list[fs[i].Name]);
                            //throw ce;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                return false;
            }
            return true;
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
                    CodeGrp = GroupBuilder.ToGroup<string>(SecurityCodes, SecurityCodes.Length/parent.CodeGrpCnt);
                }
                return CodeGrp;
            }
        }

        //默认的
        public string DefaultDataUrl;
        //public int DefaultUseXmlModel;
        public string DefaultUseDataType;//HTML,XML,JSON,TXT
        public string DefaultDataDecode;
        public void setDataPointType(DataTypePoint _dtp)
        {
            parent = _dtp;
        }
    }

    public class ExDataConfigClass
    {
        /*
         <item key="InterfaceUrl" value="https://www.52cp.cn/ah11x5/api/miss?"/>
		  <item key="interfaceKey" value="gd11x5"/>
		  <item key="args" value="lottery,pos,target,period,sort,desc"/>
             */
        public string FromDataBase;
        public string MissHtmlUrl;
        public string InterfaceUrl;
        public string interfaceKey;
        public string keyReg;
        public string argsModel;
        public string dataFormat;
        public string convertClass;
        public string dataColumns;
        public string periods;
    }

    public class WebInfoClass:DetailStringClass
    {
        public WebInfoClass()//必须要见无参数构造函数，为反射实例化准备
        {

        }
        public WebInfoClass(Dictionary<string, string> list)
        {
            
            DataTypePoint.ReadData(this.GetType(), this, list);
    
        }
        public string siteName;
        public string useHost;
        public string connectVerUrl;
        public string cookieKey;
        public string cookieKeyXPath;
        public string loginPageUrl;
        public string userAuthUrl;// https://c.jhc3w.com/Account/Auth
        public string userAuthModel;
        public string betUrl;//
        public string betModel;
        public string betRecModel;
        public string gameInfoUrl;//
        public string amountInfoUrl;//
        public string userInfoUrl;
    }

    public class LotteryPropertyClass
    {

    }
    public class XxYProperty: LotteryPropertyClass
    {
        /*
              (ret as CombinLottery_ExpectListProcess).AllNums = 11;
                            (ret as CombinLottery_ExpectListProcess).SelectNums = 5;
                            (ret as CombinLottery_ExpectListProcess).strAllTypeOdds = "2,6,24,168,28,8,3";
                            (ret as CombinLottery_ExpectListProcess).strCombinTypeOdds = "20,60";
                            (ret as CombinLottery_ExpectListProcess).strPermutTypeOdds = "4,40,360";
             */
        public int AllNums;
        public int SelectNums;
        public string strAllTypeOdds;
        public string strCombinTypeOdds;
        public string strPermutTypeOdds;
    }
}
