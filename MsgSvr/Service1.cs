using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.WXMsgCom;

namespace MsgSvr
{
    public partial class Service1 : ServiceBase
    {

        WebInterfaceClass wif = null; 
        public Service1()
        {
            InitializeComponent();
            wif = new WebInterfaceClass();
        }

        protected override void OnStart(string[] args)
        {
            wif.Start();
        }

        protected override void OnStop()
        {
            wif.Stop();
        }
    }
}
