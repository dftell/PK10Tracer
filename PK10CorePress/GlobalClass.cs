using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using BaseObjectsLib;
using LogLib;
using ProbMathLib;
namespace PK10CorePress
{
    [Serializable]
    public class GlobalClass : DetailStringClass,iLog
    {
        public GlobalClass()
        {
            //_logname = "config";
        }
        static Dictionary<string, Dictionary<string, string>> _SystemDbTables;
        public static Dictionary<string, Dictionary<string, string>> SystemDbTables
        {
            get
            {
                if (_SystemDbTables == null)
                {
                    if (DataTypes == null)
                    {
                        return null;
                    }
                    _SystemDbTables = new Dictionary<string, Dictionary<string, string>>();
                    foreach (string key in DataTypes.Keys)
                    {
                        if (sSysParams.ContainsKey(key))
                        {
                            _SystemDbTables.Add(key, sSysParams[key]);
                        }
                    }
                }
                return _SystemDbTables;
            }
        }

        static Dictionary<string, string> _dataType;
        public static Dictionary<string, string> DataTypes
        {
            get 
            {
                if (_dataType == null)
                {
                    if (sSysParams == null)
                        return null;

                    if (!sSysParams.ContainsKey("DataType"))
                    {
                        ToLog("初始化品种清单","获取品种分类定义错误");
                        return null;
                    }
                    _dataType = sSysParams["DataType"];
                }
                return _dataType;
            }
        }

        static Dictionary<string, Dictionary<string, string>> sSysParams;
        Dictionary<string, Dictionary<string, string>> SysParams
        {
            get
            {
                if (sSysParams == null)
                {
                    sSysParams = new Dictionary<string, Dictionary<string, string>>();
                }
                return sSysParams;
            }
        }
        public static string Title = "快乐赛车";
        public static string strLoginUrlModel = "http://www.wolfinv.com/PK10/App/login.asp?User={0}&Password={1}";
        public static string strRequestInstsURL = "http://www.wolfinv.com/pk10/app/requestinsts.asp";
        public static string dbServer = "www.wolfinv.com";
        public static string dbName =  "PK10db";
        public static string dbUser =  "sa";
        public static string dbPwd =  "bolts";
        public static string TXFFC_url = "http://www.off0.com/index.php";
        public static string PK10_url = "http://d.apiplus.net/newly.do?token=tf066705d12dcb288k&code=bjpk10&format=xml&rows=100";
        ExpectList t_newExpectData;
        bool b_AllowExchange;
        public static XmlDocument XmlDoc;

        public static Dictionary<string, AmoutSerials> AllSerialSettings;

        #region 自定义属性
        public string InstHost
        {
            get
            {
                if (SysParams.ContainsKey("System"))
                {
                    if (SysParams["System"].ContainsKey("InstHost"))
                    {
                        return SysParams["System"]["InstHost"];
                    }
                }
                return null;
            }
        }

        public int ChipUnit
        {
            get
            {
                if (SysParams.ContainsKey("Exchange"))
                {
                    if (SysParams["Exchange"].ContainsKey("ChipUnit"))
                    {
                        return int.Parse(SysParams["Exchange"]["ChipUnit"]);
                    }
                }
                return 2;
            }
        }

        public string SysUser
        {
            get
            {
                if (SysParams.ContainsKey("System"))
                {
                    if (SysParams["System"].ContainsKey("SysUser"))
                    {
                        return SysParams["System"]["SysUser"];
                    }
                }
                return null;
            }
        }
        public static Int64 DefaultMaxLost;
        public Int64 DefMaxLost
        {
            get
            {
                if (SysParams.ContainsKey("System"))
                {
                    if (SysParams["System"].ContainsKey("DefMaxLost"))
                    {
                        DefaultMaxLost = Int64.Parse(SysParams["System"]["DefMaxLost"]);
                        return DefaultMaxLost;
                    }
                }
                return 0;
            }
        }

        public int DefFirstAmt
        {
            get
            {
                if (SysParams.ContainsKey("System"))
                {
                    if (SysParams["System"].ContainsKey("DefFirstAmt"))
                    {
                        return int.Parse(SysParams["System"]["DefFirstAmt"]);
                    }
                }
                return 0;
            }
        }

