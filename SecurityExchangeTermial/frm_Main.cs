using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TradeAPI;
namespace SecurityExchangeTermial
{
    public partial class frm_Main : Form
    {
        public frm_Main()
        {
            InitializeComponent();
        }
        void Login()
        { 
        TradeX.OpenTdx();

 
        string sHost = "222.240.176.149";
        short nPort = 8002;
        string sVersion = "8.0";
        short nYybId = 2100;
        string sAccountNo = "net828@163.com";
        string sTradeAccountNo = "210000020732";
        string sPassword = "850114";
        string sTxPassword = "";

        StringBuilder sErrInfo = new StringBuilder(256);
        StringBuilder sResult = new StringBuilder(1024 * 1024);

        Console.Write("\n2 - Login ... ");
            int clientId = TradeX.Logon(sHost, nPort, sVersion, nYybId, sAccountNo, sTradeAccountNo, sPassword, sTxPassword, sErrInfo);
            if (clientId< 0)
            {

                return;
            }
            else
            {
                Console.WriteLine("ok\n");
            }


            if (true)
            {
                Console.WriteLine("\n3 - QueryData");

                //
                Console.WriteLine("\t 0 - 查询资金 QueryData(client_id, 0)");
                TradeX.QueryData(clientId, 0, sResult, sErrInfo);
                Console.WriteLine("查询资金结果:\n" + sResult + " " + sErrInfo);

                //
                Console.WriteLine("\t 1 - 查询股份 QueryData(client_id, 1)");
                TradeX.QueryData(clientId, 0, sResult, sErrInfo);

                Console.WriteLine("查询股份结果:\n" + sResult + " " + sErrInfo);

                //
                Console.WriteLine("\t 2 - 查询当日委托 QueryData(client_id, 2)");
                TradeX.QueryData(clientId, 0, sResult, sErrInfo);

                Console.WriteLine("查询当日委托结果:\n" + sResult + " " + sErrInfo);

                //
                Console.WriteLine("\t 3 - 查询当日成交 QueryData(client_id, 3)");
                TradeX.QueryData(clientId, 0, sResult, sErrInfo);
                Console.WriteLine("查询当日成交结果:\n" + sResult); // + " " + sErrInfo);

                //
                Console.WriteLine("\t 4 - 查询可撤单 QueryData(client_id, 4)");
                TradeX.QueryData(clientId, 0, sResult, sErrInfo);
                Console.WriteLine("查询可撤单结果:\n" + sResult); // + " " + sErrInfo);

                //
                Console.WriteLine("\t 5 - 查询股东代码 QueryData(client_id, 5)");
                TradeX.QueryData(clientId, 0, sResult, sErrInfo);
                Console.WriteLine("查询股东代码结果:\n" + sResult); // + " " + sErrInfo);

                Console.WriteLine("\t按回车键继续......");
                Console.ReadKey();
            }

            //
            //
            //
            if (true)
            {
                Console.WriteLine("\n4 - QueryHistoryData");

                //
                Console.WriteLine("\t 0 - 历史委托 QueryHistoryData(client_id, 0)");
                TradeX.QueryHistoryData(clientId, 0, "20161201", "20161212", sResult, sErrInfo);
                Console.WriteLine("历史委托结果:\n" + sResult); // + " " + sErrInfo);

                //
                Console.WriteLine("\t 1 - 历史成交 QueryHistoryData(client_id, 1)");
                TradeX.QueryHistoryData(clientId, 1, "20161201", "20161212", sResult, sErrInfo);
                Console.WriteLine("历史成交结果:\n" + sResult); // + " " + sErrInfo);

                //
                Console.WriteLine("\t 2 - 资金流水 QueryHistoryData(client_id, 2)");
                TradeX.QueryHistoryData(clientId, 2, "20161201", "20161212", sResult, sErrInfo);
                Console.WriteLine("资金流水结果:\n" + sResult); // + " " + sErrInfo);

                //
                Console.WriteLine("\t 3 - 交割单 QueryHistoryData(client_id, 3)");
                TradeX.QueryHistoryData(clientId, 3, "20161201", "20161212", sResult, sErrInfo);
                Console.WriteLine("交割单结果:\n" + sResult); // + " " + sErrInfo);

                Console.WriteLine("\t按回车键继续......");
                Console.ReadKey();
            }

            //
            //
            //

            /*
             * 下委托交易证券
             *
             * <param name="ClientID">客户端ID</param>
             * <param name="Category">表示委托的种类，0买入 1卖出  2融资买入  3融券卖出   4买券还券   5卖券还款  6现券还券</param>
             * <param name="PriceType">表示报价方式 0上海限价委托 深圳限价委托 1(市价委托)深圳对方最优价格  2(市价委托)深圳本方最优价格  3(市价委托)深圳即时成交剩余撤销  4(市价委托)上海五档即成剩撤 深圳五档即成剩撤 5(市价委托)深圳全额成交或撤销 6(市价委托)上海五档即成转限价
             * <param name="Gddm">股东代码, 交易上海股票填上海的股东代码；交易深圳的股票填入深圳的股东代码</param>
             * <param name="Zqdm">证券代码</param>
             * <param name="Price">委托价格</param>
             * <param name="Quantity">委托数量</param>
             */

            if (true)
            {
                Console.WriteLine("\n5 - SendOrder(client_id, 0, 4, \"p001001001005793\", \"600600\", 0, 100)");
                Console.WriteLine();
                Console.WriteLine("              买单, 4(市价委托)上海五档即成剩撤 深圳五档即成剩撤, 100股");
                TradeX.SendOrder(clientId, 0, 4, "p001001001005793", "600600", 0, 100, sResult, sErrInfo);
                Console.WriteLine("SendOrder结果:\n" + sResult); // + " " + sErrInfo);
                Console.WriteLine();

                Console.WriteLine("\t按回车键继续......");
                Console.ReadKey();

                Console.WriteLine("\t 查看可撤单");
                TradeX.QueryData(clientId, 4, sResult, sErrInfo);
                Console.WriteLine("可撤单:\n" + sResult); // + " " + sErrInfo);
                Console.WriteLine();

                Console.WriteLine("\t 当日成交");
                TradeX.QueryData(clientId, 3, sResult, sErrInfo);
                Console.WriteLine("当日成交:\n" + sResult); // + " " + sErrInfo);
                Console.WriteLine();

                Console.WriteLine("\t按回车键继续......");
                Console.ReadKey();
            }


            //
            //
            //
            if (true)
            {
                Console.WriteLine("\n7 - GetQuote(client_id, \"600600\") -- 获取5档报价");
                TradeX.GetQuote(clientId, "600600", sResult, sErrInfo);
                Console.WriteLine("获取五档报价结果:\n" + sResult); // + " " + sErrInfo);
                Console.WriteLine();

                Console.WriteLine("\t按回车键继续......");
                Console.ReadKey();
            }

            //return;

            /*

print "8 - GetQuotes(client_id, [ \"600600\", \"000001\", \"601928\" ]) -- 批量获取5档报价\n"
res, err = TradeX.GetQuotes(client_id, [ "600600", "000001", "601928" ] )
print "批量获取五档报价结果:\n", res, " ", err
print
for q in res :
  print q
  print 
print
             * */

            //
            //
            //
            Console.WriteLine();
            Console.WriteLine("测试行情API, 按回车键继续...");
            Console.ReadKey();


            //
            //
            //
            if (true)
            {
                Console.WriteLine("\n*** TdxHq_Connect(\"14.17.75.71\", 7709)");

                bool bRet = TradeX.TdxHq_Connect("14.17.75.71", 7709, sResult, sErrInfo);
Console.WriteLine(bRet);
                Console.WriteLine(sResult); // + " " + sErrInfo);
                Console.WriteLine();

                Console.WriteLine("\t按回车键继续......");
                Console.ReadKey();
            }


            //
            //
            if (true)
            {
                //
                Console.WriteLine("\n*** TdxHq_GetSecurityCount(1) - 上海");

                short nCount = 0;
bool bRet = TradeX.TdxHq_GetSecurityCount(1, ref nCount, sErrInfo);
Console.WriteLine(bRet);
                Console.WriteLine(nCount);
                Console.WriteLine(sErrInfo);
                Console.WriteLine();

                //
                Console.WriteLine("\n    TdxHq_GetSecurityCount(0) - 深圳");

                nCount = 0;
                bRet = TradeX.TdxHq_GetSecurityCount(0, ref nCount, sErrInfo);
                Console.WriteLine(bRet);
                Console.WriteLine(nCount);
                Console.WriteLine(sErrInfo);
                Console.WriteLine();

                Console.WriteLine("\t按回车键继续......");
                Console.ReadKey();
            }


            //
            //
            //
            if (true)
            {
                Console.WriteLine("\n    TdxHq_GetSecurityList(0, 0)");

                short nCount = 0;
bool bRet = TradeX.TdxHq_GetSecurityList(0, 0, ref nCount, sResult, sErrInfo);
Console.WriteLine(bRet);
                Console.WriteLine(nCount);
                Console.WriteLine(sResult);
                Console.WriteLine(sErrInfo);
                Console.WriteLine();

                Console.WriteLine("\t按回车键继续......");
                Console.ReadKey();
            }


            //
            //
            //
            if (true)
            {
                Console.WriteLine("\n*** TdxHq_GetMinuteTimeData(0, \"000001\")");

                bool bRet = TradeX.TdxHq_GetMinuteTimeData(0, "000001", sResult, sErrInfo);
Console.WriteLine(bRet);
                Console.WriteLine(sResult);
                Console.WriteLine(sErrInfo);
                Console.WriteLine();

                Console.WriteLine("\t按回车键继续......");
                Console.ReadKey();
            }


            //
            //
            //
            if (true)
            {
                Console.WriteLine("\n*** TdxHq_GetSecurityBars(8, 0, \"000001\", 0, 10)");

                string sZqdm = "000001";
short nStart = 0;
short nCount = 10;
bool bRet = TradeX.TdxHq_GetSecurityBars(8, 0, sZqdm, nStart, ref nCount, sResult, sErrInfo);
Console.WriteLine(bRet);
                Console.WriteLine(nCount);
                Console.WriteLine(sResult);
                Console.WriteLine(sErrInfo);
                Console.WriteLine();

                Console.WriteLine("\t按回车键继续......");
                Console.ReadKey();
            }


            //
            //
            //
            if (true)
            {
                Console.WriteLine("\n*** TdxHq_GetIndexBars(4, 1, \"000001\", 0, 10)");

                string sZqdm = "000001";
short nStart = 0;
short nCount = 10;
bool bRet = TradeX.TdxHq_GetIndexBars(4, 1, sZqdm, nStart, ref nCount, sResult, sErrInfo);
Console.WriteLine(bRet);
                Console.WriteLine(nCount);
                Console.WriteLine(sResult);
                Console.WriteLine(sErrInfo);
                Console.WriteLine();

                Console.WriteLine("\t按回车键继续......");
                Console.ReadKey();
            }


            //
            //
            //
            if (true)
            {
                Console.WriteLine("\n*** TdxHq_GetMinuteTimeData(0, \"000001\")");

                string sZqdm = "000001";
bool bRet = TradeX.TdxHq_GetMinuteTimeData(0, sZqdm, sResult, sErrInfo);
Console.WriteLine(bRet);
                Console.WriteLine(sResult);
                Console.WriteLine(sErrInfo);
                Console.WriteLine();

                Console.WriteLine("\t按回车键继续......");
                Console.ReadKey();
            }


            //
            //
            //
            if (true)
            {
                Console.WriteLine("\n*** TdxHq_GetHistoryMinuteTimeData(1, \"600600\", 20161209)");

                bool bRet = TradeX.TdxHq_GetHistoryMinuteTimeData(1, "600600", 20161209, sResult, sErrInfo);
Console.WriteLine(bRet);
                Console.WriteLine(sResult);
                Console.WriteLine(sErrInfo);
                Console.WriteLine();

                Console.WriteLine("\t按回车键继续......");
                Console.ReadKey();
            }


            //
            //
            //
            if (true)
            {
                Console.WriteLine("\n*** TdxHq_GetTransactionData(0, \"000001\", 0, 100)");

                short nCount = 100;
bool bRet = TradeX.TdxHq_GetTransactionData(0, "000001", 0, ref nCount, sResult, sErrInfo);
Console.WriteLine(bRet);
                Console.WriteLine(nCount);
                Console.WriteLine(sResult);
                Console.WriteLine(sErrInfo);
                Console.WriteLine();

                Console.WriteLine("\t按回车键继续......");
                Console.ReadKey();
            }


            //
            //
            //
            if (true)
            {
                Console.WriteLine("\n*** TdxHq_GetHistoryTransactionData(0, \"000001\", 0, 100, 20161209)");

                short nCount = 10;
int nDate = 20161209;
bool bRet = TradeX.TdxHq_GetHistoryTransactionData(0, "000001", 0, ref nCount, nDate, sResult, sErrInfo);
Console.WriteLine(bRet);
                Console.WriteLine(nCount);
                Console.WriteLine(sResult);
                Console.WriteLine(sErrInfo);
                Console.WriteLine();

                Console.WriteLine("\t按回车键继续......");
                Console.ReadKey();
            }


            //
            //
            //
            if (true)
            {
                Console.WriteLine("\n*** TdxHq_GetCompanyInfoCategory(0, \"000001\")");

                bool bRet = TradeX.TdxHq_GetCompanyInfoCategory(0, "000001", sResult, sErrInfo);
Console.WriteLine(bRet);
                Console.WriteLine(sResult);
                Console.WriteLine(sErrInfo);
                Console.WriteLine();

                Console.WriteLine("\t按回车键继续......");
                Console.ReadKey();
            }


            //
            //
            //
            if (true)
            {
                Console.WriteLine("\n*** TdxHq_GetCompanyInfoContent(0, \"000001\", \"000001.txt\", 480982, 30097)");

                bool bRet = TradeX.TdxHq_GetCompanyInfoContent(0, "000001", "000001.txt", 480982, 30097, sResult, sErrInfo);
Console.WriteLine(bRet);
                Console.WriteLine(sResult);
                Console.WriteLine(sErrInfo);
                Console.WriteLine();

                Console.WriteLine("\t按回车键继续......");
                Console.ReadKey();
            }


            //
            //
            //
            if (true)
            {
                Console.WriteLine("\n*** TdxHq_GetFinanceInfo(0, \"000001\")");

                bool bRet = TradeX.TdxHq_GetFinanceInfo(0, "000001", sResult, sErrInfo);
Console.WriteLine(bRet);
                Console.WriteLine(sResult);
                Console.WriteLine(sErrInfo);
                Console.WriteLine();

            }

            Console.WriteLine();
            Console.WriteLine("\t按回车键结束演示......");
            Console.ReadKey();

            //
            //
            //

            TradeX.TdxHq_Disconnect();
            TradeX.Logoff(clientId);
            TradeX.CloseTdx();

        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            string sHost = txt_host.Text;
            short nPort = short.Parse(txt_hostPort.Text);
            string sVersion = txt_VersionNo.Text;
            short nYybId = short.Parse(txt_SecDept.Text);
            string sAccountNo = txt_AmtAccount.Text;
            string sTradeAccountNo = txt_ExchAccount.Text;
            string sPassword = txt_ExchPwd.Text;
            string sTxPassword = "";

            StringBuilder sErrInfo = new StringBuilder(256);
            StringBuilder sResult = new StringBuilder(1024 * 1024);

            Console.Write("\n2 - Login ... ");
            int clientId = TradeX.Logon(sHost, nPort, sVersion, nYybId, sAccountNo, sTradeAccountNo, sPassword, sTxPassword, sErrInfo);
            if (clientId < 0)
            {
                MessageBox.Show(sErrInfo.ToString());
                return;
            }
            else
            {
                Console.WriteLine("ok\n");
            }
        }
    }
}
