using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.LogLib;
using System.Reflection;
namespace WolfInv.com.PK10CorePress
{
    ////public abstract class CommExpectListProcess :BaseObjectsLib.CommExpectListProcess<TimeSerialData>
    ////{
    ////    protected ExpectList data;
    ////    protected CommExpectListProcess(ExpectList _data) : base(_data)
    ////    {
    ////        data = ConvertionExtensions.CopyTo<ExpectList>(base.Parent_data);
    ////    }
    
    ////}

    public abstract class BaseCollection: BaseObjectsLib.BaseCollection<TimeSerialData>
    {
        public ExpectList orgData;
    }
    /// <summary>
    /// pk10处理
    /// </summary>
    public class ExpectListProcess: PermutLottery_ExpectListProcess
    {
        //protected ExpectList data;
        public ExpectListProcess(ExpectList _data):base(_data)
        {
            data = _data;
            //data = ConvertionExtensions.CopyTo<ExpectList>(base.Parent_data);
        }
                            

        List<Dictionary<int, string>> getNoDispCars(int reviewCnt)
        {
            List<Dictionary<int, string>> ret = new List<Dictionary<int, string>>();
            int lastId = data.Count - 1;
            string RestModel = "1234567890";
            for (int i = 0; i < Math.Min(reviewCnt,data.Count); i++)
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

        
        public override BaseCollection<TimeSerialData> getSerialData(int reviewCnt, bool ByNo=true)
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
            ret.orgData = this.data.LastDatas(Math.Min(reviewCnt, data.Count),false);//as ExpectList<TimeSerialData>;// new ExpectList(this.data.LastDatas(Math.Min(reviewCnt, data.Count)).Table);//？为什么要指定长度？因为回测时输入的原始数据太长？
            return ret;
        }

