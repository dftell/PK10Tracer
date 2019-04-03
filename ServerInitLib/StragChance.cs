using System;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.Strags;
namespace WolfInv.com.ServerInitLib
{
    public class StragChance<T> : MarshalByRefObject where T : TimeSerialData
    {
        public BaseStragClass<T> Strag;
        public ChanceClass<T> Chance;
        public StragChance(BaseStragClass<T> _Strag, ChanceClass<T> _Chance)
        {
            Strag = _Strag;
            Chance = _Chance;
        }
    }
}
