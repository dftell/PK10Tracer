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
using WolfInv.com.GuideLib.LinkGuid;
using PK10Server;
namespace Test_Win
{
    public partial class Form2 : Form
    {
        GlobalObj gb;
        DateTime begT;
        string[] useCodes;
        DataTypePoint dtp;
        public Form2(GlobalObj _gb)
        {
            gb = _gb;
            InitializeComponent();
            if (GlobalClass.TypeDataPoints.ContainsKey("CN_Stock_A"))
            {
                dtp = GlobalClass.TypeDataPoints["CN_Stock_A"];
            }
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
            BaseDataTable bdt = CommWSSToolClass.GetMarketsStocks(this.gb.w,
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
            
            //string code = "600519"; 
            ////if(!dtp.RuntimeInfo.SecurityCodes.Contains(code))
            ////{
            ////    MessageBox.Show(string.Format("证券{0}不存在！",code));
            ////    return;
            ////}
            int MaxThrdCnt = int.Parse(this.txt_maxThreadCnt.Text);
            int GrpUnitCnt = int.Parse(this.txt_GrpUnitCnt.Text);
            int ThrdInterVal = int.Parse(this.txt_ThrdInterval.Text);
            CommMarketClass cmc = new CommMarketClass(dtp);
            string strendT = this.txt_endT.Text;
            string strbegT = this.txt_begT.Text;
            int[] omds;
            if(useCodes == null)
                useCodes =  cmc.GetMarketsStocks("000001", strendT, 500, true, out omds, true, false);
            string[] currCodes = new string[useCodes.Length];
            
            try
            {
                Array.Copy(useCodes, 0, currCodes, 0, currCodes.Length);
                List<string[]> codeGrps = GroupBuilder.ToGroup<string>(currCodes, currCodes.Length / dtp.ThreadUnitCnt);
                begT = DateTime.Now;
                DisplayCnt(currCodes.Length,codeGrps.Count, 0, 0,0);

                SecurityGrpReader<StockMongoData> sgr = new SecurityGrpReader<StockMongoData>();
                sgr.CheckEvent = DisplayCnt;
                MongoDataDictionary<StockMongoData> data = sgr.GetResult(codeGrps,(a)=>
                    {
                        SecurityReader sr = new SecurityReader(dtp.DataType,dtp.HistoryTable,a);
                        MongoDataDictionary<StockMongoData> res = sr.GetAllCodeDateSerialDataList<StockMongoData>(strbegT, strendT);
                        return res;
                    }, dtp.MaxThreadCnt, 1);
                DisplayCnt(currCodes.Length, codeGrps.Count, MaxThrdCnt, 0,data.Count);
                begT = DateTime.Now;
                List<MongoDataDictionary <StockMongoData>> datalist = GroupBuilder.ToGroup<StockMongoData>(data, currCodes.Length / dtp.ThreadUnitCnt);
                sgr.CheckEvent = DisplayCnt;

                data = sgr.GetResult(datalist, (a) =>
                {
                    return new SecurityReader(dtp.DataType).FQ(a, dtp.RuntimeInfo.XDXRList);
                },dtp.MaxThreadCnt, 1);
                DisplayCnt(currCodes.Length, codeGrps.Count, MaxThrdCnt, 0, data.Count);
                ////    reader.GetAllCodeDateSerialDataList<StockMongoData>("2015-07-15", "2019-03-27");
                ////data = reader.Stock_FQ(data, dtp.RuntimeInfo.XDXRList);

                ////MongoReturnDataList<StockMongoData> fqdata = reader.Stock_FQ(code, data);
                ////////int n = 0;
                ////////StockMongoData testobj = null;
                ////////var test = fqdata.Where(a => (a.date == "2015-07-15"));
                ////////testobj = test.First();
                ////////MessageBox.Show(string.Format("{0},{1}", testobj.date, testobj.close));
                ////////n = fqdata.Count/2;
                ////////MessageBox.Show(string.Format("{0},{1}", fqdata[n].date, fqdata[n].close));
                ////////n = fqdata.Count-1;
                ////////MessageBox.Show(string.Format("{0},{1}", fqdata[n].date, fqdata[n].close));
                ////EMA ema1 = new EMA(fqdata.ToList(a => a.close).ToArray(),12);
                ////EMA ema2 = new EMA(fqdata.ToList(a => a.close).ToArray(), 26);
                ////Matrix e1 = ema1.GetResult();
                ////Matrix e2 = ema2.GetResult();
                ////int n = fqdata.Count - 1;
                ////MessageBox.Show(string.Format("12:{0:f2},26:{1:f2}",e1[n-1,0],e2[n-1,0]));
                ////MACD macd = new MACD(fqdata.ToList(a => a.close).ToArray());
                ////Matrix ret = macd.GetResult();

                ////int cnt = 10;
                ////for (int i = 0; i < cnt; i++)
                ////{
                ////    MessageBox.Show(string.Format("{0}:{4:f2}=>Diff:{1:f2};DEA:{2:f2};MACD:{3:f2}", fqdata[n - i].date, ret.Detail[n - i, 0], ret.Detail[n - i, 1], ret.Detail[n - i, 2],fqdata[n-i].close));
                ////}
            }
            catch(Exception ce)
            {
                MessageBox.Show(ce.Message);
            }
        }

        void DisplayCnt(int total,int grpcnt,int finishedgrp,int inpoolcnt,int rescnt)
        {
            try
            {
                progressBar1.Maximum = total;//设置最大长度值
                progressBar1.Value = rescnt;//设置当前值
                                            //progressBar1.Step = 5;//设置没次增长多少
                DateTime curr = DateTime.Now;
                double secs = curr.Subtract(begT).TotalSeconds;
                string str = string.Format("Total:{3};Grp:{2};Finished:{0};{1} in Pool;Result:{4};rate:{5:f2}%;Time:{6}", finishedgrp, inpoolcnt, grpcnt,total,rescnt,rescnt*100.00/(total*1.00), secs);
                this.lbl_process.Text  = str;
                this.lbl_process.Refresh();
            }
            catch(Exception ce)
            {

            }
        }

        private void btn_svrmgr_Click(object sender, EventArgs e)
        {
            frm_StragMonitor<TimeSerialData> frm = new frm_StragMonitor<TimeSerialData>();
            frm.Show();
        }
    }
}
