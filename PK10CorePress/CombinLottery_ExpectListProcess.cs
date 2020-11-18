using System;
using System.Collections.Generic;
using System.Text;
using WolfInv.com.BaseObjectsLib;
using System.Data;
using WolfInv.com.SecurityLib;

namespace WolfInv.com.PK10CorePress
{
    public class ExpectListProcessBuilder<T> where T : TimeSerialData
    {
        ExpectList<T> data;
        DataTypePoint dtp;
        public ExpectListProcessBuilder(DataTypePoint _dtp, ExpectList<T> _data)
        {
            dtp = _dtp;
            data = _data;
        }

        public CommExpectListProcess<T> getProcess()
        {
            CommExpectListProcess<T> ret = null;

            if (dtp.IsSecurityData == 1)
            {
                //ret = new SecurityListProcess<T>(data);
                ret = new SecurityListProcess<T>(data) as CommExpectListProcess<T>;
                
            }
            else
            {
                DataTable dt = new DataTable();
                ExpectList el = new ExpectList();
                lock (data)
                {
                    dt = data.Table.Copy();
                    el = new ExpectList(dt);
                }
                switch (dtp.DataType)
                {
                    
                    case "PK10":
                    case "XYFT":

                        {
                            
                                ret = new ExpectListProcess(el) as CommExpectListProcess<T>;// ConvertionExtensions.CopyTo<CommExpectListProcess<T>>(new ExpectListProcess(new ExpectList(data.Table)));
                                ret.AllNums = 10;
                                ret.SelectNums = 10;
                            ret.TenToZero = true;
                            break;
                        }
                    case "SCKL12":
                    case "NLKL12":
                        {
                            
                                ret = new CombinLottery_ExpectListProcess(el) as CommExpectListProcess<T>;
                                ret.AllNums = dtp.AllNums;
                                ret.SelectNums = dtp.SelectNums;
                                (ret as CombinLottery_ExpectListProcess).strAllTypeOdds = dtp.strAllTypeOdds;
                                (ret as CombinLottery_ExpectListProcess).strCombinTypeOdds = dtp.strCombinTypeOdds;
                                (ret as CombinLottery_ExpectListProcess).strPermutTypeOdds = dtp.strPermutTypeOdds;
                           
                            break;
                        }
                    case "GDKL11":
                    default:
                        {
                            lock (data.Table)
                            {
                                ret = new CombinLottery_ExpectListProcess(el) as CommExpectListProcess<T>;
                                ret.AllNums = dtp.AllNums;
                                ret.SelectNums = dtp.SelectNums;
                                ret.TenToZero = dtp.TenToZero;
                                (ret as CombinLottery_ExpectListProcess).strAllTypeOdds = dtp.strAllTypeOdds;
                                (ret as CombinLottery_ExpectListProcess).strCombinTypeOdds = dtp.strCombinTypeOdds;
                                (ret as CombinLottery_ExpectListProcess).strPermutTypeOdds = dtp.strPermutTypeOdds;
                            }
                            break;
                        
                        }
                }
            }
            return ret;
        }
    }

