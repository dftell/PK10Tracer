using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using WolfInv.com.PK10CorePress;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.RemoteObjectsLib;
using WolfInv.com.SecurityLib;
namespace WolfInv.com.WebCommunicateClass
{
    /// <summary>
    /// 指令集类
    /// </summary>
    public class RequestClass : RecordObject, IList<ChanceClass>, iSerialJsonClass<RequestClass>
    {
        public Action<string,int, SelectTimeInstClass> SelectTimeChanged;
        public string Expect;
        public string LastTime;
        public string LastOpenCode;
        FullInsts objFullInsts;
        public FullInsts Full
        {
            get
            {
                return objFullInsts;
            }
            set
            {
                //ToAdd Chance process
                try
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    FullInsts fi = js.Deserialize<FullInsts>(value.ToString());
                    if (fi == null) return;
                    List<chanceList> cs = fi.ChanceList;
                    if (cs == null)
                        return;
                    _datas.Clear();
                    for (int i = 0; i < cs.Count; i++)
                    {
                        _datas.Add(cs[i]);
                    }
                }
                catch (Exception ce)
                {
                    string strErr = ce.Message;
                }
            }
        }

        public string Insts;
        public string Error;
        string LoginName;
        List<ChanceClass> _datas = new List<ChanceClass>();

        public int IndexOf(ChanceClass item)
        {
            return _datas.IndexOf(item);
        }

        public void Insert(int index, ChanceClass item)
        {
            _datas.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _datas.RemoveAt(index);
        }

        public ChanceClass this[int index]
        {
            get
            {
                return _datas[index];
            }
            set
            {
                _datas[index] = value;
            }
        }

        public void Add(ChanceClass item)
        {
            _datas.Add(item);
        }

        public void Clear()
        {
            _datas.Clear();
        }

        public bool Contains(ChanceClass item)
        {
            return _datas.Contains(item);
        }

        public void CopyTo(ChanceClass[] array, int arrayIndex)
        {
            _datas.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _datas.Count; }
            ////set 
            ////{//应对序列化赋值
            ////} 
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(ChanceClass item)
        {
            return _datas.Remove(item);
        }

