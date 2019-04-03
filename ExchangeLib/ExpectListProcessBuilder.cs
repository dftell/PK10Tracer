using WolfInv.com.PK10CorePress;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.SecurityLib;
namespace WolfInv.com.ExchangeLib
{
    public class ExpectListProcessBuilder<T> where T:TimeSerialData
    {
        ExpectList<T> data;
        public ExpectListProcessBuilder(ExpectList<T> _data)
        {
            data = _data;
        }

        public CommExpectListProcess<T> getProcess()
        {
            CommExpectListProcess<T> ret = null;
            switch (data.UseType.DataType)
            {
                case ("PK10"):
                    {
                        ret = new ExpectListProcess(data as ExpectList) as CommExpectListProcess<T>;
                        break;
                    }
                case (""):
                    {
                        break;
                    }
                default:
                    {

                        ret = new SecurityListProcess<T>(data);
                        break;
                    }
            }
            return ret;
        }
    }

    public class ReaderBuilder
    {
        DataTypePoint dtp;
        string[] codes;
        public ReaderBuilder(DataTypePoint _dtp,string[] _codes)
        {
            dtp = _dtp;
            codes = _codes;
        }

        public DataReader getReader()
        {
            DataReader ret = null;
            switch (dtp.DataType)
            {
                case ("PK10"):
                    {
                        ret = new ExpectReader();
                        break;
                    }
                case (""):
                    {
                        break;
                    }
                default:
                    {

                        ret = new SecurityReader(dtp.DataType,dtp.NewestTable, codes);
                        break;
                    }
            }
            return ret;
        }
    }
}
