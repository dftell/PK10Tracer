using System;
using System.Collections.Generic;
using System.Text;
using WolfInv.com.PK10CorePress;
using System.Reflection;
using System.Data;
using System.ComponentModel;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using WolfInv.com.BaseObjectsLib;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using WolfInv.com.ProbMathLib;
namespace WolfInv.com.Strags
{
    public interface IFindChance
    {
        List<ChanceClass> getChances(BaseCollection sc, ExpectData ed);
    }
    
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
    [XmlInclude(typeof(Strag_SingleBayesClass))]
    [XmlInclude(typeof(Strag_SimpleMaxEntryClass))]
    public abstract class StragClass : BaseStragClass<TimeSerialData>, iDbFile,IFindChance, ITraceChance, WolfInv.com.BaseObjectsLib.ISpecAmount
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




        public ExpectList LastUseData()
        {
            return _LastUseData as ExpectList;
        }


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

        public abstract List<ChanceClass> getChances(BaseCollection sc, ExpectData ed);

        public override List<ChanceClass<TimeSerialData>> getChances(BaseCollection<TimeSerialData> sc, ExpectData<TimeSerialData> ed)
        {
            return getChances(sc, ed) as List<ChanceClass<TimeSerialData>>;
        }
        public bool CheckNeedEndTheChance<T>(ChanceClass<T> cc, bool LastExpectMatched) where T : TimeSerialData
        {
            return CheckNeedEndTheChance(cc, LastExpectMatched);
        }

        public bool CheckNeedEndTheChance(ChanceClass cc, bool LastExpectMatched)
        {
            return true;
        }
        public long getChipAmount<T>(double RestCash, ChanceClass<T> cc, AmoutSerials amts) where T : TimeSerialData
        {
            return getChipAmount(RestCash, cc, amts);
        }

        public long getChipAmount(double RestCash, ChanceClass cc, AmoutSerials amts)
        {
            int chips = 0;
            int maxcnt = amts.MaxHoldCnts[chips];
            int bShift = 0;
            int eShift = 0;
            int bHold = cc.HoldTimeCnt;// HoldCnt - CurrChancesCnt + 1;
            if (cc.IncrementType == InterestType.CompoundInterest)
            {
                if (cc.AllowMaxHoldTimeCnt > 0 && cc.HoldTimeCnt > cc.AllowMaxHoldTimeCnt)
                {
                    return 0;
                }
                return (long)Math.Floor(cc.FixRate.Value * RestCash / cc.ChipCount);
            }
            //Log("获取机会金额处理", string.Format("当前持有次数：{0}", HoldCnt));
            
            if (bHold > maxcnt)
            {
                Log("风险", "通用重复策略开始次数达到最大上限", string.Format("机会{0}持有次数达到{1}次总投入金额已为{2}", cc.ChanceCode, bHold, "未知"));
                bShift = (int)maxcnt * 2 / 3;
            }
            int HoldCnt = bHold;
            int bRCnt = (bHold % (maxcnt + 1)) + bShift - 1;
            int eRCnt = (HoldCnt % (maxcnt + 1)) + eShift - 1;
            if (cc.ChipCount < 4)//如果是4码以下取平均值
            {
                return (amts.Serials[chips][bRCnt] + amts.Serials[chips][eRCnt]) / 2;
            }
            //四码以上取最大值，防止投入不够导致亏损
            return amts.Serials[chips][eRCnt];

        }


        public string strObjectName
        {
            get { return "策略"; }
        }

        public bool IsTracing { get; set; }
        #endregion



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

    public interface ISelfSetting
    {
        
        StagConfigSetting getInitStagSetting();
    }
}
