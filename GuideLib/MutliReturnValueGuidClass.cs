using WolfInv.com.GuideLib;
namespace WolfInv.com.GuideLib
{
    /// <summary>
    /// 多返回值指标类
    /// </summary>
    public abstract class MutliReturnValueGuidClass : GuidBaseClass
    {
        int IOType;
        public string ReturnValueName;
        protected abstract int getIOValue();
        protected abstract void InitClass();
    }

}

