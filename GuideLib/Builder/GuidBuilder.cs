using System;

using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace WolfInv.com.GuideLib
{
    /// <summary>
    /// 多维指标工厂类
    /// </summary>
    public abstract class GuidBuilder : CommDataBuilder
    {
        public GuidBuilder(CommDataIntface cdi, GuidBaseClass guidClass):base(cdi,guidClass)
        {
            
        }

        public abstract MTable getRecords(string[] sectors, DateTime dt);
        
    }

}

