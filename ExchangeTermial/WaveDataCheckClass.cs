using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
//using WolfInv.Com.SocketLib;

namespace ExchangeTermial
{
    /// <summary>
    /// 波数据检车类
    /// </summary>
    public class WaveDataCheckClass
    {
        int minStepLen = 5;//极值最小步长

        //建立一个标准数据表
        public static DataTable CreateStandardDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Value", typeof(double));
            dt.Columns.Add("TimeWeight", typeof(int));
            dt.Columns.Add("Net", typeof(double));
            dt.Columns.Add("isMin", typeof(int));
            dt.Columns.Add("isMax", typeof(int));
            return dt;
        }

        public static WaveDataCheckResultClass<T> getCheckResult<T>(List<TimeSerialPoint<T>> Data,double InitValue, NetSelectBasicValue NetSelectMode, int minStepLen = 5, double maxVal = double.MinValue, double minVal = double.MaxValue)
        {
            WaveDataCheckResultClass<T> ret = new WaveDataCheckResultClass<T>();
            try
            {
                ret.SerialData = Data;//原始排列数据
                ret.orgData = InitTable<T>(Data, InitValue, NetSelectMode, maxVal, minVal);
                
                ret.MaxPoint = new MaxMinPoint();
                ret.MinPoint = new MaxMinPoint();
                if (ret.orgData == null)
                {
                    ret.CheckMsg = "Init the basic table occur an error!";
                    return ret;
                }
                List<TimeSerialPoint<T>> nets = ret.orgData.Select("1=1","Id").ToList().Select(a=> 
                {
                    return new TimeSerialPoint<T>(((int)a["TimeWeight"]), (T)Convert.ChangeType(a["Net"],typeof(T)));
                }).ToList();
                List<ExtremeClass<T>> extrs = getExtremeList<T>(nets, minStepLen);//获得净值的极值
                ret.RunAreaList = getAllDrawDowns(extrs,InitValue);
                

            }
            catch(Exception ce)
            {
                ret.CheckMsg = ce.Message;
                return ret;
            }
            ret.Succ = true;
            return ret;
        }


        static DataTable InitTable<T>(List<TimeSerialPoint<T>> Data, double InitValue, NetSelectBasicValue NetSelectMode, double maxVal = double.MinValue, double minVal = double.MaxValue)
        {
            DataTable ret = null; 
            try
            {
                ret = CreateStandardDataTable();
                int idx = 0;
                int maxIdx=0, minIdx=0;
                /*
                 dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("Value", typeof(double));
                dt.Columns.Add("Net", typeof(double));
                dt.Columns.Add("isLowest", typeof(int));
                dt.Columns.Add("isHightest", typeof(int));
                 */
                if(Data == null || Data.Count== 0)
                {
                    return ret;
                }
                double SerCurrVal = InitValue;
                Data.ForEach(a =>
                {
                    DataRow dr = ret.NewRow();
                    dr["Id"] = idx;
                    double currVal = (double)Convert.ChangeType(Data[idx].Value, typeof(Double));
                    int tw = Data[idx].TimeWeight;
                    //SerCurrVal += currVal;
                    dr["Value"] = currVal;
                    dr["TimeWeight"] = tw;
                    if(currVal > maxVal)//如果大于极大值
                    {
                        if(ret.Rows.Count>maxIdx)
                        {
                            ret.Rows[maxIdx]["isMax"] = 0;
                        }
                        maxIdx = idx;
                        maxVal = currVal;
                        dr["isMax"] = 1;
                    }
                    if (currVal < maxVal)//如果小于极小值
                    {
                        if (ret.Rows.Count > minIdx)
                        {
                            ret.Rows[minIdx]["isMin"] = 0;
                        }
                        minIdx = idx;
                        minVal = currVal;
                        dr["isMin"] = 1;
                    }
                    ret.Rows.Add(dr);
                    idx++;
                });
                double basicVal = (double)ret.Rows[0]["Value"];
                if(NetSelectMode == NetSelectBasicValue.InitValue)
                {
                    basicVal = InitValue;
                }
                if(NetSelectMode == NetSelectBasicValue.MinValue)
                {
                    basicVal = minVal;
                }
                for(int i=0;i<ret.Rows.Count;i++)
                {
                    ret.Rows[i]["Net"] = ((double)ret.Rows[i]["Value"]) / InitValue ;
                }
            }
            catch(Exception ce)
            {
                return null;
            }
            return ret;
        }

        static List<RunAreaInfo> getAllDrawDowns<T>(List<ExtremeClass<T>> list, double InitValue)
        {
            List<RunAreaInfo> ret = new List<RunAreaInfo>();
            ExtremeClass<T> lastObj = list.First();
            list.ForEach(a => {
                if(a.Equals(lastObj))//第一个跳过
                {
                    return;
                }
                RunAreaInfo ri = new RunAreaInfo();
                ri.IsRaise = (a.type == ExtremeClass<T>.ExtremeType.Max);//如果是最大值，一定是上升
                ri.StartIndex = lastObj.SerialId;
                ri.EndIndex = a.SerialId;
                ri.Len = ri.EndIndex - ri.StartIndex + 1;
                ri.Weight = a.TimeWeight - lastObj.TimeWeight;//当期权重
                ri.DrawDownRate = (a.DValue - lastObj.DValue);
                ri.StartValue = lastObj.Net;
                ri.EndValue = a.Net;
                ret.Add(ri);
                lastObj = a;
            });
            return ret;
        }
        

