using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.GuideLib
{
    public abstract class CommDataIntface<T> where T:TimeSerialData
    {
        public abstract MongoDataDictionary<T> getData();
    }

    public class SecurityDataInterface<T>:CommDataIntface<T> where T:TimeSerialData
    {
        MongoDataDictionary<T> Data;
        public SecurityDataInterface(MongoDataDictionary<T> data)
        {
            Data = data;
        }

        public SecurityDataInterface(ExpectList<T> data)
        {
            Data = new MongoDataDictionary<T>(data);
        }

        public override MongoDataDictionary<T> getData()
        {
            return Data;
        }
    }

}

