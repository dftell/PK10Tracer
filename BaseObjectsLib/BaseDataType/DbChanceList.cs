using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using WolfInv.com.LogLib;
using System.Reflection;
namespace WolfInv.com.BaseObjectsLib
{
    public class DbChanceList<T> : DisplayAsTableClass,IDictionary<Int64?, ChanceClass<T>> where T : TimeSerialData
    {
        DataTable _dt;
        Dictionary<Int64?, ChanceClass<T>> list;
        public DbChanceList()
        {
            list = new Dictionary<Int64?, ChanceClass<T>>();
        }

        public DbChanceList(DataTable dt)
        {
            list = new Dictionary<Int64?, ChanceClass<T>>();
            FillByTable(dt);
        }

        public void  Add(long? key, ChanceClass<T> value)
        {
            if(key == null)
            {
                return;
            }
            if (list.ContainsKey(key))
            {
                LogableClass.ToLog("错误","插入机会错误","存在相同的Key");
                return;
            }
            list.Add(key, value);
        }

        public bool  ContainsKey(long? key)
        {
            return list.ContainsKey(key);
        }

        public ICollection<long?>  Keys
        {
            get { return list.Keys; }
        }

        public bool  Remove(long? key)
        {
            if (!list.ContainsKey(key))
            {
                return false;
            }
            list.Remove(key);
            return true;
        }

        public bool  TryGetValue(long? key, out ChanceClass<T> value)
        {
            value = null;
            if (!list.ContainsKey(key))
            {
                return false;
            }
            value = list[key];
            return true;
        }

        public ICollection<ChanceClass<T>>  Values
        {
            get { return list.Values; }
        }

        public ChanceClass<T>  this[long? key]
        {
	        get 
	        {
                if(list.ContainsKey(key))
                    return list[key];
                return null;
	        }
	          set 
	        {
                if (list.ContainsKey(key))
                {
                    list[key] = value;
                }
	        }
        }

        public void  Add(KeyValuePair<long?,ChanceClass<T>> item)
        {
            if (!list.ContainsKey(item.Key))
            {
                list.Add(item.Key,item.Value);
            }
        }

        public void  Clear()
        {
            list.Clear();
        }

        public bool  Contains(KeyValuePair<long?,ChanceClass<T>> item)
        {
            if (list.ContainsKey(item.Key) && list[item.Key].Equals(item.Value))
            {
                return true;
            }
            return false;
        }

        public void  CopyTo(KeyValuePair<long?,ChanceClass<T>>[] array, int arrayIndex)
        {
            list.Select(t => t);
        }

        public int  Count
        {
            get { return list.Count; }
        }

        public bool  IsReadOnly
        {
            get { return true; }
        }

        public bool  Remove(KeyValuePair<long?,ChanceClass<T>> item)
        {
            if (list.ContainsKey(item.Key))
            {
                list.Remove(item.Key);
                return true;
            }
            return false;
        }

        public IEnumerator<KeyValuePair<long?,ChanceClass<T>>>  GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator  System.Collections.IEnumerable.GetEnumerator()
        {
 	        throw new NotImplementedException();
        }


        static List<MemberInfo> TableBuffs;

        public void FillByTable(DataTable dt)
        {
            _dt = dt;
            List<ChanceClass<T>> ret = this.FillByTable<ChanceClass<T>>(dt,ref TableBuffs);
            string str = "";
            TableBuffs.ForEach(t=>str= str + "," + t.Name);
            //Log("获取到表",str);
            for (int i = 0; i < ret.Count; i++)
            {
                //Log("未关闭数据展示", string.Format("idx:{0};code:{1}",ret[i].ChanceIndex,ret[i].ChanceCode));
                if (!this.ContainsKey(ret[i].ChanceIndex))
                {
                    this.Add(ret[i].ChanceIndex, ret[i]);
                }
                else
                {
                    Log("错误","机会已存在", string.Format("Idx:{0};Sid:{1};Code:{2}",ret[i].ChanceIndex,ret[i].StragId,ret[i].ChanceCode));
                }
            }
            //list = ret.ToDictionary(t=>t.ChanceIndex,t=>t);
        }

        public DataTable Table
        {
            get
            {
                if (_dt == null)
                {
                    _dt = ToTable<ChanceClass<T>>(list.Values.ToList<ChanceClass<T>>(), false);
                }
                return _dt;
            }
        }

        ////public int Save(string DataOwner)
        ////{
        ////    return new PK10ExpectReader().SaveChances(list.Values.ToList<ChanceClass>(), DataOwner);
        ////}

        
    }


    
}
