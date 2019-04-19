using System;
using System.Collections.Generic;
using System.Text;
using WolfInv.com.BaseObjectsLib;
using System.Reflection;
namespace WolfInv.com.BaseObjectsLib
{
    public delegate bool EventCheckNeedEndTheChance<T>(ChanceClass<T> CheckCc, bool LastExpectMatched) where T : TimeSerialData;

    public class ChanceClass<T> : BaseChance<T>,IConvertible where T:TimeSerialData
    {
        public ChanceClass():base()
        {
            ChanceIndex = null;
        }
        #region 以下字段支持现场恢复，在数据库中保存
        public string StragId { get; set; }
        public string UserId { get; set; }
        public double Odds { get; set; }
        public int IsTracer { get; set; }//db字段
        public double MinWinRate { get; set; }
        public InterestType IncrementType { get; set; }
        public double? FixRate { get; set; }
        public Int64? FixAmt { get; set; }
        string _guid;
        public string GUID //统一处理用
        {
            get
            {
                if (_guid == null || _guid.Trim().Length == 0)
                {
                    _guid = Guid.NewGuid().ToString();
                }
                return _guid;
            }
            set
            {
                if (_guid == null || _guid.Trim().Length == 0)
                    _guid = value;
            }
        }
        /// <summary>
        /// 自我跟踪
        /// </summary>
        public bool Tracerable
        {
            get
            {
                if (IsTracer == 1)
                {
                    return true;
                }
                return false;
            }
        }
        

        #endregion

        #region 以下属性支持回测以及计算
        public int MatchChips =0;
        public int AllowMaxHoldTimeCnt { get; set; }
        public int LastMatchTimesId { get; set; }

        public ExpectData<T> InputExpect;
        //public string FromStrag;
        //public string StragParams;
        public bool NeedConditionEnd { get; set; }
        //public int MaxHoldTimeCnt;
        
        public List<int> MatchTimesList = new List<int>();
        public EventCheckNeedEndTheChance<T> OnCheckTheChance;

        public string AssetId { get; set; }//增加所属资产单元id
        #endregion

        
        //以下为trace属性
        public bool Matched(ExpectData<T> data)
        {
            int tmp = 0;
            return Matched(data, out tmp);
        }

        public bool Matched(ExpectData<T> data, out int MatchCnt,bool getRev)
        {
            //MatchCnt = 0;
            ExpectList<T> el = new ExpectList<T>();
            el.Add(data);
            return Matched(el, out MatchCnt, getRev);
        }

        public bool Matched(ExpectData<T> data, out int MatchCnt)
        {
            //MatchCnt = 0;
            ExpectList<T> el = new ExpectList<T>();
            el.Add(data);
            return Matched(el, out MatchCnt,false);
        }

        public bool Matched(ExpectList<T> data, out int MatchCnt)
        {
            MatchCnt = 0;
            return Matched(data, out MatchCnt, false);
        }

        public bool Matched(ExpectList<T> el, out int MatchCnt, bool getRev) 
        {
            //ExpectData data = el.LastData;
            string[] strArr = ChanceCode.Split('+');
            MatchCnt = 0;
            int begid = -1;//默认为0，多期再去寻找起点
            if (el.Count > 1)//多期考虑
            {
                if (long.Parse(this.ExpectCode) < long.Parse(el.FirstData.Expect))
                {
                    Log("错误", "匹配是否需要关闭错误", "需要关闭的机会早于数据以前,无论是否中奖，立即关闭！");
                    return true;
                }
                begid = el.IndexOf(this.ExpectCode);
                if (begid < 0)
                {
                    Log("错误", "匹配是否需要关闭错误", "需要关闭的机会不在数据中,无论是否中奖，立即关闭！");
                    return true;
                }
            }
            //////ExpectData inputEd = el[begid];
            //////Log("计算服务", "获取到期号信息", string.Format("expect:{0};openCode:{1}",inputEd.Expect,inputEd.OpenCode));
            for (int ei = begid+1; ei < el.Count; ei++)
            {
                ExpectData<T> data = el[ei];
                for (int k = 0; k < strArr.Length; k++)
                {
                    //Log("计算服务", string.Format("循环检查进入期数后的期数是否命中机会：{0}", ChanceCode), string.Format("expect:{0};openCode:{1}", data.Expect, data.OpenCode));
                    string UseCode = getRev ? getRevChance(strArr[k]) : strArr[k];
                    string[] arr = UseCode.Trim().Split('/');
                    string strSer = arr[0].Trim();
                    string strCar = arr[1].Trim();
                    for (int i = 0; i < strSer.Length; i++)
                    {
                        string strSerNo = strSer.Substring(i, 1).Trim();
                        int iNo = int.Parse(strSerNo);
                        if (iNo == 0)
                            iNo = 10;
                        for (int j = 0; j < strCar.Length; j++)
                        {
                            string strCarNo = strCar.Substring(j, 1).Trim();
                            if (strCarNo == data.ValueList[iNo - 1])
                            {
                                MatchCnt++;
                            }
                        }
                        
                    }
                   
                }
                if (MatchCnt > 0)//任何一期命中都需要关闭
                    break;
            }
            if (MatchCnt > 0)
                return true;
            return false;
        }

