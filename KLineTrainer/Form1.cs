using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.SecurityLib;
using WolfInv.com.StrategyLibForWD;
namespace KLineTrainer
{
    public partial class Form1 : Form
    {
        class CurrData
        {
            ExpectList data;
            public CurrData(ExpectList d)
            {
                data = d;
            }
            public List<double[]> prices;
            public double[] vols;
            public List<int> buyPoints = new List<int>();
            public List<int> sellPoints = new List<int>();
            public int currLength
            {
                get
                {
                    return vols.Length;
                }
            }

            public bool Holding
            {
                get
                {
                    return buyPoints.Count > sellPoints.Count ? true : false;
                }
            }

            public List<double> gainedList
            {
                get
                {
                    List<double> ret = new List<double>();

                    int len = Math.Max(0, Math.Min(buyPoints.Count, sellPoints.Count));
                    if (len > 0)
                    {
                        for (int i = 0; i < len; i++)
                        {
                            ret.Add(100 * data.Serials[sellPoints[i]].CLOSE - data.Serials[buyPoints[i]].CLOSE / data.Serials[sellPoints[i]].CLOSE);
                        }
                    }
                    return ret;
                }
            }

            public double currGained
            {
                get
                {
                    List<double> ret = new List<double>();
                    int len = Math.Max(0, Math.Min(buyPoints.Count, sellPoints.Count));
                    if (buyPoints.Count > len)
                    {
                        return 100 * (data.Serials[data.Count - 1].CLOSE - data.Serials[buyPoints[buyPoints.Count - 1]].CLOSE) / data.Serials[buyPoints[buyPoints.Count - 1]].CLOSE;
                    }
                    return 0;
                }

            }

            public bool Next()
            {
                if (currLength == data.Count)
                    return false;
                int currLen = currLength;
                List<double> tmp = prices[0].ToList();
                tmp.Add(data.Serials[currLen].HIGH);
                prices[0] = tmp.ToArray();
                tmp = prices[1].ToList();
                tmp.Add(data.Serials[currLen].LOW);
                prices[1] = tmp.ToArray();
                tmp = prices[2].ToList();
                tmp.Add(data.Serials[currLen].OPEN);
                prices[2] = tmp.ToArray();
                tmp = prices[3].ToList();
                tmp.Add(data.Serials[currLen].CLOSE);
                prices[3] = tmp.ToArray();
                tmp = vols.ToList();
                tmp.Add(data.Serials[currLen].VOLUME);
                vols = tmp.ToArray();
                return true;
            }
        }

        public ExpectList TrainData;
        public string[] Codes;
        public int[] OnMarketDays;
        public HashSet<string> usedCodes=new HashSet<string>();
        int useMinDays = 500;
        public DateTime[] MarketDays;
        public HashSet<string> useGuider = new HashSet<string>();
        int tainDays = 120;
        CurrData currData ;
        public Form1()
        {
            InitializeComponent();
            bool succ = init();
            NextGame();
        }

        public bool init()
        {
            try
            {

                Codes = Program.dtp.RuntimeInfo.SecurityCodes;
                OnMarketDays = Program.dtp.RuntimeInfo.SecurityInfoList.Select(a => a.Value.OnMarketDays).ToArray();
                MarketDays = Program.dtp.RuntimeInfo.HistoryDateList.Select(a => DateTime.Parse(a)).ToArray();
            }
            catch
            {
                return false;
            }
            return true;
        }

        void NextGame()
        {
            string[] restList = Codes.Where(a => !usedCodes.Contains(a)).ToArray();
            Random rd = new Random();
            int index = rd.Next(restList.Length);
            string useCode = restList[index];
            int useMarketDays = OnMarketDays[index];//该股票上市天数
            int dayDiff = useMarketDays - useMinDays;//最小长度差
            
            int endIndex = MarketDays.Length - (dayDiff - rd.Next(dayDiff)) - 1;//随机取的日期在整体上市日中的位置，停牌有差距

            DateTime endt = MarketDays[endIndex];
            DateTime begt = MarketDays[endIndex - useMinDays];//中间停牌有差距
            ExpectList res = CommWSDToolClass.GetSerialData(useCode.WDCode(), begt, endt);
            if(res.Count< useMinDays/2)//如果停牌天数太多
            {
                begt = MarketDays[endIndex - useMinDays - (useMinDays - res.Count)];//多加停牌天数
                res = CommWSDToolClass.GetSerialData(useCode.WDCode(), begt, endt);//重新再取一次
            }
            if(res.Count< useMinDays / 2)
            {
                NextGame();//重新取一次，这个票不管了
                return;
            }
            if(!usedCodes.Contains(useCode))
                usedCodes.Add(useCode);
            List<double[]> prices = new List<double[]>();
            
            
            prices.Add(res.Take(res.Count - tainDays).Select(a => a.Value.HIGH).ToArray());
            prices.Add(res.Take(res.Count - tainDays).Select(a => a.Value.LOW).ToArray());
            prices.Add(res.Take(res.Count - tainDays).Select(a => a.Value.OPEN).ToArray());
            prices.Add(res.Take(res.Count - tainDays).Select(a => a.Value.CLOSE).ToArray());
            double[] vols = res.Take(res.Count - tainDays).Select(a => a.Value.VOLUME).ToArray();
            currData = new CurrData(res);
            currData.prices = prices;
            currData.vols = vols;
            DrawCharts(prices,vols);
        }

        void DrawCharts(List<double[]> prices,double[] vols)
        {
            DrawMainKLines(prices);
            DrawVolumne(vols);
        }

        void DrawMainKLines(List<double[]> prices)
        {
            this.chart1.Series[0]["PriceUpColor"] = "Red";
            this.chart1.Series[0]["PriceDownColor"] = "Green";
            this.chart1.Series[0].Points.DataBindY(prices[0]);
            for (int i = 0; i < this.chart1.Series[0].Points.Count; i++)
            {
                this.chart1.Series[0].Points[i].YValues[0] = prices[0][i];
                this.chart1.Series[0].Points[i].YValues[1] = prices[1][i];
                this.chart1.Series[0].Points[i].YValues[2] = prices[2][i];
                this.chart1.Series[0].Points[i].YValues[3] = prices[3][i];
            }
        }

        void DrawVolumne(double[] vols)
        {
            this.chart1.Series[1].Points.DataBindY(vols);
        }

        void refreshChartArea()
        {
            float height = 0;
            for (int i = 0; i < this.chart1.ChartAreas.Count; i++)
            {
                this.chart1.ChartAreas[i].Position.Auto = false;
                this.chart1.ChartAreas[i].Position.X = 0;
                this.chart1.ChartAreas[i].Position.Y = height;
                this.chart1.ChartAreas[i].Position.Width = 100;
                if (i == 0)
                    this.chart1.ChartAreas[i].Position.Height = 50;
                else
                {
                    this.chart1.ChartAreas[i].Position.Height = 50 / (this.chart1.ChartAreas.Count - 1);
                }
                height = height + this.chart1.ChartAreas[i].Position.Height;
            }
        }


        private void btn_next_Click(object sender, EventArgs e)
        {
            NextGame();
        }

        private void btn_continue_Click(object sender, EventArgs e)
        {
            ContinueGo();
        }

        void ContinueGo()
        {
            if(!currData.Next())
            {
                return;
            }
            DrawCharts(currData.prices, currData.vols);
        }
    }
}
