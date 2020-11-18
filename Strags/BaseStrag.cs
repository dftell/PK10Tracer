using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WolfInv.com.PK10CorePress;
using WolfInv.com.GuideLib;
using System.Reflection;
using System.Data;
using System.ComponentModel;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using WolfInv.com.BaseObjectsLib;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using WolfInv.com.Strags.KLXxY;
using WolfInv.com.Strags.MLStragClass;
using WolfInv.com.Strags.Security;
namespace WolfInv.com.Strags
{
    public interface IFindChance<T> where T : TimeSerialData
    {
        List<ChanceClass<T>> getChances(BaseCollection<T> sc, ExpectData<T> ed);
    }

    /// <summary>
    /// 策略基类
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
    [XmlInclude(typeof(StragClass))]
    [XmlInclude(typeof(strag_MarkovClass))]
    [XmlInclude(typeof(Strag_SimpleShiftClass))]
    [XmlInclude(typeof(ReferIndexStragClass))]
    [XmlInclude(typeof(FirstBuyPoint_StragClass<TimeSerialData>))]
    public abstract class BaseStragClass<T> : DisplayAsTableClass, IFindChance<T>, ISelfSetting where T : TimeSerialData
    {
        string _guid;
        [DescriptionAttribute("GUID"),
        DisplayName("GUID"),
        CategoryAttribute("全局设置")]
        public string GUID
        {
            get
            {
                if (_guid == null || _guid.Trim().Length == 0)
                {
                    _guid = Guid.NewGuid().ToString();
                }
                return _guid;
            }
            set
            {
                if(_guid == null || _guid.Trim().Length==0)
                    _guid = value;
            }
        }

        [DescriptionAttribute("策略描述"),
        DisplayName("策略描述"),
        CategoryAttribute("全局设置"),
        DefaultValueAttribute("XXXXXXXXXXXXXXXXXXXX策略")]
        public string StragScript { get; 
             set; }


        [DescriptionAttribute("策略类型名"),
        DisplayName("策略类型名"),
        CategoryAttribute("全局设置")]
        public string StragTypeName { 
            get 
            {
                return this.GetType().FullName;
            } 
        }
        [DescriptionAttribute("策略所属彩种"),
        DisplayName("策略所属彩种"),
            DefaultValueAttribute("PK10"),
        CategoryAttribute("全局设置")]
        public string StragLotteryName
        {
            get;set;
        }
        /// <summary>
        /// debug info
        /// </summary>
        GuideResultSet _Guides = null;
        public GuideResultSet Guides()
        {
            return _Guides;
        }
        public void SetGuides(GuideResultSet value)
        {
            _Guides = value;
        }
        protected string _StragClassName;
        [DescriptionAttribute("策略名称"),
        DisplayName("策略名称"),
        CategoryAttribute("全局设置")]
        public string StragClassName 
        { 
            get 
            {
                return _StragClassName;
            } 
        }


        [DescriptionAttribute("按Kelly公式计算出的保本胜率乘以本系数作为概率选号的基本条件（必须大于1)"),
        DisplayName("Kelly保本胜率系数"),
        CategoryAttribute("入场条件"),
        DefaultValueAttribute(1.001)]
        public double MinWinRate { get; set; }

        /// <summary>
        /// 回览期数
        /// </summary>
        [DescriptionAttribute("回览期数"),
        DisplayName("回览期数"),
        CategoryAttribute("入场条件"),
        DefaultValueAttribute(500)]
        public int ReviewExpectCnt { get; set; }

        /// <summary>
        /// 最大持有期数
        /// </summary>
        [DescriptionAttribute("最大持有期数"),
        DisplayName("最大持有期数"),
        CategoryAttribute("入场条件"),
        DefaultValueAttribute(500)]
        public int MaxHoldCnt { get; set; }

        /// <summary>
        /// 是否固定注数
        /// </summary>
        [DescriptionAttribute("是否为固定注数"),
        DisplayName("是否为固定注数"),
        CategoryAttribute("入场条件"),
        DefaultValueAttribute(true)]
        public bool FixChipCnt { get; set; }

        /// <summary>
        /// 注数
        /// </summary>
        [DescriptionAttribute("注数"),
        DisplayName("注数"),
        CategoryAttribute("入场条件"),
        DefaultValueAttribute(5)]
        public int ChipCount { get; set; }

        /// <summary>
        /// 是否允许重复
        /// </summary>
        [DescriptionAttribute("是否允许重复下入未结束的机会"),
        DisplayName("是否允许重复下入未结束的机会"),
        CategoryAttribute("持仓设置"),
        DefaultValueAttribute(false)]
        public bool AllowRepeat { get; set; }

        
        [DescriptionAttribute("是否用车号视图"),
        DisplayName("按车号视图"),
        CategoryAttribute("数据设置"),
        DefaultValueAttribute(true)]
        public bool BySer{get;set;}
        
        public int getHoldTimeCnt()
        {
            return 1;
        }
        [DescriptionAttribute("入场最小次数"),
        DisplayName("入场最小次数"),
        CategoryAttribute("入场条件"),
        DefaultValueAttribute(5)]
        public int InputMinTimes { get; set; }

