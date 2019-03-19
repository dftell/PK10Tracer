using System;
//using System.Web.Mvc;
using System.ComponentModel;
namespace WolfInv.com.BaseObjectsLib
{

    [DescriptionAttribute("增长类型")]
    [Serializable]
    public enum InterestType
    {
        [DescriptionAttribute("单利")]
        SimpleInterest,
        [DescriptionAttribute("复利")]
        CompoundInterest
    }

    [DescriptionAttribute("追踪类型")]
    [Serializable]
    public enum TraceType
    {
        [DescriptionAttribute("穷追")]
        NoLimitTrace,
        [DescriptionAttribute("择时")]
        WaveTrace,
        [DescriptionAttribute("单次")]
        OnceTrace
    }
}
