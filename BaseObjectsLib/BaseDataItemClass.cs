using System;
using System.Linq;
using System.Data;
using System.Reflection;
namespace WolfInv.com.BaseObjectsLib
{
    public class BaseDataItemClass : PrintClass, iFillable
    {
        //WindAPI w;
        /// <summary>
        /// 万得内部名称，专供指数顶类
        /// </summary>
        public string SecCode;
        //public void AllowBuild(WindAPI _w)
        //{
        //    w = _w;
        //}
        public DateTime DateTime;
        public string WindCode, Sec_Name;
        public string riskadmonition_date;//摘帽时间

        public bool IsST
        {
            get
            {
                return IsSTInDay(DateTime) > 0;
            }
        }

        public int IsSTInDay(DateTime endt)
        {
            if (this.riskadmonition_date == null) //未被初始化
            {
                return -1;//不知道
            }
            else
            {
                if (this.riskadmonition_date.Length == 0)//从未被ST过
                {
                    return 1;
                }
                else
                {
                    string[] res = this.riskadmonition_date.Split(',');
                    var vv = from n in res
                             let ST = n.Split('：')[0].Trim()
                             let Date = n.Split('：')[1].Trim()
                             select new { ST, Date };
                    foreach (var key in vv)
                    {
                        string st = key.ST + key.Date;
                    }
                    var ss = from n in vv
                             where str2Date(n.Date) < endt
                             orderby n.Date descending
                             select n.ST; //比比较日期小的戴帽摘帽信息按日期倒序
                    if (ss.Count<string>() == 0) //如果不存在
                    {
                        return 0;
                    }
                    else //如果是去，就是摘帽，非ST
                    {
                        string str = ss.First<string>();
                        if (str.Substring(0, 1) == "去")//刚去
                        {
                            return 0;
                        }

                        return 1;
                    }
                }
            }
        }

        public bool ExchangeNormal
        {
            get
            {

                if (this.SecType == SecType.Equit)
                {
                    if (this.trade_status == null)
                    {
                        return false;
                    }
                    if (this.trade_status.Trim() == "交易")
                        return true;
                    else
                        return false;
                }
                else if (this.SecType == SecType.Index)
                {
                    if (float.IsNaN((float)this.Close) || float.IsInfinity((float)this.Close))//指数的收盘价为空，返回状态为非正常
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// 自然日数，非交易日
        /// </summary>


        public DateTime str2Date(string str)
        {
            return Convert.ToDateTime(string.Format("{0}/{1}/{2}", str.Substring(0, 4), str.Substring(4, 2), str.Substring(6, 2)));
        }

        public DateTime Ipo_date;
        public decimal Open, High, Low, Close, Volume, pct_chg;
        public string trade_status;//交易
        public int susp_days, maxupordown;
        public string sec_type;
        public SecType SecType
        {
            get
            {
                if (sec_type == "普通股")
                {
                    return SecType.Equit;
                }
                else
                {
                    if (sec_type.IndexOf("指数") > 0)
                    {
                        return SecType.Index;
                    }
                }
                return SecType.Other;
            }
        }
        public BaseDataItemClass()
        {


        }

        public BaseDataItemClass(DataRow dr)
        {

            FillByDataRow(dr);
        }

        public void FillByDataRow(DataRow dr)
        {
            Type MyInfo = this.GetType();
            foreach (FieldInfo n in MyInfo.GetFields())
            {
                if (dr.Table.Columns.Contains(n.Name.ToUpper()))
                {
                    if (dr[n.Name.ToUpper()] != null && dr[n.Name.ToUpper()] != DBNull.Value)
                        n.SetValue(this, Convert.ChangeType(dr[n.Name.ToUpper()], n.FieldType));
                }
            }
        }

        public DataRow FillRow(DataRow dr)
        {
            Type MyInfo = this.GetType();
            foreach (FieldInfo n in MyInfo.GetFields())
            {
                if (dr.Table.Columns.Contains(n.Name.ToUpper()))
                    dr[n.Name.ToUpper()] = Convert.ChangeType(n.GetValue(this), n.FieldType);
            }
            return dr;
        }

        DateTime[] OnMarketDays = new DateTime[0] { };

        /// <summary>
        /// 上市交易日数，不包括停牌，且是大约数。
        /// </summary>
        public int OnMarketDayCount
        {
            get
            {
                if (OnMarketDays.Length == 0)
                {
                    if (this.SecType == SecType.Equit)
                    {
                        //OnMarketDays =  WDDayClass.getTradeDates(w,this.Ipo_date,this.DateTime);
                        return this.DateTime.Subtract(this.Ipo_date).Days * 5 / 7;//OnMarketDays.Length;
                    }
                }
                return 99999;
            }
        }
    }

}
