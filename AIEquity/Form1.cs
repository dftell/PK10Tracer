using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using WAPIWrapperCSharp;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
//using WolfInv.com.PK10CorePress;
//using WolfInv.com.StrategyLibForWD;
using WolfInv.com.StrategyLibForWD;
using WolfInv.com.WDDataInit;

namespace AIEquity
{
    public partial class Form1<T> : Form where T : TimeSerialData
    {
        bool LoadCompleted = false;
        //DateTime begT=DateTime.MinValue;
        PriceAdj useAdj = PriceAdj.Fore;
        Cycle useCyc = Cycle.Day;
        public Form1(string sec, string begt, string endt, int plusLen)
        {

            InitializeComponent();
            this.statusStrip1.BringToFront();
            if (!string.IsNullOrEmpty(sec))
            {

                if (!Program<T>.loaded)
                {
                    Program<T>.Init(endt.ToDate().AddDays(plusLen), this.refreshProcess);
                }
                //int len = (int)endt.ToDate().Subtract(begt.ToDate()).TotalDays + plusLen;
                QuerySec(sec, begt, endt, plusLen);
            }
            this.tabControl1.BringToFront();
            this.panel_DayInfo.BringToFront();
        }

        public void QuerySec(string sec, string begt, string endt, int len)
        {
            execQuery(sec, begt, len, endt, PriceAdj.Beyond);
        }

        private void btn_AICheck_Click(object sender, EventArgs e)
        {
            if (ddl_Adj.SelectedIndex >= 0)
            {
                useAdj = ddl_Adj.SelectedIndex == 0 ? PriceAdj.Fore : ddl_Adj.SelectedIndex == 1 ? PriceAdj.Beyond : PriceAdj.UnDo;
            }
            if(ddl_cycle.SelectedIndex>=0)
            {
                useCyc = (Cycle)ddl_cycle.SelectedIndex;
            }
            string endDate = this.txt_EndDate.Text.Trim();
            execQuery(this.txt_StockCode.Text, this.txt_InputDate.Text, string.IsNullOrEmpty(endDate)?230:0 , endDate , useAdj,useCyc);
        }

