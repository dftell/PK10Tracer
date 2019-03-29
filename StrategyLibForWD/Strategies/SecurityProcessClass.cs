//using CFZQ_JRGCDB;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.StrategyLibForWD
{
    public class SecurityProcessClass
    {

        public BaseDataItemClass SecInfo;
        public bool Enable;
        public SecurityProcessClass()
        {
        }

        public SecurityProcessClass(BaseDataItemClass dr)
        {
            if (dr == null) return;
            SecInfo = dr;
            Enable = false;
        }
    }
}