        public bool AllowExchange
        {
            get
            {
                if(SysParams.Count > 0)
                {
                    b_AllowExchange= SysParams["System"]["AllowExchange"] == "1"? true:false;
                }
                return b_AllowExchange;
            }
            set
            {
                b_AllowExchange = value;
            }
        }

        public int TotalCnt
        {
            get
            {
                if(SysParams.Count > 0)
                    return int.Parse((SysParams["System"]["TotalCnt"]));
                return 0;
            }
        }

        public bool AllowHedge
        {
            get
            {
                if(SysParams.Count > 0)
                    return SysParams["System"]["AllowHedge"]=="1"?true:false;
                return false;
            }
        }

        public bool JoinHedge
        {
            get
            {
                if(SysParams.Count > 0)
                    return SysParams["System"]["JoinHedge"]=="1"?true:false;
                return false;
            }
        }

        public long HedgeTimes
        {
            get
            {
                if(SysParams.Count > 0)
                    return long.Parse(SysParams["System"]["HedgeTimes"]);
                return 0;
            }
        }

        public int IsClient{get{
            if(SysParams.Count > 0)
                return int.Parse(SysParams["System"]["IsClient"]);
            return 0;
        }}

        string _ClentUserName;
        public string ClientUserName
        {
            get
            {
                if (_ClentUserName == null)
                {
                    if (SysParams.Count > 0)
                        _ClentUserName = SysParams["System"]["ClientUsername"];
                    return "";
                }
                return _ClentUserName;
            }
            set
            {
                _ClentUserName = value;
            }
        }

        string _ClientUserPwd;
        public string ClientPassword
        {
            get
            {
                if (_ClientUserPwd == null)
                {
                    if (SysParams.Count > 0)
                        _ClientUserPwd = SysParams["System"]["ClientPassword"];
                    return "";
                }
                return _ClientUserPwd;
            }
            set
            {
                _ClientUserPwd = value;
            }
        }

        public string LoginUrlModel{get{
            if(SysParams.Count > 0)
                return SysParams["System"]["LoginUrlModel"];
            return "";
        }}

        public string LoginDefaultHost{get{
            if(SysParams.Count > 0)
                return SysParams["System"]["LoginDefaultHost"];
            return "";
        }}

        public string LoginHostList{get{
            if(SysParams.Count > 0)
                return SysParams["System"]["LoginHostList"];
            return "";
        }}

        public int LoginInstFillOrEnCode{get{
            if(SysParams.Count > 0)
                return int.Parse(SysParams["System"]["LoginInstFillOrEnCode"]);
            return 0;
        }}


        public int LoginInFrame{get{
            if(SysParams.Count > 0)
                return int.Parse(SysParams["System"]["LoginInFrame"]);
            return 0;
        }}

        public int MinChips{get{
            if(SysParams.Count > 0)
                return int.Parse(SysParams["System"]["MinChips"]);
            return 0;
        }}


        public int SingleColMinTimes
        {
            get{
            if(SysParams.Count > 0)
                return int.Parse(SysParams["System"]["SingleColMinTimes"]);
            return 0;
        }}
        

        public int StartCols{get{
            if(SysParams.Count > 0)
                return int.Parse(SysParams["System"]["StartCol"]);
            return 0;
        }}
        double _Odds = double.NaN;
        public double Odds
        {
            get
            {
                if (double.IsNaN(_Odds))
                {
                    if (SysParams.ContainsKey("System") && SysParams["System"].ContainsKey("Odds"))
                        _Odds = double.Parse(SysParams["System"]["Odds"]);
                }
                return _Odds;
            }
            set
            {
                _Odds = value;
            }
        }

        public long InterVal{get{
            if(SysParams.Count > 0)
                return long.Parse(SysParams["System"]["InterVal"]);
            return 0;
        }}

        //////public int BackColor{get{
        //////    BackColor*/ return RGB(int.Parse((SysParams["System"]["BackColor_R"]);, int.Parse((SysParams["System"]["BackColor_G"]);, int.Parse((SysParams["System"]["BackColor_B"]);)
        //////}}

        public long HistoryFromPage
        {
            get
            {
                if(SysParams.Count > 0)
                    return long.Parse(SysParams["Research"]["FromPage"]);
                return 0;
            }
        }

        public long NewestHistoryExpect
        {
            get
            {
                if(SysParams.Count > 0)
                    return long.Parse(SysParams["Research"]["NewestHistoryExpect"]);
                return 0;
            }
        }