        void execQuery(string code, string begDate, int len, string endDate, PriceAdj adj = PriceAdj.UnDo,Cycle cyc = Cycle.Day)
        {
            try
            {
                this.Text = code;
                DateTime startT = begDate.ToDate().AddDays(-200);
                if (string.IsNullOrEmpty(endDate))
                {
                    endDate = begDate.ToDate().AddDays(300).WDDate();
                }
                DateTime endT = endDate.ToDate().AddDays(len);
                useAdj = adj;
                var items = Program<T>.AllEquits;
                var item = default(KeyValuePair<string, string>);
                if (items.Count() == 0)
                {
                    //MessageBox.Show("无数据");
                    //return;
                }
                else
                {
                    item = items.First();
                }
                var matchItem = items.Where(a => a.Key.Contains(code.ToUpper()) || a.Value.Contains(txt_StockCode.Text));
                if (matchItem.Count() > 0)
                {
                    item = matchItem.First();
                }
                else
                {
                    MongoReturnDataList<T> res = Program<T>.getEquitSerialData(code, "", true, 0, startT.WDDate(), endT.WDDate());
                    if (res == null)
                        return;
                    if (!Program<T>.AllEquits.ContainsKey(code))
                        Program<T>.AllEquits.Add(code, "自定义股票");
                    items = Program<T>.AllEquits;
                    matchItem = items.Where(a => a.Key.Contains(code.ToUpper()) || a.Value.Contains(txt_StockCode.Text));
                    item = matchItem.First();
                }
                //txt_InputDate.Text = item.Value.Last().Value.DateTime;
                MongoReturnDataList<T> elitem = Program<T>.getEquitSerialData(item.Key, item.Value);
                string[] dates = elitem.Keys.ToArray();
                if (dates.IndexOf(startT.WDDate()) < 0 || dates.IndexOf(endT.WDDate()) < 0)
                {
                    elitem = Program<T>.getEquitSerialData(item.Key, item.Value, true, len, startT.WDDate(), endT.WDDate());
                }
                txt_StockCode.Text = item.Key;
                DateTime currDate = begDate.ToDate();
                //int N = int.Parse(txt_HoldRate.Text);
                string mk = "SH";
                if (code.StartsWith("0"))
                {
                    mk = "SZ";
                }
                string fullCode = code;
                if (code.Split('.').Length == 1)
                    fullCode = string.Format("{0}.{1}", code, mk);
                var el = elitem.AfterDate(startT.WDDate());
                if (el.Count == 0)
                    return;
                KLineData<T> klines = new KLineData<T>(el.Last().Expect, el, useAdj,useCyc);

                if (klines.Length == 0)
                    return;
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

                for (int i = 0; i < ema20.Length; i++)
                {
                    outArr[i] = string.Format("{0}  {1}{2}{3}", el.Select(a => a.Expect).ToArray()[i], closes[i].ToString().PadRight(10), arr5[i].ToEquitPrice().ToString().PadRight(10), ema20[i].ToEquitPrice());
                }
                this.txt_result.Lines = outArr.Reverse().ToArray();// ema20.Reverse().Select(a => a.ToString()).ToArray();
                this.chart1.Series[0].Name = "收盘价";
                //this.chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                //this.chart1.Series[0].Points.DataBindY(closes);
                this.chart1.Series[0].ToolTip = "日期:#VALX;高:#VALY1;低:#VALY2;开:#VALY3;收:#VALY4;涨:#VALY5";
                this.chart1.Series[0]["PriceUpColor"] = "Red";
                this.chart1.Series[0]["PriceDownColor"] = "Green";
                this.chart1.Series[0].Points.DataBindXY(klines.Expects, klines.RaiseRates);
                int startId = 0;
                int endId = 0;
                for (int i = 0; i < this.chart1.Series[0].Points.Count; i++)
                {
                    if (klines.Expects[i] == begDate.ToDate().WDDate())//多转换一次，防止被变为日期格式
                    {
                        startId = i;
                    }
                    if (klines.Expects[i] == endDate.ToDate().WDDate())
                    {
                        endId = i;
                    }
                    this.chart1.Series[0].Tag = klines;
                    this.chart1.Series[0].Points[i].YValues[0] = klines.Highs[i];
                    this.chart1.Series[0].Points[i].YValues[1] = klines.Lows[i];
                    this.chart1.Series[0].Points[i].YValues[2] = klines.Opens[i];
                    this.chart1.Series[0].Points[i].YValues[3] = klines.Closes[i];
                    //this.chart1.Series[0].Points[i].YValues[4] = klines.RaiseRates[i];
                }
                this.chart1.Series[1].ChartType = SeriesChartType.Line;
                this.chart1.Series[1].ChartArea = "mainChart";
                this.chart1.Series[1].Name = "10日极值线";
                this.chart1.Series[1].Points.DataBindXY(klines.Expects, maxminLines10);

                this.chart1.Series[2].ChartType = SeriesChartType.Line;
                this.chart1.Series[2].ChartArea = "mainChart";
                this.chart1.Series[2].Name = "MA(20)";
                this.chart1.Series[2].Points.DataBindXY(klines.Expects, arr20);

                this.chart1.Series[3].Legend = "macd";
                this.chart1.Series[3].ChartArea = "subChart";
                this.chart1.Series[3].Name = "MACD";
                this.chart1.Series[3].ChartType = SeriesChartType.Column;
                MACDCollection macds = closes.MACD();
                this.chart1.Series[3].Points.DataBindXY(klines.Expects, macds.MACDs);
                
                this.chart1.Series[4].Legend = "macd";
                this.chart1.Series[4].ChartArea = "subChart";
                this.chart1.Series[4].Name = "MACD极值";
                this.chart1.Series[4].ChartType = SeriesChartType.Line;
                this.chart1.Series[4].Points.DataBindXY(klines.Expects, macds.MACDs.MaxMinArray(5, true));
                this.chart1.Series[4].Color = Color.Yellow;

                /*
                this.chart1.Series[5].Legend = "macd";
                this.chart1.Series[5].ChartArea = "subChart";
                this.chart1.Series[5].Name = "DIF";
                this.chart1.Series[5].Color = Color.Gray;
                this.chart1.Series[5].ChartType = SeriesChartType.Line;
                this.chart1.Series[5].Points.DataBindXY(klines.Expects, macds.DIFs);
                
                this.chart1.Series[7].Legend = "macd";
                this.chart1.Series[7].ChartArea = "subChart";
                this.chart1.Series[7].Name = "DEA";
                this.chart1.Series[7].ChartType = SeriesChartType.Line;
                this.chart1.Series[7].Points.DataBindXY(klines.Expects, macds.DEAs);
                this.chart1.Series[7].Color = Color.White;
                */
                this.chart1.Series[8].Legend = "macd";
                this.chart1.Series[8].ChartArea = "subChart";
                this.chart1.Series[8].Name = "DEA极值";
                this.chart1.Series[8].ChartType = SeriesChartType.Line;
                this.chart1.Series[8].Points.DataBindXY(klines.Expects, macds.DEAs.MaxMinArray(5,true));
                this.chart1.Series[8].Color = Color.White;

                this.chart1.Series[6].Legend = "vol";
                this.chart1.Series[6].ChartArea = "volChart";
                this.chart1.Series[6].Name = "Vol";
                this.chart1.Series[6].ChartType = SeriesChartType.Column;
                this.chart1.Series[6].Points.DataBindXY(klines.Expects, klines.Vols);
                for (int i = 0; i < macds.MACDs.Length; i++)
                {
                    this.chart1.Series[3].Points[i].Color = macds.MACDs[i] > 0 ? Color.Red : Color.Green;
                    this.chart1.Series[6].Points[i].Color = klines.Closes[i]>klines.Opens[i] ? Color.Red : Color.Green;
                }
                chart1.ChartAreas[0].AxisY.IsStartedFromZero = false;
                CustomLabel cl_beg = new CustomLabel();
                cl_beg.Text = "开始日期:" + begDate;
                cl_beg.FromPosition = startId;
                this.chart1.Series[0].Points[startId].MarkerSize = 20;
                this.chart1.Series[0].Points[startId].MarkerStyle = MarkerStyle.Triangle;//.SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.Arrow;

                this.chart1.Series[0].Points[endId].MarkerSize = 20;
                this.chart1.Series[0].Points[endId].MarkerStyle = MarkerStyle.Diamond;
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
            catch (Exception ce)
            {

            }

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

        public void refreshProcess(int cnt, int total, EquitProcess<T>.EquitUpdateResult res)
        {
            if (this?.toolStripProgressBar1 == null)
                return;
            int val = this.toolStripProgressBar1.Value;
            int currVal = 100 * cnt / total;
            if (cnt >= total)
            {
                LoadCompleted = true;
                this.Invoke(new SetProcessIncream(Incream), toolStripProgressBar1, -100, false);
                this.Invoke(new SetStatusLabel(refreshStatusLabel), this.toolStripStatusLabel1, string.Format("Completed!历时{0}秒", DateTime.Now.Subtract(Program<T>.begT).TotalSeconds));
                return;
            }
            //this.toolStripProgressBar1.Increment(currVal-val);
            try
            {
                this.Invoke(new SetProcessIncream(Incream), toolStripProgressBar1, currVal - val, true);
                string txt = string.Format("总计:{0};已完成:{1};进度:{2}%;历时{3}秒。", total, cnt, currVal, DateTime.Now.Subtract(Program<T>.begT).TotalSeconds);
                this.Invoke(new SetStatusLabel(refreshStatusLabel), this.toolStripStatusLabel1, txt);
            }
            catch
            {

            }
        }
        delegate void SetProcessIncream(ToolStripProgressBar tpb, int cnt, bool visible = true);
        void Incream(ToolStripProgressBar tpb, int cnt, bool visible = true)
        {
            tpb.Visible = visible;
            tpb.Increment(cnt);
        }

        delegate void SetStatusLabel(ToolStripStatusLabel label, string str);

        void refreshStatusLabel(ToolStripStatusLabel label, string str)
        {
            label.Text = str;
        }

        private void chart1_KeyUp(object sender, KeyEventArgs e)
        {
            Chart chart = sender as Chart;
            if (e.KeyCode == Keys.Up)
            {

                for (int i = 0; i < chart.ChartAreas.Count; i++)
                {
                    chart.ChartAreas[i].AxisX.ScrollBar.Enabled = false;
                    double zoomfactor = 2;   //设置缩放比例
                    chart.ChartAreas[i].AxisX.ScaleView.Zoomable = true;
                    double vMax = chart.ChartAreas[i].AxisX.ScaleView.ViewMaximum;
                    double vMin = chart.ChartAreas[i].AxisX.ScaleView.ViewMinimum;

                    double v = lastPostion;
                    double xmouseponit = chart.ChartAreas[i].AxisX.PixelPositionToValue(v);    //获取鼠标在chart中x坐标
                    double xratio = (vMin - xmouseponit) / (xmouseponit - vMax);      //计算当前鼠标基于坐标两侧的比值，后续放大缩小时保持比例不变


                    double xspmovepoints = Math.Round((xmouseponit - vMin) * (zoomfactor - 1) / zoomfactor, 1);    //计算x轴起点需要右移距离,保留一位小数
                    double xepmovepoints = Math.Round(vMax - xmouseponit - xratio * (xmouseponit - vMin - xspmovepoints), 1);    //计算x轴末端左移距离，保留一位小数
                    double viewsizechange = xspmovepoints + xepmovepoints;         //计算x轴缩放视图缩小变化尺寸
                    chart.ChartAreas[i].AxisX.ScaleView.Zoom(xspmovepoints, xepmovepoints);
                    //chart.ChartAreas[0].AxisX.ScaleView.Size -= viewsizechange;        //设置x轴缩放视图大小
                    //chart.ChartAreas[0].AxisX.ScaleView.Position += xspmovepoints;        //设置x轴缩放视图起点，

                    //chart.ChartAreas[i].AxisX.ScaleView.Zoom();
                }
            }
            if (e.KeyCode == Keys.Down)
            {
                for (int i = 0; i < chart.ChartAreas.Count; i++)
                {
                    double zoomfactor = 1 / 2;   //设置缩放比例
                    chart.ChartAreas[i].AxisX.ScaleView.Zoomable = true;
                    double vMax = chart.ChartAreas[i].AxisX.ScaleView.ViewMaximum;
                    double vMin = chart.ChartAreas[i].AxisX.ScaleView.ViewMinimum;

                    double v = lastPostion;
                    double xmouseponit = chart.ChartAreas[i].AxisX.PixelPositionToValue(v);    //获取鼠标在chart中x坐标
                    double xratio = (vMin - xmouseponit) / (xmouseponit - vMax);      //计算当前鼠标基于坐标两侧的比值，后续放大缩小时保持比例不变


                    double xspmovepoints = Math.Round((xmouseponit - vMin) * (zoomfactor - 1) / zoomfactor, 1);    //计算x轴起点需要右移距离,保留一位小数
                    double xepmovepoints = Math.Round(vMax - xmouseponit - xratio * (xmouseponit - vMin - xspmovepoints), 1);    //计算x轴末端左移距离，保留一位小数
                    double viewsizechange = xspmovepoints + xepmovepoints;         //计算x轴缩放视图缩小变化尺寸
                    chart.ChartAreas[i].AxisX.ScaleView.Zoom(xspmovepoints, xepmovepoints);

                }
            }
            if (e.KeyCode == Keys.Left)
            {
                double v = lastPostion;
                HitTestResult myTestResult = chart.HitTest((int)chart.ChartAreas[0].CursorX.Position, (int)chart.ChartAreas[0].CursorY.Position);
                int pIndex = myTestResult.PointIndex;
                double total = (chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum - chart.ChartAreas[0].AxisX.ScaleView.ViewMinimum);
                //chart.ChartAreas[0].AxisX.ScaleView.
                double oneLen = total / (chart.Series[0].Points.Count + 1);
                for (int i = 0; i < chart.ChartAreas.Count; i++)
                {
                    //DataPoint dp = myTestResult.Series.Points[Math.Max(0, pIndex - 1)];
                    double currV = chart.ChartAreas[i].CursorX.Position;

                    chart.ChartAreas[i].CursorX.Position = chart.ChartAreas[i].CursorX.Position - oneLen;
                }
            }
            if (e.KeyCode == Keys.Right)
            {
                double v = lastPostion;
                HitTestResult myTestResult = chart.HitTest((int)chart.ChartAreas[0].CursorX.Position, (int)chart.ChartAreas[0].CursorY.Position);
                int pIndex = myTestResult.PointIndex;
                double total = (chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum - chart.ChartAreas[0].AxisX.ScaleView.ViewMinimum);
                //chart.ChartAreas[0].AxisX.ScaleView.
                double oneLen = total / (chart.Series[0].Points.Count + 1);
                for (int i = 0; i < chart.ChartAreas.Count; i++)
                {
                    //DataPoint dp = myTestResult.Series.Points[Math.Max(0, pIndex - 1)];
                    double currV = chart.ChartAreas[i].CursorX.Position;

                    chart.ChartAreas[i].CursorX.Position = chart.ChartAreas[i].CursorX.Position + oneLen;
                }
            }
            if((e.KeyCode >=Keys.D0 && e.KeyCode <=Keys.D9) || (e.KeyCode>=Keys.NumPad0 && e.KeyCode<=Keys.NumPad9))
            {
                this.panel_wolfGenus.Visible = true;
                this.panel_wolfGenus.BringToFront();
                int val = e.KeyCode - Keys.D0;
                if(val>10)
                {
                    val = e.KeyCode - Keys.NumPad0;
                }
                this.txt_search.Text = string.Format("{0}{1}", this.txt_search.Text.Trim(),val);
                this.txt_search.Select(this.txt_search.Text.Length, 0);
            }
        }

        private void chart1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void chart1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            this.chart1_KeyUp(chart1, e);
        }

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

        }
        double lastPostion = 0;


