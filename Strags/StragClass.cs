using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
using GuideLib;
using System.Reflection;
using System.Data;
using System.ComponentModel;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using BaseObjectsLib;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using ProbMathLib;
namespace Strags
{
    /// <summary>
    /// 交易策略
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(strag_CommCombOldClass))]
    [XmlInclude(typeof(strag_CommLongMissBackBalanceClass))]
    [XmlInclude(typeof(strag_CommLongTimeBalanceForOldCombClass))]
    [XmlInclude(typeof(strag_CommOldClass))]
    [XmlInclude(typeof(strag_CommProbabilityDistributionClass))]
    [XmlInclude(typeof(strag_grownclass))]
    [XmlInclude(typeof(strag_CommRepeatTracerClass))]
    [XmlInclude(typeof(strag_PoissonRandomClass))]
    [XmlInclude(typeof(Strag_CombLongOldClass))]
    [XmlInclude(typeof(Strag_BinomialDistrClass))]
    [XmlInclude(typeof(Strag_StdDevWaveClass))]
    [XmlInclude(typeof(strag_CommJumpClass))]    
    public abstract class StragClass : BaseStragClass, IFindChance, ISelfSetting,iDbFile
    {
        public StragClass():base()
        {
            CommSetting = new SettingClass();
            //SetDefaultValueAttribute();
            StagSetting = new StagConfigSetting();
            StagSetting = this.getInitStagSetting();
        }
        

        public new void Log(string topic, string txt)
        {
            Log(this.StragClassName, topic, txt);
        }

        [DescriptionAttribute("个性化设置"),
        DisplayName("个性化设置"),
        CategoryAttribute("其他设置"),
        Editor(typeof(SerialObjectEdit<StagConfigSetting>), typeof(UITypeEditor))]
        public StagConfigSetting StagSetting { get; set; }

        /// <summary>
        /// 策略配置
        /// </summary>
        [DescriptionAttribute("通用设置"),
        DisplayName("通用设置"),
        CategoryAttribute("其他设置"),
        Editor(typeof(SerialObjectEdit<SettingClass>), typeof(UITypeEditor))]
        public SettingClass CommSetting { get; set; }

        

        public static DataTable getAllStrags()
        {
            Assembly ass = typeof(StragClass).Assembly;
            Assembly[] assArr = new Assembly[1] { ass };
            List<Type> types = new List<Type>();
            //var types = AppDomain.CurrentDomain.GetAssemblies()
    ////        var types = assArr
    ////.SelectMany(a => a.GetTypes().Where(t => (t.IsAbstract == false
    ////                                && (
    ////                                t.BaseType == typeof(StragClass)
    ////                                || (t.BaseType.BaseType != null && t.BaseType.BaseType == typeof(StragClass))
    ////                                || (t.BaseType.BaseType != null && t.BaseType.BaseType.BaseType != null && t.BaseType.BaseType.BaseType == typeof(StragClass))
    ////                                ))))
    ////.ToArray();
            Type[] AllType = ass.GetTypes();
            for (int i = 0; i < AllType.Length; i++)
            {
                Type t = AllType[i];
                while (t.BaseType != null)//寻找基类是stragclass的所有非抽象类
                {
                    if (t.BaseType.Equals(typeof(StragClass)))
                    {
                        if (!AllType[i].IsAbstract)
                        {
                            types.Add(AllType[i]);
                            break;
                        }
                    }
                    t = t.BaseType;
                }
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("text");
            dt.Columns.Add("value", typeof(string));
            dt.Columns.Add("class", typeof(Type));
            for (int i = 0; i < types.Count; i++)
            {
                DataRow dr = dt.NewRow();
                DisplayNameAttribute attribute = Attribute.GetCustomAttribute(types[i], typeof(DisplayNameAttribute),false) as DisplayNameAttribute;
                string[] names =  types[i].FullName.Split('.');
                if (attribute != null)
                {
                    names = new string[] { attribute.DisplayName };
                }
                dr["text"] = names[names.Length - 1];
                dr["value"] = types[i].ToString();
                dr["class"] = types[i];
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public static StragClass getStragByName(string className)
        {
            Assembly asmb = typeof(StragClass).Assembly;// Assembly.LoadFrom("EnterpriseServerBase.dll");
            Type sct = asmb.GetType(className);
            StragClass sc = Activator.CreateInstance(sct) as StragClass;
            return sc;
        }

        public static DataTable GetTableByStragList(List<StragClass> Strags)
        {
            DataTable dt = new DataTable();

            string strcols = "策略ID,策略描述,按车号视图,策略参数,策略名称,策略类名";
            string[] cols = strcols.Split(',');
            for (int i = 0; i < cols.Length; i++)
            {
                dt.Columns.Add(cols[i]);
            }
            for (int i = 0; i < Strags.Count; i++)
            {
                StragClass jcls = Strags[i];
                DataRow dr = dt.NewRow();
                dr[0] = jcls.GUID;
                dr[1] = jcls.StragScript;
                dr[2] = jcls.BySer;
                dr[3] = jcls.StagSetting.ToString();
                dr[4] = jcls.StragClassName;
                dr[5] = jcls.StragTypeName;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public abstract List<ChanceClass> getChances(CommCollection sc, ExpectData ed);

        public abstract StagConfigSetting getInitStagSetting();


        //public abstract Int64 getChipAmount(double RestCash, ChanceClass cc, AmoutSerials ams);//查找类默认单利为1，复利为赔率保本对应的凯利公式所算出来的比例
        

        ////public override Int64 getChipAmount1(double RestCash, ChanceClass cc,AmoutSerials ams)//查找类默认单利为1，复利为赔率保本对应的凯利公式所算出来的比例
        ////{
        ////    if (cc.IncrementType == InterestType.SimpleInterest)
        ////        return 1;
        ////    else
        ////    {
        ////        return 1;//return (long)Math.Floor(RestCash* KellyMethodClass.KellyFormula(cc.ChipCount,1,this.CommSetting.GetGlobalSetting().Odds,));
        ////    }
        ////    //return this.CommSetting.GetGlobalSetting().UnitChipArray(cc.ChipCount)[0];//默认返回第一个
        ////}


        public abstract Type getTheChanceType();


        public string AssetUnitId { get; set; }


        #region  支持propertygrid默认信息
        static List<StragClass> DBfiles;
        
        static StragClass()
        {
            DBfiles = new List<StragClass>();
            _strDbXml = GlobalClass.ReReadStragList();
        }
        public List<T> getAllListFromDb<T>()
        {
            List<T> ret = new List<T>();
            if (strDbXml == null)
            {
                return ret;
            }
            return getObjectListByXml<T>(strDbXml);
        }

        static string _strDbXml;
        public string strDbXml
        {
            get 
            {
                return _strDbXml; 
            }
        }
       

        public bool AllowMutliSelect
        {
            get { return false; }
        }

        public string strKey
        {
            get { return "GUID"; }
        }

        public bool SaveDBFile<T>(List<T> list)
        {
            return GlobalClass.SaveStragList(getXmlByObjectList<T>(list));
        }

        public string strKeyValue()
        {
            return this.GUID;
        }

        public string strObjectName
        {
            get { return "策略"; }
        }
        #endregion


        
    }
    
    
 
    public class StragChance:MarshalByRefObject
    {
        public StragClass Strag;
        public ChanceClass Chance;
        public StragChance(StragClass _Strag, ChanceClass _Chance)
        {
            Strag = _Strag;
            Chance = _Chance;
        }
    }

    /// <summary>
    /// 概率检查类
    /// </summary>
    public interface IProbCheckClass
    {
        [DisplayName("标准差倍数"),
        DefaultValueAttribute(1.5)
        ]
        double StdvCnt { get; set; }
    }



    public abstract class ProbWaveSelectStragClass : ChanceTraceStragClass
    {
        
        protected Dictionary<string, double> RateDic;
        public GuideResult LocalWaveData;
        Dictionary<string, MGuide> _UseMainGuides;
        public Dictionary<string, MGuide> UseMainGuides()
        {
            return _UseMainGuides;
        }

        public void SetUseMainGuides(Dictionary<string, MGuide> value)
        {
            _UseMainGuides = value;
        }
        Dictionary<string, Int64> _UseAmountList = new Dictionary<string,long>();
        public Dictionary<string, Int64> UseAmountList()
        {
            return _UseAmountList;
        }
        public void UseAmountList(Dictionary<string, Int64> value)
        {
            _UseAmountList = value;
        }
        public GuideResultSet BaseWaves()
        {
            //get
            //{
            return new GuideResultSet(RateDic);
            //}
        }

        public GuideResultSet GuideWaves()
        {
            //get
            //{
                GuideResultSet ret = new GuideResultSet();
                if (UseMainGuides() == null || UseMainGuides().Count == 0) return ret;
                int len = UseMainGuides().Last().Value.CurrValues.Length;
                for (int i = 0; i < len; i++)
                {
                    Dictionary<string, double> dic = new Dictionary<string, double>();
                    foreach (string key in UseMainGuides().Keys)
                    {
                        dic.Add(key, UseMainGuides()[key].CurrValues[i]);
                    }
                    ret.NewTable(dic);
                }
                return ret;
            //}
        }

        public abstract bool CheckEnableIn();
        public abstract bool CheckEnableOut();

        /// <summary>
        /// 统一下注数量
        /// </summary>
        /// <param name="RestCash"></param>
        /// <param name="cc"></param>
        /// <returns></returns>
        public override Int64 getChipAmount(double RestCash, PK10CorePress.ChanceClass cc, AmoutSerials ams)
        {
            ////if (this.UseAmountList.ContainsKey(this.LastUseData.LastData.Expect))
            ////{
            ////    //return this.UseAmountList[this.LastUseData.LastData.Expect];
            ////}
            if (cc.IncrementType == InterestType.SimpleInterest) return (int)Math.Floor(this.CommSetting.InitCash * 0.01);
            double p = (double)(cc.ChipCount / this.CommSetting.Odds);
            double Normal_p = (double)cc.ChipCount / 10;
            double _MinRate = Normal_p + this.MinWinRate * (p - Normal_p);
            p = _MinRate;
            double b = (double)this.CommSetting.Odds;
            double q = 1 - p;
            double rate = (p * b - q) / b;
            double cs = Math.Sqrt(this.CommSetting.MaxHoldingCnt);
            Int64 AllAmount = (Int64)Math.Floor(RestCash * rate / cc.ChipCount);
            //this.UseAmountList.Add(this.LastUseData.LastData.Expect, AllAmount);
            return AllAmount;
        }

        
    }


    public abstract class ChanceTraceStragClass :StragClass, ITraceChance,ISpecAmount
    {
        bool _IsTracing;
        public bool IsTracing
        {
            get
            {
                return _IsTracing;
            }
            set
            {
                _IsTracing = value;
            }
        }
        public abstract bool CheckNeedEndTheChance(ChanceClass cc, bool LastExpectMatched);

        public abstract long getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts);
    }

    /// <summary>
    /// 整体标准差追踪类
    /// </summary>
    public abstract class TotalStdDevTraceStragClass : ChanceTraceStragClass
    {
        Dictionary<string, List<double>> AllStdDev;
        public Dictionary<string, List<double>> getAllStdDev()
        {
            return AllStdDev;
        }

        public void setAllStdDev(Dictionary<string, List<double>> value)
        {
            AllStdDev = value;
        }
    }

    public interface ISelfSetting
    {
        
        StagConfigSetting getInitStagSetting();
    }
}
