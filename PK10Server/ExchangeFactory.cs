using System.Collections.Generic;
using WolfInv.com.BaseObjectsLib;
namespace PK10Server
{
    class ExchangeFactory<T> where T:TimeSerialData
    {
        public GlobalSettingClass gobj;
        public SettingClass setting;
        public CurrInfo<T> currInfo;
        public List<ChanceClass<T>> ExecExchange(ExpectData<T> data)
        {
            List<ChanceClass<T>> ret = new List<ChanceClass<T>>();
            //////////ExchanceClass ec = new ExchanceClass();
            //////////for (int i = 0; i < gobj.CurrStags.Count; i++)
            //////////{

            //////////}
            return ret;
        }
    }

    
}
