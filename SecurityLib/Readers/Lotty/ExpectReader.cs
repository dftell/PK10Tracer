using System.Linq;
using System.Web;
using WolfInv.com.LogLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public class PK10ExpectReader<T> : CommExpectReader<T> where T : TimeSerialData
    {
        public PK10ExpectReader()
            : base()
        {
            this.strDataType = "PK10";
            InitTables();
        }

        
    }
    
    public class ExpectReader<T> : PK10ExpectReader<T> where T : TimeSerialData
    {
        public ExpectReader()
            : base()
        {
            //strNewestTable = "Newestdata";
            //strHistoryTable = "historydata";

        }
    }

    public class PK10ProbWaveDataInterface<T> : PK10ExpectReader<T> where T : TimeSerialData
    {
        public PK10ProbWaveDataInterface()
        {
            strResultTable = "tmp_PK10_LongTermAll_ProbWaveTable";
            InitTables();
        }
    }

    public class DataReaderBuild
    {
        public static DataReader<T>  CreateReader<T>(string strType,string docName,string[] codes) where T:TimeSerialData
        {
            if(!GlobalClass.TypeDataPoints.ContainsKey(strType))
            {
                return null;
            }
            DataTypePoint dtp = GlobalClass.TypeDataPoints[strType];
            DataReader<T> ret = null;
            switch(strType)
            {
                case "CAN28":
                {
                        ret = new CAN28ExpectReader<T>();
                        break;
                }
                case "TXFFC":
                    {
                        ret = new TXFFCExpectReader<T>();
                        break;
                    }
                case "CN_Stock_A":
                {
                        ret = new SecurityReader<T>(strType,docName,codes);
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
                case "PK10":
                    {
                        ret = new ExpectReader<T>();
                        break;
                    }
                default:
                    {
                        ret = new KL11_ExpectReader<T>(strType);
                        break;
                    }
            }
            return ret;
        }

        public static CommExpectReader<T> CreateInstance<T>(string strtype) where T:TimeSerialData
        {
            CommExpectReader<T> ret = null;
            switch (strtype)
            {
                case "PK10":
                    {
                        ret = new PK10ExpectReader<T>();
                        break;
                    }
                case "TXFFC":
                    {
                        ret = new TXFFCExpectReader<T>();
                        break;
                    }
                case "":
                    {
                        break;
                    }

            }

            return ret;
        }
    }


}
