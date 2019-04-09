using System;
using MongoDB.Driver;
using System.Collections.Generic;
using MongoDB.Bson;
using System.Data;
using System.Linq;
namespace WolfInv.com.BaseObjectsLib
{
    public class MongoReturnDataList<T> : List<T>, ICloneable where T : MongoData
    {
        public MongoReturnDataList(List<T> list)
        {
            list?.ForEach(p => this.Add(p));
        }

        public MongoReturnDataList()
        {

        }

        public DataTable GetMainDataTable()
        {
            DataTable dt = DisplayAsTableClass.ToTable<T>(this);
            return dt;
        }

        public DataTable GetExtendDataTable<EXT>() where EXT:MongoData
        { 
            List<EXT> Exlist = this.ToList<EXT>(a => a.ExtentData as EXT);
            DataTable dtExData = DisplayAsTableClass.ToTable<EXT>(Exlist);
            return dtExData;
        }

        public MongoReturnDataList<T> Query<Field>(string col,Field val) 
        {
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>();
            try
            {
                BsonElement bs = new BsonElement(col,BsonValue.Create(val));
                //LogLib.LogableClass.ToLog("查询条件", string.Format("{0}=>{1}",col,val));
                List<T> list = this.FindAll(p => p.Match(bs)==true);
                
                //this.Clear();
                list?.ForEach(p => ret.Add(p));
            }
            catch(Exception ce)
            {
                LogLib.LogableClass.ToLog(string.Format("结果链查询错误[{0}]", ce.Message),ce.StackTrace);
            }
            return ret;
        }

        public MongoReturnDataList<T> Query(BsonDocument func)
        {
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>();
            try
            {
                List<T> list = this.FindAll(p => (p as MongoData).Match(func));
                //this.Clear();
                list.ForEach(p => ret.Add(p));
            }
            catch (Exception ce)
            {
                LogLib.LogableClass.ToLog("结果链查询", ce.Message);
            }
            return ret;
        }

        public object Clone()
        {
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>();
            this.ForEach(a => ret.Add(a.Clone() as T));
            return ret;
        }
        public MongoReturnDataList<T> Copy(bool Deeply = false)
        {            MongoReturnDataList<T> ret = new MongoReturnDataList<T>();
            this.ForEach(a => ret.Add(Deeply ? a.Clone<T>():a));
            return ret;
        }

        public List<TField> Loc<TField>(int[] indies, string FieldName)
        {
            List<TField> ret = new List<TField>();
            foreach (int i in indies)
            {
                T doc = this[i];
                ret.Add(ConvertionExtensions.GetValue<T, TField>(doc, FieldName));
            }
            return ret;
        }

        /// <summary>
        /// 定位
        /// </summary>
        /// <typeparam name="T">本身类</typeparam>
        /// <typeparam name="SubT">子对象</typeparam>
        /// <typeparam name="TField">定位字段名</typeparam>
        /// <param name="indies"></param>
        /// <param name="func"></param>
        /// <param name="FieldName"></param>
        /// <returns></returns>
        public List<TField> Loc<Tdata, TField>(int[] indies,Func<T,Tdata> func,string FieldName)
        {
            List<TField> ret = new List<TField>();
            foreach (int i in indies)
            {
                T doc = this[i];
                ret.Add(ConvertionExtensions.GetValue<Tdata, TField>(func(this[i]), FieldName));
            }
            return ret;
        }

        public TField Loc<TField>(int index, string fieldname)
        {
            int[] inds = new int[] { index };
            List<TField> list = Loc<TField>(inds, fieldname);
            if (list == null)
                return default(TField);
            return list[0];
        }



        
        public List<TField> ToList<TField>(Func<T,TField> func)
        {
            List<TField> ret = new List<TField>();
            this.ForEach(a=>ret.Add(func(a)));
            return ret;
        }

        public MongoReturnDataList<T> GetDataByIndies(int[] indies)
        {
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>();
            for (int i=0;i< indies.Length; i++)
            {
                ret.Add(this[indies[i]]);
            }
            return ret;
        }

        public MongoReturnDataList<T> GetFirstData(int len)
        {
            int[] Arr = new int[len];
            for (int i = 0; i < len; i++)
                Arr[i] = i;
            return GetDataByIndies(Arr);
        }

        public MongoReturnDataList<T> GetLastData(int len)
        {
            int[] Arr = new int[len];
            for (int i = this.Count-len; i < Count; i++)
                Arr[i-(this.Count - len)] = i;
            return GetDataByIndies(Arr);
        }

    }

    
    

}
;
