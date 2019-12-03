using WolfInv.com.WinInterComminuteLib;

namespace WolfInv.com.ChargeLib
{
    public class ChargeRemoteClass : RemoteServerClass
    {
        static ChargeOperator _Operate;
        public ChargeOperator Operate
        {
            get
            {
                return _Operate;
            }
            set
            {
                _Operate = value;
            }
        }
    }

}
