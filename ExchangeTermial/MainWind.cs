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
namespace ExchangeTermial
{
    public partial class MainWind : Form
    {
        List<Timer> times; 
        public MainWind()
        {
            InitializeComponent();
        }

        bool Init()
        {
            
            return true;
        }

        void InitTimes()
        {
            foreach (string key in GlobalClass.TypeDataPoints.Keys)
            {
                DataTypePoint dtp = GlobalClass.TypeDataPoints[key];
                Timer tm = new Timer();
                tm.Tag = dtp;
                tm.Interval = (int)dtp.ReceiveSeconds * 1000;
                tm.Tick += refreshData;
                tm.Enabled = false;
                times.Add(tm);
            }
        }   
 
        private void MainWind_Load(object sender, EventArgs e)
        {

        }

        void StartTimes()
        {
            times.ForEach(a => {
                DataTypePoint dtp = a.Tag as DataTypePoint;
                if (dtp == null)
                {
                    return;
                }
                a.Enabled = true;
                a.Start();
            });
        }

        void EndTimes()
        {
            times.ForEach(a => {
                a.Enabled = false;
                a.Stop();
            });
        }
        private void refreshData(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

    }
}
