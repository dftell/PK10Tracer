using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using BaseObjectsLib;
namespace PK10CorePress
{

    public class FullInsts
    {
        public int Count;
        public List<chanceList> ChanceList;

        public override string ToString()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(this);
        }
    }

    public class chanceList : ChanceClass
    {
        public override string ToString()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(this);
        }
    }

    /// <summary>
    /// 指令集类
    /// </summary>
    public class RequestClass:RecordObject, IList<ChanceClass>,iSerialJsonClass<RequestClass>
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

            RequestClass ret = null ;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ret = js.Deserialize<RequestClass>(str);
            return ret;
        }

        public string getUserInsts(GlobalClass setting)
        {
            string ret = "";
            List<string> allTxt = new List<string>();
            AmoutSerials amts = setting.DefaultHoldAmtSerials;
            for(int i=0;i<this.Count;i++)
            {
                ChanceClass cc = this[i];
                string strccNewInst = "";
                if(cc.ChanceCode.Trim().Length == 0) 
                {
                    continue;
                }
                if(cc.UnitCost == 0)
                    continue;
                //修改为自动计算出来的结果
                //Int64 Amt = cc.UnitCost*setting.SerTotal(cc.ChipCount);
                int chips = cc.ChipCount-1;
                int maxcnt = amts.MaxHoldCnts[chips];
                int bShift = 0;
                if(cc.HoldTimeCnt > maxcnt)
                    bShift = (int)maxcnt*2/3;
                int RCnt =  (cc.HoldTimeCnt % (maxcnt+1)) + bShift;
                Int64 Amt = amts.Serials[cc.ChipCount - 1][RCnt];
                if(cc.ChanceType != 2)//非对冲
                {
                    if(cc.ChanceType == 1)//一次性下注
                    {
                        Amt = cc.UnitCost*setting.SerTotal(1);
                    }
                    strccNewInst = string.Format("{0}/{1}",cc.ChanceCode.Replace("+",string.Format("/{0}+",Amt)),Amt);
                }
                else
                {
                    if(!setting.JoinHedge)//不参与对冲
                    {
                        continue;
                    }
                    Amt = cc.UnitCost*setting.HedgeTimes;//对冲倍数
                    if(setting.AllowHedge)
                    {
                        Int64 BaseAmt = cc.BaseCost*setting.HedgeTimes;
                        strccNewInst = string.Format("{0}/{1}+{2}/{3}",cc.ChanceCode,Amt+BaseAmt,ChanceClass.getRevChance(cc.ChanceCode) ,BaseAmt);

                    }
                    else
                    {
                        strccNewInst = string.Format("{0}/{1}",cc.ChanceCode,Amt);
                    }
                }
                allTxt.Add(strccNewInst);
            }
            return string.Join(" ", allTxt.ToArray()).Trim();
             
        }
    }

    public class InstClass
    {
        public String ruleId;
        public String betNum;//'车数*前后区间所有车号数量
        public String itemTimes;//
        public String selNums;//具体下注码
        public String jsOdds;//赔率
        public int priceMode;//价格模式

        public String GetJson()
        {
            String outStr = "";
            try
            {
                if (outStr.Length == 0)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    return js.Serialize(this);
                    ////JSONObject js = new JSONObject();
                    
                    ////js.put("ruleId", ruleId);
                    ////js.put("betNum", betNum);
                    ////js.put("itemTimes", itemTimes);
                    ////js.put("selNums", selNums);
                    ////js.put("jsOdds", jsOdds);
                    ////js.put("priceMode", priceMode);
                    ////outStr = js.toString();
                }
            }
            catch (Exception ce)
            {
                return "";
            }
            return outStr;
        }
    }



}
