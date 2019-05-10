using System;
using System.Timers;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.SecurityLib;
using WolfInv.com.PK10CorePress;
namespace DataRecSvr
{
    public partial class ReceiveService<T> :SelfDefBaseService<T> where T:TimeSerialData
    {
        System.Timers.Timer Tm_ForPK10 = new System.Timers.Timer();
        System.Timers.Timer Tm_ForTXFFC = new System.Timers.Timer();
        DateTime PK10_LastSignTime=DateTime.MaxValue;
        System.Timers.Timer tm = new System.Timers.Timer();
        Int64 lastFFCNo;
        public CalcService<T> CalcProcess;
        GlobalClass glb = new GlobalClass();
        int MissExpectEventPassCnt = 0;
        int MaxMissEventCnt = 15;
        public ReceiveService()
        {
            InitializeComponent();
            try
            {
                this.ServiceName = "定时刷新接收数据服务";
                Tm_ForPK10.Enabled = false;
                Tm_ForPK10.AutoReset = true;
                
                Tm_ForPK10.Interval = GlobalClass.TypeDataPoints["PK10"].ReceiveSeconds * 1000;
                Tm_ForPK10.Elapsed += new ElapsedEventHandler(Tm_ForPK10_Elapsed);
                Tm_ForTXFFC.Enabled = false;
                Tm_ForTXFFC.AutoReset = true;
                Tm_ForTXFFC.Interval = 10*1000+GlobalClass.TypeDataPoints["TXFFC"].ReceiveSeconds * 1000;
                Tm_ForTXFFC.Elapsed += new ElapsedEventHandler(Tm_ForTXFFC_Elapsed);
                tm.Interval = 5 * 1000;
                tm.Enabled = false;
                tm.AutoReset = true;
            }
            catch(Exception e)
            {
                Log("定时刷新接收数据服务错误！", string.Format("{0}:{1}",e.Message,e.StackTrace),true);
            }
            //tm.Elapsed += new ElapsedEventHandler(tm_Elapsed);        
        }

        

