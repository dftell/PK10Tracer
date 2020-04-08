using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.ProbMathLib;

namespace WolfInv.com.PK10CorePress
{
    public class CommCollection_KLXxY : PK10CorePress.CommCollection
    {
        DataTable _dt;
        public int AllNums { get; set; }
        public int SelNums { get; set; }

        public string strAllTypeOdds { get; set; }
        public string strCombinTypeOdds { get; set; }
        public string strPermutTypeOdds { get; set; }
        public override DataTable Table
        {
            get
            {
                if(_dt == null)
                {
                    _dt = new DataTable();
                    if (isByNo)
                    {
                        for (int i = 0; i < SelNums; i++)
                        {
                            _dt.Columns.Add(string.Format("{0}", (i + 1) % (SelNums+1)).PadLeft(2,'0'));
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= this.AllNums; i++)
                        {
                            _dt.Columns.Add(i.ToString().PadLeft(2,'0'));
                        }
                        //_dt.Columns.Add("0");
                    }

                    for (int i = 0; i < Data[0].Count; i++)
                    {
                        DataRow dr = _dt.NewRow();
                        if (isByNo)
                        {
                            for (int j = 0; j < SelNums; j++)
                            {
                                dr[(j + 1).ToString().PadLeft(2,'0')] = Data[j][i];
                            }
                        }
                        else
                        {
                            for (int j = 0; j < AllNums; j++)
                            {
                                dr[(j+1).ToString().PadLeft(2,'0')] = Data[j][i];
                            }
                        }
                        _dt.Rows.Add(dr);
                    }
                }
                return _dt;
            }
        }

        public override DataTableEx CarDistributionTable => throw new NotImplementedException();

        public override DataTableEx CarTable {

            get
            {
                if (_CarTable != null) return _CarTable;
                _CarTable = new DataTableEx();
                _CarTable.Columns.Add("Id", typeof(int));
                //_CarTable.Columns.Add("Expect", typeof(string));
                if (isByNo)
                {
                    for (int i = 0; i < SelNums; i++)
                    {
                        _CarTable.Columns.Add(string.Format("{0}", (i + 1) % 10).PadLeft(2,'0'));
                    }
                }
                else
                {
                    for (int i = 1; i < AllNums; i++)
                    {
                        _CarTable.Columns.Add(i.ToString());
                    }

                }

                for (int i = 0; i < Data[0].Count; i++)
                {
                    DataRow dr = _CarTable.NewRow();
                    if (isByNo)
                    {
                        for (int j = 0; j < SelNums; j++)
                        {
                            dr[j] = (this.orgData[i]).ValueList[j];
                        }
                    }
                    else
                    {
                        for (int j = 0; j < AllNums; j++)
                        {
                            for (int c = 0; c < this.orgData[i].ValueList.Length; c++)
                            {
                                if(int.Parse(this.orgData[i].ValueList[c])-1 == j)
                                {
                                    dr[this.orgData[i].ValueList[c].PadLeft(2,'0')] = 1;
                                }
                                else
                                {
                                    dr[this.orgData[i].ValueList[c].PadLeft(2, '0')] = 0;
                                }
                            }
                        }
                    }
                    dr["Id"] = Data[0].Count - i;
                    _CarTable.Rows.Add(dr);

                }
                return _CarTable;


            }
        }

        public override DataTableEx SerialDistributionTable => throw new NotImplementedException();

        public override bool isByNo { get; set; }

        public override int FindLastDataExistCount(int StartPos, int lng, string StrKey, string val)
        {
            throw new NotImplementedException();
        }

        public override int FindLastDataExistCount(int lng, string StrPos, string key)
        {
            throw new NotImplementedException();
        }

        public override string FindSpecColumnValue(int id, string strKey)
        {
            throw new NotImplementedException();
        }

        public override List<double> getAllDistrStdDev(int n, int c)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, double> getAllShiftCnt(int ReviewCnt, int TrainCnt)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, Matrix> getC_K_NStep(int reviewCnt, int StepCnt)
        {
            throw new NotImplementedException();
        }

        public override List<double> getEntropyList(int reviewCnt)
        {
            throw new NotImplementedException();
        }

        public override DataTableEx getSubTable(int cnt, int n)
        {
            throw new NotImplementedException();
        }