        static List<ExtremeClass<T>> getExtremeList<T>(List<TimeSerialPoint<T>> data,int minInterLen)
        {
            List<ExtremeClass<T>> ret = new List<ExtremeClass<T>>();
            List<ExtremeClass<T>> tmp = new List<ExtremeClass<T>>();
            int idx = 0;
            data.ForEach(a =>
            {
                ExtremeClass<T> obj = new ExtremeClass<T>();
                obj.SerialId = idx;
                obj.Value = a.Value;
                obj.TimeWeight = a.TimeWeight;
                obj.Net = Convert.ToDouble(a.Value);
                obj.type = ExtremeClass<T>.ExtremeType.None;
                tmp.Add(obj);
                idx++;
            });
            bool GTMinInterLen = false;
            GTMinInterLen = isGTMinInterLen(tmp,minInterLen);
            while(!GTMinInterLen)
            {

                //Todo:求出极值
                List<ExtremeClass<T>> lastTmp = tmp;
                var list = tmp.Where(a => (a.type != ExtremeClass<T>.ExtremeType.None));
                bool NoEx = false;
                if(list.Count()==0)
                {
                    NoEx = true;
                    list = tmp;
                }
                if(NoEx)
                {
                    tmp = getMaxMinList(tmp);
                }
                else
                {                    
                    List<ExtremeClass<T>> maxList = tmp.Where(a => a.type == ExtremeClass<T>.ExtremeType.Max).ToList();
                    List<ExtremeClass<T>> minList = tmp.Where(a => a.type == ExtremeClass<T>.ExtremeType.Min).ToList();
                    List<ExtremeClass<T>> tmpMaxList = getMaxMinList(maxList);//大值取极值，消除中间值
                    List<ExtremeClass<T>> tmpMinList = getMaxMinList(minList);//小值取极值，消除中间值
                    bool isRise = isRiseList(tmp);
                    List<ExtremeClass<T>> tmpList = new List<ExtremeClass<T>>();
                    tmpMaxList.ForEach(a => tmpList.Add(a));
                    tmpMinList.ForEach(a => tmpList.Add(a));
                    tmpList = tmpList.OrderBy(a => a.SerialId).ToList();//合并大值的极值和小值的极值，重新按序列排序
                    tmp = getMaxMinList(tmpList);//再重新取极值
                }
                if(lastTmp.Count == tmp.Count)//无法再放大
                {
                    break;
                }
                GTMinInterLen = isGTMinInterLen(tmp,minInterLen);//如果还是有小于最小间隔的情况，继续取
            }
            ret = tmp;
            return ret;
        }

        static bool isRiseList<T>(List<ExtremeClass<T>> tmp)
        {
            bool isRise = false;
            if (tmp[0].DValue < tmp[1].DValue)//上升
            {
                isRise = true;
            }
            return isRise;
        }

        static List<ExtremeClass<T>> getMaxMinList<T>(List<ExtremeClass<T>> tmp)
        {
            Dictionary<int, double> calcList = new Dictionary<int, double>();
            for (int i = 1; i < tmp.Count - 1; i++)
            {
                calcList.Add(i, (tmp[i - 1].DValue - tmp[i].DValue) * (tmp[i].DValue - tmp[i + 1].DValue));
            }
            List<int> keys = calcList.Where(a => a.Value < 0).Select(a => a.Key).ToList();
            double test = (tmp[0].DValue - tmp[keys[0]].DValue) * (tmp[keys[0]].DValue - tmp[keys[1]].DValue);
            if (test < 0)//第一个节点如果是非同向，增加节点
            {
                keys.Insert(0, 0);
            }
            else//同向替换
            {
                keys[0] = 0;
            }
            int klen = keys.Count;
            test = (tmp[tmp.Count - 1].DValue - tmp[keys[klen - 1]].DValue) * (tmp[keys[klen - 1]].DValue - tmp[keys[klen - 2]].DValue);
            if (test < 0) //最后一个节点，非同向增加
            {
                keys.Add(tmp.Count - 1);
            }
            else //同向替换
            {
                keys[klen] = tmp.Count - 1;
            }
            bool isRise = false;
            if (tmp[keys[1]].DValue > tmp[keys[0]].DValue)//上升
            {
                isRise = true;
            }
            List<ExtremeClass<T>> tmpList = new List<ExtremeClass<T>>();
            foreach (int i in keys)
            {
                ExtremeClass<T> obj = tmp[i];
                obj.type = isRise ? ExtremeClass<T>.ExtremeType.Min : ExtremeClass<T>.ExtremeType.Max;
                isRise = !isRise;//下次反转
                tmpList.Add(obj);
            }
            return tmpList;
        }

        static bool isGTMinInterLen<T>(List<ExtremeClass<T>> data,int interVal)
        {
            
            var list = data.Where(a => { return a.type != ExtremeClass<T>.ExtremeType.None; });
            if (list.Count<ExtremeClass<T>>() == 0)
                return false;
            int len = list.Count();
            list = list.Skip(1).Take(len-2);//跳过第一个和最后一个
            var lastEx = list.First();
            foreach(var ex in list)
            {
                if(!ex.Equals(lastEx))
                {
                    if(ex.TimeWeight - lastEx.TimeWeight <interVal)//如果前后序列号之差小于最小间隔，就是间隔太小
                    {
                        return false;
                    }
                    lastEx = ex;
                }
            }
            return true;
        }
        
    }   

    
}
