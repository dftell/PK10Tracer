using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace LotteryDataCenter
{
    public partial class receiveCenter : ServiceBase
    {
        public mainWindow frm;
        public receiveCenter()
        {
            InitializeComponent();
          
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }

        public void Start(string[] args)
        {

        }
    }
}
