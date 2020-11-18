using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WolfInv.com.WDDataInit;
using System.Threading;
using WolfInv.com.LogLib;
using WolfInv.com.BaseObjectsLib;
namespace WDDRecSvr
{
    public partial class Service1<T> : ServiceBase where T:TimeSerialData
    {
        public bool debug;
        Timer loopTimer;
        public Service1()
        {
            InitializeComponent();
            
        }

        public void Start(bool debugTheSvr = false)
        {
            WDDataInit<T>.Debug = debugTheSvr;
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            WDDataInit<T>.Init();
            if(WDDataInit<StockMongoData>.Loaded)
            {
                //WDDataInit<T>.finishedMsg = afterUpdateEquit;
            }
            int allSecs = 0;
            Dictionary<string, string> allEquites = WDDataInit<T>.AllSecurities;
            if(allEquites!= null)
            {
                allSecs = allEquites.Count;
            }
            LogableClass.ToLog("接收日志","需要接收股票数为", allSecs.ToString());
            loopTimer = new Timer(new TimerCallback(UpdateEquitData), null, 1*1000, 1000 * 60 * 60 * 8);
        }

        

        void UpdateEquitData(object obj)
        {
            Task.Factory.StartNew(() =>
            {
                DateTime curr = DateTime.Now;
                LogableClass.ToLog("接收日志", "开始接收", curr.ToString());
                //Program.wxlog.Log("开始接收", curr.ToString());
                WDDataInit<T>.finishedMsg = refreshMsg;
                WDDataInit<T>.loadAllEquitSerials(10,5, true,false,10,true);//只要看最后极少的数量即可
                
                LogableClass.ToLog("接收日志", "接收完成", string.Format("历时{0}分钟！", DateTime.Now.Subtract(curr).TotalMinutes));
                //Program.wxlog.Log("接收完成", string.Format("历时{0}分钟！", DateTime.Now.Subtract(curr).TotalMinutes));
            }
            );
        }

        void refreshMsg(int cnt, int total, EquitProcess<T>.EquitUpdateResult res)
        {
            Task.Factory.StartNew(() =>
            {
                if(!res.succ || !string.IsNullOrEmpty(res.Msg) )
                {
                    LogableClass.ToLog("错误", string.Format("证券{0}[{1}]", res.name,res.code), string.Format("{0}:{1}", res.Msg,res.MsgDetail));
                }
                if ((cnt % 100) == 0 || cnt == total)
                {
                    LogableClass.ToLog("接收日志", string.Format("已完成{0}", cnt), string.Format("共计:{0}.", total));
                }

            });
        }



        protected override void OnStop()
        {
        }
    }
}
