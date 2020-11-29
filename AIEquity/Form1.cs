using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WAPIWrapperCSharp;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
//using WolfInv.com.PK10CorePress;
//using WolfInv.com.StrategyLibForWD;
using WolfInv.com.StrategyLibForWD;
using WolfInv.com.WDDataInit;

namespace AIEquity
{
    public partial class Form1<T> : Form where T:TimeSerialData
    {
        bool LoadCompleted = false;
        //DateTime begT=DateTime.MinValue;
        public Form1()
        {
            InitializeComponent();
            this.statusStrip1.BringToFront();
            
        }

        private void btn_AICheck_Click(object sender, EventArgs e)
        {
            
            var items = Program<T>.AllEquits.Where(a => {
                
                return true;
            });
            if(items.Count() == 0)
            {
                MessageBox.Show("无数据");
                return;
            }
            var item = items.First();
            var matchItem = items.Where(a => a.Key.Contains(txt_StockCode.Text.ToUpper())||a.Value.Contains(txt_StockCode.Text));
            if(matchItem.Count()>0)
            {
                item = matchItem.First();
            }
            else
            {
                return;
            }
            //txt_InputDate.Text = item.Value.Last().Value.DateTime;
            MongoReturnDataList<T> elitem = Program<T>.getEquitSerialData(item.Key,item.Value);
            txt_StockCode.Text = item.Key;
            DateTime currDate = DateTime.Parse(txt_InputDate.Text);
            int N = int.Parse(txt_HoldRate.Text);
            string code = txt_StockCode.Text.Trim();
            string mk = "SH";
            if(code.StartsWith("0"))
            {
                mk = "SZ";
            }
            string fullCode = string.Format("{0}.{1}",code,mk);

            var el = elitem.AfterDate(txt_InputDate.Text.WDDate());
            KLineData<T> klines = new KLineData<T>(el);
            double[] closes = klines.Closes;// el.Select(a => (a as StockMongoData).close).ToArray();// Close;
            double[] arr5 = closes.MA(5);
            double[] arr10 = closes.MA(10);
            double[] arr20 = closes.MA(20);
            double[] arr60 = closes.MA(60);
            double[] ema20 = closes.EMA(12);
            double[] vol = klines.Vols;// el.Select(a=> (a as StockMongoData).vol).ToArray();
            string[] outArr = new string[ema20.Length];
            double[] maxminLines10 = closes.MaxMinArray(10, true);
            double[] maxminLines15 = closes.MaxMinArray(15, true);
            double[] maxminLines20 = closes.MaxMinArray(20, true);
            double[] maxminLines30 = closes.MaxMinArray(30, true);
            for (int i=0;i<ema20.Length;i++)
            {
                outArr[i] = string.Format("{0}  {1}{2}{3}", el.Select(a=> (a as StockMongoData).Expect).ToArray()[i], closes[i].ToString().PadRight(10),arr5[i].ToEquitPrice().ToString().PadRight(10),ema20[i].ToEquitPrice()); 
            }
            this.txt_result.Lines = outArr.Reverse().ToArray();// ema20.Reverse().Select(a => a.ToString()).ToArray();
            this.chart1.Series[0].Name = "收盘价";
            this.chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            this.chart1.Series[0].Points.DataBindY(closes);
            /*
            this.chart1.Series[0]["PriceUpColor"] = "Red";
            this.chart1.Series[0]["PriceDownColor"] = "Green";
            this.chart1.Series[0].Points.DataBindXY(el.Select(a=>a.Expect).ToArray(),el.Select(a=> (a as StockMongoData).high).ToArray());
            for (int i = 0; i < this.chart1.Series[0].Points.Count; i++)
            {
                this.chart1.Series[0].Points[i].YValues[0] = el.Select(a=> (a as StockMongoData).high).ToArray()[i];
                this.chart1.Series[0].Points[i].YValues[1] = el.Select(a => (a as StockMongoData).low).ToArray()[i];
                this.chart1.Series[0].Points[i].YValues[2] = el.Select(a => (a as StockMongoData).open).ToArray()[i];
                this.chart1.Series[0].Points[i].YValues[3] = el.Select(a => (a as StockMongoData).close).ToArray()[i];
            }
            */
            this.chart1.Series[1].Name = "10日极值线";
            this.chart1.Series[1].Points.DataBindY(maxminLines10);
            this.chart1.Series[2].Name = "15日极值线";
            this.chart1.Series[2].Points.DataBindY(maxminLines15);
            this.chart1.Series[3].Name = "20日极值线";
            this.chart1.Series[3].Points.DataBindY(maxminLines20);
            
            this.chart1.Series[4].Name = "30日极值线";
            this.chart1.Series[4].Points.DataBindY(maxminLines30);
            chart1.ChartAreas[0].AxisY.IsStartedFromZero = false;
            //MessageBox.Show(bdt.Count.ToString());
            /*
            //WDDayClass
            WD_SubscriptTimeDataClass wdst = new WD_SubscriptTimeDataClass();
            wdst.getData("open,close,hight,low,vol", "", 1, true);
            //SystemGlobal.AllSecSet
            GLClassProcess glpro = new GLClassProcess();
            string ret = glpro.UpdateAllGLNumbers(DateTime.Today);
            List<GLThreadProcess> threadpool = glpro.CheckPin as List<GLThreadProcess>;
            List<string> msgs = new List<string>();
            */

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.txt_InputDate.Text = DateTime.Now.AddYears(-1).WDDate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
            this.txt_InputDate.Text = "http://www.wolfinv.com/LSI/wxMainFrame.aspx";
            long tick = AccessTockenClass.GetTimeStamp(DateTime.UtcNow);
            this.txt_AvgPrice.Text = tick.ToString();
            this.txt_HoldRate.Text = "kldjfkldffdklfdjfdl";
            this.txt_jsAPITicket.Text = AccessTockenClass.getJsApiToken(this.txt_InputDate.Text, this.txt_HoldRate.Text, tick);
            this.txt_StockCode.Text = AccessTockenClass.JsToken;
            */
        }

        public void refreshProcess(int cnt,int total,EquitProcess<T>.EquitUpdateResult res)
        {
            
            int val = this.toolStripProgressBar1.Value;
            int currVal = 100 * cnt / total;
            if(cnt>=total)
            {
                LoadCompleted = true;
                this.Invoke(new SetProcessIncream(Incream), toolStripProgressBar1, -100,false);
                this.Invoke(new SetStatusLabel(refreshStatusLabel), this.toolStripStatusLabel1, string.Format("Completed!历时{0}秒",DateTime.Now.Subtract(Program<T>.begT).TotalSeconds));
                return;
            }
            //this.toolStripProgressBar1.Increment(currVal-val);
            try
            {
                this.Invoke(new SetProcessIncream(Incream), toolStripProgressBar1, currVal - val,true);
                string txt = string.Format("总计:{0};已完成:{1};进度:{2}%;历时{3}秒。", total, cnt, currVal, DateTime.Now.Subtract(Program<T>.begT).TotalSeconds);
                this.Invoke(new SetStatusLabel(refreshStatusLabel),this.toolStripStatusLabel1, txt);
            }
            catch
            {

            }
        }
        delegate void SetProcessIncream(ToolStripProgressBar tpb , int cnt,bool visible=true);
        void Incream(ToolStripProgressBar tpb,  int cnt,bool visible=true)
        {
            tpb.Visible = visible;
            tpb.Increment(cnt);
        }

        delegate void SetStatusLabel(ToolStripStatusLabel label, string str);

        void refreshStatusLabel(ToolStripStatusLabel label, string str)
        {
            label.Text = str;
        }

    }
}
