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
            strNewestTable = "Newestdata";
            strHistoryTable = "historydata";

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
                case "PK10":
                {
                        ret = new ExpectReader();
                        break;
                }
                case "CN_Stock_A":
                {
                        ret = new SecurityReader(strType,docName,codes);
                        break;
                }
                default:
                    {
                        ret = new ExpectReader();
                        break;
                    }
            }
            return ret;
        }
    }
}
