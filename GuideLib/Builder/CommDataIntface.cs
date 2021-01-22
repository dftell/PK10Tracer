using System.Collections.Concurrent;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.GuideLib
{
    public abstract class CommDataIntface<T> where T:TimeSerialData
    {
        public abstract ConcurrentDictionary<string, MongoReturnDataList<T>> getData();
    }

    public class SecurityDataInterface<T>:CommDataIntface<T> where T:TimeSerialData
    {
        ConcurrentDictionary<string, MongoReturnDataList<T>> Data;
        public SecurityDataInterface(MongoDataDictionary<T> data)
        {
            Data = data;
        }

        public SecurityDataInterface(ExpectList<T> data)
        {
            Data = data.MongoData;//new MongoDataDictionary<T>(data,true);
        }

        public override ConcurrentDictionary<string, MongoReturnDataList<T>> getData()
        {
            return Data;
        }
    }

}

