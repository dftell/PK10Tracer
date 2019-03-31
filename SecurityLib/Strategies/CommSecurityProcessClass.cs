//using CFZQ_JRGCDB;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public class CommSecurityProcessClass
    {

        public BaseDataItemClass SecInfo;
        public bool Enable;
        public CommSecurityProcessClass()
        {
        }

        public CommSecurityProcessClass(BaseDataItemClass dr)
        {
            if (dr == null) return;
            SecInfo = dr;
            Enable = false;
        }
    }
}