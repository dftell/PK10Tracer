using System.Collections.Generic;
using System.Data;
using WolfInv.com.SecurityLib;
using WolfInv.com.BaseObjectsLib;
using System.Linq;
using System;

namespace WolfInv.com.PK10CorePress
{
    public class MissDataBuff
    {
        public List<MissDataSerial> SerialDiffs;
        public string LastExpect;
        public MissDataBuff()
        {
            SerialDiffs = new List<MissDataSerial>();
        }
        public void InitList(DataTable noOpList, string idName, int N)
        {
            for (int i = 1; i < noOpList.Columns.Count; i++)
            {
                MissDataSerial mds = new MissDataSerial(N, noOpList.Columns[i].ColumnName);
                mds.InitListItem(noOpList, idName, noOpList.Columns[i].ColumnName);
                SerialDiffs.Add(mds);
            }
        }

        public Dictionary<string, MissDataItem> getMissDataitem()
        {
            Dictionary<string, MissDataItem> ret = new Dictionary<string, MissDataItem>();
            for (int i = 0; i < SerialDiffs.Count; i++)
            {
                string key = SerialDiffs[i].ItemName;
                MissDataItem item = SerialDiffs[i].getMissDataitem();
                if (!ret.ContainsKey(key))
                {
                    ret.Add(key, item);
                }
            }
            return ret;
        }
    }

    public class MissDataSerial
    {
        public string ItemName;
        int Cycle;//周期
        public MissDataSerial(int n, string name)
        {
            ItemName = name;
            Cycle = n;
            OpList = new List<int>();
        }
        /// <summary>
        /// 1元素之差
        /// </summary>
        public List<int> OpList;

        public MissDataItem getMissDataitem()
        {
            MissDataItem ret = new MissDataItem();
            ret.num = ItemName;
            ret.missList = OpList;
            int len = this.OpList.Count;
            if(len == 0)
            {
                
                return ret;
            }
            List<int> list = this.OpList;
            ret.times = len;
            ret.miss = list[len - 1].ToString();
            if(list.Count>1)
                ret.miss1 = list[len - 2].ToString();
            if(list.Count>2)
                ret.miss2 = list[len - 3].ToString();
            ret.max_miss = list.Max().ToString();
            if (len == 1)
                ret.avg_miss = list[0].ToString();
            else
                ret.avg_miss = string.Format("{0:f2}",list.Sum() / len);
            ret.investment = list[len - 1];
            if (list.Count > 1)
                ret.supplement = list[len - 2] - list[len - 1];
            else
                ret.supplement = list[0];
            return ret;
        }


        public void InitListItem(DataTable noOpList, string idName, string valName)
        {
            DataRow[] drs = noOpList.Select(string.Format("[{0}]=1", valName), string.Format("{0} asc", idName));//获取所有按idName排序valName为1的记录。
            int[] indexArr = drs.Select(a => int.Parse(a[idName].ToString())).ToArray();//获得值为1的索引数列
            int[] diffArr = getIntArrayDiff(indexArr, Cycle);
            if(indexArr.Length==0)
            {
                diffArr = new int[] { Cycle};
            }
            OpList = diffArr.ToList();
            if(OpList.Sum() < Cycle-1)
            {

            }
        }

        public MissDataSerial getAdd(string name, int val)
        {
            MissDataSerial ret = new MissDataSerial(Cycle, name);
            try
            {
                int first = OpList[0];
                int last = Math.Min(Cycle, OpList[OpList.Count - 1] + 1);
                if (OpList.Count == 1)//全是它本身
                {
                    ret.OpList.Add(last);
                    if (val == 1)
                    {
                        ret.OpList.Add(0);
                    }
                    return ret;
                }
                if(OpList.Count == 2)
                {
                    if(first > 1)
                    {
                        ret.OpList.Add(first - 1);
                    }
                    ret.OpList.Add(last);
                    if (val == 1)
                    {
                        ret.OpList.Add(0);
                    }
                    return ret;
                }
                if (first > 1)
                {
                    ret.OpList.Add(first - 1);
                }
                for (int i = 1; i < OpList.Count - 1; i++)//从第二个点到倒数第二个点全部加入
                {
                    ret.OpList.Add(OpList[i]);
                }
                ret.OpList.Add(last);
                if (val == 1)
                {
                    ret.OpList.Add(0); //当前值为0，加入数组
                }
                if (ret.OpList.Sum()<Cycle-1)//当且仅当val=1时出现
                {
                    if(val ==0)
                    {

                    }
                }
            }
            catch(Exception e)
            {

            }
            return ret;
        }

