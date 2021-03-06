﻿using WolfInv.com.PK10CorePress;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.SecurityLib;
namespace WolfInv.com.ExchangeLib
{
    public class ExpectListProcessBuilderForAll<T> where T : TimeSerialData
    {
        ExpectList<T> data;
        DataTypePoint dtp;
        public ExpectListProcessBuilderForAll(DataTypePoint _dtp, ExpectList<T> _data)
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
                    case "XYFT":

                        {
                            ret = new ExpectListProcess(new ExpectList(data.Table.Copy())) as CommExpectListProcess<T>;// ConvertionExtensions.CopyTo<CommExpectListProcess<T>>(new ExpectListProcess(new ExpectList(data.Table)));
                            ret.AllNums = 10;
                            ret.SelectNums = 10;
                            break;
                        }
                    case "SCKL12":
                    case "NLKL12":
                        {
                            ret = new CombinLottery_ExpectListProcess(new ExpectList(data.Table)) as CommExpectListProcess<T>;
                            ret.AllNums = dtp.AllNums;
                            ret.SelectNums = dtp.SelectNums;
                            ret.TenToZero = false;
                            
                            (ret as CombinLottery_ExpectListProcess).strAllTypeOdds = dtp.strAllTypeOdds;
                            (ret as CombinLottery_ExpectListProcess).strCombinTypeOdds = dtp.strCombinTypeOdds;
                            (ret as CombinLottery_ExpectListProcess).strPermutTypeOdds = dtp.strPermutTypeOdds;

                            break;
                        }
                    case "GDKL11":
                    default:
                        {
                            ret = new CombinLottery_ExpectListProcess(new ExpectList(data.Table)) as CommExpectListProcess<T>;
                            ret.AllNums = dtp.AllNums;
                            ret.SelectNums = dtp.SelectNums;
                            ret.TenToZero = dtp.TenToZero;
                            (ret as CombinLottery_ExpectListProcess).strAllTypeOdds = dtp.strAllTypeOdds;
                            (ret as CombinLottery_ExpectListProcess).strCombinTypeOdds = dtp.strCombinTypeOdds;
                            (ret as CombinLottery_ExpectListProcess).strPermutTypeOdds = dtp.strPermutTypeOdds;
                            break;
                        }
                   
                }
            }
            return ret;
        }
    }


    public class ReaderBuilder<T> where T:TimeSerialData
    {
        DataTypePoint dtp;
        string[] codes;
        public ReaderBuilder(DataTypePoint _dtp,string[] _codes)
        {
            dtp = _dtp;
            codes = _codes;
        }

        public DataReader<T> getReader()
        {
            DataReader<T> ret = null;
            if(dtp.IsSecurityData==1)
            {
                ret = new SecurityReader<T>(dtp.DataType, dtp.NewestTable, codes);
            }
            else
            {
                //ret = new ExpectReader();
                switch (dtp.DataType)
                {
                    case "PK10":
                        {
                            ret = new ExpectReader<T>();
                            break;
                        }
                    case "SCKL12":
                        {
                            ret = new SCKL12_ExpectReader<T>();
                            break;
                        }
                    case "NLKL12":
                        {
                            ret = new NLKL12_ExpectReader<T>();
                            break;
                        }
                    case "GDKL11":
                        {
                            ret = new GDKL11_ExpectReader<T>();
                            break;
                        }
                    case "XYFT":
                        {
                            ret = new XYFT_ExpectReader<T>();
                            break;
                        }
                    case "CQSSC":
                        {
                            ret = new CQSSC_ExpectReader<T>();
                            break;
                        }
                    case "XJSSC":
                        {
                            ret = new XJSSC_ExpectReader<T>();
                            break;
                        }
                    case "TJSSC":
                        {
                            ret = new TJSSC_ExpectReader<T>();
                            break;
                        }
                    case "JSK3":
                        {
                            ret = new JSK3_ExpectReader<T>();
                            break;
                        }
                    default:
                        {
                            ret = new KL11_ExpectReader<T>(dtp.DataType);
                            break;
                        }
                }
            }
           
            return ret;
        }
    }
}
