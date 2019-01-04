using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using PK10CorePress;
using System.Timers;
using LogLib;
using WinInterComminuteLib;
using ServerInitLib;
using System.Threading;
namespace DataRecSvr
{
    partial class ReceiveService :SelfDefBaseService
    {
        System.Timers.Timer Tm_ForPK10 = new System.Timers.Timer();
        System.Timers.Timer Tm_ForTXFFC = new System.Timers.Timer();
        DateTime PK10_LastSignTime=DateTime.MaxValue;
        System.Timers.Timer tm = new System.Timers.Timer();
        Int64 lastFFCNo;
        public CalcService CalcProcess;
        public ReceiveService()
        {
            InitializeComponent();
            this.ServiceName = "接收数据服务";
            Tm_ForPK10.Enabled = false;
            Tm_ForPK10.AutoReset = true;
            Tm_ForPK10.Interval = 5*60*1000;
            Tm_ForPK10.Elapsed += new ElapsedEventHandler(Tm_ForPK10_Elapsed);
            Tm_ForTXFFC.Enabled = false;
            Tm_ForTXFFC.AutoReset = true;
            Tm_ForTXFFC.Interval = 60*1000;
            Tm_ForTXFFC.Elapsed += new ElapsedEventHandler(Tm_ForTXFFC_Elapsed);
            tm.Interval = 5 * 1000;
            tm.Enabled = false;
            tm.AutoReset = true;
            //tm.Elapsed += new ElapsedEventHandler(tm_Elapsed);        
        }

        

        void Tm_ForTXFFC_Elapsed(object sender, ElapsedEventArgs e)
        {
            ReceiveTXFFCData();
        }

        void Tm_ForPK10_Elapsed(object sender, ElapsedEventArgs e)
        {
            ReceivePK10CData();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
            Tm_ForPK10.Enabled = true;
            Log("开始服务", "开始接收数据！");
            Tm_ForPK10_Elapsed(null, null);
            Log("开始服务", "接收数据成功！");
            Tm_ForTXFFC.Enabled = true;
            Tm_ForTXFFC_Elapsed(null, null);
            //tm.Enabled = true;
            //tm_Elapsed(null, null);
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            Tm_ForPK10.Enabled = false;
            Tm_ForTXFFC.Enabled = false;
            Log("停止服务", "服务停止成功！");
        }

        private void ReceivePK10CData()
        {
            Log("接收数据", "准备接收数据");
            PK10_HtmlDataClass hdc = new PK10_HtmlDataClass();
            ExpectList el = hdc.getExpectList();
            ////if(el != null && el.Count>0)
            ////{
            ////    Log("接收到的最新数据",el.LastData.ToString());
            ////}
            if (el == null || el.Count == 0)
            {
                Log("接收数据", "未接收到数据");
                return;
            }
            DateTime CurrTime = DateTime.Now;
            try
            {
                PK10ExpectReader rd = new PK10ExpectReader();
                ExpectList currEl = rd.ReadNewestData(DateTime.Today.AddDays(-1));
                
                if ((currEl == null || currEl.Count == 0) || (el.Count > 0 && currEl.Count > 0 && el.LastData.ExpectIndex > currEl.LastData.ExpectIndex))//获取到新数据
                {
                    Log("接收到数据", string.Format("接收到数据！{0}", el.LastData.ToString()));
                    int CurrMin = DateTime.Now.Minute % 5;
                    int CurrSec = DateTime.Now.Second;
                    this.Tm_ForPK10.Interval = (CurrMin % 5 < 2 ? 2 : 7 - CurrMin) * 60000 - (CurrSec + 5) * 1000;//5分钟以后见,减掉5秒不断收敛时间，防止延迟接收
                    if (rd.SaveNewestData(rd.getNewestData(el, currEl)) > 0)
                    {
                        CurrDataList = rd.ReadNewestData(DateTime.Now.AddDays(-10));//前十天的数据 尽量的大于reviewcnt,免得需要再取一次数据
                        CurrExpectNo = el.LastData.Expect;
                        Program.AllServiceConfig.LastDataSector = CurrDataList;
                        AfterReceiveProcess();
                    }
                    else
                    {
                        Log("保存数据错误", "保存数据数量为0！");
                    }
                }
                else
                {
                    Log("接收到非最新数据",string.Format("id:{0}",el.LastData.Expect));
                    if (CurrTime.Hour < 9)//如果在9点前
                    {
                        //下一个时间点是9：07
                        DateTime TargetTime = DateTime.Today.AddHours(9).AddMinutes(8);
                        this.Tm_ForPK10.Interval = TargetTime.Subtract(CurrTime).TotalMilliseconds;
                    }
                    else
                    {
                        Log("接收数据", "未接收到数据！");
                        if (CurrTime.Subtract(PK10_LastSignTime).Minutes > 7)//如果离上期时间超过7分钟，说明数据未接收到，那不要再频繁以10秒访问服务器
                        {
                            this.Tm_ForPK10.Interval = 60 * 1000;
                        }
                        else //一般未接收到，10秒以后再试
                        {
                            this.Tm_ForPK10.Interval = 10 * 1000;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log(e.Message,e.StackTrace);
            }
            Log("接收数据", string.Format("当前访问时间为：{0},{1}秒后继续访问！",CurrTime.ToString(),this.Tm_ForPK10.Interval/1000));
            //FillOrgData(listView_TXFFCData, currEl);
        }

        private void ReceiveTXFFCData()
        {

            int secCnt = DateTime.Now.Second;
            TXFFC_HtmlDataClass hdc = new TXFFC_HtmlDataClass();
            ExpectList el = hdc.getExpectList();
            if (el == null || el.LastData == null)
            {
                Tm_ForTXFFC.Interval = 5 * 1000;
                return;
            }
            if (Int64.Parse(el.LastData.Expect) > lastFFCNo)
            {
                lastFFCNo = Int64.Parse(el.LastData.Expect);
                if (secCnt > 10)
                {
                    Tm_ForTXFFC.Interval = (7 + 60 - secCnt) * 1000;
                }
                else if (secCnt < 7)
                {
                    Tm_ForTXFFC.Interval = (7 - secCnt) * 1000;
                }
                else
                {
                    Tm_ForTXFFC.Interval = 60 * 1000;
                }
            }
            else
            {
                Tm_ForTXFFC.Interval = 5 * 1000;
                return;
            }
            //Log("保存分分彩数据", string.Format("期号:{0}",lastFFCNo));
            TXFFCExpectReader rd = new TXFFCExpectReader();
            ExpectList currEl = rd.ReadNewestData(DateTime.Now.AddDays(-1));
            rd.SaveNewestData(rd.getNewestData(el, currEl));
            currEl = rd.ReadNewestData(DateTime.Now.AddDays(-1));
            //FillOrgData(listView_TXFFCData, currEl);
        }

        void AfterReceiveProcess()
        {
            //刷新客户端数据

            //////if(CalcProcess.AllowCalc)//测试用，以后需要删除
            //////{
            //////    CalcProcess.StartService();
            //////}
            Log("转移到计算服务", "准备进行计算");
            this.CalcProcess.Calc();
        }

        
    }

}
