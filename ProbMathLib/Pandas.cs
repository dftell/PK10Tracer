using System;
using System.Collections.Generic;
namespace WolfInv.com.ProbMathLib
{
    public class Pandas
    {

        public static List<X> ShiftX<X>(List<X> thisList, int n)
        {
            List<X> ret = new List<X>();
            ret.AddRange(thisList.ToArray());
            if (n == 0)
                return ret;
            int cnt = ret.Count;
            
            if (n > 0)
            {
                while (ret.Count > cnt - Math.Abs(n))//先减掉行数
                {
                    ret.RemoveAt(ret.Count - 1);
                }
                for (int i = 0; i < n; i++)
                {
                    ret.Insert(0, default(X));
                }
            }
            else
            {
                for(int i=0;i<Math.Abs(n);i++)
                {
                    ret.RemoveAt(0);
                    ret.Add(default(X));
                }
                
            }
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
        public static List<T> Concat<T,TAdd, TSrcField>(List<T> src, List<TAdd> src1, Func<T, TSrcField> SrcIndexFunc, Func<TAdd, TSrcField> TargetIndexFunc, Action<T, TAdd, bool> OperaterFunc) where T:class where TAdd:class
        {
            List<T> ret = src;
            Dictionary<TSrcField, int> AllDic1 = new Dictionary<TSrcField, int>();
            for (int i = 0; i < src1.Count; i++)
            {
                //TField val =  src1.Loc<LastSubObjectSrc, TField>(i, LocTargetFunc, IndexName);
                TSrcField val = TargetIndexFunc(src1[i]);
                if (!AllDic1.ContainsKey(val))
                {
                    AllDic1.Add(val, i);
                }
            }
            for (int i = 0; i < ret.Count; i++)
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


        public static List<T> FillNa<T,TField, SubClass>(List<T> list,Func<T, SubClass> LocSubField, Func<T, TField> LocFieldFunc, Action<T, SubClass, TField> OperaterFunc, FillType fileType = FillType.None, TField DefaultVals = default(TField)) where T:class where SubClass :class
        {
            if (list.Count == 0)
                return list;
            //TField validVal = this.Loc<SubClass, TField>(0, LocSubField, field);
            TField validVal = LocFieldFunc(list[0]);
            SubClass ReplaceVal = default(SubClass);
            ReplaceVal = LocSubField(list[0]);
            if (fileType == FillType.None)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    TField CurrVal = LocFieldFunc(list[i]);
                    SubClass ReplacedVal = LocSubField(list[i]);
                    if (CurrVal == null || CurrVal.Equals(default(TField)))
                    {
                        OperaterFunc(list[i], ReplaceVal, CurrVal);
                    }
                }
            }
            else if (fileType == FillType.FFill)
            {


                for (int i = 0; i < list.Count; i++)
                {
                    //TField CurrVal = this.Loc<SubClass, TField>(i, LocSubField, field);
                    TField CurrVal = LocFieldFunc(list[i]);
                    SubClass ReplacedVal = LocSubField(list[i]);
                    if (CurrVal == null || CurrVal.Equals(default(TField)))
                    {
                        OperaterFunc(list[i], ReplaceVal, CurrVal);
                    }
                    else
                    {
                        ReplaceVal = ReplacedVal;
                    }
                }
            }
            else
            {
                validVal = LocFieldFunc(list[list.Count - 1]);
                ReplaceVal = LocSubField(list[list.Count - 1]);
                for (int i = list.Count - 2; i >= 0; i--)
                {
                    //TField CurrVal = this.Loc<SubClass, TField>(i, LocSubField, field);
                    TField CurrVal = LocFieldFunc(list[i]);
                    SubClass ReplacedVal = LocSubField(list[i]);
                    if (CurrVal == null || CurrVal.Equals(default(TField)))
                    {
                        //////if (validVal == null ||)//如果替换值为空，就算是空值也不管，需要吗？
                        //////{
                        //////    continue;
                        //////}
                        //ConvertionExtensions.SetValue<T,SubClass,TField>(this[i],fieldFunc, field, validVal);
                        //ReplacedVal = ReplaceVal;
                        OperaterFunc(list[i], ReplaceVal, CurrVal);
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
            return list;
        }

        public static List<double?> Cumprod(List<double?> input)
        {
            List<double?> ret = new List<double?>();
            if (input.Count <= 1)
                return input;
            double? sum = null;
            input.ForEach((a) => 
            {
                if (a == null)
                {
                    ret.Add(sum);
                    return;
                }
                double? val = null;
                if (sum == null)
                    val = a;
                else
                    val = sum * a;
                ret.Add(val);
                sum = val;
            });
            return ret;
        }

        public static List<T> Recovert<T>(List<T> list) 
        {
            T[] ret = new T[list.Count];
            int cnt = list.Count;
            ret[list.Count / 2 + 1] = list[list.Count / 2 + 1];
            for (int i=0;i<=list.Count/2;i++)
            {
                ret[i] = list[cnt -1 - i];
                ret[cnt - 1 - i] = list[i];
            }
            
            List<T> retlist = new List<T>();
            retlist.AddRange(ret);
            return retlist;
        }
    }

    public enum FillType
    {
        BFill,
        FFill,
        None
    }
}
