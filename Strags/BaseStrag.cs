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
namespace Strags
{
    

    /// <summary>
    /// 策略基类
    /// </summary>
    public abstract class BaseStragClass : DisplayAsTableClass
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
        public int MaxHoldCnt;

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
        
        
        


        /// <summary>
        /// 最后在用数据
        /// </summary>
        ExpectList _LastUseData;
        public ExpectList LastUseData()
        {
            return _LastUseData;
        }

        public void SetLastUserData(ExpectList value)
        {
            _LastUseData = value;
        }
   }
}
