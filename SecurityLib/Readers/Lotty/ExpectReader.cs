using System.Linq;
using System.Web;
using WolfInv.com.LogLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.SecurityLib
{
    public class PK10ExpectReader : CommExpectReader
    {
        public PK10ExpectReader()
            : base()
        {
            this.strDataType = "PK10";
            InitTables();
        }
    }
    
    public class ExpectReader : PK10ExpectReader
    {
        public ExpectReader()
            : base()
        {
            //strNewestTable = "Newestdata";
            //strHistoryTable = "historydata";

        }
    }

    public class PK10ProbWaveDataInterface : PK10ExpectReader
    {
        public PK10ProbWaveDataInterface()
        {
            strResultTable = "tmp_PK10_LongTermAll_ProbWaveTable";
            InitTables();
        }
    }

    public class DataReaderBuild
    {
        public static DataReader  CreateReader(string strType,string docName,string[] codes)
        {
            DataTypePoint dtp = GlobalClass.TypeDataPoints[strType];
            DataReader ret = null;
            switch(strType)
            {
                case "CAN28":
                {
                        ret = new CAN28ExpectReader();
                        break;
                }
                case "TXFFC":
                    {
                        ret = new TXFFCExpectReader();
                        break;
                    }
                case "CN_Stock_A":
                {
                        ret = new SecurityReader(strType,docName,codes);
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
                case "PK10":
                default:
                    {
                        ret = new ExpectReader();
                        break;
                    }
            }
            return ret;
        }

        public static CommExpectReader CreateInstance(string strtype)
        {
            CommExpectReader ret = null;
            switch (strtype)
            {
                case "PK10":
                    {
                        ret = new PK10ExpectReader();
                        break;
                    }
                case "TXFFC":
                    {
                        ret = new TXFFCExpectReader();
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
