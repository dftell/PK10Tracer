using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace WolfInv.com.BaseObjectsLib
{
    public abstract class BaseCollection<T> : IBaseCollection where T : TimeSerialData
    {
        public int AllNums;
        public int SelNums;
        public abstract AmoutSerials getOptSerials(DataTypePoint dtp, string type, int len, double odds, Int64 MaxValue, int FirstAmt, bool NeedAddFirst);
        public abstract DataTable getTableFromSpecCondition(DataTypePoint dtp,string currExpect, int Period, string strPos, string strTaget, params object[] others);
        /// <summary>
        /// 外部现成的统计数据，可以直接供策略统计分析，不必自己重新从数据库中计算结果
        /// </summary>
        public abstract DataSet ExDataTable(DataTypePoint dtp, string expect, Func<DataTypePoint,string, DataSet> convertFunc);
        public abstract DataTable Table { get; }
        public abstract DataTableEx CarDistributionTable { get; }
        public abstract DataTableEx CarTable { get; }
        public abstract DataTableEx SerialDistributionTable { get; }

        public abstract bool isByNo { get; set; }

        public List<Dictionary<int, string>> Data;
        public ExpectList<T> ___orgData;
        public abstract List<double> getAllDistrStdDev(int n,int c);
        public abstract DataTableEx getSubTable(int cnt, int n);
        public abstract int FindLastDataExistCount(int StartPos, int lng, string StrKey, string val);
        public abstract int FindLastDataExistCount(int lng, string StrPos, string key);
        public abstract string FindSpecColumnValue(int id, string strKey);
        public abstract Dictionary<string, double> getAllShiftCnt(int ReviewCnt, int TrainCnt);
        public abstract Dictionary<string, Matrix> getC_K_NStep(int reviewCnt, int StepCnt);
        public abstract List<double> getEntropyList(int reviewCnt);

        public abstract bool isMatch(string code, string currCode);

        public abstract Dictionary<string,T1> getFeatureDic<T1>(bool bySer,string strModel="{0}/{1}");
    }

    public interface IBaseCollection
    {
        /// <summary>
        /// 从指定条件获得表，兼容外部Web和本地数据库
        /// </summary>
        /// <param name="dtp"></param>
        /// <param name="Period"></param>
        /// <param name="strPos"></param>
        /// <param name="strTaget"></param>
        /// <returns></returns>
        DataTable getTableFromSpecCondition(DataTypePoint dtp, string currExpect, int Period, string strPos, string strTaget,params object[] others);
        DataSet ExDataTable(DataTypePoint dtp, string expect, Func<DataTypePoint,string, DataSet> convertFunc);
        DataTableEx CarDistributionTable { get; }
        DataTableEx CarTable { get; }
        DataTableEx SerialDistributionTable { get; }
        DataTable Table { get; }

        int FindLastDataExistCount(int StartPos, int lng, string StrKey, string val);
        int FindLastDataExistCount(int lng, string StrPos, string key);
        string FindSpecColumnValue(int id, string strKey);
        List<double> getAllDistrStdDev(int reviewCnt, int stepLong);
        Dictionary<string, double> getAllShiftCnt(int ReviewCnt, int TrainCnt);
        Dictionary<string, Matrix> getC_K_NStep(int reviewCnt, int StepCnt);
        List<double> getEntropyList(int reviewCnt);
        DataTableEx getSubTable(int FromId, int lng);
    }

    public class LongDrgTableProcessor
    {
        public Dictionary<int,Dictionary<int,LongDrgNumberInfo>> AllLongDrgInfs;
        public LongDrgTableProcessor(DataTable dt,int minRows)
        {
            AllLongDrgInfs = new Dictionary<int, Dictionary<int, LongDrgNumberInfo>>();
            
            for (int r = minRows; r < dt.Rows.Count; r++)
            {
                for(int c=0;c<dt.Columns.Count;c++)
                {
                    string col = dt.Columns[c].ColumnName;
                    int icol = int.Parse(col);
                    Dictionary<int,LongDrgNumberInfo> colDrgInfs = new Dictionary<int, LongDrgNumberInfo>();
                    if (!AllLongDrgInfs.ContainsKey(icol))
                    {
                        AllLongDrgInfs.Add(icol, colDrgInfs);
                    }
                    else
                    { 
                        colDrgInfs = AllLongDrgInfs[icol];
                    }
                    string val = dt.Rows[r][col].ToString();
                    val.ToList().ForEach(a => {
                        string num = a.ToString();
                        if (num.Trim().Length == 0)
                            return;
                        int inum = int.Parse(num);
                        if(!colDrgInfs.ContainsKey(inum))
                        {
                            colDrgInfs.Add(inum, new LongDrgNumberInfo() { Pos=inum, Long = minRows-1, displayLong = 0 });
                        }
                        colDrgInfs[inum].Long++;
                        colDrgInfs[inum].displayLong++;
                    });
                }
            }
        }
    }

    

    public class LongDrgNumberInfo
    {
        public int Pos;
        public int Long;
        public int displayLong;
        
    }
}