        public int MutliColMinTimes
        {
            get
            {
                if(SysParams.Count > 0)
                    return int.Parse(SysParams["System"]["MutliColMinTimes"]);
                return 0;
            }
        }

        public int SingleCarRepeatCnt{get{
            if(SysParams.Count > 0)
                return int.Parse(SysParams["Research"]["SingleCarRepeatCnt"]);
            return 0;
        }}

        public string RepeatCheckCnt{get{
            if(SysParams.Count > 0)
                return SysParams["Research"]["RepeatCheckCnt"];
            return "";
        }}

        public int ResearchStartCol{get{
            if(SysParams.Count > 0)
                return int.Parse(SysParams["Research"]["StartCol"]);
            return 0;
        }}
        public string ValidOldestHistoryExpect{get{
            if(SysParams.Count > 0)
                return SysParams["Research"]["ValidOldestHistoryExpect"];
            return "";
        }}

        public long AssetInitCash{get{
            if(SysParams.Count > 0)
                return int.Parse(SysParams["Asset"]["InitCash"]);
            return 0;
        }}

        public int AssetCosted{
            get
            {
                if(SysParams.Count > 0)
                    return int.Parse(SysParams["Asset"]["Costed"]);
                return 0;
            }
            set
            {
                SysParams["Asset"]["Costed"] = value.ToString();
            }
        }

        public float AssetGained{
            get
            {
                if(SysParams.Count > 0)
                    return float.Parse(SysParams["Asset"]["Gained"]);
                return 0;
            }
            set
            {
                SysParams["Asset"]["Gained"] = value.ToString();
            }
        }

        public float AssetAChanceMaxRate
        {
            get
            {
                if(SysParams.Count > 0)
                    return float.Parse(SysParams["Asset"]["AChanceMaxRate"]);
                return 0;
            }
        }

        public float AssetTotalMaxRate
        {
            get
            {
                if(SysParams.Count > 0)
                    return float.Parse(SysParams["Asset"]["TotalMaxRate"]);
                return 0;
            }
        }

        public float AssetTotal{
            get{
        
                if(SysParams.Count > 0)
                    return float.Parse(SysParams["Asset"]["TotalCash"]);
                return 0;
            }
            set
            {
                SysParams["Asset"]["TotalCash"] = value.ToString();
            }
        }


        public int MinTimeForChance(int Times)
        {
                if(SysParams.Count > 0)
                {
                    string strItem = string.Format("MinTimesFor{0}",Times);
                    return int.Parse(SysParams["Exchange"][strItem]);
                }
                return 0;
        }

        public string[] UnitChipArray(int Cols)
        {
            if(SysParams.Count > 0)/*UnitChipArray*/ 
            {
                if(!SysParams.ContainsKey("Exchange")) return new string[0];
                if(!SysParams["Exchange"].ContainsKey("Serial"+Cols.ToString())) return new string[0];
                if(SysParams["Exchange"]["Serial"+Cols.ToString()]!=null)
                    return SysParams["Exchange"]["Serial" + Cols.ToString()].Split(',');
            }
            return new string[0];
        }

        public ExpectData NewestExpectData
        {
            get
            {        
                if(t_newExpectData != null) 
                    return t_newExpectData[0];
                return null;
            }
        }

        public ExpectList CurrExpectData
        {
            get
            {
                return t_newExpectData;
            }
        }

        public void SetCurrExpectData(ExpectList el)
        {
            t_newExpectData =el;
        }


        public int SerTotal(int Cols)
        {
            if(SysParams.Count > 0)
            {
                return int.Parse(SysParams["Exchange"]["SerTotal" +Cols.ToString()]);
            }
            return 0;
        }
        #endregion

        public static AmoutSerials? _DefaultHoldAmtSerials = null;
        public AmoutSerials DefaultHoldAmtSerials
        {
            get
            {
                if (_DefaultHoldAmtSerials == null )
                {
                    _DefaultHoldAmtSerials = getOptSerials(double.Parse(sSysParams["System"]["Odds"]), Int64.Parse(sSysParams["System"]["DefMaxLost"]), int.Parse(sSysParams["System"]["DefFirstAmt"]));
                }
                return _DefaultHoldAmtSerials.Value;
            }
        }

        public static DbClass getCurrDb()
        {
            return new DbClass(dbServer, dbUser, dbPwd, dbName);
        }
        
