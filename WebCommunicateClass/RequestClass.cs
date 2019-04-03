using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using WolfInv.com.PK10CorePress;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.WebCommunicateClass
{
    /// <summary>
    /// 指令集类
    /// </summary>
    public class RequestClass : RecordObject, IList<ChanceClass>, iSerialJsonClass<RequestClass>
    {
        public string Expect;
        public string LastTime;
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

        public string getUserInsts(GlobalClass setting)
        {
            string ret = "";
            List<string> allTxt = new List<string>();
            AmoutSerials amts = setting.DefaultHoldAmtSerials;
            for (int i = 0; i < this.Count; i++)
            {
                ChanceClass cc = this[i];
                string strAssetId = cc.AssetId;
                int AssetCnt = 0;
                if(setting.AssetUnits.ContainsKey(strAssetId))
                {
                    AssetCnt = setting.AssetUnits[strAssetId];
                }
                string strccNewInst = "";
                if (cc.ChanceCode.Trim().Length == 0)
                {
                    continue;
                }
                if (cc.UnitCost == 0)
                    continue;
                //修改为自动计算出来的结果
                //Int64 Amt = cc.UnitCost*setting.SerTotal(cc.ChipCount);
                int chips = cc.ChipCount - 1;
                int maxcnt = amts.MaxHoldCnts[chips];
                int bShift = 0;
                if (cc.HoldTimeCnt > maxcnt)
                    bShift = (int)maxcnt * 2 / 3;
                int RCnt = (cc.HoldTimeCnt % (maxcnt + 1)) + bShift -1;
                Int64 Amt = amts.Serials[cc.ChipCount - 1][RCnt]*AssetCnt;
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
            return string.Join(" ", allTxt.ToArray()).Trim();

        }
    }
}