        public void check()
        { }

        public static int[] getIntArrayDiff(int[] arr, int Cycle)
        {
            int[] ret = new int[] { };
            if (arr.Length == 0)
                return arr;
            List<int> list = arr.ToList();
            list.Insert(0, 0);
            list.Add(Cycle - 1);

            arr = list.ToArray();
            int[] bArr = arr.Skip(1).ToArray();
            int[] sArr = arr.Take(arr.Length - 1).ToArray();
            list = new List<int>();
            for (int i = 0; i < bArr.Length; i++)
            {
                list.Add(bArr[i] - sArr[i]);
            }
            return list.ToArray();
            if (arr.Length == 1)
            {
                if (arr[0] == 0)
                {
                    arr[0] = Cycle - 1;
                    ret = arr;
                }
                else if (arr[0] == Cycle - 1)
                {
                    ret = new int[] { Cycle - 1, 0 };
                }
                else
                {
                    ret = new int[] { Cycle - 1 - arr[0], arr[0] - 1 };
                }
                return ret;
            }


            return null;
        }
    }

    public class MissDataCalcClass
    {

        BaseCollection bc;
        static Dictionary<string, MissDataBuff> AllBuffs;
        public MissDataCalcClass(BaseCollection sc)
        {
            bc = sc;
        }

        public Dictionary<string,MissDataItem> getMissData(ExpectList data, DataTypePoint dtp, string currExpect, int peroid, string pos, string target, object[] others)
        {
            Dictionary<string, MissDataItem> ret = new Dictionary<string, MissDataItem>();
            string key = string.Format("{0}_{1}_{2}_{3}", dtp.DataType, peroid,pos,target);
            if (AllBuffs == null)
            {
                AllBuffs = new Dictionary<string, MissDataBuff>();
            }
            MissDataBuff mdb = null;
            if (!AllBuffs.ContainsKey(key))
            {
                DataTable dt = bc.getTableFromSpecCondition(dtp, currExpect, peroid, pos, target, others);
                if(dt == null)
                {
                    return ret;
                }
                mdb = new MissDataBuff();
                mdb.LastExpect = currExpect;
                mdb.InitList(dt, "id", peroid);
                AllBuffs.Add(key, mdb);
            }
            else
            {
                mdb = AllBuffs[key];
                string strNext = ExpectReader.getNextExpectNo(mdb.LastExpect, dtp);
                string lastExpect = data.LastData.Expect.Trim();
                string openCode = data.LastData.OpenCode;
                if(mdb.LastExpect.Trim().Equals(lastExpect))
                {

                }
                else if(strNext.Trim().Equals(lastExpect))
                {
                    for (int i = 0; i < mdb.SerialDiffs.Count; i++)
                    {
                        string strKey = mdb.SerialDiffs[i].ItemName;
                        int val = bc.isMatch(strKey, openCode) ? 1 : 0;
                        mdb.SerialDiffs[i] = mdb.SerialDiffs[i].getAdd(strKey,val);
                    }
                }                
                else //防止跳，或者中断后一次多条记录
                {
                    DataTable dt = bc.getTableFromSpecCondition(dtp, currExpect, peroid, pos, target, others);
                    if (dt == null)
                    {
                        return ret;
                    }
                    mdb.LastExpect = currExpect;
                    mdb.InitList(dt, "id", peroid);
                }
                mdb.LastExpect = lastExpect;
            }
            ret = mdb.getMissDataitem();
            return ret;
        }


    }
}