        static GlobalClass()
        {

            ReadConfig();
            ReReadStragList();
        }
        
        static string _strStragJsons = null;
        
        static string _strStragPlanXml = null;
        
        public string getStragXml()
        {
            return _strStragJsons;
        }

        public void setStragXml(string value)
        {
            if (_strStragJsons != value)
            {
                if (value != null)
                {
                    _strStragJsons = value;
                }
            }
            if (_strStragJsons != null)
            {
                SaveStragList(_strStragJsons);
            }
        }
       
        

        static void ReadConfig()
        {
            sSysParams = new Dictionary<string, Dictionary<string, string>>();
            XmlDocument doc = new XmlDocument();
            string strPath = typeof(GlobalClass).Assembly.Location;
            string strXmlPath = Path.GetDirectoryName(strPath) + "\\config.xml";
            try
            {
                doc.Load(strXmlPath);

            }
            catch (Exception ce)
            {
                ToLog("读取配置文件错误", ce.Message);
                return;
            }
            XmlDocument configDoc = doc;
            XmlNodeList configs = configDoc.SelectNodes("root/configs/config");
            for (int i = 0; i < configs.Count; i++)
            {
                XmlNode node = configs[i];
                string TypeName = node.Attributes["type"].Value;
                Dictionary<string, string> configtypeDir = new Dictionary<string, string>();
                XmlNodeList configitems = node.SelectNodes("./item");
                for (int j = 0; j < configitems.Count; j++)
                {
                    string name = configitems[j].Attributes["key"].Value;
                    string val = configitems[j].Attributes["value"].Value;
                    if (!configtypeDir.ContainsKey(name))
                        configtypeDir.Add(name, val);
                    else
                        configtypeDir[name] = val;
                }
                sSysParams.Add(TypeName, configtypeDir);
            }
        }

        public static string ReReadStragList()
        {
            _strStragJsons = ReadFile("StragList.db");
            return _strStragJsons;
        }

        public static bool SaveStragList(string str)
        {
            if (_strStragJsons == str)
                return true;
            _strStragJsons = str;
            return SaveFile("StragList.db", str);
        }

        public static string ReReadAssetUnitList()
        {
            return ReadFile("AssetUnits.db");
        }

        public static bool SaveAssetUnits(string str)
        {
            return SaveFile("AssetUnits.db", str);
        }

        

        

        public static string getStragRunningPlan(bool UseNewestData)
        {
            bool ReadNewest = false;
            if (UseNewestData)//需要使用最新数据
            {
                ReadNewest = true;
            }
            if (_strStragPlanXml == null || _strStragPlanXml.Trim().Length == 0)//老数据为空
            {
                ReadNewest = true;
            }
            if(ReadNewest)
                _strStragPlanXml = ReadFile("StragRunPlan.db");
            return _strStragPlanXml; 
        }

        public static bool setStragRunningPlan(string strXml)
        {
            bool ret = false;
            if (strXml == null)
                return false;
            if (_strStragPlanXml!= null && strXml.Trim() == _strStragPlanXml.Trim())
            {
                return true;
            }
            ret = SaveFile("StragRunPlan.db", strXml);
            if (ret)
            {
                _strStragPlanXml = strXml;
            }
            return ret;
        }
        
        static string ReadFile(string filename)
        {
            StreamReader sr = null;
            string strContent = null;
            string strPath = typeof(GlobalClass).Assembly.Location;
            string strJsonPath = string.Format("{0}\\{1}", Path.GetDirectoryName(strPath), filename);
            try
            {
                sr = new StreamReader(strJsonPath);
                strContent = sr.ReadToEnd();
                sr.Close();
            }
            catch(Exception e)
            {
                ToLog("错误",string.Format("读取文件[{0}]错误",filename),e.Message);
                return null;
            }
            return strContent;
        }

