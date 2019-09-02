using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WolfInv.com.WXMsgCom;
namespace MsgSvr
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            WebInterfaceClass wif = new WebInterfaceClass();
            try
            {
                if(!WebInterfaceClass.IPCCreated)
                {
                    MessageBox.Show("端口未正常建立！无法运行！");
                    return;
                }
                wif.SetDisplayMethod(true);
                wif.Init();
                wif.Start();
            }
            catch(Exception ce)
            {

            }
            
            //Application.Run();
            ////ServiceBase[] ServicesToRun;
            ////ServicesToRun = new ServiceBase[]
            ////{
            ////    new Service1()
            ////};
            ////ServiceBase.Run(ServicesToRun);
        }
    }
}