        public override DataTable getTableFromSpecCondition(DataTypePoint dtp,string expect, int Period, string strPos, string strTaget, params object[] others)
        {
            DataTable dt = new DataTable();
            if(dtp.ExDataConfig.FromDataBase != "1")
            {
                int iPos = int.Parse(strPos);
                string strType = "";
                if(iPos < 9) //2~8为任N
                {
                    strType = "A" ;
                }
                else if(iPos<30) //11,14为直选2，3
                {
                    strType = "P";
                }
                else //31，32为组2，3
                {
                    strType = "C" ;
                }
                return getCombMatchMetrix(Period,strType, int.Parse(strTaget));
            }
            else
            {
                return null;
            }
            return dt;
        }

        DataTable getCombMatchMetrix(int period, string strType,int N)
        {
            DataTable dt = new DataTable();
            if (orgData.Count < period)//数据长度必须要不小于期数
                return null;
            List<string> combList = new List<string>();
            if (strType == "P")
            {
                return null;
            }
            else//A,C
            {
                CombinClass allNcmb = CombinClass.CreateNumCombin(AllNums, N);//获得所有N个
                combList = allNcmb;
            }
            dt.Columns.Add("id",typeof(int));
            combList.ForEach(a => dt.Columns.Add(a, typeof(int)));
            ExpectList currList = new ExpectList() ;
            for (int i=orgData.Count-period;i<orgData.Count;i++)
            {
                ExpectData<TimeSerialData> ed = new ExpectData<TimeSerialData>();
                ed.OpenCode = orgData[i].OpenCode;
                ed.Expect = orgData[i].Expect;
                
                currList.Add(ed);
            }
            
            for(int i=0;i<currList.Count;i++)
            {
                DataRow dr = dt.NewRow();
                dr["id"] = i;
                string opencode = currList[i].OpenCode;
                opencode = CombinGenerator.ResortNumString(opencode, ",");
                CombinClass occ = null;
                if (N <= SelNums)
                {
                    occ = new CombinClass(opencode, N);
                    combList.ForEach(a => {
                        dr[a] = occ.Contains(a) ? 1 : 0; 
                    });
                }
                else
                {
                    combList.ForEach(a =>
                    {
                        occ = new CombinClass(a, SelNums);
                        if(occ.Contains(opencode))
                        {
                            dr[a] = 1;
                        }
                        else
                        {
                            dr[a] = 0;
                        }
                    });
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public override bool isMatch(string code, string currCode)
        {
            int N = code.Split(',').Length;
            string opencode = CombinGenerator.ResortNumString(currCode, ",");
            if (N <= SelNums)
            {
                CombinClass occ = new CombinClass(opencode, N);
                return occ.Contains(code);
            }
            else
            {
                return new CombinClass(code, SelNums).Contains(opencode);
            }
            
        }

        public double getProbTimes(DataTypePoint dtp,int peroid, string strPos, string strTaget, params object[] others)
        {
            return peroid * getProb(dtp, strPos, strTaget, others);
        }
        public double getProb(DataTypePoint dtp , string strPos, string strTaget, params object[] others)
        {
            int iPos = 0;
            string strType = "";
            if (int.TryParse(strPos, out iPos))
            {

                if (iPos < 9) //2~8为任N
                {
                    strType = "A";
                }
                else if (iPos < 30) //11,14为直选2，3
                {
                    strType = "P";
                }
                else //31，32为组2，3
                {
                    strType = "C";
                }
            }
            else
            {
                strType = strPos;
            }
            Decimal allprob = 0;
            Decimal iMatch = 0;
            int iTarget = int.Parse(strTaget);
            switch(strType)
            {
                case "P":
                    {
                        iMatch = 1;
                        allprob = ProbMath.GetFactorial(this.AllNums, iTarget);
                        break;
                    }
                case "C":
                    {
                        iMatch = 1;
                        allprob = ProbMath.GetCombination(this.AllNums, iTarget);
                        break;
                    }
                case "A":
                default:
                    {
                        allprob = ProbMath.GetCombination(this.AllNums, iTarget);
                        if (iTarget<=this.SelNums)
                        {
                            iMatch = ProbMath.GetCombination(this.SelNums, iTarget);
                            
                        }
                        else
                        {
                            iMatch = ProbMath.GetCombination(this.AllNums-this.SelNums,iTarget-this.SelNums);
                        }
                        break;
                    }
            }
            return (double)(iMatch/allprob);
        }

        static Dictionary<string, AmoutSerials> AllSerialSettings;

        public AmoutSerials getOptSerials(DataTypePoint dtp,string type, int len, double odds, Int64 MaxValue, int FirstAmt, bool NeedAddFirst)
        {
            double prob = getProb(dtp, type, len.ToString(), null);
            double rodds = odds / prob/10;
            return getOptSerials_detail(type, len, rodds, MaxValue, FirstAmt, NeedAddFirst);
        }
         AmoutSerials getOptSerials_detail(string type,int len,double odds, Int64 MaxValue, int FirstAmt, bool NeedAddFirst)
        {
            string model = "key_{0}_{1}_{2}_{3}_{4}";
            string key = string.Format(model,type,len, odds, MaxValue, FirstAmt);
            if (AllSerialSettings == null)
                AllSerialSettings = new Dictionary<string, AmoutSerials>();
            if (AllSerialSettings.ContainsKey(key))
                return AllSerialSettings[key];
            AmoutSerials retval = new AmoutSerials();
            if (double.IsNaN(odds) || MaxValue == 0)
            {
                return retval;
            }
            int calcLen = 1;
            int[] ret = new int[calcLen];
            double[] Rates = new double[calcLen];
            Int64[] MaxSum = new Int64[calcLen];
            Int64[][] Serials = new Int64[calcLen][];
            //int MaxValue = 20000;
            //double odds = 9.75;
            for (int i = 0; i < ret.Length; i++)
            {
                Int64[] Ser = new Int64[0];
                int MaxCnts = 1;
                double bRate = 0.0005;
                double stepRate = 0.001;
                Int64 CurrSum = getSum(i + 1, MaxCnts, FirstAmt, odds, 0, out Ser);
                while (CurrSum < MaxValue)//计算出在指定资金范围内保本所能达到的次数
                {
                    MaxCnts++;
                    CurrSum = getSum(i + 1, MaxCnts, 1, odds, 0, out Ser);
                }
                MaxCnts--; //回退1
                long TestSum = getSum(i + 1, MaxCnts, FirstAmt, odds, bRate, out Ser);
                if (TestSum > MaxValue)//如果最少盈利下所需资金大于指定值，所能达到的次数减一
                {
                    bRate = 0;
                    stepRate = 0.0001;
                    CurrSum = getSum(i + 1, MaxCnts, FirstAmt, odds, bRate, out Ser);
                }
                else
                {
                    CurrSum = TestSum;
                }
                Int64 LastSum = CurrSum;
                double LastRate = bRate;
                while (CurrSum < MaxValue)
                {
                    LastSum = CurrSum;
                    LastRate = bRate;
                    bRate += stepRate;
                    CurrSum = getSum(i + 1, MaxCnts, FirstAmt, odds, bRate, out Ser);
                }
                CurrSum = getSum(i + 1, MaxCnts, FirstAmt, odds, LastRate, out Ser);
                if (i == 0)//对于单注，多加10个元素，给重复策略用。
                {
                    getSum(i + 1, MaxCnts + 10, FirstAmt, odds, LastRate, out Ser);
                }
                if (NeedAddFirst)
                {
                    List<long> list = Ser.ToList();
                    ret[i] = MaxCnts + 1;
                    list.Insert(0, FirstAmt);
                    Rates[i] = bRate - stepRate;
                    MaxSum[i] = LastSum + FirstAmt;
                    Serials[i] = list.ToArray();
                }
                else
                {
                    ret[i] = MaxCnts;
                    Rates[i] = bRate - stepRate;
                    MaxSum[i] = LastSum;
                    Serials[i] = Ser;
                }
            }
            retval.MaxHoldCnts = ret;
            retval.MaxRates = Rates;
            retval.Serials = Serials;
            if (!AllSerialSettings.ContainsKey(key))//防止计算过程中有其他设置请求了
            {
                AllSerialSettings.Add(key, retval);
            }
            return retval;
        }

        static Int64 getSum(int chips, int holdcnt, int firstAmt, double odd, double MinWRate, out Int64[] serial)
        {
            Int64 sum = 0;
            serial = new Int64[holdcnt];
            for (int i = 1; i <= holdcnt; i++)
            {
                serial[i - 1] = FixCountMethods.getTheAmount(chips, i, firstAmt, odd, MinWRate);
                sum += serial[i - 1] * chips;
            }
            return sum;
        }
    }

}
