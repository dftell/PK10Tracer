using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PK10CorePress;
using System.Data;
using ProbMathLib;
using System.ComponentModel;
namespace Strags
{

    [DescriptionAttribute("通用概率分布选号策略"),
        DisplayName("通用概率分布选号策略")]
    public abstract class strag_CommProbabilityDistributionClass : StragClass,IProbCheckClass
    {
        
   
        public strag_CommProbabilityDistributionClass():base()
        {
            _StragClassName = "通用概率分布选号策略";
            this.StagSetting = this.getInitStagSetting();
        }

        public override List<ChanceClass> getChances(CommCollection sc, ExpectData ed)
        {
            List<ChanceClass> ret = new List<ChanceClass>();
            DataTableEx dt = null;
            if (this.BySer)
            {
                dt = sc.SerialDistributionTable;
            }
            else
            {
                dt = sc.CarDistributionTable;
            }
            if (dt == null)
            {
                throw new Exception("无法获得概率分布表！");
            }
            string strCodes = "";
            int AllChipCnt = 0;
            for (int i = 0; i < 10; i++)
            {
                //获得各项的最小的
                List<double> coldata = null;
                string strCol = string.Format("{0}",(i+1)%10);
                dt.getColumnData(strCol, ref coldata);
                double avgval = coldata.Average();
                double stdval = ProbMath.CalculateStdDev(coldata);
                string strSql = string.Format("[{0}]<{1}", strCol, avgval - this.StdvCnt * stdval);
                string strSort = string.Format("[{0}] asc", "Id");
                DataRow[] drs = dt.Select(strSql,strSort);
                if (drs.Length < this.ChipCount)
                {
                    continue;
                }
                
                string strCode = "";
                StringBuilder sb = new StringBuilder();
                bool Matched = false;
                for (int j = 0; j < drs.Length; j++)
                {
                    string strId = drs[j]["Id"].ToString();
                    int RowCnt = sc.FindLastDataExistCount(this.InputMinTimes, strCol, strId);
                    if (RowCnt > 0)//任何一个不匹配最近5期内出现，不满足条件
                    {
                        Matched = false;
                        break;
                    }
                    sb.Append(drs[j]["Id"].ToString());
                    Matched = true;
                }
                if (!Matched) continue;
                AllChipCnt += drs.Length;
                if (BySer)
                    strCode = string.Format("{0}/{1}", strCol, sb.ToString());
                else
                    strCode = string.Format("{0}/{1}", sb.ToString(), strCol);
                if (strCode.Length > 0)
                    strCodes = string.Format("{0}{1}{2}",strCodes,strCodes.Length>0?"+":"",strCode);                                                                                                                                                                                                                                           
            }
            if (strCodes.Length < 2*(this.ChipCount+2)) 
                return ret;
            ChanceClass cc = new ChanceClass();
            cc.SignExpectNo = ed.Expect;
            cc.ChanceType = 3;
            cc.InputTimes = 1;
            cc.strInputTimes = "1";
            cc.AllowMaxHoldTimeCnt = 1;
            cc.InputExpect = ed;
            cc.ChipCount = AllChipCnt;
            cc.ChanceCode = strCodes;
            cc.CreateTime = ed.OpenTime;
            cc.Closed = false;
            ret.Add(cc); 
            return ret;
        }

        double _stdcnt;

        //public abstract StagConfigSetting getInitStagSetting();
        
        public double StdvCnt
        {
            get
            {
                return _stdcnt;
            }
            set
            {
                _stdcnt = value;
            }
        }
    }


}
