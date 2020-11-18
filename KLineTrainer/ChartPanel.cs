using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KLineTrainer
{
    public partial class ChartPanel : UserControl
    {
        public ChartPanel()
        {
            InitializeComponent();
        }

        void refreshChartArea()
        {
            float height = 0;
            for(int i=0;i<this.chart1.ChartAreas.Count;i++)
            {
                this.chart1.ChartAreas[i].Position.Auto = false;
                this.chart1.ChartAreas[i].Position.X = 0;
                this.chart1.ChartAreas[i].Position.Y = height;
                this.chart1.ChartAreas[i].Position.Width = 100;
                if (i==0)
                    this.chart1.ChartAreas[i].Position.Height = 50;
                else
                {
                    this.chart1.ChartAreas[i].Position.Height = 50 /(this.chart1.ChartAreas.Count-1) ;
                }
                height = height + this.chart1.ChartAreas[i].Position.Height;
            }
        }

        private void ChartPanel_Load(object sender, EventArgs e)
        {
            refreshChartArea();
        }
    }
}
