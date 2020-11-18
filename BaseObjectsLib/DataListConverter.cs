using System.Collections.Generic;
using System.Data;
namespace WolfInv.com.BaseObjectsLib
{
    public abstract class DataListConverter<T> where T: TimeSerialData
    {
   

        public static DataSet ToDataSet(List<T> list,string keyName)
     {
            DataSet ds = new DataSet();
            if (list == null)
                return null;
            Dictionary<string, List<T>> AllList = new Dictionary<string, List<T>>();
            for (int i = 0; i < list.Count; i++)
            {
                string key = (list[i] as DetailStringClass).getValue(keyName).ToString();
                List<T> sublist = new List<T>();
                if (!AllList.ContainsKey(key))
                {
                    AllList.Add(key, sublist);
                }
                sublist = AllList[key];
                sublist.Add(list[i]);
                AllList[key] = sublist;
            }
            foreach (string key in AllList.Keys)
            {
                DataTable dt = DisplayAsTableClass.ToTable<T>(AllList[key]);
                dt.TableName = key;
                ds.Tables.Add(dt);
            }
            return ds;
        }

        public static MongoDataDictionary<T> ToDirectionary(MongoReturnDataList<T> list, string keyName)
        {
            MongoDataDictionary<T> ret = new MongoDataDictionary<T>();
            if (list == null)
                return null;
            for (int i = 0; i < list.Count; i++)
            {
                DetailStringClass obj = list[i] as DetailStringClass;
                string key = obj.getValue(keyName)?.ToString();
                MongoReturnDataList<T> sublist = new MongoReturnDataList<T>();
                if (!ret.ContainsKey(key))
                {
                    ret.Add(key, sublist);
                }
                sublist = ret[key];
                sublist.Add(list[i]);
                ret[key] = sublist;
            }
            return ret; 
        }
        
    }
}