        [DescriptionAttribute("入场最大次数"),
        DisplayName("入场最大次数"),
        CategoryAttribute("入场条件"),
        DefaultValueAttribute(50)]
        public int InputMaxTimes { get; set; }

        /// <summary>
        /// 是否排除大小
        /// </summary>
        [DescriptionAttribute("是否排除全大全小"),
        DisplayName("是否排除全大全小"),
        CategoryAttribute("入场条件"),
        DefaultValueAttribute(false)]
        public bool ExcludeBS{get;set;}//排除大小

        [DescriptionAttribute("是否排除全单全双"),
        DisplayName("是否排除全单全双"),
        CategoryAttribute("入场条件"),
        DefaultValueAttribute(false)]
        public bool ExcludeSD { get; set; }//排除单双

        [DescriptionAttribute("是否只考虑全大全小"),
        DisplayName("是否只考虑全大全小"),
        CategoryAttribute("入场条件"),
        DefaultValueAttribute(false)]
        public bool OnlyBS{get;set;}//只考虑大小

        [DescriptionAttribute("是否只考虑全单全双"),
        DisplayName("是否只考虑全单全双"),
        CategoryAttribute("入场条件"),
        DefaultValueAttribute(false)]
        public bool OnlySD { get; set; }//只考虑单双

        [DescriptionAttribute("是否按条件求反下注"),
        DisplayName("是否按条件求反下注"),
        CategoryAttribute("入场条件"),
        DefaultValueAttribute(false)]
        public bool GetRev{get;set;}//求反
        [DescriptionAttribute("个性化设置"),
        DisplayName("个性化设置"),
        CategoryAttribute("其他设置"),
        Editor(typeof(SerialObjectEdit<StagConfigSetting>), typeof(UITypeEditor))]
        public StagConfigSetting StagSetting { get; set; }
        [DescriptionAttribute("是否需要合并同期机会作为一个机会"),
        DisplayName("同期合并机会"),
        CategoryAttribute("输出设置"),
        DefaultValueAttribute(false)]
        public bool MergeChances { get; set; }

        /// <summary>
        /// 策略配置
        /// </summary>
        [DescriptionAttribute("通用设置"),
        DisplayName("通用设置"),
        CategoryAttribute("其他设置"),
        Editor(typeof(SerialObjectEdit<SettingClass>), typeof(UITypeEditor))]
        public SettingClass CommSetting { get; set; }


        public string AssetUnitId { get; set; }


        /// <summary>
        /// 最后在用数据
        /// </summary>
        protected ExpectList<T> _LastUseData;
        public ExpectList<T> LastUseData()
        {
            return _LastUseData;
        }

        public void SetLastUserData(ExpectList<T> value)
        {
            _LastUseData = value;
        }


        public abstract List<ChanceClass<T>> getChances(BaseCollection<T> sc, ExpectData<T> ed);

        public abstract StagConfigSetting getInitStagSetting();

        protected DataTypePoint UsingDpt;
        public void setDataTypePoint(DataTypePoint dtp)
        {
            UsingDpt = dtp;
        }
            

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


        public static DataTable getAllStrags(string filterKey,ref Dictionary<string,Type> outTypes)
        {
            outTypes = new Dictionary<string, Type>();
            Assembly ass = typeof(BaseStragClass<T>).Assembly;
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
                string[] arr;
                if (!string.IsNullOrEmpty(filterKey))
                {
                    arr = t.FullName.Split('.');
                    if(!arr.ToList().Contains(filterKey))//没有包含关键字
                    {
                        continue;
                    }
                }
                while (t.BaseType != null)//寻找基类是stragclass的所有非抽象类
                {
                    if (t.BaseType.Name.Equals(typeof(BaseStragClass<T>).Name))
                    {
                        if (!AllType[i].IsAbstract)
                        {
                            outTypes.Add(AllType[i].FullName,AllType[i]);
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
                DisplayNameAttribute attribute = Attribute.GetCustomAttribute(types[i], typeof(DisplayNameAttribute), false) as DisplayNameAttribute;
                string[] names = types[i].FullName.Split('.');
                if (attribute != null)
                {
                    names = new string[] { attribute.DisplayName };
                }
                dr["text"] = names[names.Length - 1];
                dr["value"] = types[i].FullName.ToString();
                dr["class"] = types[i];
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public static BaseStragClass<T> getStragByName(string className)
        {
            Assembly asmb = typeof(BaseStragClass<T>).Assembly;// Assembly.LoadFrom("EnterpriseServerBase.dll");
            Type sct = asmb.GetType(className);
            BaseStragClass<T> sc = Activator.CreateInstance(sct) as BaseStragClass<T>;
            return sc;
        }

        public static DataTable GetTableByStragList(List<BaseStragClass<T>> Strags)
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
                BaseStragClass<T> jcls = Strags[i];
                DataRow dr = dt.NewRow();
                dr[0] = jcls.GUID;
                dr[1] = jcls.StragScript;
                dr[2] = jcls.BySer;
                dr[3] = jcls.StagSetting?.ToString();
                dr[4] = jcls.StragClassName;
                dr[5] = jcls.StragTypeName;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public bool IsBackTest;

        public object RunningPlan;

        public long allowInvestmentMaxValue;

        public abstract Type getTheChanceType();


    }
}