        void Tm_ForTXFFC_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                ReceiveTXFFCData();
            }
            catch(Exception ce)
            {
                Log("接收TXFFC错误", string.Format("{0}：{1}", ce.Message, ce.StackTrace),true);
            }
        }

        void Tm_ForPK10_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                ReceivePK10CData();
            }
            catch (Exception ce)
            {
                Log("接收PK10错误", string.Format("{0}：{1}", ce.Message, ce.StackTrace),true);
            }
        }

        public void Start()
        {
            OnStart(null);
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
            DateTime CurrTime = DateTime.Now;
            long RepeatMinutes = GlobalClass.TypeDataPoints["PK10"].ReceiveSeconds / 60;
            long RepeatSeconds = GlobalClass.TypeDataPoints["PK10"].ReceiveSeconds;
            PK10_HtmlDataClass hdc = new PK10_HtmlDataClass(GlobalClass.TypeDataPoints["PK10"]);
            ExpectList<T> tmp = hdc.getExpectList<T>();
            if (tmp == null || tmp.Count == 0)
            {
                this.Tm_ForPK10.Interval = RepeatSeconds / 20 * 1000;
                Log("尝试接收数据", "未接收到数据,数据源错误！",true);
                return;
            }
            ExpectList el = new ExpectList(tmp.Table);
            ////if(el != null && el.Count>0)
            ////{
            ////    Log("接收到的最新数据",el.LastData.ToString());
            ////}
            if (el == null || el.Count == 0)
            {
                this.Tm_ForPK10.Interval = RepeatSeconds / 20 * 1000;
                Log("尝试接收数据", "未接收到数据,转换数据源错误！",true);
                return;
            }
            
            DateTime StartTime = CurrTime.Date.Add(GlobalClass.TypeDataPoints["PK10"].ReceiveStartTime.TimeOfDay);
            Log("当日开始时间", StartTime.ToLongTimeString());
            int PassCnt = (int)Math.Floor(CurrTime.Subtract(StartTime).TotalMinutes / RepeatMinutes);
            Log("经历过的周期", PassCnt.ToString());
            DateTime PassedTime = StartTime.AddMinutes(PassCnt * RepeatMinutes);//正常的上期时间
            Log("前期时间", PassedTime.ToLongTimeString());
            DateTime FeatureTime = StartTime.AddMinutes((PassCnt + 1) * RepeatMinutes);//正常的下期时间
            Log("后期时间", FeatureTime.ToLongTimeString());
            DateTime NormalRecievedTime = StartTime.AddSeconds((PassCnt + 1.4) * RepeatSeconds);//正常的接收到时间
            Log("当期正常接收时间", FeatureTime.ToLongTimeString());
            try
            {
                PK10ExpectReader rd = new PK10ExpectReader();
                //ExpectList currEl = rd.ReadNewestData(DateTime.Today.AddDays(-1*glb.CheckNewestDataDays));//改为10天，防止春节连续多天不更新数据
                ExpectList<T> currEl = rd.ReadNewestData<T>(DateTime.Today.AddDays(-1 * GlobalClass.TypeDataPoints["PK10"].CheckNewestDataDays)); ;//改从PK10配置中获取
                if ((currEl == null || currEl.Count == 0) || (el.Count > 0 && currEl.Count > 0 && el.LastData.ExpectIndex > currEl.LastData.ExpectIndex))//获取到新数据
                {
                    Log("接收到数据", string.Format("接收到数据！{0}", el.LastData.ToString()),true);
                    //Program.AllServiceConfig.wxlog.Log("接收到数据", string.Format("接收到数据！{0}", el.LastData.ToString()));
                    PK10_LastSignTime = CurrTime;
                    long CurrMin = DateTime.Now.Minute % RepeatMinutes;
                    int CurrSec = DateTime.Now.Second;
                    //this.Tm_ForPK10.Interval = (CurrMin % RepeatMinutes < 2 ? 2 : 7 - CurrMin) * 60000 - (CurrSec + RepeatMinutes) * 1000;//5分钟以后见,减掉5秒不断收敛时间，防止延迟接收
                    this.Tm_ForPK10.Interval = FeatureTime.Subtract(CurrTime).TotalMilliseconds;
                    if (rd.SaveNewestData(rd.getNewestData<T>(new ExpectList<T>(el.Table), currEl)) > 0)
                    {
                        CurrDataList = rd.ReadNewestData<T>(DateTime.Now.AddDays(-1* GlobalClass.TypeDataPoints["PK10"].CheckNewestDataDays));//前十天的数据 尽量的大于reviewcnt,免得需要再取一次数据
                        CurrExpectNo = el.LastData.Expect;
                        Program.AllServiceConfig.LastDataSector =new ExpectList<TimeSerialData>(CurrDataList.Table) ;
                        //2019/4/22日出现错过732497，732496两期记录错过，但是732498却收到的情况，同时，正好在732498多次重复策略正好开出结束，因错过2期导致一直未归零，
                        //一直长时间追号开出近60期
                        //为避免出现这种情况
                        //判断是否错过了期数，如果错过期数，将所有追踪策略归零，不再追号,也不再执行选号程序，
                        //是否要连续停几期？执行完后，在接收策略里面发现前10期有不连续的情况，直接跳过，只接收数据不执行选号。
                        if (MissExpectEventPassCnt > 0)//如果出现错期
                        {
                            Log("接收到错期数据", string.Format("接收到数据！{0}", el.LastData.ToString()), true);
                            if (MissExpectEventPassCnt <= MaxMissEventCnt)//超过最大平稳期，置零,下次再计算
                            {
                                MissExpectEventPassCnt = 0;
                            }
                            else //继续跳过
                            {
                                MissExpectEventPassCnt++;
                            }
                        }
                        else//第一次及平稳期后进行计算
                        {
                            bool res = AfterReceiveProcess();
                            if (res == false)
                                this.Tm_ForPK10.Interval = RepeatSeconds / 20 * 1000;
                            if (CurrDataList.MissExpectCount() > 1)//执行完计算(关闭所有记录)后再标记为已错期
                            {
                                MissExpectEventPassCnt = 1;
                            }
                        }
                    }
                    else
                    {
                        this.Tm_ForPK10.Interval = RepeatSeconds / 20 * 1000;
                        Log("保存数据错误", "保存数据数量为0！",true);
                    }
                }
                else
                {
                    Log("接收到非最新数据",string.Format("id:{0}",el.LastData.Expect),false);
                    if (CurrTime.Hour < 9)//如果在9点前
                    {
                        //下一个时间点是9：07 //9:30
                        DateTime TargetTime = DateTime.Today.AddHours(9).AddMinutes(30);
                        this.Tm_ForPK10.Interval = TargetTime.Subtract(CurrTime).TotalMilliseconds;
                    }
                    else
                    {
                        Log("接收数据", "未接收到数据！");
                        //if (NormalRecievedTime > CurrTime)
                        //{
                        //    this.Tm_ForPK10.Interval =  NormalRecievedTime.AddMinutes(1).Subtract(CurrTime).TotalMilliseconds;
                        //}
                        //else
                        //{
                            //this.Tm_ForPK10.Interval = RepeatSeconds / 20 * 1000;
                            if (CurrTime.Subtract(PK10_LastSignTime).TotalMinutes > RepeatMinutes + RepeatMinutes * 2 / 5)//如果离上期时间超过2/5个周期，说明数据未接收到，那不要再频繁以10秒访问服务器
                            {
                                this.Tm_ForPK10.Interval = RepeatSeconds / 5 * 1000;
                            }
                            else //一般未接收到，10秒以后再试，改为50分之一个周期再试
                            {
                                this.Tm_ForPK10.Interval = RepeatSeconds / 20 * 1000;
                            }
                        //}
                    }
                }
            }
            catch (Exception e)
            {
                Log(e.Message,e.StackTrace);
            }
            //Log("接收数据", string.Format("当前访问时间为：{0},{1}秒后继续访问！",CurrTime.ToString(),this.Tm_ForPK10.Interval/1000),false);
            //FillOrgData(listView_TXFFCData, currEl);
        }

        private void ReceiveTXFFCData()
        {

            int secCnt = DateTime.Now.Second;
            TXFFC_HtmlDataClass hdc = new TXFFC_HtmlDataClass(GlobalClass.TypeDataPoints["PK10"]);
            ExpectList<T> tmp = hdc.getExpectList<T>();
            if (tmp == null || tmp.Count == 0)
            {
                Tm_ForTXFFC.Interval = 5 * 1000;
                return;
            }
            ExpectList el = new ExpectList(tmp.Table);
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
            ExpectList<T> currEl = rd.ReadNewestData<T>(DateTime.Now.AddDays(-1));
            ExpectList<T> SaveData = rd.getNewestData<T>(new ExpectList<T>(el.Table), currEl);
            if (SaveData.Count > 0)
            {
                //Log("Save count!", SaveData.Count.ToString());
                rd.SaveNewestData(SaveData);
            }
            currEl = rd.ReadNewestData<T>(DateTime.Now.AddDays(-1));
            //FillOrgData(listView_TXFFCData, currEl);
        }

        bool AfterReceiveProcess()
        {
            //刷新客户端数据

            //////if(CalcProcess.AllowCalc)//测试用，以后需要删除
            //////{
            //////    CalcProcess.StartService();
            //////}
            Log("转移到计算服务", "准备进行计算");
            try
            {
                this.CalcProcess.Calc();
            }
            catch(Exception e)
            {
                Log("计算错误", string.Format("{0}：{1}",e.Message,e.StackTrace));
                return false;
            }
            return true;
        }

        
    }

}
