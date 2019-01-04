using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using BaseObjectsLib;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Timers;
using System.Windows.Forms.Design;
using System.Xml;
using System.Xml.Serialization;
using System.Data;
using System.Reflection;
using PK10CorePress;
using Strags;
namespace ExchangeLib
{
    /// <summary>
    /// 策略运行计划类
    /// </summary>
    [Serializable]
    [DescriptionAttribute("策略运行计划类，包括服务开始后是否自动运行，策略执行到期时间"),
        DisplayName("策略运行计划")]
    public class StragRunPlanClass : DisplayAsTableClass,iDbFile
    {

        public StragRunPlanClass():base()
        {
            
        }
        string _guid;
        [DisplayName("GUID"),
        Category("计划基本信息"),
        Description("计划编号")]
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
                if (_guid == null || _guid.Trim().Length == 0)
                    _guid = value;
            }
        }
        [DisplayName("计划名称"),
        Category("计划基本信息"),
        Description("计划名称")]
        public string Plan_Name
        {
            get;
            set;
        }

        [DisplayName("策略信息"),
        Category("策略信息"),
        Description("策略编号"),
        Editor(typeof(StagPickerEditor), typeof(UITypeEditor))]
        public StragClass PlanStrag { get; set; }
        [DisplayName("策略GUID"),
        Category("策略信息"),
        Description("策略编号")]
        public string Strag_GUID { get { if (PlanStrag != null) return PlanStrag.GUID; return ""; } }
        

        [DisplayName("策略名称"),
        Category("策略信息"),
        Description("策略名称")]
        public string StragName { get { if (PlanStrag != null) return PlanStrag.StragTypeName; return ""; } }

        [DisplayName("策略描述"),
        Category("策略信息"),
        Description("策略描述")]
        public string StragDescript { get { if (PlanStrag != null) return PlanStrag.StragScript; return ""; } }

        [DisplayName("是否自动运行"),
        Category("运行设置"),
        Description(@"当服务载入策略后 
                    true:自动运行策略;
                    false:需手动开始此策略")]
        public bool AutoRunning{get;set;}
        [DisplayName("策略到期时间"),
        Category("运行设置"),
        Description("超过改时间的策略无法载入"),
        DefaultValueAttribute(typeof(DateTime),"2049/12/31")]
        public DateTime ExpiredTime { get; set; }

        


        [DisplayName("每日策略开始时间"),
        Category("运行设置"),
        Description("该时刻以前的时间策略不执行"),
        Editor(typeof(TimePickerEditor),typeof(UITypeEditor)),
        DefaultValueAttribute("09:07:00")]
        public string DailyStartTime { get; set; }

        [DisplayName("每日策略结束时间"),
        Category("运行设置"),
        Description("该时刻以后的时间策略不执行"),
        Editor(typeof(TimePickerEditor), typeof(UITypeEditor)),
        DefaultValueAttribute("23:59:00")]
        public string DailyEndTime { get; set; }

        [DisplayName("下注类型"),
        Category("输出类型"),
        Description("0，一次性机会，1，正常跟踪机会，2，对冲机会"),
        DefaultValueAttribute(1)]
        public int OutPutType { get; set; }

        [DisplayName("增长类型"),
        Category("运行设置"),
        Description("复利/单利")]
        public InterestType IncreamType { get; set; }

        [DisplayName("所属资产单元"),
        Category("资金管理"),
        Description("如果是复利，必须加入资产单元，以便按复利进行投资"),
        Editor(typeof(CommPickerEditor<AssetUnitClass>), typeof(UITypeEditor))]
        public AssetUnitClass AssetUnitInfo{ get; set; }


        [DisplayName("固定比例"),
        Category("资金管理"),
        Description("如果是复利，设置固定的比例，如果不设置，将自动按保本比例计算"),
        DefaultValueAttribute(0.01)]
        public double? FixRate  { get; set; }

        [DisplayName("初次下注金额"),
        Category("资金管理"),
        Description("如果是单利，设置初次下注金额，如果不设置，将自动按系统设置的初始金额下注"),
        DefaultValueAttribute(1)]
        public Int64? FixAmt { get; set; }

        [DisplayName("初始资金"),
        Category("资金管理"),
        Description("初始资金"),
        DefaultValueAttribute(20000)]
        public Int64 InitCash { get; set; }

        [DisplayName("允许亏损(金额)"),
        Category("资金管理"),
        Description("策略资金上限"),
        DefaultValueAttribute(20000)]
        public Int64 MaxLostAmount { get; set; }

        [DisplayName("最大亏损比(%)"),
        Category("资金管理"),
        Description("允许资金最大亏损比例%（相对初始资金）"),
        DefaultValueAttribute(100)]
        public double MaxLostRate_Pre { get; set; }

        /// <summary>
        /// 允许最大持仓次数
        /// </summary>
        [DescriptionAttribute("允许选出机会最大持仓次数"),
        DisplayName("允许选出机会最大持仓次数"),
        CategoryAttribute("持仓设置"),
        DefaultValueAttribute(100)]
        public int AllowMaxHoldTimeCnt { get; set; }

        [DisplayName("计划建立时间"),
        Category("计划基本信息"),
        Description("计划建立时间")]
        public DateTime CreateTime { get; set; }

        [DisplayName("计划所属人"),
        Category("计划基本信息"),
        Description("计划建立人"),
        DefaultValueAttribute("Admin")]
        public string Creator { get; set; }

        public bool Running;
        public List<StragRunPlanClass> getPlanListByXml(string strXml)
        {
            return null;
        }

            #region  支持propertygrid默认信息
        static List<StragClass> DBfiles;

        static StragRunPlanClass()
        {
            DBfiles = new List<StragClass>();
            _strDbXml = GlobalClass.getStragRunningPlan(true);
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
            get { return true; }
        }

        public string strKey
        {
            get { return "GUID"; }
        }

        public bool SaveDBFile<T>(List<T> list)
        {
            return GlobalClass.setStragRunningPlan(getXmlByObjectList<T>(list));
        }

        public string strKeyValue()
        {
            return this.GUID;
        }

        public string strObjectName
        {
            get { return "策略运行计划"; }
        }
        #endregion

    }

    public class TimePickerEditor : UITypeEditor
    {
	    IWindowsFormsEditorService editorService;
	    DateTimePicker picker = new DateTimePicker();

	    public TimePickerEditor()
	    {
	        picker.Format = DateTimePickerFormat.Custom;
	        picker.CustomFormat = "HH:mm";
	        picker.ShowUpDown = true;
	    }

	    public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
	    {
	        return UITypeEditorEditStyle.DropDown;//显示下拉按钮
	    }

	    public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
	    {
	        string time;
	        if (provider != null)
	        {
		    this.editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
	        }
	        if (this.editorService != null)
	        {
		        if (value == null)
		        {
		            picker.Value = DateTime.Parse("00:00" as string);
		        }
		        else
		        {
		            time = value.ToString();
		            //picker.Value = DateTime.Parse(time);
		        }
		        this.editorService.DropDownControl(picker);
		        value = picker.Value.ToShortTimeString();
	        }
	        return value;
	    }
    }

    public class StagPickerEditor : UITypeEditor
    {
        List<StragClass> AllList;
        Dictionary<string,StragRunPlanClass> AllPlans;
        IWindowsFormsEditorService editorService;
        StragPicker picker ;
        public StagPickerEditor()
        {
            
            //picker = new StragsPicker(list.ToList<StragClass>());
        }

       

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
	        {
		        this.editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
	        }
            if (this.editorService != null)
            {
                AllList = StragClass.getObjectListByXml<StragClass>(GlobalClass.ReReadStragList());
                AllPlans = StragRunPlanClass.getObjectListByXml<StragRunPlanClass>(GlobalClass.getStragRunningPlan(true)).ToDictionary(t => t.GUID, t => t);
                List<StragClass> list = AllList.Where(t => AllPlans.ContainsKey(t.GUID) == false).ToList<StragClass>();
                picker = new StragPicker(AllList);//支持一组合对多相同策略
                editorService.ShowDialog(picker);
                if (picker.SelectedStrag == null)
                {
                    value = null;
                }
                else
                {
                    value = picker.SelectedStrag;
                    //picker.Hide();
                }
                ////if (value == null)
                ////{
                ////    picker.Show();
                ////    picker.Visible = true;
                ////}
                ////else
                ////{
                ////}
            }
            return value;
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return false;
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;//显示下拉按钮
        }
    }

  
}
