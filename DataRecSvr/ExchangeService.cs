using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace DataRecSvr
{
    partial class ExchangeService :SelfDefBaseService
    {
        bool AllowExchange = false;
        public ExchangeService()
        {
            InitializeComponent();
            this.ServiceName = "交易服务";
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
            AllowExchange = true;
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            AllowExchange = false;
        }

        
    }
}