        static bool SaveFile(string filename,string strContent)
        {
            string strPath = typeof(GlobalClass).Assembly.Location;
            string strJsonPath =string.Format("{0}\\{1}",Path.GetDirectoryName(strPath) ,filename);
            try
            {
                StreamWriter sw = new StreamWriter(strJsonPath, false);
                sw.Write(strContent);
                sw.Close();
                //ToLog("保存策略", "成功");
            }
            catch(Exception c)
            {
                ToLog("错误",string.Format("保存到文件[{0}]错误:{1}",filename,strContent), string.Format("{0}",c.Message));
                return false;
            }
            return true;
        }
        /// <summary>
        /// 计算最优投注金额数组
        /// </summary>
        /// <param name="odds">赔率</param>
        /// <param name="MaxValue">最大投入</param>
        /// <returns></returns>
        public static AmoutSerials getOptSerials(double odds, Int64 MaxValue, int FirstAmt)
        {
            return getOptSerials(odds, MaxValue, FirstAmt, false);
        }
        /// <summary>
        /// 计算最优投注金额数组
        /// </summary>
        /// <param name="odds">赔率</param>
        /// <param name="MaxValue">最大投入</param>
        /// <returns></returns>
        public static AmoutSerials getOptSerials(double odds, Int64 MaxValue,int FirstAmt,bool NeedAddFirst)
        {
            string model = "key_{0}_{1}_{2}";
            string key = string.Format(model, odds, MaxValue, FirstAmt);
            if (AllSerialSettings == null)
                AllSerialSettings = new Dictionary<string, AmoutSerials>();
            if (AllSerialSettings.ContainsKey(key))
                return AllSerialSettings[key];
            AmoutSerials retval = new AmoutSerials();
            if (double.IsNaN(odds) || MaxValue == 0)
            {
                return retval;
            }
            int[] ret = new int[8];
            double[] Rates = new double[8];
            Int64[] MaxSum = new Int64[8];
            Int64[][] Serials = new Int64[8][];
            //int MaxValue = 20000;
            //double odds = 9.75;
            for (int i = 0; i < ret.Length; i++)
            {
                Int64[] Ser = new Int64[0];
                int MaxCnts = 1;
                double bRate = 0.0005;
                double stepRate = 0.001;
                Int64 CurrSum = getSum(i + 1, MaxCnts, FirstAmt, odds, 0, out Ser);
                while (CurrSum < MaxValue)//计算出在指定资金范围内保本所能达到的次数
                {
                    MaxCnts++;
                    CurrSum = getSum(i + 1, MaxCnts, 1, odds, 0, out Ser);
                }
                MaxCnts--; //回退1
                long TestSum = getSum(i + 1, MaxCnts, FirstAmt, odds, bRate, out Ser);
                if (TestSum > MaxValue)//如果最少盈利下所需资金大于指定值，所能达到的次数减一
                {
                    bRate = 0;
                    stepRate = 0.0001;
                    CurrSum = getSum(i + 1, MaxCnts, FirstAmt, odds, bRate, out Ser);
                }
                else
                {
                    CurrSum = TestSum;
                }
                Int64 LastSum = CurrSum;
                while (CurrSum < MaxValue)
                {
                    LastSum = CurrSum;
                    bRate += stepRate;
                    CurrSum = getSum(i + 1, MaxCnts, FirstAmt, odds, bRate, out Ser);
                }
                if (NeedAddFirst)
                {
                    List<long> list = Ser.ToList();
                    ret[i] = MaxCnts + 1;
                    list.Insert(0, FirstAmt);
                    Rates[i] = bRate - stepRate;
                    MaxSum[i] = LastSum + FirstAmt;
                    Serials[i] = list.ToArray();
                }
                else
                {
                    ret[i] = MaxCnts ;
                    Rates[i] = bRate - stepRate;
                    MaxSum[i] = LastSum ;
                    Serials[i] = Ser;
                }
            }
            retval.MaxHoldCnts = ret;
            retval.MaxRates = Rates;
            retval.Serials = Serials;
            if (!AllSerialSettings.ContainsKey(key))//防止计算过程中有其他设置请求了
            {
                AllSerialSettings.Add(key, retval);
            }
            return retval;
        }

        static Int64 getSum(int chips, int holdcnt, int firstAmt, double odd, double MinWRate, out Int64[] serial)
        {
            Int64 sum = 0;
            serial = new Int64[holdcnt];
            for (int i = 1; i <= holdcnt; i++)
            {
                serial[i - 1] = FixCountMethods.getTheAmount(chips, i, firstAmt, odd, MinWRate);
                sum += serial[i - 1] * chips;
            }
            return sum;
        }
    }

    public struct AmoutSerials
    {
        public int[] MaxHoldCnts;
        public double[] MaxRates;
        /// <summary>
        /// 
        /// </summary>public Int64[] MaxSum = new Int64[8];
        public Int64[][] Serials;
    }
}
