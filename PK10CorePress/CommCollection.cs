using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BaseObjectsLib;
using LogLib;
using System.Reflection;
using ProbMathLib;
namespace PK10CorePress
{
    public class CommCollection
    {
        //机器学习-上期训练数据
        static Dictionary<string, int[,]> LastTrainInstances;
        //机器学习-上期回顾数据
        static Dictionary<string, int[,]> LastReviewInstances;
        
        
        public ExpectList orgData;
        DataTable _dt;
        DataTableEx _SerialDistributionTalbe;
        DataTableEx _CarDistributionTable;
        public bool isByNo;

        public List<Dictionary<int, string>> Data;
        public DataTable Table
        {
            get
            {
                if (_dt != null) return _dt;
                _dt = new DataTable();
                if (isByNo)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        _dt.Columns.Add(string.Format("{0}", (i + 1) % 10));
                    }
                }
                else
                {
                    for (int i = 1; i < 10; i++)
                    {
                        _dt.Columns.Add(i.ToString());
                    }
                    _dt.Columns.Add("0");
                }

                for (int i = 0; i < Data[0].Count; i++)
                {
                    DataRow dr = _dt.NewRow();
                    if (isByNo)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            dr[string.Format("{0}", (j + 1) % 10)] = Data[j][i];
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            dr[j.ToString()] = Data[j][i];
                        }
                    }
                    _dt.Rows.Add(dr);
                }
                return _dt;
            }
        }

        DataTableEx _CarTable;
        public DataTableEx CarTable
        {
            get
            {
                if (_CarTable != null) return _CarTable;
                _CarTable = new DataTableEx();
                _CarTable.Columns.Add("Id", typeof(int));
                //_CarTable.Columns.Add("Expect", typeof(string));
                if (isByNo)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        _CarTable.Columns.Add(string.Format("{0}", (i + 1) % 10));
                    }
                }
                else
                {
                    for (int i = 1; i < 10; i++)
                    {
                        _CarTable.Columns.Add(i.ToString());
                    }
                    _CarTable.Columns.Add("0");
                }

                for (int i = 0; i < Data[0].Count; i++)
                {
                    DataRow dr = _CarTable.NewRow();
                    if (isByNo)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            dr[string.Format("{0}", (j + 1) % 10)] = this.orgData[i].ValueList[j];
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            dr[this.orgData[i].ValueList[j]] = (j + 1) % 10;
                        }
                    }
                    dr["Id"] = Data[0].Count - i;
                    _CarTable.Rows.Add(dr);

                }
                return _CarTable;
            }
        }

        public DataTableEx getSubTable(int FromId, int lng)
        {
            //DataTableEx ret = null;
            DataTableEx DataCopy = CarTable.Copy();
            if (lng * 2 > DataCopy.Rows.Count)
            {
                for (int i = FromId + lng; i < DataCopy.Rows.Count; i++)
                    DataCopy.Rows.RemoveAt(i);
                for (int i = FromId - 1; i >= 0; i--)
                    DataCopy.Rows.RemoveAt(i);
                return DataCopy;
            }
            DataTableEx ret = DataCopy.Clone() as DataTableEx;
            for (int i = FromId; i < FromId + lng; i++)
            {
                ret.Rows.Add(DataCopy.Rows[i].ItemArray);
            }
            return ret;
        }

        public DataTableEx SerialDistributionTable
        {
            get
            {
                if (_SerialDistributionTalbe == null)
                {
                    //ToAdd:
                    _SerialDistributionTalbe = GetDistribTable();
                }
                return _SerialDistributionTalbe;
            }
        }

        public DataTableEx CarDistributionTable
        {
            get
            {
                if (_CarDistributionTable == null)
                {
                    //ToAdd:
                    _CarDistributionTable = GetDistribTable();
                }
                return _CarDistributionTable;
            }
        }

        DataTableEx GetDistribTable()
        {
            DataTableEx ret = new DataTableEx();
            ret.Columns.Add("Id", typeof(int));
            if (true)
            {
                //ret = new DataTable();
                if (isByNo)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        ret.Columns.Add(string.Format("{0}", (i + 1) % 10));
                    }
                }
                else
                {
                    for (int i = 1; i < 10; i++)
                    {
                        ret.Columns.Add(i.ToString());
                    }
                    ret.Columns.Add("0");
                }
                for (int i = 0; i < 10; i++) //10*10的表
                {
                    DataRow dr = ret.NewRow();
                    dr["Id"] = (i + 1) % 10;
                    ret.Rows.Add(dr);
                }
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        ret.Rows[(i + 1) % 10][j.ToString()] = this.CarTable.Select(string.Format("[{0}]={1}", (i + 1) % 10, j)).Length;
                    }
                }
                return ret;
            }
        }

        public int FindLastDataExistCount(int lng, string StrPos, string key)
        {
            //return this.CarTable.Select(string.Format("[Id]<={2} and [{0}]={1}", StrPos, key, lng)).Length;
            return FindLastDataExistCount(0, lng, StrPos, key);
        }

        /// <summary>
        /// 离当前次数startpos的存在次数
        /// </summary>
        /// <param name="StartPos">离当前次数的位置，当前次数为0</param>
        /// <param name="lng"></param>
        /// <param name="StrPos"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public int FindLastDataExistCount(int StartPos, int lng, string StrKey, string val)
        {
            int Cnt = this.CarTable.Rows.Count;
            string sql = string.Format("([Id]>={3} and [Id]<={2}+{3}) and [{0}]={1}", StrKey, val, lng, StartPos);
            return this.CarTable.Select(sql).Length;
        }

        public string FindSpecColumnValue(int id, string strKey)
        {
            string sql = string.Format("([Id]={0}", id);
            DataRow[] drs = this.CarTable.Select(sql);
            if (!this.CarTable.Columns.Contains(strKey)) return null;
            if (drs.Length != 1) return null;
            return drs[0][strKey].ToString();
        }

        //获得整体偏差度
        public List<double> getAllDistrStdDev(int reviewCnt,int stepLong)
        {
            reviewCnt = 10;
            stepLong = 5;
            //Dictionary<string, double> ret = new Dictionary<string, double>();
            double ret = 0;
            Dictionary<string,double> CntList = new Dictionary<string,double>();
            string key_model = "{0}_{1}";
            List<double> AllColsList = new List<double>();
            for (int i = 0; i < 10; i++)//遍历所有表列
            {
                string strCol = string.Format("{0}",(i+1) %10);
                List<double> colList = new List<double>();
                for (int val = 0; val < 10; val++)
                {
                    string strKey = string.Format(key_model, strCol, val);
                    List<double> keyInReviewCnt = new List<double>();
                    for (int r = 0; r < reviewCnt; r++)
                    {
                        string strVal = val.ToString();
                        int ExistCnt = FindLastDataExistCount(r, stepLong, strCol, strVal);
                        keyInReviewCnt.Add((double)ExistCnt);
                    }
                    double KeyInReviewStdDev = ProbMath.CalculateStdDev(keyInReviewCnt.ToArray());
                    CntList.Add(strKey, KeyInReviewStdDev);
                    colList.Add(KeyInReviewStdDev);
                }
                double colStdDev = ProbMath.CalculateStdDev(colList.ToArray());
                AllColsList.Add(colStdDev);
            }
            ret =ProbMath.CalculateStdDev(CntList.Values.ToArray());
            AllColsList.Add(ret);
            return AllColsList;
        }

        /// <summary>
        /// 获得所有车号/名次的熵
        /// </summary>
        /// <param name="reviewCnt"></param>
        /// <param name="stepLong"></param>
        /// <returns></returns>
        public List<double> getEntropyList(int reviewCnt)
        {
            //Dictionary<string, double> ret = new Dictionary<string, double>();
            double ret = 0;
            Dictionary<string, double> CntList = new Dictionary<string, double>();
            string key_model = "{0}_{1}";
            List<double> AllColsList = new List<double>();
            for (int i = 0; i < 10; i++)//遍历所有表列
            {
                string strCol = string.Format("{0}", (i + 1) % 10);
                List<double> colList = new List<double>();
                for (int val = 0; val < 10; val++)
                {
                    string strKey = string.Format(key_model, strCol, val);
                    //////List<double> keyInReviewCnt = new List<double>();
                    //////for (int r = 0; r < reviewCnt; r++)
                    //////{
                    //////    string strVal = val.ToString();
                    int ExistCnt = FindLastDataExistCount(reviewCnt, strKey, val.ToString());
                    //////    keyInReviewCnt.Add((double)ExistCnt);
                    //////}
                    //////double KeyInReviewStdDev = ProbMath.CalculateStdDev(keyInReviewCnt.ToArray());
                    double p = (double)(ExistCnt/reviewCnt);
                    CntList.Add(strKey, p);
                    colList.Add(p);
                }
                double colEntropy = EntropyClass.GetEntropy(colList.ToArray());//获得各车/次熵值
                AllColsList.Add(colEntropy);
            }
            ret = ProbMath.CalculateStdDev(CntList.Values.ToArray());
            AllColsList.Add(ret);
            return AllColsList;
        }

        /// <summary>
        /// 获取一步转移概率矩阵(Transition Probability Matrix)
        /// </summary>
        /// <param name="reviewCnt"></param>
        /// <returns></returns>
        public Dictionary<string, Matrix> getC_K_NStep(int reviewCnt,int StepCnt)
        {
            Dictionary<string, Matrix> retlist = new Dictionary<string, Matrix>();
            for (int i = 0; i < 10; i++)//遍历所有表列
            {
                Matrix ret = new Matrix(10, 10);
                ////for (int c = 0; c < 10; c++)//初始化矩阵
                ////{
                ////    for (int j = 0; j < 10; j++)
                ////    {
                ////        ret.Detail[c, j] = 0;
                ////    }
                ////}
                string strCol = string.Format("{0}", (i + 1) % 10);
                for (int v = 0; v < reviewCnt; v++)
                {
                    int id = reviewCnt + v + StepCnt;
                    string val = this.FindSpecColumnValue(id,strCol);
                    for (int s = 1; s <= StepCnt; s++)//检查通过n步转移后的值
                    {
                        int tid = id + s;
                        string CheckVal = this.FindSpecColumnValue(tid, strCol);
                        double cnt = ret.Detail[int.Parse(val), int.Parse(CheckVal)];
                        ret.Detail[int.Parse(val), int.Parse(CheckVal)] = cnt + 1;
                        //////if (CheckVal != val)//只有转移才加1
                        //////{
                        //////    break;
                        //////}
                        //////else
                        //////{
                        //////}
                    }
                }
                for (int c = 0; c < 10; c++)//初始化矩阵
                {
                    for (int j = 0; j < 10; j++)
                    {
                        ret.Detail[c, j] = ret.Detail[c, j] / reviewCnt;
                    }
                }
                retlist.Add(strCol, ret);
            }
            return retlist;
        }
    
        /// <summary>
        /// 返回-5~5之间的证数
        /// </summary>
        /// <param name="orgBit"></param>
        /// <param name="NewBit"></param>
        /// <param name="BySer"></param>
        /// <returns></returns>
        public static int getShiftBitCnt(int orgBit,int NewBit)
        {
            if(orgBit == 0) orgBit =10;
            if(NewBit == 0) NewBit = 10;
            if(orgBit == NewBit) return 0;
            int iShift = NewBit - orgBit;
            int AbsShift = Math.Abs(iShift);
            //1=>1 = 0; 1=>2 = 1;1=>6=5;1=>7=-4;1=>10=-1;6=>5=-1;6=>1=5;7=>1=4;0=>1=1
            int iSign = iShift / AbsShift;
            if (AbsShift > 5)
            {
                iShift = iSign * -1 * (10 - AbsShift);
            }
            return iShift;//
        }

        public static int getShiftBitCnt(string orgBit, string NewBit)
        {
            return getShiftBitCnt(int.Parse(orgBit), int.Parse(NewBit));
        }

        public static int getDSSwitch(int orgBit, int NewBit)
        {
            if ((orgBit % 2)==1 & (NewBit % 2)==1) return 0;
            return 1;
        }

        public static int getDSSwitch(string orgBit, string NewBit)
        {
            return getDSSwitch(int.Parse(orgBit), int.Parse(NewBit));
        }

        public static int getDXSwitch(int orgBit, int NewBit)
        {
            if (orgBit == 0) orgBit = 10;
            if (NewBit == 0) NewBit = 10;
            if (orgBit>5 & NewBit >5) return 0;
            return 1;
        }

        public static int getDXSwitch(string orgBit, string NewBit)
        {
            return getDXSwitch(int.Parse(orgBit), int.Parse(NewBit));
        }

        /// <summary>
        /// 机器学习-下期位移
        /// </summary>
        /// <param name="ReviewCnt">计算数据长度</param>
        /// <param name="TrainCnt">训练数据长度</param>
        /// <returns></returns>
        public Dictionary<string,double> getAllShiftCnt(int ReviewCnt,int TrainCnt)
        {
            ExpectData ed = orgData.LastData;
            string lastExpectId = long.Parse(ed.Expect).ToString();
            if (orgData.Count < ReviewCnt + TrainCnt + 1)
            {
                return null;
            }
            int[,] CurrTrainSet = new int[10,TrainCnt];
            int[,] CurrReviewSet = new int[10,ReviewCnt];
            ////if (!LastReviewShiftDistr.ContainsKey(lastExpectId))
            ////{
                
            ////    //LastReviewShiftDistr
            ////}
            return null;
        }

        /// <summary>
        /// 获得指定区间的分类数组
        /// </summary>
        /// <param name="fromRow"></param>
        /// <param name="RowCnt"></param>
        /// <param name="classifyType">分类类型，1，偏移 2，奇偶 3，大小</param>
        /// <returns></returns>
        int[,] getSpecRangeClassifyArray(int fromRow,int RowCnt,int classifyType)
        {
            int[,] ret = new int[10,RowCnt];
            for (int col = 0; col < 10; col++)
            {
                int[,] shiftArr = new int[10,RowCnt];
                string strcol = string.Format("{0}", (col+1)%10);
                for (int r = 0; r < RowCnt; r++)
                {
                    string val = this.FindSpecColumnValue(fromRow + r, strcol);//原始值
                    string nextval = this.FindSpecColumnValue(fromRow + r + 1, strcol);//转移值
                    int ishift = -99999;
                    switch (classifyType)
                    {
                        case 1://偏移
                            ishift = getShiftBitCnt(val, nextval);
                            break;
                        case 2://单双
                            ishift = getDSSwitch(val, nextval);
                            break;
                        case 3://大小
                            ishift = getDXSwitch(val, nextval);
                            break;
                              
                    }
                    ret[col, r] = ishift;
                }
            }
            return ret;
        }
    
    
        
    
    } 

    /// <summary>
    /// 按车号集合，和按名次集合命名搞反了
    /// </summary>
    public class SerialCollection : CommCollection
    {
        public SerialCollection()
        {
            isByNo = true;
        }
    }

    /// <summary>
    /// 按排名集合,和按车号集合命名搞反了
    /// </summary>
    public class CarCollection : CommCollection
    {
        public CarCollection()
        {
            isByNo = false;
        }
    }

}
