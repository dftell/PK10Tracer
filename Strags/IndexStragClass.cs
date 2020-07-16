using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.PK10CorePress;

namespace WolfInv.com.Strags
{
    /// <summary>
    /// 引用索引策略类
    /// 该类策略有一个引用的索引和历史状态
    /// 该索引是获得机会及计算投注金额的重要条件，
    /// 同时，获得机会的时候会计算出当前策略当期对应的索引值，补充进索引中去，供下一期使用
    /// 
    /// </summary>
    [XmlInclude(typeof(Strag_SimpleShiftClass))]
    public abstract class ReferIndexStragClass:StragClass
    {
        public List<IndexExpectData> Indexs;
        protected List<List<StringDoubleData>> CurrIndex ;
        public void ClearTmpData()
        {
            CurrIndex = null;
        }
        
        protected List<List<List<StringDoubleData>>> getUseIndexsData(int cnt)
        {
            var res = getUseIndexs(cnt);
            return res.Select(a=>a.Datas).ToList();
        }

        protected List<IndexExpectData> getUseIndexs(int cnt)
        {
            long expect = 0;
            if (!long.TryParse(this.LastUseData()?.LastData?.Expect, out expect))
            {
                return new List<IndexExpectData>();
            }
            var ret = Indexs.Where(a => {
                long pexpect = 0;
                if (!long.TryParse(a.Expect, out pexpect))
                {
                    return false;
                }
                if (pexpect < expect)
                {
                    return true;
                }
                return false;
            });

            var res = ret.OrderByDescending(a => long.Parse(a.Expect)).Take(cnt).OrderBy(a => a.Expect);
            return res.ToList();
        }


        protected List<List<StringDoubleData>> getCurrIndex(ExpectData ed)
        {
            var currIndex = Indexs.Where(a => a.Expect == ed.Expect);
            if (currIndex.Count() == 1)
                return currIndex.First().Datas;
            return new List<List<StringDoubleData>>();
        }

        protected abstract List<List<StringDoubleData>> calcCurrIndex(BaseCollection sc);

        protected abstract bool NeedIn(string label, List<List<double>> features, List<double> currVal,ref List<double> TargetList, int AllNum);

        protected abstract bool NeedEnd(string label, List<List<double>> features, List<double> currVal, List<double> TargetList, int allNum, int HoldCnt);


    }
    
}
