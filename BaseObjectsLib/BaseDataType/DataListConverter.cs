using System.Collections.Generic;
using System.Data;
namespace WolfInv.com.BaseObjectsLib
{
    public abstract class DataListConverter
    {
   

        public static DataSet ToDataSet<T>(List<T> list,string keyName)
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

        public static MongoDataDictionary ToDirectionary<T>(List<T> list, string keyName)
        {
            MongoDataDictionary ret = new MongoDataDictionary();
            if (list == null)
                return null;
            for (int i = 0; i < list.Count; i++)
            {
                string key = (list[i] as DetailStringClass).getValue(keyName).ToString();
                List<IObjectId> sublist = new List<IObjectId>();
                if (!ret.ContainsKey(key))
                {
                    ret.Add(key, sublist);
                }
                sublist = ret[key];
                sublist.Add(list[i] as IObjectId);
                ret[key] = sublist;
            }
            return ret; 
        }
        
    }
}
