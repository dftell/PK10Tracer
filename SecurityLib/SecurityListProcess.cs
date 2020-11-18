using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public class SecurityListProcess <T>: CommExpectListProcess<T> where T :TimeSerialData
    {
        DataTypePoint dtp;
        //public ExpectList<T> orgData;
        public SecurityListProcess(ExpectList<T> _data) :base(_data)
        {
            dtp = GlobalClass.TypeDataPoints["CN_Stock_A"];
        }

        public override List<Dictionary<int, string>> getNoDispNums(int reviewCnt)
        {
            return null;
        }

        public override BaseCollection<T> getSerialData(int reviewCnt, bool ByNo)
        {
            BaseCollection<T> ret = new SecurityCollection<T>();
            CommMarketClass cmc = new CommMarketClass(dtp);
            //cmc.GetMarketsStocks("000001", strendT, 500, true, true, false);
            //ret.___orgData = data;
            return ret;
        }

        /// <summary>
        /// 初步简单过滤，获得当前可用的股票，可以附加条件，如当日是否停牌，是否ST，均线之上等
        /// </summary>
        /// <returns></returns>
        BaseCollection<T> SampleFilterSecurity() 
        {
            SecurityCollection<T> ret = new SecurityCollection<T>();
            ExpectList<T> orgData = Parent_data;
            ret.___orgData = Parent_data;

            return ret;
        }
    }


    public class SecurityCollection<T> : BaseCollection<T> where T :TimeSerialData
    {
        public override DataTableEx CarDistributionTable => throw new NotImplementedException();

        public override DataTableEx CarTable => throw new NotImplementedException();

        public override DataTableEx SerialDistributionTable => throw new NotImplementedException();

        public override bool isByNo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override DataTable Table => throw new NotImplementedException();

        public override DataSet ExDataTable(DataTypePoint dtp, string expect,Func<DataTypePoint,string,DataSet> func)
        {
            return null;
        }

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

        public override Dictionary<string, T1> getFeatureDic<T1>(bool bySer, string strModel = "{0}/{1}")
        {
            throw new NotImplementedException();
        }

        public override AmoutSerials getOptSerials(DataTypePoint dtp, string type, int len, double odds, long MaxValue, int FirstAmt, bool NeedAddFirst)
        {
            throw new NotImplementedException();
        }

        public override DataTableEx getSubTable(int cnt, int n)
        {
            throw new NotImplementedException();
        }

        public override DataTable getTableFromSpecCondition(DataTypePoint dtp, string currExpect, int Period, string strPos, string strTaget, params object[] others)
        {
            throw new NotImplementedException();
        }

        public override bool isMatch(string code, string currCode)
        {
            throw new NotImplementedException();
        }
    }

}
