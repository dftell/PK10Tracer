using System.Linq;
using System.Web;
using WolfInv.com.LogLib;
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

}