        public IEnumerator<ChanceClass> GetEnumerator()
        {
            return _datas.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public RequestClass getObjectByJsonString(string str)
        {

            RequestClass ret = null;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ret = js.Deserialize<RequestClass>(str);
            return ret;
        }

        public SelectTimeInstClass getSelectTimeAmt(DataTypePoint dtp,string expectNo,int currCnt,Func<string,string,int,SelectTimeInstClass> getRemoteCnt)
        {
            SelectTimeInstClass ret = getRemoteCnt(dtp.DataType, expectNo, currCnt);
            return ret;
        }
        Dictionary<string, int> SelectTimeDic;
        public string getUserInsts(GlobalClass setting,DataTypePoint dtp,string NewExpectNo,string forweb, Func<string, string, int, SelectTimeInstClass> getRemoteCnt)
        {
            string ret = "";
            List<string> allTxt = new List<string>();
            AmoutSerials amts = setting.DefaultHoldAmtSerials;
            for (int i = 0; i < this.Count; i++)
            {
                ChanceClass cc = this[i];
                string strAssetId = cc.AssetId;
                int AssetCnt = 0;
                
                string strccNewInst = "";
                if (cc.ChanceCode.Trim().Length == 0)
                {
                    continue;
                }
                if (cc.UnitCost == 0)
                    continue;
                string strCurrExpect = cc.ExpectCode;
                int calcHoldTimes = (int)DataReader.getInterExpectCnt(strCurrExpect, NewExpectNo,dtp);
                if(cc.HoldTimeCnt+1<calcHoldTimes)
                {
                    continue;
                }
                if(!cc.Tracerable)//只要是非跟踪策略
                {
                    if(calcHoldTimes > 1)//和要下注的期数大于1就过滤，因为不要跟踪，只有当期才要下注
                    {
                        continue;
                    }
                }
                if (setting.AssetUnits.ContainsKey(strAssetId))
                {
                    AssetInfoConfig aic = setting.AssetUnits[strAssetId];
                    if (aic == null)
                        continue;
                    AssetCnt = aic.value;
                    if (aic.NeedSelectTimes == 1)
                    {
                        int newVal = 0;
                        if (SelectTimeDic == null)
                            SelectTimeDic = new Dictionary<string, int>();
                        if (SelectTimeDic.ContainsKey(cc.GUID))
                        {
                            newVal = SelectTimeDic[cc.GUID];
                        }
                        else
                        {
                            SelectTimeInstClass obj = getSelectTimeAmt(dtp, cc.ExpectCode, AssetCnt, getRemoteCnt);
                            if (obj == null)
                                continue;
                            newVal = obj.RetCnt;
                            SelectTimeChanged?.Invoke(strAssetId,AssetCnt, obj);
                            AssetCnt = obj.RetCnt;
                            setting.AssetUnits[strAssetId].value = newVal;
                            SelectTimeDic.Add(cc.GUID, newVal);
                        }
                        AssetCnt = newVal;
                    }
                }
                //修改为自动计算出来的结果
                //Int64 Amt = cc.UnitCost*setting.SerTotal(cc.ChipCount);
                int chips = cc.ChipCount - 1;
                int maxcnt = 1;
                if(amts.MaxHoldCnts.Length< chips)//只支持小注数策略，大注数全部使用1
                    maxcnt = amts.MaxHoldCnts[chips];
                int bShift = 0;
                if (cc.HoldTimeCnt > maxcnt)
                    bShift = (int)maxcnt * 2 / 3;
                int RCnt = (cc.HoldTimeCnt % (maxcnt + 1)) + bShift -1;
                Int64 Amt = 1;
                if(amts.MaxHoldCnts.Length < chips)
                    Amt = amts.Serials[chips][RCnt]*AssetCnt;
                if (cc.ChanceType != 2)//非对冲
                {
                    if (cc.ChanceType == 1)//一次性下注
                    {
                        Amt = cc.UnitCost * setting.SerTotal(1)* AssetCnt;
                    }
                    strccNewInst = string.Format("{0}/{1}", cc.ChanceCode.Replace("+", string.Format("/{0}+", Amt)), Amt);
                }
                else
                {
                    if (!setting.JoinHedge)//不参与对冲
                    {
                        continue;
                    }
                    Amt = cc.UnitCost * setting.HedgeTimes* AssetCnt;//对冲倍数
                    if (setting.AllowHedge)
                    {
                        Int64 BaseAmt = cc.BaseCost * setting.HedgeTimes* AssetCnt;
                        strccNewInst = string.Format("{0}/{1}+{2}/{3}", cc.ChanceCode, Amt + BaseAmt, ChanceClass.getRevChance(cc.ChanceCode), BaseAmt);

                    }
                    else
                    {
                        strccNewInst = string.Format("{0}/{1}", cc.ChanceCode, Amt);
                    }
                }
                allTxt.Add(strccNewInst);
            }
            GlobalClass.SetConfig(forweb);
            return string.Join(" ", allTxt.ToArray()).Trim();

        }
    
    }

    public class SelectTimeInstClass: RecordObject,iSerialJsonClass<SelectTimeInstClass>
    {
        public string Expect;
        public string DataType;
        public int RequestCnt;
        public int RetCnt;
        public string Error;
        public List<MatchGroupClass> List;
        public SelectTimeInstClass()
        {

        }

        public SelectTimeInstClass getObjectByJsonString(string str)
        {
            SelectTimeInstClass ret = null;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ret = js.Deserialize<SelectTimeInstClass>(str);
            return ret;
        }
    }

    public class MatchGroupClass: DisplayAsTableClass
    {
        public int SerNo;
        public string ExpectCode;//":"20200405041","
        public int times;//":10,"
        public int chipCount;//":19
    }
}
