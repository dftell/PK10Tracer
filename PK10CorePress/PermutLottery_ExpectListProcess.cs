using System;
using System.Collections.Generic;
using System.Text;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.PK10CorePress
{
    /// <summary>
    /// 排列类彩票处理
    /// </summary>
    public class PermutLottery_ExpectListProcess : BaseObjectsLib.CommExpectListProcess<TimeSerialData> //PK10CorePress.CommExpectListProcess
    {
        protected ExpectList data;
        public PermutLottery_ExpectListProcess(ExpectList _data) : base(_data)
        {
            data = _data;
            //data = ConvertionExtensions.CopyTo<ExpectList>(base.Parent_data);
        }


        protected virtual List<Dictionary<int, string>> getNoDispCars(int reviewCnt)
        {
            List<Dictionary<int, string>> ret = new List<Dictionary<int, string>>();
            int lastId = data.Count - 1;
            string RestModel = "1234567890";
            for (int i = 0; i < Math.Min(reviewCnt, data.Count); i++)
            {
                ExpectData currExpect = data[lastId - i];//  data[lastId - i].CopyTo<ExpectData>();
                Dictionary<int, string> lastData = null;
                Dictionary<int, string> newData = new Dictionary<int, string>();
                if (i > 0)
                {
                    lastData = ret[i - 1];
                }
                for (int j = 0; j < 10; j++)
                {
                    string LastString = RestModel;
                    if (lastData != null)
                    {
                        LastString = lastData[j];//取得最后一次的字符串
                    }
                    LastString = LastString.Replace(currExpect.ValueList[j], "");
                    newData.Add(j, LastString);
                }
                ret.Add(newData);
            }
            List<Dictionary<int, string>> reSortRet = new List<Dictionary<int, string>>();
            for (int i = 0; i < 10; i++)
            {
                Dictionary<int, string> tmp = new Dictionary<int, string>();
                for (int j = 0; j < ret.Count; j++)
                {
                    tmp.Add(j, ret[j][i]);
                }
                reSortRet.Add(tmp);
            }

            return reSortRet;
        }

        /// <summary>
        /// 主入口
        /// </summary>
        /// <param name="reviewCnt"></param>
        /// <param name="ByNo"></param>
        /// <returns></returns>
        public override BaseCollection<TimeSerialData> getSerialData(int reviewCnt, bool ByNo = true)
        {
            PK10CorePress.BaseCollection ret = null;
            if (ByNo)
            {
                ret = new SerialCollection();
                ret.Data = getNoDispCars(reviewCnt);
            }
            else
            {
                ret = new CarCollection();
                ret.Data = getNoDispNums(reviewCnt);
            }
            //LogableClass.ToLog("获取视图集合时赋值原始数据", string.Format("到底做什么用的真忘记了{0}", reviewCnt));
            ret.orgData = this.data.LastDatas(Math.Min(reviewCnt, data.Count), false);//as ExpectList<TimeSerialData>;// new ExpectList(this.data.LastDatas(Math.Min(reviewCnt, data.Count)).Table);//？为什么要指定长度？因为回测时输入的原始数据太长？
            return ret;
        }

        public override List<Dictionary<int, string>> getNoDispNums(int reviewCnt)
        {
            List<Dictionary<int, string>> ret = new List<Dictionary<int, string>>();
            int lastId = data.Count - 1;
            string RestModel = "1234567890";
            for (int i = 0; i < Math.Min(reviewCnt, data.Count); i++)
            {
                ExpectData currExpect = data[lastId - i];//.CopyTo<ExpectData>();
                Dictionary<int, string> lastData = null;
                Dictionary<int, string> newData = new Dictionary<int, string>();
                if (i > 0)
                {
                    lastData = ret[i - 1];
                }
                for (int j = 0; j < 10; j++)
                {
                    string carNo = currExpect.ValueList[j];
                    int carId = int.Parse(carNo);
                    string LastString = RestModel;
                    if (lastData != null)
                    {
                        LastString = lastData[carId];//取得最后一次的字符串
                    }
                    string strNo = string.Format("{0}", (j + 1) % 10);
                    LastString = LastString.Replace(strNo, "");
                    newData.Add(carId, LastString);
                }
                ret.Add(newData);
            }
            List<Dictionary<int, string>> reSortRet = new List<Dictionary<int, string>>();
            for (int i = 0; i < 10; i++)
            {
                Dictionary<int, string> tmp = new Dictionary<int, string>();
                for (int j = 0; j < ret.Count; j++)
                {
                    tmp.Add(j, ret[j][i]);
                }
                reSortRet.Add(tmp);
            }

            return reSortRet;
        }

        public FullRepeatInfo getRepeatDic(int buffs, int reviewCnt, bool byNo)
        {
            FullRepeatInfo ret = new FullRepeatInfo();
            ret.BuffsLong = buffs;
            ret.BuffsRepeatInfos = new Dictionary<int, RepeatInfo>();
            ret.ReviewLong = reviewCnt;
            ret.ReviewRepeatInfos = new Dictionary<int, RepeatInfo>();
            string RestModel = "1234567890";

            return ret;
        }

        public Dictionary<int, RepeatInfo> getRepeatInfo(int frm, int cnt, bool byNo)
        {
            Dictionary<int, RepeatInfo> ret = new Dictionary<int, RepeatInfo>();
            int lastId = data.Count - 1;
            string RestModel = "1234567890";

            if (!byNo)
            {
                for (int j = 0; j < 10; j++)
                {
                    Dictionary<int, int> holdcnts = new Dictionary<int, int>();
                    Dictionary<int, StringBuilder> holddets = new Dictionary<int, StringBuilder>();
                    RepeatInfo ri = new RepeatInfo();
                    StringBuilder sb = new StringBuilder();
                    for (int i = frm; i < cnt; i++)
                    {
                        int val = int.Parse(data[i].ValueList[j]);
                        if (holddets.ContainsKey(val))
                        {
                            sb = holddets[val];

                        }
                        else
                        {
                            holddets.Add(val, sb);
                        }
                        holddets[val].Append(val.ToString());
                    }
                    foreach (int key in holddets.Keys)
                    {
                        RepeatItem ritem = new RepeatItem();
                        ritem.RepeatNo = key;
                        ritem.Cnt = holddets[key].Length;
                        ritem.Detail = holddets[key].ToString().Split(',');
                        ri.DetailList.Add(ritem);
                    }
                    ret.Add((j + 1) % 10, ri);
                }
            }
            else
            {
            }

            return ret;
        }


    }

}
