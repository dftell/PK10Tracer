using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WAPIWrapperCSharp;
using WolfInv.com.StrategyLibForWD;
using WolfInv.com.CFZQ_LHProcess;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.SecurityLib;
using WolfInv.com.ServerInitLib;
namespace Test_Win
{
    public partial class Form2 : Form
    {
        GlobalObj gb;
        public Form2(GlobalObj _gb)
        {
            gb = _gb;
            InitializeComponent();
            //gb.Sys = new SystemGlobal(gb.w);

        }

        private void btn_gl_Click(object sender, EventArgs e)
        {
            Form1 frm = new Form1();
            frm.ShowDialog();
            
        }

        private void btn_forSI_Click(object sender, EventArgs e)
        {
            frm_siII frm = new frm_siII(gb);
            frm.ShowDialog();
        }

        public void UpdateBar()
        {
            this.progressBar1.Value = 1;
            this.progressBar1.Maximum = 100;

        }

        private void button1_Click(object sender, EventArgs e)
        {

            //RunResultClass ret = CommWDToolClass.GetSetBaseData(gb.w,"000613.SZ,300369.SZ,002512.SZ,600721.SH,300344.SZ,000100.SZ",DateTime.Today);
            ////MACDGuidProcess mp = new MACDGuidProcess(gb.w);
            ////RunResultClass ret = mp.getDateSerialResult("000100.SZ",Convert.ToDateTime("2017/7/7"), DateTime.Today);
            ////BaseDataProcess bp = new BaseDataProcess(gb.w);
            ////RunResultClass bret = bp.getDateSerialResult("000100.SZ", Convert.ToDateTime("2017/7/7"), DateTime.Today);
            //RunResultClass ret =mp.getSetDataResult("000613.SZ,300369.SZ,002512.SZ,600721.SH,300344.SZ,000100.SZ", null, DateTime.Today);
           //RunResultClass ret = CommWDToolClass.(gb.w, "000613.SZ,300369.SZ,002512.SZ,600721.SH,300344.SZ,000100.SZ", DateTime.Today);
            //BaseDataTable dt = CommWDToolClass.GetBaseData(gb.w, "000613.SZ,300369.SZ,002512.SZ,600721.SH,300344.SZ,000100.SZ", Convert.ToDateTime("2018/3/10"), Cycle.Day, PriceAdj.Beyond);
            
            string code = new SIIIClass().SummaryCode;
            BaseDataTable bdt = CommWDToolClass.GetMarketsStocks(this.gb.w,
                "000300.SH",
                DateTime.Today,
                200,
                true,
                true,
                false,
                null);
            MessageBox.Show(bdt.Count.ToString());

            return;
            //////////MessageBox.Show(WDDayClass.LastTradeDay(gb.w,"002161.SZ",DateTime.Today).ToShortDateString());
            //////////return;
            //////////BaseDataTable dt = CommWDToolClass.GetBaseSerialData(gb.w, "000002.SZ", Convert.ToDateTime("2015/12/7"), Convert.ToDateTime("2016/7/7"));
            //////////if (dt != null)
            //////////{
            //////////    MessageBox.Show(string.Format("交易日数量：{0},股票实际交易日：{1}",dt.Count,dt.AvaliableData.Count));
            //////////    ////MTable tab = ret.Result;
            //////////    ////tab.Union(bret.Result);
            //////////    ////MACDTable mtab = new MACDTable(tab.GetTable());
            //////////    ////MessageBox.Show(mtab[0].ToClassInfo());
            //////////    //MessageBox.Show(new BaseDataTable(ret.Result)[5].IsST(DateTime.Today).ToString());
            //////////}

        }

        private void btn_swhyCommQuery_Click(object sender, EventArgs e)
        {
            frm_Query frm = new frm_Query(this.gb);
            frm.Show();
        }

        private void btn_SubScript_Click(object sender, EventArgs e)
        {
            frm_HYMonitor frm = new frm_HYMonitor();
            frm.Show();
        }

        private void btn_TestDayData_Click(object sender, EventArgs e)
        {
            DataTypePoint dtp =  GlobalClass.TypeDataPoints["CN_Stock_A"];
            string code = "600519";
            if(!dtp.RuntimeInfo.SecurityCodes.Contains(code))
            {
                MessageBox.Show(string.Format("证券{0}不存在！",code));
                return;
            }
            SecurityReader reader = new SecurityReader("CN_Stock_A", dtp.HistoryTable, new string[1] { code });
            try
            {
                MongoReturnDataList<StockMongoData> data = reader.GetAllCodeDateSerialDataList<StockMongoData>("2015-07-15", "2019-03-27")?[code];


                MongoReturnDataList<StockMongoData> fqdata = reader.Stock_FQ(code, data);
                int n = 0;
                StockMongoData testobj = null;
                var test = fqdata.Where(a => (a.date == "2015-07-15"));
                testobj = test.First();
                MessageBox.Show(string.Format("{0},{1}", testobj.date, testobj.close));
                n = fqdata.Count/2;
                MessageBox.Show(string.Format("{0},{1}", fqdata[n].date, fqdata[n].close));
                n = fqdata.Count-1;
                MessageBox.Show(string.Format("{0},{1}", fqdata[n].date, fqdata[n].close));

            }
            catch(Exception ce)
            {
                MessageBox.Show(ce.Message);
            }
        }
    }
}
