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
using System.Threading.Tasks;
using System.Threading;
using WolfInv.com.ShareLotteryLib;
using WolfInv.com.WXMessageLib;
namespace HappyShareLottery
{
    partial class wxMessageReceiveSvr : ServiceBase
    {
        public WebInterfaceClass wif;
        public frm_planMonitor Monitor;
        Timer recTimer;
        int TimerInterval = 100;
        public wxMessageReceiveSvr(WebInterfaceClass w,frm_planMonitor frm)
        {
            wif = w;
            Monitor = frm;
            InitializeComponent();
            if (Program.plancolls == null)
                Program.plancolls = new ShareLotteryPlanCollection();
            //Program.wif.MsgProcessCompleted += refreshMsg;
            //Action<object, List<wxMessageClass>> handle = new Action<object, List<wxMessageClass>>(Program.plancolls.MsgProcess.RefreshMsg);
            //Program.wif.MsgProcessCompleted_ForExtralInterfaceInvoke(handle);
            try
            {
                Program.plancolls.MsgProcess.SendMsg += wif.SendMsg;
                Program.plancolls.MsgProcess.SendImgMsg += wif.SendImgMsg;
                Program.plancolls.MsgProcess.SendUrlImgMsg += wif.SendUrlImgMsg;
                Program.plancolls.MsgProcess.SharePlanChanged += Monitor.refreshTab;
                Program.plancolls.MsgProcess.RobotUnionId = wif.UnionId;
                Program.plancolls.MsgProcess.RobotNikeName = wif.UserNike;
                Program.plancolls.MsgProcess.RobotUserName = wif.UserName;
                Program.plancolls.MsgProcess.MsgChanged += Monitor.refreshMsg;
                recTimer = new Timer(ReceiveMsg, null, 0, TimerInterval);

            }
            catch(Exception e)
            {

            }
        }

        void ReceiveMsg(object obj)
        {
            try
            {
                List<wxMessageClass> newMsgs = wif.getNewestMsg();
                if (newMsgs.Count > 0)
                {
                    Task.Run(() =>
                    {
                        Program.plancolls.MsgProcess.RefreshMsg(obj, newMsgs);
                    });
                }
            }
            catch(Exception ce)
            {

            }
        }

        public void Start()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
        }
    }
}
