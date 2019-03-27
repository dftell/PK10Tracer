using System;
using MongoDB.Driver;
using System.Collections.Generic;
using MongoDB.Bson;
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

        public MongoReturnDataList<T> ShiftX(int n)
        {
            MongoReturnDataList<T> ret = this;
            int cnt = ret.Count;
            while (ret.Count > cnt - Math.Abs(n))//先减掉行数
            {
                ret.RemoveAt(ret.Count - 1);
            }
            if (n > 0)
            {
                for (int i = 0; i < n; i++)
                {
                    ret.Insert(0, default(T));
                }
            }
            else
            {
                ret.Add(default(T));
            }
            return ret;
        }

        public object Clone()
        {
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>();
            this.ForEach(a => ret.Add(a));
            return ret;
        }
        public MongoReturnDataList<T> Copy()
        {
            MongoReturnDataList<T> ret = new MongoReturnDataList<T>();
            this.ForEach(a => ret.Add(a));
            return ret;
        }

        /// <summary>
        /// 连接，所有类型必须是MongoData类型
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <typeparam name="TAdd"></typeparam>
        /// <typeparam name="TField"></typeparam>
        /// <param name="src"></param>
        /// <param name="src1"></param>
        /// <param name="IndexType"></param>
        /// <param name="IndexName"></param>
        /// <returns></returns>
        public static MongoReturnDataList<T> Concat<TAdd,TSrcField>(MongoReturnDataList<T> src, MongoReturnDataList<TAdd> src1,Func<T, TSrcField> SrcIndexFunc,Func<TAdd, TSrcField> TargetIndexFunc,Action<T,TAdd,bool> OperaterFunc) where TAdd:MongoData
        {
            MongoReturnDataList<T> ret = src;
            Dictionary<TSrcField, int> AllDic1 = new Dictionary<TSrcField, int>();
            for(int i=0;i<src1.Count;i++)
            {
                //TField val =  src1.Loc<LastSubObjectSrc, TField>(i, LocTargetFunc, IndexName);
                TSrcField val = TargetIndexFunc(src1[i]);
                if (!AllDic1.ContainsKey(val))
                {
                    AllDic1.Add(val, i);
                }
            }
            for(int i=0;i<ret.Count;i++)
            {
                //TField val = ret.Loc<LastSubObject,TField>(i, LocSrcFunc, IndexName);
                TSrcField val = SrcIndexFunc(ret[i]);
                if (AllDic1.ContainsKey(val))//如果存在被加对象索引值所对应的行，将被加对象设为加入扩展对象
                {

                    OperaterFunc(ret[i], src1[AllDic1[val]], AllDic1.ContainsKey(val));
                }
                else
                {
                    OperaterFunc(ret[i], null, false);
                }
                
            }
            return ret;
        }

        public void FillNa<TField>(string field, FillType fileType = FillType.None, TField val = default(TField))
        {

            if (fileType == FillType.None)
                return;

            if (this.Count == 0)
                return;
            object lastvalue = val;
            TField validVal = this.Loc<TField>(0, field);
            if (val != null)
                validVal = val;
            if (fileType == FillType.FFill)
            {
                for (int i = 1; i < this.Count; i++)
                {
                    TField CurrVal = this.Loc<TField>(i, field);
                    if (CurrVal == null)
                    {
                        if (validVal == null)//如果替换值为空，就算是空值也不管，需要吗？
                        {
                            continue;
                        }
                        ConvertionExtensions.SetValue<T,TField>(this[i], field, validVal);
                    }
                    else
                    {
                        if (val == null)
                            validVal = CurrVal;
                    }
                }
            }
            else
            {
                validVal = this.Loc<TField>(this.Count-1, field);
                if (val != null)
                    validVal = val;
                for (int i=this.Count-2;i>=0;i--)
                {
                    TField CurrVal = this.Loc<TField>(i, field);
                    if (CurrVal == null)
                    {
                        if (validVal == null)//如果替换值为空，就算是空值也不管，需要吗？
                        {
                            continue;
                        }
                        ConvertionExtensions.SetValue<T, TField>(this[i], field, validVal);
                    }
                    else
                    {
                        if (val == null)
                            validVal = CurrVal;
                    }
                }
            }
        }

        public void FillNa<TField, SubClass>(Func<T, SubClass> LocSubField, Func<T, TField> LocFieldFunc, Action<T, SubClass,TField> OperaterFunc, FillType fileType = FillType.None, TField DefaultVals = default(TField))
        {

            if (fileType == FillType.None)
                return;

            if (this.Count == 0)
                return;
            //TField validVal = this.Loc<SubClass, TField>(0, LocSubField, field);
            TField validVal = LocFieldFunc(this[0]);
            SubClass ReplaceVal = default(SubClass);
            if (DefaultVals != null )
            {
                OperaterFunc(this[0], ReplaceVal,DefaultVals);
            }
            if (fileType == FillType.FFill)
            {
                for (int i = 1; i < this.Count; i++)
                {
                    //TField CurrVal = this.Loc<SubClass, TField>(i, LocSubField, field);
                    TField CurrVal = LocFieldFunc(this[i]);
                    SubClass ReplacedVal = LocSubField(this[i]);
                    if (CurrVal == null || CurrVal.Equals(default(TField)))
                    {
                        //////if (validVal == null ||)//如果替换值为空，就算是空值也不管，需要吗？
                        //////{
                        //////    continue;
                        //////}
                        //ConvertionExtensions.SetValue<T,SubClass,TField>(this[i],fieldFunc, field, validVal);
                        //ReplacedVal = ReplaceVal;
                        OperaterFunc(this[i], ReplaceVal,CurrVal);
                    }
                    else
                    {
                        //if (val == null || val.Equals(default(TField)))
                        //{
                        //    //validVal = CurrVal;
                        //ReplacedVal = ReplaceVal;
                        ReplaceVal = ReplacedVal;

                        //}
                    }
                }
            }
            else
            {
                validVal = LocFieldFunc(this[this.Count-1]);
                ReplaceVal = default(SubClass);
                if (DefaultVals != null)
                {
                    OperaterFunc(this[this.Count - 1], ReplaceVal, DefaultVals);
                }
                for (int i = Count-2; i >0; i--)
                {
                    //TField CurrVal = this.Loc<SubClass, TField>(i, LocSubField, field);
                    TField CurrVal = LocFieldFunc(this[i]);
                    SubClass ReplacedVal = LocSubField(this[i]);
                    if (CurrVal == null || CurrVal.Equals(default(TField)))
                    {
                        //////if (validVal == null ||)//如果替换值为空，就算是空值也不管，需要吗？
                        //////{
                        //////    continue;
                        //////}
                        //ConvertionExtensions.SetValue<T,SubClass,TField>(this[i],fieldFunc, field, validVal);
                        //ReplacedVal = ReplaceVal;
                        OperaterFunc(this[i], ReplaceVal, CurrVal);
                    }
                    else
                    {
                        //if (val == null || val.Equals(default(TField)))
                        //{
                        //    //validVal = CurrVal;
                        //ReplacedVal = ReplaceVal;
                        ReplaceVal = ReplacedVal;

                        //}
                    }
                }
            }
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

        bool Updated = false;

        
        public List<TField> ToList<TField>(Func<T,TField> func)
        {
            List<TField> ret = new List<TField>();
            this.ForEach(a=>ret.Add(func(a)));
            return ret;
        }

    }

    public enum FillType
    {
        BFill,
        FFill,
        None
    }
    

}
;
