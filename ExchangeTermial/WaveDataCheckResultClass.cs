using System.Collections.Generic;
using System.Data;
using System.Linq;
//using WolfInv.Com.SocketLib;

namespace ExchangeTermial
{
    public class WaveDataCheckResultClass<T>
    {
        public bool Succ;
        
        public string CheckMsg;
        /// <summary>
        /// 原始数据
        /// </summary>
        public List<TimeSerialPoint<T>> SerialData;
        public DataTable orgData;
        /// <summary>
        /// 最大极值
        /// </summary>
        public MaxMinPoint MaxPoint;
        /// <summary>
        /// 最小极值
        /// </summary>
        public MaxMinPoint MinPoint;
        /// <summary>
        /// 运行信息列表
        /// </summary>
        public List<RunAreaInfo> RunAreaList;
        /// <summary>
        /// 最大回撤
        /// </summary>
        public RunAreaInfo MaxDrawDown
        {
            get
            {
                if(RunAreaList == null)
                {
                    return null;
                }

                return null;
            }
        }
        /// <summary>
        /// 最后回撤
        /// </summary>
        public RunAreaInfo LastDrawDown;
        /// <summary>
        /// 最长周期回撤
        /// </summary>
        public RunAreaInfo MaxHoldDrawDown;
        /// <summary>
        /// 最后上涨
        /// </summary>
        public RunAreaInfo LastRaise;
        /// <summary>
        /// 最后一个运行区间
        /// </summary>
        public RunAreaInfo LastArea
        {
            get
            {
                return RunAreaList.Last();
            }
        }



        public DataTable SamplestTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Weight");
            dt.Columns.Add("Net");
            DataRow dr = dt.NewRow();
            dt.Rows.Add(new object[] {RunAreaList.First().StartIndex, RunAreaList.First().Weight, RunAreaList.First().StartValue});
            for(int i=0;i< RunAreaList.Count;i++)
            {
                dt.Rows.Add(new object[] { RunAreaList[i].EndIndex,RunAreaList[i].Weight, RunAreaList[i].EndValue });
            }
            return dt;
        }

    }

    
}