        public bool isPure
        {
            get
            {
                if (ChanceCode == null || ChanceCode.Trim().Length == 0)
                {
                    throw new Exception("机会为空！");
                }
                if (ChanceCode.IndexOf("+") > 0)
                {
                    return false;
                }
                string[] arr = ChanceCode.Trim().Split('/');
                if (arr[0].Length > 0 && arr[1].Length > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public static bool IsPureCode(string code)
        {
            if (code == null || code.Trim().Length == 0)
            {
                throw new Exception("机会为空！");
            }
            if (code.IndexOf("+") > 0)
            {
                return false;
            }
            string[] arr = code.Trim().Split('/');
            if (arr[0].Length > 1 && arr[1].Length > 1)
            {
                return false;
            }
            return true;
        }

        public static string getRevChance(string code)
        {
            if (!IsPureCode(code)) return code;
            string[] arr = code.Trim().Split('/');
            string Lcode = arr[0].Trim().Length > 1 ? arr[0] : arr[1];
            string strModel = "1234567890";
            string strRet = strModel;
            for (int i = 0; i < Lcode.Length; i++)
            {
                strRet = strRet.Replace(Lcode.Substring(i, 1), "");
            }
            if (arr[0].Trim().Length > 1)
                arr[0] = strRet;
            else
                arr[1] = strRet;
            return string.Join("/", arr);
        }

        public static bool IsSingleOrDoubleChance(string code)
        {
            if (code == null || code.Trim().Length == 0)
            {
                throw new Exception("机会为空！");
            }
            if (code.IndexOf("+") > 0)
            {
                return false;
            }
            string[] arr = code.Trim().Split('/');
            string UseCode = arr[0].Length > arr[1].Length ? arr[0] : arr[1];
            if (UseCode.Length == 1) return true;
            return isSD(UseCode);
        }

        public static bool IsBigOrSmall(string code)
        {
            if (code == null || code.Trim().Length == 0)
            {
                throw new Exception("机会为空！");
            }
            if (code.IndexOf("+") > 0)
            {
                return false;
            }
            string[] arr = code.Trim().Split('/');
            string UseCode = arr[0].Length > arr[1].Length ? arr[0] : arr[1];
            if (UseCode.Length == 1) return true;
            return isBS(UseCode);
        }

        public static bool isSD(string PureCode)//判断单双
        {
            int scnt = 0;
            if (PureCode.Length <= 2) return false;
            for (int i = 0; i < PureCode.Length; i++)
            {
                int iVal = int.Parse(PureCode.Substring(i, 1));
                if ((iVal % 2) == 1) //单数
                {
                    scnt++;
                }
            }
            if (scnt == PureCode.Length || scnt == 5 || scnt == (PureCode.Length - 5))//全单或全双或全5或长度-5
            {
                return true;
            }
            if (PureCode.Length > 3)
            {
                if (scnt == 1 || scnt == PureCode.Length - 1)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool isBS(string PureCode)//判断大小
        {
            int scnt = 0;
            if (PureCode.Length <= 2) return false;
            for (int i = 0; i < PureCode.Length; i++)
            {
                int iVal = int.Parse(PureCode.Substring(i, 1));
                if (iVal < 6 && iVal > 0) //小数
                {
                    scnt++;
                }
            }
            if (scnt == PureCode.Length || scnt == 5 || scnt == (PureCode.Length - 5))//全单或全双或全5或长度-5
            {
                return true;
            }
            if (PureCode.Length > 3)
            {
                if (scnt == 1 || scnt == PureCode.Length - 1)
                {
                    return true;
                }
            }
            return false;
        }

        public static int getSameNoCnt(string code1, string code2)
        {
            string testStr = code2;
            for (int i = 0; i < code1.Length; i++)
            {
                testStr = testStr.Replace(code1.Substring(i, 1), "");
            }
            return code2.Length - testStr.Length;
        }

        public static List<string> getAllSubCode(string OrgCode, int subCodeLng)
        {
            List<string> strRet = new List<string>();
            if (OrgCode.Trim().Length == subCodeLng)
            {
                strRet.Add(OrgCode);
                return strRet;
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            for (int j = 0; j <= OrgCode.Length - subCodeLng; j++)
            {
                List<string> codes = new List<string>();
                FillN(strRet, codes, OrgCode, j, subCodeLng - 1);
            }
            return strRet;
        }

        static void FillN(List<string> list, List<string> subCode, string code, int start, int n)
        {
            subCode.Add(code.Substring(start, 1));
            if (n == 0)
            {
                list.Add(String.Join("", subCode.ToArray()));
                return;
            }
            else
            {
                for (int i = start + 1; i < code.Length; i++)
                {
                    string[] codes = subCode.ToArray();
                    List<string> newlist = new List<string>();
                    newlist.AddRange(subCode);
                    FillN(list, newlist, code, i, n - 1);
                }
            }
        }

        public static int getChipsByCode(string strcode)
        {
            int ret = 0;
            string[] plusArr = strcode.Split('+');
            for (int p = 0; p < plusArr.Length; p++)
            {
                string code = plusArr[p];
                string[] arrs = code.Split('/');
                ret = ret + arrs[0].Length * arrs[1].Length;
            }
            return ret;
        }

        public string GetCodeKey(bool bySer)
        {
            string[] arr = this.ChanceCode.Split('/');
            if (bySer)
            {
                return arr[0];
            }
            return arr[1];
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            object ret = Activator.CreateInstance(conversionType);
            Type t = conversionType;
            Type myt = this.GetType();
            MemberInfo[] mis =  t.GetMembers();
            MemberInfo[] mymis = myt.GetMembers();
            Dictionary<string, MemberInfo> mydic = new Dictionary<string, MemberInfo>();
            for (int i = 0; i < mymis.Length; i++)
            {
                if (mymis[i] is PropertyInfo || mymis[i] is FieldInfo)
                {

                    if (!mydic.ContainsKey(mymis[i].Name))
                    {
                        mydic.Add(mymis[i].Name, mymis[i]);
                    }
                    else
                    {
                        Log("错误","强制转换时成员名重复", mymis[i].Name);
                    }
                }
            }
            //mydic =  mymis.ToDictionary(a => a.Name,a=>a);
            for (int i = 0; i < mis.Length; i++)
            {
                MemberInfo mi = mis[i];
                if (mi is PropertyInfo || mi is FieldInfo)
                {
                    if (mi is PropertyInfo)
                    {
                        if (!(mi as PropertyInfo).CanWrite)//只读，跳过
                        {
                            continue;
                        }
                    }
                    if (mydic.ContainsKey(mis[i].Name))
                    {
                        MemberInfo mymi = mydic[mis[i].Name];
                        Type mit = null;
                        if (mi is PropertyInfo)
                        {
                            mit = (mi as PropertyInfo).PropertyType;
                        }
                        if (mi is FieldInfo)
                            mit = (mi as FieldInfo).FieldType;
                        if (mit == null) continue;
                        object val = (mymi is PropertyInfo) ? (mymi as PropertyInfo).GetValue(this, null) : (mymi as FieldInfo).GetValue(this);
                        if (val == null) continue;
                        if (val is IConvertible)
                        {
                            val = ConvertionExtensions.ConvertTo((IConvertible)val, mit);
                        }
                        else
                        {
                            continue;
                        }
                        //Log("自定义类型转换",string.Format("{0}={1}",mymi.Name,val));
                        if (mi is PropertyInfo)
                            (mi as PropertyInfo).SetValue(ret, val, null);
                        else
                            (mi as FieldInfo).SetValue(ret, val);
                    }
                }
            }
            return ret;
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }
    }


    
}
