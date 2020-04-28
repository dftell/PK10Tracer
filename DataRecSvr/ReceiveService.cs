using System;
using System.Timers;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.SecurityLib;
using WolfInv.com.PK10CorePress;
using WolfInv.com.ServerInitLib;
using WolfInv.com.ExchangeLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace DataRecSvr
{
    public partial class ReceiveService<T> :SelfDefBaseService<T> where T:TimeSerialData
    {
        Dictionary<string, self_Timer> RecTimers;
        System.Timers.Timer Tm_ForPK10 = new System.Timers.Timer();
        //System.Timers.Timer Tm_ForTXFFC = new System.Timers.Timer();
        //System.Timers.Timer Tm_ForCAN28 = new Timer();
        DateTime PK10_LastSignTime=DateTime.MaxValue;
        //////////System.Timers.Timer tm = new System.Timers.Timer();
        Int64 lastFFCNo;
        Int64 lastCAN28No;
        public CalcService<T> CalcProcess;
        GlobalClass glb = new GlobalClass();
        int MissExpectEventPassCnt = 0;
        int MaxMissEventCnt = 20;// 15;//错期发生后需要经历的最大平稳期数，从15改为20
        public ReceiveService()
        {
            InitializeComponent();
            RecTimers = new Dictionary<string, self_Timer>();
            try
            {
                this.ServiceName = "定时刷新接收数据服务";
                foreach(string key in GlobalClass.TypeDataPoints.Keys)
                {
                    //if(key.Equals("PK10"))//暂时跳过
                    //{
                    //    continue;
                    //}
                    self_Timer tm = new self_Timer();
                    tm.Name = key;
                    tm.Enabled = false;
                    tm.AutoReset = true;
                    tm.Interval = GlobalClass.TypeDataPoints[key].ReceiveSeconds * 1000;
                    tm.Elapsed += Tm_Elapsed;
                    RecTimers.Add(key, tm);
                }
                //if (GlobalClass.DataTypes.ContainsKey("PK10"))
                //{
                //    Tm_ForPK10.Enabled = false;
                //    Tm_ForPK10.AutoReset = true;

                //    Tm_ForPK10.Interval = GlobalClass.TypeDataPoints["PK10"].ReceiveSeconds * 1000;
                //    Tm_ForPK10.Elapsed += new ElapsedEventHandler(Tm_ForPK10_Elapsed);

                //}
                //////if (GlobalClass.DataTypes.ContainsKey("TXFFC"))
                //////{

                //////    Tm_ForTXFFC.Enabled = false;
                //////    Tm_ForTXFFC.AutoReset = true;
                //////    Tm_ForTXFFC.Interval = 10 * 1000 + GlobalClass.TypeDataPoints["TXFFC"].ReceiveSeconds * 1000;
                //////    Tm_ForTXFFC.Elapsed += new ElapsedEventHandler(Tm_ForTXFFC_Elapsed);
                //////}
                //////if (GlobalClass.DataTypes.ContainsKey("CAN28"))
                //////{

                //////    Tm_ForCAN28.Enabled = false;
                //////    Tm_ForCAN28.AutoReset = true;
                //////    Tm_ForCAN28.Interval = 10 * 1000 + GlobalClass.TypeDataPoints["CAN28"].ReceiveSeconds * 1000;
                //////    Tm_ForCAN28.Elapsed += new ElapsedEventHandler(Tm_ForCAN28_Elapsed);
                //////}
                //////////////tm.Interval = 5 * 1000;
                //////////////tm.Enabled = false;
                //////////////tm.AutoReset = true;
            }
            catch(Exception e)
            {
                Log("定时刷新接收数据服务错误！", string.Format("{0}:{1}",e.Message,e.StackTrace),true);
            }
            //tm.Elapsed += new ElapsedEventHandler(tm_Elapsed);        
        }

        private void Tm_Elapsed(object sender, ElapsedEventArgs e)
        {
            self_Timer tm = sender as self_Timer;
            string key = tm.Name;
            try
            {
                ReceiveData(key, tm, null, null, true);
            }
            catch(Exception ce)
            {
                Log(string.Format("接收{0}数据错误",key), string.Format("{0}：{1}", ce.Message, ce.StackTrace), true);
            }
        }

        //private void Tm_ForCAN28_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    try
        //    {
        //        ReceiveData("CAN28",Tm_ForCAN28,null,null,false);
        //    }
        //    catch (Exception ce)
        //    {
        //        Log("接收加拿大28错误", string.Format("{0}：{1}", ce.Message, ce.StackTrace), true);
        //    }
        //}

        //void Tm_ForTXFFC_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    try
        //    {
        //        ReceiveTXFFCData();
        //    }
        //    catch(Exception ce)
        //    {
        //        Log("接收TXFFC错误", string.Format("{0}：{1}", ce.Message, ce.StackTrace),true);
        //    }
        //}

        //void Tm_ForPK10_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    try
        //    {
        //        ReceivePK10CData();
        //    }
        //    catch (Exception ce)
        //    {
        //        Log("接收PK10错误", string.Format("{0}：{1}", ce.Message, ce.StackTrace),true);
        //    }
        //}

        public void Start()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
            //if (GlobalClass.DataTypes.ContainsKey("PK10"))
            //{
            //    Tm_ForPK10.Enabled = true;
            //    Log("开始服务", "开始接收数据！");
            //    Tm_ForPK10_Elapsed(null, null);
            //}
            ////if (GlobalClass.DataTypes.ContainsKey("TXFFC"))
            ////{
            ////    Log("开始服务", "接收数据成功！");
            ////    Tm_ForTXFFC.Enabled = true;
            ////    Tm_ForTXFFC_Elapsed(null, null);
            ////}
            ////if (GlobalClass.DataTypes.ContainsKey("CAN28"))
            ////{
            ////    Log("开始服务", "接收数据成功！");
            ////    Tm_ForCAN28.Enabled = true;
            ////    Tm_ForCAN28_Elapsed(null, null);
            ////}
            foreach(self_Timer tm in RecTimers.Values)
            {
                //tm.Interval = 1000;
                //tm.Enabled = true;
                tm.Enabled = true;
                Tm_Elapsed(tm, null);
                Log(string.Format("{0}开始服务", tm.Name), "开始接收数据",true);
                //Tm_Elapsed(tm, null);
            }
            //tm.Enabled = true;
            //tm_Elapsed(null, null);

        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            //Tm_ForPK10.Enabled = false;
            //Tm_ForTXFFC.Enabled = false;
            foreach(self_Timer tm in RecTimers.Values)
            {
                tm.Enabled = false;
            }
            Log("停止服务", "服务停止成功！",true);
        }
        /*
        private void ReceivePK10CData()
        {
            DataTypePoint dtp = GlobalClass.TypeDataPoints["PK10"];
            Log("接收数据", "准备接收数据");
            DateTime CurrTime = DateTime.Now;
            long RepeatMinutes = GlobalClass.TypeDataPoints["PK10"].ReceiveSeconds / 60;
            long RepeatSeconds = GlobalClass.TypeDataPoints["PK10"].ReceiveSeconds;
            PK10_HtmlDataClass hdc = new PK10_HtmlDataClass(GlobalClass.TypeDataPoints["PK10"]);
            ExpectList<T> tmp = hdc.getExpectList<T>();
            if (tmp == null || tmp.Count == 0)
                this.Tm_ForPK10.Interval = RepeatSeconds / 20 * 1000;
                Log("尝试接收数据", "未接收到数据,数据源错误！",true);
            {
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
            //Log("当日开始时间", StartTime.ToLongTimeString());
            int PassCnt = (int)Math.Floor(CurrTime.Subtract(StartTime).TotalMinutes / RepeatMinutes);
            //Log("经历过的周期", PassCnt.ToString());
            DateTime PassedTime = StartTime.AddMinutes(PassCnt * RepeatMinutes);//正常的上期时间
            //Log("前期时间", PassedTime.ToLongTimeString());
            DateTime FeatureTime = StartTime.AddMinutes((PassCnt + 1) * RepeatMinutes);//正常的下期时间
            //Log("后期时间", FeatureTime.ToLongTimeString());
            DateTime NormalRecievedTime = StartTime.AddSeconds((PassCnt + 1.4) * RepeatSeconds);//正常的接收到时间
            //Log("当期正常接收时间", FeatureTime.ToLongTimeString());
            try
            {
                PK10ExpectReader rd = new PK10ExpectReader();
                //ExpectList currEl = rd.ReadNewestData(DateTime.Today.AddDays(-1*glb.CheckNewestDataDays));//改为10天，防止春节连续多天不更新数据
                ExpectList<T> currEl = rd.ReadNewestData<T>(DateTime.Today.AddDays(-1 * GlobalClass.TypeDataPoints["PK10"].CheckNewestDataDays)); ;//改从PK10配置中获取
                if ((currEl == null || currEl.Count == 0) || (el.Count > 0 && currEl.Count > 0 && el.LastData.ExpectIndex > currEl.LastData.ExpectIndex))//获取到新数据
                {
                    Log("接收到数据", string.Format("接收到数据！{0}", el.LastData.OpenCode),true);
                    //Program.AllServiceConfig.wxlog.Log("接收到数据", string.Format("接收到数据！{0}", el.LastData.ToString()));
                    PK10_LastSignTime = CurrTime;
                    long CurrMin = DateTime.Now.Minute % RepeatMinutes;
                    int CurrSec = DateTime.Now.Second;
                    //this.Tm_ForPK10.Interval = (CurrMin % RepeatMinutes < 2 ? 2 : 7 - CurrMin) * 60000 - (CurrSec + RepeatMinutes) * 1000;//5分钟以后见,减掉5秒不断收敛时间，防止延迟接收
                    this.Tm_ForPK10.Interval = FeatureTime.Subtract(CurrTime).TotalMilliseconds;
                    if (rd.SaveNewestData(rd.getNewestData<T>(new ExpectList<T>(el.Table), currEl)) > 0)
                    {
                        CurrDataList = rd.ReadNewestData<T>(DateTime.Now.AddDays(-1* GlobalClass.TypeDataPoints["PK10"].CheckNewestDataDays));//前十天的数据 尽量的大于reviewcnt,免得需要再取一次数据
                        if(CurrDataList == null)
                        {
                            this.Tm_ForPK10.Interval = RepeatSeconds / 20 * 1000;
                            Log("计算最新数据错误", "无法获取最新数据发生错误，请检查数据库是否正常！", true);
                            return;
                        }
                        CurrExpectNo = el.LastData.Expect;
                        Program.AllServiceConfig.LastDataSector = new ExpectList<TimeSerialData>(CurrDataList.Table) ;
                        //2019/4/22日出现错过732497，732496两期记录错过，但是732498却收到的情况，同时，正好在732498多次重复策略正好开出结束，因错过2期导致一直未归零，
                        //一直长时间追号开出近60期
                        //为避免出现这种情况
                        //判断是否错过了期数，如果错过期数，将所有追踪策略归零，不再追号,也不再执行选号程序，
                        //是否要连续停几期？执行完后，在接收策略里面发现前10期有不连续的情况，直接跳过，只接收数据不执行选号。
                        CurrDataList.UseType = GlobalClass.TypeDataPoints["PK10"];
                        if (CurrDataList.MissExpectCount() > 1 || MissExpectEventPassCnt > 0)//如果出现错期
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
                            CalcProcess.DataPoint = dtp;
                            CalcProcess.ReadDataTableName = null;//为证券计算用
                            CalcProcess.Codes = null;//为支持证券计算用
                            bool res = AfterReceiveProcess(CalcProcess);
                            if (res == false)
                                this.Tm_ForPK10.Interval = RepeatSeconds / 20 * 1000;
                            CurrDataList.UseType = dtp;
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
                        Log("接收数据", "未接收到新数据！");
                        //if (NormalRecievedTime > CurrTime)
                        //{
                        //    this.Tm_ForPK10.Interval =  NormalRecievedTime.AddMinutes(1).Subtract(CurrTime).TotalMilliseconds;
                        //}
                        //else
                        //{
                            //this.Tm_ForPK10.Interval = RepeatSeconds / 20 * 1000;
                            if (CurrTime.Subtract(PK10_LastSignTime).TotalMinutes > RepeatMinutes + RepeatMinutes * 2 / 5)//如果离上期时间超过2/5个周期，说明数据未接收到，那不要再频繁以10秒访问服务器
                            {
                                this.Tm_ForPK10.Interval = RepeatSeconds / 20 * 1000;
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
                Log(e.Message,e.StackTrace,true);
            }
            //Log("接收数据", string.Format("当前访问时间为：{0},{1}秒后继续访问！",CurrTime.ToString(),this.Tm_ForPK10.Interval/1000),false);
            //FillOrgData(listView_TXFFCData, currEl);
        }
       
        private void ReceiveTXFFCData()
        {

            int secCnt = DateTime.Now.Second;
            TXFFC_HtmlDataClass hdc = new TXFFC_HtmlDataClass(GlobalClass.TypeDataPoints["TXFFC"]);
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
         */
        private void  ReceiveData(string DataType,Timer useTimer,string strReadTableName,string[] codes,bool NeedCalc = false)
        {
            DataReader rd = DataReaderBuild.CreateReader(DataType, strReadTableName, codes);
            if(rd == null)
            {
                Log("核心运行数据丢失，建议立即重启服务！", string.Format("无法找到类型{0}！当前类型字典包含有为:{1]",DataType,string.Join(",",GlobalClass.TypeDataPoints.Keys.ToList())), true);
                return;
            }
            lock (rd)
            {
                HtmlDataClass hdc = null;
                DataTypePoint dtp = GlobalClass.TypeDataPoints[DataType];
                Log("接收数据", "准备接收数据");
                int DiffHours = 0;
                int DiffMinutes = 0;
                if (dtp.DiffHours != 0)
                {
                    DiffHours = dtp.DiffHours;
                    DiffMinutes = dtp.DiffMinutes;
                }
                DateTime CurrTime = DateTime.Now.AddHours(DiffHours).AddMinutes(DiffMinutes);
                long RepeatMinutes = dtp.ReceiveSeconds / 60;
                long RepeatSeconds = dtp.ReceiveSeconds;
                hdc = HtmlDataClass.CreateInstance(dtp);
                ExpectList<T> tmp = hdc.getExpectList<T>();
                if (tmp == null || tmp.Count == 0)
                {
                    
                    useTimer.Interval = RepeatSeconds / 20 * 1000;

                    Log("尝试接收数据", "未接收到数据,数据源错误！", glb.ExceptNoticeFlag);
                    return;
                }
                ExpectList el = new ExpectList(tmp.Table);
                ////if(el != null && el.Count>0)
                ////{
                ////    Log("接收到的最新数据",el.LastData.ToString());
                ////}
                if (el == null || el.Count == 0)
                {
                    useTimer.Interval = RepeatSeconds / 20 * 1000;
                    Log("尝试接收数据", "未接收到数据,转换数据源错误！", glb.ExceptNoticeFlag);
                    return;
                }
                
                DateTime StartTime = CurrTime.Date.Add(dtp.ReceiveStartTime.TimeOfDay);
                //Log("当日开始时间", StartTime.ToLongTimeString());
                int PassCnt = (int)Math.Floor(CurrTime.Subtract(StartTime).TotalMinutes / RepeatMinutes);
                //Log("经历过的周期", PassCnt.ToString());
                DateTime PassedTime = StartTime.AddMinutes(PassCnt * RepeatMinutes);//正常的上期时间
                //Log("前期时间", PassedTime.ToLongTimeString());
                DateTime FeatureTime = StartTime.AddMinutes((PassCnt + 1) * RepeatMinutes);//正常的下期时间
                 //Log("后期时间", FeatureTime.ToLongTimeString());
                DateTime NormalRecievedTime = StartTime.AddSeconds((PassCnt + 1.4) * RepeatSeconds);//正常的接收到时间
                //Log("当期正常接收时间", FeatureTime.ToLongTimeString());
                try
                {
                    //PK10ExpectReader rd = new PK10ExpectReader();

                    //ExpectList currEl = rd.ReadNewestData(DateTime.Today.AddDays(-1*glb.CheckNewestDataDays));//改为10天，防止春节连续多天不更新数据
                    //ExpectList<T> currEl = rd.ReadNewestData<T>(DateTime.Today.AddDays(-1 * dtp.CheckNewestDataDays)); ;//改从PK10配置中获取
                    //Log("接收第一期数据", el.FirstData.Expect, true);
                    //ExpectList<T> currEl = rd.ReadNewestData<T>(dtp.NewRecCount);
                    ExpectList<T> currEl = rd.ReadNewestData<T>(DateTime.Now.AddDays(-1 * dtp.CheckNewestDataDays)); //前十天的数据 //2020.4.8 尽量的大于reviewcnt, 免得需要再取一次数据, 尽量多取，以防后面策略调用需要上万条数据，还要防止放假中间间隔时间较长
                    ExpectList<T> NewList = rd.getNewestData<T>(new ExpectList<T>(el.Table), currEl);//新列表是最新的数据在前的，后面使用时要反序用
                    
                    //if ((currEl == null || currEl.Count == 0) || (el.Count > 0 && currEl.Count > 0 && el.LastData.ExpectIndex > currEl.LastData.ExpectIndex))//获取到新数据
                    if (NewList.Count > 0)
                    {
                        string lastExpect = currEl?.LastData?.Expect;
                        string newestExpect = el.LastData.Expect;
                        int interExpect = 1;//间隔数量
                        if(lastExpect!= null)
                        {
                            interExpect = (int) DataReader.getInterExpectCnt(lastExpect, newestExpect, dtp);
                        }
                        if (currEl.Count>0)
                            Log(string.Format("接收到{0}数据", DataType), string.Format("接收到数据！新数据：[{0},{1}],老数据:[{2},{3}]", el.LastData.Expect, el.LastData.OpenCode, currEl.LastData.Expect, currEl.LastData.OpenCode), glb.NormalNoticeFlag);
                        //Program.AllServiceConfig.wxlog.Log("接收到数据", string.Format("接收到数据！{0}", el.LastData.ToString()));
                        PK10_LastSignTime = CurrTime;
                        long CurrMin = DateTime.Now.Minute % RepeatMinutes;
                        int CurrSec = DateTime.Now.Second;
                        //useTimer.Interval = (CurrMin % RepeatMinutes < 2 ? 2 : 7 - CurrMin) * 60000 - (CurrSec + RepeatMinutes) * 1000;//5分钟以后见,减掉5秒不断收敛时间，防止延迟接收
                        useTimer.Interval = FeatureTime.Subtract(CurrTime).TotalMilliseconds;
                        //ExpectList<T>  NewList = rd.getNewestData<T>(new ExpectList<T>(el.Table), currEl);
                        string[] expects = NewList.DataList.Select(a=>a.Expect).ToArray();
                        //Log("存在数据",string.Format("共{0}期:[{1}]", currEl.Count,string.Join(",", currEl.DataList.Select(a=>a.Expect).ToArray())),true);
                        if (NewList.Count == 0)
                        {
                            Log("待保存数据数量为0", "无须保存！", glb.ExceptNoticeFlag);
                            useTimer.Interval = RepeatSeconds  * 1000;
                            return;
                        }
                        int savecnt = rd.SaveNewestData(NewList);
                        if (savecnt > 0)
                        {
                            Log("保存数据条数", string.Format("{0}条数！",savecnt), glb.NormalNoticeFlag);
                            ////CurrDataList = rd.ReadNewestData<T>(DateTime.Now.AddDays(-1 * dtp.CheckNewestDataDays));//前十天的数据 尽量的大于reviewcnt,免得需要再取一次数据,尽量多取，以防后面策略调用需要上万条数据，还要防止放假中间间隔时间较长
                            ////if (CurrDataList == null) //只是测试，要不要？
                            ////{
                            ////    useTimer.Interval = RepeatSeconds / 20 * 1000;
                            ////    Log("计算最新数据错误", "无法获取最新数据发生错误，请检查数据库是否正常！", glb.ExceptNoticeFlag);
                            ////    return;
                            ////}
                            CurrExpectNo = el.LastData.Expect;

                            //Program.AllServiceConfig.LastDataSector = new ExpectList<TimeSerialData>(CurrDataList.Table);
                            //2019/4/22日出现错过732497，732496两期记录错过，但是732498却收到的情况，同时，正好在732498多次重复策略正好开出结束，因错过2期导致一直未归零，
                            //一直长时间追号开出近60期
                            //为避免出现这种情况
                            //判断是否错过了期数，如果错过期数，将所有追踪策略归零，不再追号,也不再执行选号程序，
                            //是否要连续停几期？执行完后，在接收策略里面发现前10期有不连续的情况，直接跳过，只接收数据不执行选号。
                            //CurrDataList.UseType = dtp;
                            if (interExpect - NewList.Count > 0  || MissExpectEventPassCnt>0)//错期特征，当前期和上期的差大于新收到数据条数，表示老数据和新数据中有缺失数据，这样的特征出现后连续10期不计算，等稳定后再说，错期后如果中间期补进来，在10期内标志仍然是真，不需要特别对其处理。
                            {
                                if(interExpect > 1 )//如果是发生错期
                                    Log(string.Format("{1}接收到错期数据，其中缺失理论数据条数为{2}条，此后{0}期将不计算，期间该彩种所有信号均无效，之后自动恢复！",MaxMissEventCnt,dtp.DataType,interExpect-NewList.Count), string.Format("接收到数据！{0}", el.LastData.ToString()), true);
                                if(interExpect <=0)
                                {
                                    Log(string.Format("{0}接收到后补的错期数据！",  dtp.DataType), string.Format("接收到数据！{0}", el.LastData.ToString()), true);
                                }
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
                                if(interExpect < 0) //补充进来的错期数据
                                {
                                    Log(string.Format("经过{0}期最大平稳期后接收到补充进来的错期数据，如果是关键期，仍然对平稳期后的信号有影响，请及时检查！",MaxMissEventCnt), string.Format("接收到数据！{0}", el.LastData.ToString()), true);//只保存数据，不做处理
                                    return;
                                }
                                bool res = false;
                                if (NeedCalc)
                                {
                                    CurrData = currEl; //开始的记录以老记录为基础
                                    
                                    //2020.4.8 为保证择时处理准确，增加对新增基础数据进行分解计算，务必保证每一条合法的记录都经过计算。错期的不在此范围内，因为本来就是错的，计算也是错误
                                    for (int i = NewList.Count-1;i>=0 ; i--)//新序列是反序，要反着用  
                                    {
                                        
                                        CurrData.Add(NewList[i]);
                                        if (NewList.Count > 1)
                                        {
                                            Log("一次接收到多期数据", string.Format("最后记录{3};当前执行！{0};{1};{2}", NewList[i].Expect, NewList[i].OpenCode, NewList[i].OpenTime,CurrData.LastData.Expect), true);//只保存数据，不做处理
                                        }
                                        CurrData.UseType = dtp;
                                        //Program.AllServiceConfig.LastDataSector = new ExpectList<TimeSerialData>(CurrDataList.Table);
                                        if (CalcProcess == null)
                                            CalcProcess = new CalcService<T>();
                                        CalcProcess.DataPoint = dtp;
                                        CalcProcess.ReadDataTableName = strReadTableName;
                                        CalcProcess.Codes = codes;
                                        res = AfterReceiveProcess(CalcProcess);
                                        if (res == false)
                                            useTimer.Interval = RepeatSeconds / 20 * 1000;
                                        else
                                            Program.AllServiceConfig.haveReceiveData = true;//每次成功接收后都将已接收到数据标志置为true，当外部调用访问后将该标志置回为false
                                        /*if (CurrData.MissExpectCount() > 1)//执行完计算(关闭所有记录)后再标记为已错期
                                        {
                                            MissExpectEventPassCnt = 1;
                                        }*/
                                    }
                                }
                            }
                        }
                        else//保存失败
                        {
                            Log("待保存数据！", string.Format("总共{0}期数据:[{1}]", NewList.Count, string.Join(";", expects)), glb.ExceptNoticeFlag);
                            useTimer.Interval = RepeatSeconds / (savecnt == 0 ? 1 : 20) * 1000;//如果为0，只是没保存，不管它，只是提示，如果保存为负，继续获取。
                            //Log("保存数据错误", string.Format("保存数据数量为{0}，间隔时间为{1}秒！", savecnt, useTimer.Interval), glb.ExceptNoticeFlag);
                        }
                    }
                    else
                    {
                        //Log("接收到非最新数据", string.Format("id:{0}", el.LastData.Expect), false);
                        if (CurrTime.Hour < StartTime.Hour)//如果在9点前
                        {
                            //下一个时间点是9：07 //9:30
                            DateTime TargetTime = DateTime.Today.AddHours(StartTime.Hour).AddMinutes(StartTime.Minute);
                            useTimer.Interval = TargetTime.Subtract(CurrTime).TotalMilliseconds;
                            DateTime realTime = DateTime.Now.Add(TargetTime.Subtract(CurrTime));
                            Log("休息时间，下一个大周期时间", realTime.ToString());
                        }
                        else
                        {
                            Log(string.Format("接收到{0}数据", DataType), string.Format("未接收到数据！{0}",CurrTime.ToString()));
                            //if (NormalRecievedTime > CurrTime)
                            //{
                            //    useTimer.Interval =  NormalRecievedTime.AddMinutes(1).Subtract(CurrTime).TotalMilliseconds;
                            //}
                            //else
                            //{
                            //useTimer.Interval = RepeatSeconds / 20 * 1000;
                            if (CurrTime.Subtract(PK10_LastSignTime).TotalMinutes > RepeatMinutes + RepeatMinutes * 2 / 5)//如果离上期时间超过2/5个周期，说明数据未接收到，那不要再频繁以10秒访问服务器
                            {
                                useTimer.Interval = RepeatSeconds / 5 * 1000;
                            }
                            else //一般未接收到，10秒以后再试，改为50分之一个周期再试
                            {
                                useTimer.Interval = RepeatSeconds / 20 * 1000;
                            }
                            //}
                        }
                    }
                }
                catch (Exception e)
                {
                    Log(e.Message, e.StackTrace,true);
                }
                //Log("接收数据", string.Format("当前访问时间为：{0},{1}秒后继续访问！",CurrTime.ToString(),useTimer.Interval/1000),false);
                //FillOrgData(listView_TXFFCData, currEl);
            }
        }

        bool AfterReceiveProcess(CalcService<T> CalcObj)
        {
            //刷新客户端数据

            //////if(CalcProcess.AllowCalc)//测试用，以后需要删除
            //////{
            //////    CalcProcess.StartService();
            //////}
            //Log("转移到计算服务", "准备进行计算");
            try
            {
                //this.CalcProcess.Calc();
                CalcObj.OnFinishedCalc = onFinished;
                Log("开始计算", "启动计算",Program.gc.NormalNoticeFlag);
                CalcObj.Calc();
            }
            catch(Exception e)
            {
                Log("计算错误", string.Format("{0}：{1}",e.Message,e.StackTrace),true);
                return false;
            }
            return true;
        }

        void onFinished(DataTypePoint dtp)
        {
            Log(dtp.DataType, "计算完成");
        }
        
    }

    class self_Timer:System.Timers.Timer
    {
        public string Name { get; set; }
    }
}
