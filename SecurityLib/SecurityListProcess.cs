using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public class SecurityListProcess <T>: CommExpectListProcess<T> where T :TimeSerialData
    {
        public SecurityListProcess(ExpectList<T> _data) :base(_data)
        {
        }

        public override List<Dictionary<int, string>> getNoDispNums(int reviewCnt)
        {
            return null;
        }

        public override BaseCollection<T> getSerialData(int reviewCnt, bool ByNo)
        {
            BaseCollection<T> ret = new SecurityCollection<T>();
            ret.orgData = data;
            return ret;
        }
    }


    public class SecurityCollection<T> : BaseCollection<T> where T :TimeSerialData
    {
        public override DataTableEx CarDistributionTable => throw new NotImplementedException();

        public override DataTableEx CarTable => throw new NotImplementedException();

        public override DataTableEx SerialDistributionTable => throw new NotImplementedException();

        public override bool isByNo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
    }

}
