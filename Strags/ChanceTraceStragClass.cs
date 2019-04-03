using WolfInv.com.PK10CorePress;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.Strags
{
    public abstract class ChanceTraceStragClass :StragClass
    {
        bool _IsTracing;
        public bool IsTracing
        {
            get
            {
                return _IsTracing;
            }
            set
            {
                _IsTracing = value;
            }
        }
       
    }
}
