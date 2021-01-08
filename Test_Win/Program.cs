using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WAPIWrapperCSharp;
using WolfInv.com.StrategyLibForWD;
using WolfInv.com.SecurityLib;
using WolfInv.com.CFZQ_LHProcess;
using WolfInv.com.ServerInitLib;
using WolfInv.com.ExchangeLib;
using WolfInv.com.LogLib;
using WolfInv.com.WinInterComminuteLib;
using DataRecSvr;
using WolfInv.com.BaseObjectsLib;
using System.IO;
using WolfInv.com.PK10CorePress;
using WDDRecSvr;
namespace Test_Win
{
    public class GlobalObj
    {
        public WindAPI w;
        public SystemGlobal Sys;
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Program<TimeSerialData>.Main();
        }
    }


    static class Program<T> where T:TimeSerialData
    {
        
        //public static ServiceSetting AllServiceConfig;
        private delegate void barDelegate(int no);
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main1()
        {
            //splitFile("share", "function(", '}');
            //return;
            LogableClass.ToLog("初始化服务器全局设置", "开始");
            InitSystem();
            LogableClass.ToLog("启动通道", "开始");
            new CommuniteClass().StartIPCServer();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            GlobalObj gb = new GlobalObj();
            ////gb.w = new WindAPI();
            ////gb.w.start();
            ///
            ReceiveService<TimeSerialData> rc = new ReceiveService<TimeSerialData>();
            rc.setGlobalClass(AllServiceConfig.gc);
            rc.CalcProcess = new CalcService<TimeSerialData>();
            rc.Start();
            Form2 frm = new Form2(gb);
            Application.Run(frm);
        }

        static void splitFile(string filename,string key,char lastkey)
        {
            string strPath = typeof(GlobalClass).Assembly.Location;
            string strJsPath = Path.GetDirectoryName(strPath) + "\\" + filename;
            StreamReader rd = File.OpenText(strJsPath);
            string StrAll = rd.ReadToEnd();
            rd.Close();
            StreamWriter wr = new StreamWriter(strJsPath + ".js");
            string strLine = StrAll;
            if(key == "function")
            {

            }
            int startPos = 1;
            int pos = strLine.IndexOf(key, startPos);
            while(pos > 0)
            {
                string restline = strLine.Substring(0,pos).Trim();
                if(restline[restline.Length-1]== lastkey)
                {
                    wr.WriteLine(restline);
                    strLine = strLine.Substring(pos);
                    startPos = 1;
                }
                else
                {
                    startPos = pos + 1;
                }
                pos = strLine.IndexOf(key, startPos);
            }
            wr.Close();
        }

        public static ServiceSetting<T> AllServiceConfig;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main2()
        {
            //splitFile("share", "function", '}');
            //return;
            try
            {
                ////DateTime dt = new DateTime(1991,4,3,12,0,1,12);
                ////DateTime bt = DateTime.Parse("1990-01-01");
                ////long val = (long)MongoDateTime.Stamp(dt);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //ServiceBase[] ServicesToRun;

                //只有接收数据是默认启动，计算服务由接收数据触发
                //ServicesToRun = new ServiceBase[]
                //{
                //    rs,sd
                //};
                LogableClass.ToLog("初始化服务器全局设置", "开始");
                InitSystem();
                LogableClass.ToLog("启动通道", "开始");
                new CommuniteClass().StartIPCServer();
                //ServiceBase.Run(ServicesToRun);
                GlobalObj gb = new GlobalObj();
                //gb.w = new WindAPI();
                //gb.w.start();
                new ReceiveService<T>().Start();
                //Service1<T> svr = new Service1<T>();
                //svr.Start(true);
                Form2 frm = new Form2(gb);
                Application.Run(frm);
            }
            catch (Exception e)
            {
                LogableClass.ToLog("初始化服务失败", e.StackTrace);
            }

        }



        static void InitSystem()
        {
            LogableClass.ToLog("构建计算服务", "开始");
            //CalcService<T> cs = new CalcService<T>();
            LogableClass.ToLog("构建接收服务", "开始");
            //ReceiveService<T> rs = new ReceiveService<T>();
            //SubscriptData sd = new SubscriptData();
            //rs.CalcProcess = cs;
            AllServiceConfig = new ServiceSetting<T>();
            AllServiceConfig.Init(null);
            AllServiceConfig.GrpThePlan(false);
            AllServiceConfig.CreateChannel(GlobalClass.TypeDataPoints.First().Key);

            AllServiceConfig.AllAssetUnits.Values.ToList().ForEach(p => p.Run(GlobalClass.TypeDataPoints.First().Value));//打开各开关
            //RemoteCommClass<ServiceSetting>.SetRemoteInst(AllServiceConfig);
            //AllServiceConfig.AllLogs = new LogInfo().GetLogAfterDate(DateTime.Today.AddHours(-1));
            DataRecSvr.Program<T>.AllServiceConfig = AllServiceConfig;
            //rs.Start();
        }
        [STAThread]
        public static void Main()
        {
            ////MissDataSerial mcc = new MissDataSerial(300);
            ////mcc.OpList = new[] { 0, 2, 3, 1, 200, 90, 1, 1, 1, 1, 0 }.ToList();
            ////for(int i=1;i<10;i++)
            ////{
            ////    mcc = mcc.getAdd(i%10==0?1:0);
            ////}
            //return;
            Main2();
            return;
            Application.Run(new frm_ImageTextRead());
            return;
            //////////测试读取数据
            //////////GlobalObj gb = new GlobalObj();
            ////////GlobalClass gc = new GlobalClass();
            ////////string str = GlobalClass.dbName;//触发静态
            ////////KL12_HtmlDataClass hd = new KL12_HtmlDataClass(GlobalClass.TypeDataPoints["SCKL12"]);
            ////////ExpectList<TimeSerialData> el = hd.getExpectList<TimeSerialData>();
            ////////return;
            splitFile("share", "function(", '}');
            //return;
            LogableClass.ToLog("初始化服务器全局设置", "开始");
            InitSystem();
            LogableClass.ToLog("启动通道", "开始");
            new CommuniteClass().StartIPCServer();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            GlobalObj gb = new GlobalObj();
            ////gb.w = new WindAPI();
            ////gb.w.start();
            ReceiveService<TimeSerialData> rc = new ReceiveService<TimeSerialData>();
            rc.Start();
            Form2 frm = new Form2(gb);
            Application.Run(frm);
        }
    }
}