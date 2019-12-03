using WolfInv.com.PK10CorePress;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.SecurityLib;
namespace WolfInv.com.ExchangeLib
{
    public class ExpectListProcessBuilder<T> where T:TimeSerialData
    {
        ExpectList<T> data;
        DataTypePoint dtp;
        public ExpectListProcessBuilder(DataTypePoint _dtp,ExpectList<T> _data)
        {
            dtp = _dtp;
            data = _data;
        }

        public CommExpectListProcess<T> getProcess()
        {
            CommExpectListProcess<T> ret = null;
            if (dtp.IsSecurityData == 1)
            {
                ret = new SecurityListProcess<T>(data);
            }
            else
            {
                switch (dtp.DataType)
                {
                    case "PK10":
                        {
                            ret = new ExpectListProcess(new ExpectList(data.Table)) as CommExpectListProcess<T>;// ConvertionExtensions.CopyTo<CommExpectListProcess<T>>(new ExpectListProcess(new ExpectList(data.Table)));
                            break;
                        }
                    case "SCKL12":
                    case "NLKL12":
                        {
                            ret = new CombinLottery_ExpectListProcess(new ExpectList(data.Table)) as CommExpectListProcess<T>;
                            (ret as CombinLottery_ExpectListProcess).AllNums = dtp.AllNums;
                            (ret as CombinLottery_ExpectListProcess).SelectNums = dtp.SelectNums;
                            (ret as CombinLottery_ExpectListProcess).strAllTypeOdds = dtp.strAllTypeOdds;
                            (ret as CombinLottery_ExpectListProcess).strCombinTypeOdds = dtp.strCombinTypeOdds;
                            (ret as CombinLottery_ExpectListProcess).strPermutTypeOdds = dtp.strPermutTypeOdds;

                            break;
                        }
                    case "GDKL11":
                        {
                            ret = new CombinLottery_ExpectListProcess(new ExpectList(data.Table)) as CommExpectListProcess<T>;
                            (ret as CombinLottery_ExpectListProcess).AllNums = dtp.AllNums;
                            (ret as CombinLottery_ExpectListProcess).SelectNums = dtp.SelectNums;
                            (ret as CombinLottery_ExpectListProcess).strAllTypeOdds = dtp.strAllTypeOdds;
                            (ret as CombinLottery_ExpectListProcess).strCombinTypeOdds = dtp.strCombinTypeOdds;
                            (ret as CombinLottery_ExpectListProcess).strPermutTypeOdds = dtp.strPermutTypeOdds;
                            break;
                        }
                    default:
                        {
                            break;
                        }
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
            if(dtp.IsSecurityData==1)
            {
                ret = new SecurityReader(dtp.DataType, dtp.NewestTable, codes);
            }
            else
            {
                //ret = new ExpectReader();
                switch (dtp.DataType)
                {
                    case "PK10":
                        {
                            ret = new ExpectReader();
                            break;
                        }
                    case "SCKL12":
                        {
                            ret = new SCKL12_ExpectReader();
                            break;
                        }
                    case "NLKL12":
                        {
                            ret = new NLKL12_ExpectReader();
                            break;
                        }
                    case "GDKL11":
                        {
                            ret = new GDKL11_ExpectReader();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
           
            return ret;
        }
    }
}