        private void chart1_MouseClick(object sender, MouseEventArgs e)
        {
            Chart chart = sender as Chart;
            lastPostion = e.X;
            chart.Focus();
            HitTestResult hit = this.chart1.HitTest(e.X,e.Y);
            chart1.Series[0].XValueType = ChartValueType.String;
            if (hit.PointIndex < 0)
                return;
            KLineData<T> kline = chart.Series[0].Tag as KLineData<T>;
            if (kline == null)
                return;
            DataPoint pt = chart.Series[0].Points[hit.PointIndex];
            this.lbl_expect.Text = kline.Expects[hit.PointIndex];
            this.lbl_ValHeigh.Text = pt.YValues[0].ToString();
            this.lbl_ValLow.Text = pt.YValues[1].ToString();
            this.lbl_ValOpen.Text = pt.YValues[2].ToString();
            this.lbl_ValClose.Text = pt.YValues[3].ToString();

            this.lbl_ValVol.Text = string.Format("{0}万", (kline.Vols[hit.PointIndex] / 10000).ToEquitPrice(1));
            this.lbl_ValZf.Text = string.Format("{0}%",(kline.RaiseRates[hit.PointIndex]).ToEquitPrice(2));
        }

        private void chart1_SizeChanged(object sender, EventArgs e)
        {
            ChartReSize();
        }

