using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
using BaseObjectsLib;
using System.ComponentModel;
using System.Drawing.Design;
namespace Strags
{
    [DisplayName("策略个性化设置")]
    public class StagConfigSetting : DetailStringClass
    {
        [DescriptionAttribute("基本设置"),
        DisplayName("基本设置"),
        CategoryAttribute("设置"),
        Editor(typeof(SerialObjectEdit<BaseTypeSetting>), typeof(UITypeEditor))]
        public BaseTypeSetting BaseType{get;set;}
        /// <summary>
        /// 长期统计，内部计算时前最小长度期需要加入最小长度
        /// </summary>
        public bool IsLongTermCalc { get; set; }
        //public bool InHolding { get; set; }
        public StagConfigSetting()
        {
            BaseType = new BaseTypeSetting();
        }
    }

    [DisplayName("策略基本设置")]
    public class BaseTypeSetting : DetailStringClass
    {
        /// <summary>
        /// 跟踪类型
        /// </summary>
        public TraceType traceType { get; set; }
 
        /// <summary>
        /// 下注序列，适合单利连续追踪
        /// </summary>
        public List<int> ChipSerial{get;set;}
        /// <summary>
        /// 单注下注比例，适合复利
        /// </summary>
        public double ChipRate{get;set;}
        /// <summary>
        /// 单注固定金额，适合单利
        /// </summary>
        public int ChipFixAmount{get;set;}
        /// <summary>
        /// 允许持有次数
        /// 无限追击为无穷，波动追击为1
        /// </summary>
        public int AllowHoldTimeCnt{get;set;}
    }
}