        public override List<Dictionary<int, string>> getNoDispNums(int reviewCnt)
        {
            List<Dictionary<int, string>> ret = new List<Dictionary<int, string>>();
            int lastId = data.Count - 1;
            string RestModel = "1234567890";
            for (int i = 0; i < Math.Min(reviewCnt, data.Count); i++)
            {
                ExpectData currExpect = data[lastId - i].CopyTo<ExpectData>();
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
                ExpectData currExpect = data[lastId - i].CopyTo<ExpectData>();
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


    /// <summary>
    /// 组合类彩票处理
    /// </summary>
    public class CombinLottery_ExpectListProcess : BaseObjectsLib.CommExpectListProcess<TimeSerialData>
    {

        /// <summary>
        /// 所有数字数
        /// </summary>
        public int AllNums { get; set; }
        /// <summary>
        /// 开出奖数字数
        /// </summary>
        public int SelectNums { get; set; }
        public string AllNumModel { get; set; }
        public string SelectNumModel { get; set; }
        public string splitor { get; set; }
        protected ExpectList data;

        public CombinLottery_ExpectListProcess(ExpectList _data) : base(_data)
        {
            data = _data;
            //data = ConvertionExtensions.CopyTo<ExpectList>(base.Parent_data);
        }

        void InitBase()
        {
            if (splitor == null)
            {
                if (AllNums > 10)//默认使用逗号分隔
                {
                    splitor = ",";
                }
            }
            if (AllNumModel == null)
            {
                string[] nums = new string[AllNums];
                for (int i = 0; i <= AllNums - 1; i++)
                {
                    nums[i] = (i + 1).ToString().PadLeft(2, '0');
                }
                AllNumModel = string.Join(splitor, nums);
            }
            if(SelectNumModel == null)
            {
                string[] nums = new string[SelectNums];
                for (int i = 0; i <= SelectNums - 1; i++)
                {
                    nums[i] = (i + 1).ToString().PadLeft(2, '0');
                }
                SelectNumModel = string.Join(splitor, nums);
            }
        }

        /// <summary>
        /// 所有数字视图
        /// </summary>
        /// <param name="reviewCnt"></param>
        /// <returns></returns>
        protected virtual List<Dictionary<int, string>> getNoDispCars(int reviewCnt)
        {
            InitBase();

            List<Dictionary<int, string>> ret = new List<Dictionary<int, string>>();
            int lastId = data.Count - 1;
            string RestModel = this.SelectNumModel;// "01，02，03，04，05，06，07，08，09，10，11，12，。。。。。。";
            for (int i = 0; i < Math.Min(reviewCnt, data.Count); i++)
            {
                ExpectData currExpect = data[lastId - i];//  data[lastId - i].CopyTo<ExpectData>();
                Dictionary<int, string> lastData = null;
                Dictionary<int, string> newData = new Dictionary<int, string>();
                if (i > 0)
                {
                    lastData = ret[i - 1];
                }
                for (int j = 0; j < AllNums; j++) //从选择的数字中寻找出现的数字
                {
                    string LastString = RestModel;
                    if (lastData != null)
                    {
                        LastString = lastData[j];//取得最后一次的字符串
                    }
                    else
                    {

                    }
                    string intmatch = (j + 1).ToString();//要匹配的数字
                    if(intmatch == "10")
                    {
                        intmatch = "0";
                    }
                    string currval = null;
                    //LastString = LastString.Replace(currExpect.ValueList[j], "");//总数字大于10的必须以符号分割，否则 101112，这种会出现错误，替换01时会影响到10，11.
                    for(int c=0;c<currExpect.ValueList.Length;c++)
                    {
                        if(currExpect.ValueList[c] == intmatch)
                        {
                            currval = (c + 1).ToString().PadLeft(2, '0');
                            break;
                        }
                    }
                    if (currval != null)//匹配到才替换
                    {
                        //01,02,03,04,05....10,11,12,第一个替换01， 后面的替换 ,12
                        string replacestr = string.Format("{0}{1}", int.Parse(currval) == 1 ? currval : splitor, int.Parse(currval) == 1 ? splitor : currval);
                        if (LastString.Split(splitor.ToCharArray()).Length > 1)
                        {
                            LastString = LastString.Replace(replacestr, "");
                        }
                        else
                        {
                            LastString = LastString.Replace(currval, "");
                        }
                    }
                    newData.Add(j, LastString);
                }
                ret.Add(newData);
            }
            List<Dictionary<int, string>> reSortRet = new List<Dictionary<int, string>>();
            for (int i = 0; i < AllNums; i++)
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


        public override BaseCollection<TimeSerialData> getSerialData(int reviewCnt, bool ByNo = true)
        {
            PK10CorePress.CommCollection ret = null;
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

        //选出数字视图
        public override List<Dictionary<int, string>> getNoDispNums(int reviewCnt)
        {
            InitBase();
            List<Dictionary<int, string>> ret = new List<Dictionary<int, string>>();
            int lastId = data.Count - 1;
            string RestModel = this.SelectNumModel;// "1234567890";
            for (int i = 0; i < Math.Min(reviewCnt, data.Count); i++)
            {
                ExpectData currExpect = data[lastId - i].CopyTo<ExpectData>();
                Dictionary<int, string> lastData = null;
                Dictionary<int, string> newData = new Dictionary<int, string>();
                if (i > 0)
                {
                    lastData = ret[i - 1];
                }
                for (int j = 0; j < SelectNums; j++)//选出的号
                {
                    string carNo = currExpect.ValueList[j];
                    int carId = int.Parse(carNo);
                    string LastString = RestModel;
                    if (lastData != null)
                    {
                        LastString = lastData[carId];//取得最后一次的字符串
                    }
                    string strNo = string.Format("{0}", (j + 1) % SelectNums).PadLeft(2,'0');

                    ////string intmatch = (j + 1).ToString();//要匹配的数字
                    ////if (intmatch == "10")
                    ////{
                    ////    intmatch = "0";
                    ////}
                    ////string currval = null;
                    //////LastString = LastString.Replace(currExpect.ValueList[j], "");//总数字大于10的必须以符号分割，否则 101112，这种会出现错误，替换01时会影响到10，11.
                    ////if (currExpect.ValueList.ToList().Contains(intmatch.ToString()))
                    ////{
                    ////    currval = (j + 1).ToString().PadLeft(2, '0');
                    ////}
                    string currval = strNo;
                    if (currval != null)//匹配到才替换
                    {
                        //01,02,03,04,05....10,11,12,第一个替换01， 后面的替换 ,12
                        string replacestr = string.Format("{0}{1}", int.Parse(currval) == 1 ? currval : splitor, int.Parse(currval) == 1 ? splitor : currval);
                        LastString = LastString.Replace(replacestr, "");
                    }
                    newData.Add(carId, LastString);
                }
                ret.Add(newData);
            }
            List<Dictionary<int, string>> reSortRet = new List<Dictionary<int, string>>();
            for (int i = 0; i < AllNums; i++)
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

        public virtual Dictionary<int, RepeatInfo> getRepeatInfo(int frm, int cnt, bool byNo)
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


    public class ExpectListProcess_KLXxY : CombinLottery_ExpectListProcess
    {
        public ExpectListProcess_KLXxY(ExpectList _data) :base(_data)
        {

        }

    }

    public class FullRepeatInfo
    {
        public int BuffsLong;
        public int ReviewLong;
        public Dictionary<int, RepeatInfo> BuffsRepeatInfos;
        public Dictionary<int, RepeatInfo> ReviewRepeatInfos;
    }

    public class RepeatInfo
    {
        public Dictionary<int, int> RepeatSummary;
        public List<RepeatItem> DetailList;
        public RepeatInfo()
        {
            RepeatSummary = new Dictionary<int, int>();
            DetailList = new List<RepeatItem>();
        }
    }

    public class RepeatItem
    {
        public int RepeatNo;
        public int Cnt;
        public string[] Detail;
        public RepeatItem()
        {
            //Detail = new List<string>();
        }
    }

}