        void ChartReSize()
        {
            return;
            float currWith = this.chart1.Width;
            float currHeight = this.chart1.Height;

            this.chart1.ChartAreas[0].Position.X = 0;
            this.chart1.ChartAreas[0].Position.Y = 0;
            this.chart1.ChartAreas[0].Position.Height = 50;
            this.chart1.ChartAreas[0].Position.Width = 100;

            this.chart1.ChartAreas[1].Position.X = 0;
            this.chart1.ChartAreas[1].Position.Y = 50;
            this.chart1.ChartAreas[1].Position.Height = 25;
            this.chart1.ChartAreas[1].Position.Width = 100;

            this.chart1.ChartAreas[2].Position.X = 0;
            this.chart1.ChartAreas[2].Position.Y = 75;
            this.chart1.ChartAreas[2].Position.Height = 25;
            this.chart1.ChartAreas[2].Position.Width = 100;


        }

        private void chart1_DockChanged(object sender, EventArgs e)
        {

        }
        Point mouseOffset;
        bool isMouseDown;
        Control moveCtrl;
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            Control ctrl = sender as Control;
            if (ctrl == null)
                return;
            if (ctrl is Form)
                return;
            Control parent = ctrl.Parent;
            if(parent != null)
            {
                if(parent.AllowDrop)
                {
                    ctrl = parent;
                }
            }
            if (!ctrl.AllowDrop)
                return;
            //Control control = sender as Control;
            int offsetX = 0;// -e.X;
            int offsetY = 0;// -e.Y;
            //判断是窗体还是控件，从而改进鼠标相对于窗体的位置
            if (!(ctrl is System.Windows.Forms.Form))
            {
                offsetX = offsetX - ctrl.Left;
                offsetY = offsetY - ctrl.Top;
            }
            //判断窗体有没有标题栏，从而改进鼠标相对于窗体的位置
            if (this.FormBorderStyle != FormBorderStyle.None)
            {
                offsetX = offsetX - SystemInformation.FrameBorderSize.Width;
                offsetY = offsetY - SystemInformation.FrameBorderSize.Height - SystemInformation.CaptionHeight;
            }
            mouseOffset = new Point(offsetX, offsetY);
            isMouseDown = true;
            moveCtrl = ctrl;
        }
    
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point mouse = Control.MousePosition;
                //mouse.Offset(mouseOffset.X, mouseOffset.Y);
                moveCtrl.Location = mouse;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button== MouseButtons.Left && isMouseDown)
            {
                isMouseDown = false;
            }
        }

        private void txt_search_TextChanged(object sender, EventArgs e)
        {
            
            if(txt_search.Text.Trim().Length == 0)
            {
                this.listView1.Items.Clear();
                return;
            }
            this.txt_search.Focus();
            if (txt_search.Text.Trim().Length < 3)
                return;
            var items = Program<T>.AllEquits.Where(a => a.Key.Contains(txt_search.Text.Trim()));
            this.listView1.Items.Clear();
            foreach(var item in items)
            {
                string key = item.Key;
                string val = item.Value;
                ListViewItem lvi = new ListViewItem(key);
                lvi.SubItems.Add(val);
                this.listView1.Items.Add(lvi);
            }
        }

        private void txt_search_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter && txt_search.Text.Trim().Length >= 3)
            {
                if(this.listView1.Items.Count>0)
                {
                    this.execQuery(this.listView1.Items[0].Text, DateTime.Today.WDDate(), 500, null,PriceAdj.Fore);
                    
                }
                this.txt_search.Text = "";
                this.panel_wolfGenus.Visible = false;
            }
        }

        private void numericUpDown_EndDate_ValueChanged(object sender, EventArgs e)
        {
            if (ddl_Adj.SelectedIndex >= 0)
            {
                useAdj = ddl_Adj.SelectedIndex == 0 ? PriceAdj.Fore : ddl_Adj.SelectedIndex == 1 ? PriceAdj.Beyond : PriceAdj.UnDo;
            }
            string endDate = this.txt_EndDate.Text.Trim().ToDate().AddDays((int)this.numericUpDown_EndDate.Value).WDDate();
            execQuery(this.txt_StockCode.Text, this.txt_InputDate.Text, string.IsNullOrEmpty(endDate) ? 200 : 0, endDate, useAdj);

        }

        private void btn_CloseGenus_Click(object sender, EventArgs e)
        {
            this.panel_wolfGenus.Visible = false;
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;
            if (ddl_Adj.SelectedIndex >= 0)
            {
                useAdj = ddl_Adj.SelectedIndex == 0 ? PriceAdj.Fore : ddl_Adj.SelectedIndex == 1 ? PriceAdj.Beyond : PriceAdj.UnDo;
            }
            string endDate = this.txt_EndDate.Text.Trim().ToDate().AddDays((int)this.numericUpDown_EndDate.Value).WDDate();
            execQuery(this.listView1.SelectedItems[0].Text, this.txt_InputDate.Text, string.IsNullOrEmpty(endDate) ? 200 : 0, endDate, useAdj);

        }
    }
}