    /// <summary>
    /// 组合类彩票处理
    /// </summary>
    public class CombinLottery_ExpectListProcess : BaseObjectsLib.CommExpectListProcess<TimeSerialData>
    {

        
        public string AllNumModel { get; set; }
        public string SelectNumModel { get; set; }
        public string strAllTypeOdds { get; set; }
        public string strCombinTypeOdds { get; set; }
        public string strPermutTypeOdds { get; set; }
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
                //if (AllNums > 10)//默认使用逗号分隔
                //{
                    splitor = ",";
                //}
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
        /// Y视图
        /// </summary>
        /// <param name="reviewCnt"></param>
        /// <returns></returns>
        protected virtual List<Dictionary<int, string>> getNoDispCars(int reviewCnt)
        {
            InitBase();

            List<Dictionary<int, string>> ret = new List<Dictionary<int, string>>();
            int lastId = data.Count - 1;
            string RestModel = this.AllNumModel;// "01，02，03，04，05，06，07，08，09，10，11，12，。。。。。。";
            for (int i = 0; i < Math.Min(reviewCnt, data.Count); i++)
            {
                Combin_ExpectData< TimeSerialData> currExpect = new Combin_ExpectData<TimeSerialData>();
                currExpect.Expect =  data[lastId - i].Expect;//  data[lastId - i].CopyTo<ExpectData>();
                currExpect.OpenCode = data[lastId - i].OpenCode;
                currExpect.OpenTime = data[lastId - i].OpenTime;
                Dictionary<int, string> lastData = null;
                Dictionary<int, string> newData = new Dictionary<int, string>();
                if (i > 0)
                {
                    lastData = ret[i - 1];
                }
                for (int j = 0; j < SelectNums; j++) //从选择的数字中寻找出现的数字
                {
                    string LastString = RestModel;
                    if (lastData != null)
                    {
                        LastString = lastData[j];//取得最后一次的字符串
                    }
                    else
                    {

                    }
                    //string intmatch = (j + 1).ToString().PadLeft(2,'0');//要匹配的数字
                    ////if(intmatch == "10")
                    ////{
                    ////    intmatch = "0";
                    ////}
                    string currval = currExpect.ValueList[j].PadLeft(2, '0');
                    //LastString = LastString.Replace(currExpect.ValueList[j], "");//总数字大于10的必须以符号分割，否则 101112，这种会出现错误，替换01时会影响到10，11.
                    //////for(int c=0;c<currExpect.ValueList.Length;c++)
                    //////{
                    //////    if(currExpect.ValueList[c].PadLeft(2,'0') == intmatch)
                    //////    {
                    //////        currval = currExpect.ValueList[c].PadLeft(2, '0');
                    //////        break;
                    //////    }
                    //////}
                    if (currval != null)//匹配到才替换
                    {
                        //01,02,03,04,05....10,11,12,第一个替换01， 后面的替换 ,12
                        string replacestr = currval;
                        LastString = LastString.Replace(replacestr, "");
                        if(LastString.StartsWith(","))
                        {
                            LastString = LastString.Substring(1);
                        }
                        if(LastString.EndsWith(","))
                        {
                            LastString = LastString.Substring(0, LastString.Length - 1);
                        }
                        LastString = LastString.Replace(splitor+splitor, splitor);
                    }
                    newData.Add(j, LastString);
                }
                ret.Add(newData);
            }
            List<Dictionary<int, string>> reSortRet = new List<Dictionary<int, string>>();
            for (int i = 0; i < SelectNums; i++)
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
            CommCollection_KLXxY ret = null;
            if (ByNo)
            {
                ret = new CommCollection_KLXxY();
                ret.Data = getNoDispCars(reviewCnt);
            }
            else
            {
                ret = new CommCollection_KLXxY();
                ret.Data = getNoDispNums(reviewCnt);
            }
            ret.isByNo = ByNo;
            ret.AllNums = this.AllNums;
            ret.SelNums = this.SelectNums;
            ret.TenToZero = this.TenToZero;
            ret.strAllTypeOdds = this.strAllTypeOdds;
            ret.strCombinTypeOdds = this.strCombinTypeOdds;
            ret.strPermutTypeOdds = this.strPermutTypeOdds;
            //LogableClass.ToLog("获取视图集合时赋值原始数据", string.Format("到底做什么用的真忘记了{0}", reviewCnt));
            ret.orgData = this.data.LastDatas(Math.Min(reviewCnt, data.Count), false);//as ExpectList<TimeSerialData>;// new ExpectList(this.data.LastDatas(Math.Min(reviewCnt, data.Count)).Table);//？为什么要指定长度？因为回测时输入的原始数据太长？
            return ret;
        }

        //X视图
        public override List<Dictionary<int, string>> getNoDispNums(int reviewCnt)
        {
            InitBase();
            List<Dictionary<int, string>> ret = new List<Dictionary<int, string>>();
            int lastId = data.Count - 1;
            string RestModel = this.SelectNumModel;// "1234567890";
            for (int i = 0; i < Math.Min(reviewCnt, data.Count); i++)
            {
                Combin_ExpectData<TimeSerialData> currExpect = new Combin_ExpectData<TimeSerialData>();
                currExpect.OpenCode = data[lastId - i].OpenCode;
                Dictionary<int, string> lastData = null;
                Dictionary<int, string> newData = new Dictionary<int, string>();
                if (i > 0)
                {
                    lastData = ret[i - 1];
                }
                for (int j = 0; j < AllNums; j++)//选出的号
                {
 
                    string LastString = RestModel;
                    if (lastData != null)
                    {
                        LastString = lastData[j];//取得最后一次的字符串
                    }
                    string currval = null;
                    for (int c=0;c< currExpect.ValueList.Length;c++)
                    {
                        if(int.Parse(currExpect.ValueList[c]) == j+1)
                        {
                            currval = (c + 1).ToString().PadLeft(2, '0');
                        }
                    }
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
                   
                    if (currval != null)//匹配到才替换
                    {
                        //01,02,03,04,05....10,11,12,第一个替换01， 后面的替换 ,12
                        LastString = LastString.Replace(currval, "");
                        if (LastString.StartsWith(","))
                        {
                            LastString = LastString.Substring(1);
                        }
                        if (LastString.EndsWith(","))
                        {
                            LastString = LastString.Substring(0, LastString.Length - 1);
                        }
                        LastString = LastString.Replace(splitor + splitor, splitor);
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

}
