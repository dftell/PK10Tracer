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

    public class CAN28ExpectReader : CommExpectReader
    {
        public CAN28ExpectReader()
        {
            strDataType = "CAN28";


            strNewestTable = "CAN28_Newestdata";
            strHistoryTable = "CAN28_historydata";
            strMissHistoryTable = "v_CAN28_HistoryData_Miss";
            strMissNewestTable = "v_CAN28_NewestData_Miss";
            InitTables();
        }
    }

    public class SCKL12_ExpectReader : CommExpectReader
    {
        public SCKL12_ExpectReader()
        {
            strDataType = "SCKL12";


            ////strNewestTable = "CAN28_Newestdata";
            ////strHistoryTable = "CAN28_historydata";
            ////strMissHistoryTable = "v_CAN28_HistoryData_Miss";
            ////strMissNewestTable = "v_CAN28_NewestData_Miss";
            InitTables();
        }
    }

    public class NLKL12_ExpectReader : CommExpectReader
    {
        public NLKL12_ExpectReader()
        {
            strDataType = "NLKL12";


            ////strNewestTable = "CAN28_Newestdata";
            ////strHistoryTable = "CAN28_historydata";
            ////strMissHistoryTable = "v_CAN28_HistoryData_Miss";
            ////strMissNewestTable = "v_CAN28_NewestData_Miss";
            InitTables();
        }
    }

    //全国快乐11彩种
    public class KL11_ExpectReader: CommExpectReader
    {

    }

    public class GDKL11_ExpectReader : KL11_ExpectReader
    {
        public GDKL11_ExpectReader()
        {
            strDataType = "GDKL11";
            InitTables();
        }
    }

}
