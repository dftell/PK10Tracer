namespace WolfInv.com.SecurityLib
{
    public class TXFFCExpectReader : CommExpectReader
    {
        public TXFFCExpectReader()
        {
            strDataType = "TXFFC";


            strNewestTable = "TXFFC_Newestdata";
            strHistoryTable = "TXFFC_historydata";
            strMissHistoryTable = "v_TXFFC_HistoryData_Miss";
            strMissNewestTable = "v_TXFFC_NewestData_Miss";
            InitTables();
        }
    }

}
