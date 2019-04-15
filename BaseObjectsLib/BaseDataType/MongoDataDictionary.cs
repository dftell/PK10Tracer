using System.Collections.Generic;
using System.Linq;
namespace WolfInv.com.BaseObjectsLib
{
    public class MongoDataDictionary<T> : Dictionary<string, MongoReturnDataList<T>> where T : MongoData
    {
        public void Union(MongoDataDictionary<T> vals)
        {
            foreach (string key in vals.Keys)
            {
                if (!this.ContainsKey(key))
                    this.Add(key, vals[key]);
            }
        }

        public MongoDataDictionary()
        {
        }

        public MongoDataDictionary(MongoReturnDataList<T>[] inputdata)
        {
            inputdata.ToList().ForEach((a)=>{
                MongoReturnDataList<T> val = a;
                if (val == null || val.Count == 0)
                    return;
                this.Add((a[0] as ICodeData).code, a);//如果有一条及以上记录，加入
            });
        }
    }
}
