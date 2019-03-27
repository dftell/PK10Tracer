using System.Collections.Generic;
namespace WolfInv.com.BaseObjectsLib
{
    public class MongoDataDictionary<T> : Dictionary<string, MongoReturnDataList<T>> where T:MongoData
    {

    }
}
