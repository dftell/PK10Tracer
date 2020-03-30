using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
//using WolfInv.com.PK10CorePress;
using WolfInv.com.Strags;
using WolfInv.com.LogLib;
using System.Threading;
using WolfInv.com.ServerInitLib;
using WolfInv.com.ExchangeLib;
using WolfInv.com.GuideLib;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.SecurityLib;
using WolfInv.com.PK10CorePress;

namespace DataRecSvr
{
    public delegate void EventFinishedCalc(DataTypePoint dtp);
    public partial class CalcService<T> : SelfDefBaseService<T> where T :TimeSerialData
    {
        public DataTypePoint DataPoint { get; set; }
        public string[] Codes { get; set; }
        public string ReadDataTableName { get; set; }
        public bool AllowCalc = false;
        int FinishedThreads = 0;
        int RunThreads = 0;
        List<Thread> ThreadPools;
        public EventFinishedCalc OnFinishedCalc;
        public bool IsTestBack { get; set; }
        /// <summary>
        /// 提供给回测用
        /// </summary>
        Dictionary<string, List<double>> SystemStdDevList;
        public DataTable getSystemStdDevList()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Expect",typeof(string));
            dt.Columns.Add("StdDev",typeof(double));
            dt.Columns.Add("StdMa20", typeof(double));
            dt.Columns.Add("StdMa5", typeof(double));
            for (int i = 0; i < 10; i++)
            {
                string strCol = string.Format("{0}", (i + 1) % 10);
                dt.Columns.Add(strCol, typeof(double));
            }
            if (SystemStdDevList == null)
            {
                return dt;
            }
            MA_del ma = new MA_del(SystemStdDevList.Values.Select(p=>p[10]).ToArray(), 20);
            MA_del ma5 = new MA_del(SystemStdDevList.Values.Select(p => p[10]).ToArray(), 5);
            double[] mavals = ma.LastValues[0];
            int c = 0;
            foreach (string key in SystemStdDevList.Keys)
            {
                //dt.Rows.Add(new object[] { key, });
                DataRow dr = dt.NewRow();
                dr[0] = key;
                List<double> list = SystemStdDevList[key];
                dr[1] = list[10];
                dr[2] = mavals[c];
                dr[3] = ma5.LastValues[0][c];
                for (int i = 0; i < 10; i++)
                {
                    string strCol = string.Format("{0}", (i + 1) % 10);
                    dr[strCol] = list[i];
                }
                dt.Rows.Add(dr);
                c++;

            }
            return dt;
        }
        /// <summary>
        /// 专为回测设置的构造函数
        /// </summary>
        /// <param name="backTest"></param>
        /// <param name="TestSetting"></param>
        public CalcService(bool backTest, ServiceSetting<T> TestSetting,Dictionary<string, StragRunPlanClass<T>> plans)
        {
            InitializeComponent();
            ThreadPools = new List<Thread>();
            this.ServiceName = "计算服务";
            IsTestBack = backTest;
            if (IsTestBack)
            {
                Program.AllServiceConfig = TestSetting as ServiceSetting<TimeSerialData>;
                //Program.AllServiceConfig.AllNoClosedChanceList = new Dictionary<string, ChanceClass<T>>();
                TestBcakPlans = plans;
            }
        }

        public Dictionary<string, StragRunPlanClass<T>> TestBcakPlans { get; set; }
        
        public CalcService()
        {
            InitializeComponent();
            this.ServiceName = "计算服务";
            ThreadPools = new List<Thread>();
            try
            {
                Log("策略运行计划初始化", "服务成功！");
            }
            catch (Exception e)
            {
                Log("策略运行计划初始化失败", e.Message);
            }
            
        }

        public void StopCalc()
        {
            //ThreadPool.stop
        }
        
        public void Calc()
        {
            try
            {
                ////if (!AllowCalc) return;
                DateTime currTime = DateTime.Now;
                ////int MaxViewCnt = Program.AllServiceConfig.AllRunningPlanGrps.Max(t=>t.Value.UseSPlans.ForEach(a=>Program.AllServiceConfig.AllStrags[a.GUID]))));
                ExpectList<T> el = CurrData;// Program.AllServiceConfig.LastDataSector;
                                            //Log("数据源", string.Format("数据条数:{0};最后数据:{1}", el.Count, el.LastData.ToString()));
                                            //if (CurrDataList.Count < MaxViewCnt)
                                            //{
                                            //    el = new ExpectReader().ReadNewestData(MaxViewCnt);
                                            //}
                                            //Log("计算准备", "获取未关闭的机会");
                Dictionary<string, ChanceClass<T>> NoClosedChances = new Dictionary<string, ChanceClass<T>>();
                NoClosedChances = CloseTheChances(this.IsTestBack);//关闭机会
                this.FinishedThreads = 0;//完成数量置0
                RunThreads = 0;

                Dictionary<string, CalcStragGroupClass<T>> testGrps = Program.AllServiceConfig.AllRunningPlanGrps as Dictionary<string, CalcStragGroupClass<T>>;
                if (!IsTestBack)
                {
                    foreach (string key in testGrps.Keys)
                    {
                        ///修改：分组不用备份，每次策略都要获取前次的状态
                        ///先剔除状态异常的计划，然后再增加新的计划
                        ///CalcStragGroupClass<T> csc = Program.AllServiceConfig.AllRunningPlanGrps[key].Copy();//只用备份，允许窗体启动曾经停止的计划
                        CalcStragGroupClass<T> csc = Program.AllServiceConfig.AllRunningPlanGrps[key] as CalcStragGroupClass<T>;
                        for (int i = csc.UseSPlans.Count - 1; i >= 0; i--)
                        {
                            //修改为检查计划的状态，前台程序修改的是计划表的状态
                            StragRunPlanClass<T> UsePlan = csc.UseSPlans[i];
                            if (!Program.AllServiceConfig.AllRunPlannings.ContainsKey(UsePlan.GUID))
                            {
                                Log("计划表中已注销", UsePlan.Plan_Name);
                                csc.UseSPlans.RemoveAt(i);
                                csc.UseStrags.Remove(UsePlan.PlanStrag.GUID);
                                continue;
                            }
                            StragRunPlanClass<T> spc = Program.AllServiceConfig.AllRunPlannings[UsePlan.GUID] as StragRunPlanClass<T>;
                            if (!spc.Running)
                            {
                                Log("计划未启动", csc.UseSPlans[i].Plan_Name);
                                csc.UseSPlans.RemoveAt(i);
                                csc.UseStrags.Remove(UsePlan.PlanStrag.GUID);
                                continue;
                            }
                            if (!InitServerClass.JudgeInRunTime(currTime, spc))
                            {
                                Log("计划超时", csc.UseSPlans[i].Plan_Name);
                                csc.UseSPlans.RemoveAt(i);
                                csc.UseStrags.Remove(UsePlan.PlanStrag.GUID); //只清除计划，不清除策略？？？？？
                                continue;
                            }
                        }
                    }
                    Dictionary<string, CalcStragGroupClass<TimeSerialData>> allGrps = Program.AllServiceConfig.AllRunningPlanGrps as Dictionary<string, CalcStragGroupClass<TimeSerialData>>;
                    //加入后续启动的计划
                    Program.AllServiceConfig.AllRunningPlanGrps = InitServerClass.InitCalcStrags<TimeSerialData>(DataPoint, ref allGrps, Program.AllServiceConfig.AllStrags as Dictionary<string, BaseStragClass<TimeSerialData>>, Program.AllServiceConfig.AllRunPlannings, Program.AllServiceConfig.AllAssetUnits, false, this.IsTestBack);
                }
                foreach (string key in Program.AllServiceConfig.AllRunningPlanGrps.Keys)//再次为计划组分配资源，保证策略和计划一直在内存。
                {
                    CalcStragGroupClass<T> csc = Program.AllServiceConfig.AllRunningPlanGrps[key] as CalcStragGroupClass<T>;
                    //增加新的计划

                    SettingClass comSetting = new SettingClass();
                    comSetting.SetGlobalSetting(Program.AllServiceConfig.gc);
                    List<ChanceClass<T>> NoClsList = NoClosedChances.Values.ToList();
                    //NoClsList.ForEach(a=>Log(a.ChanceCode,a.ToDetailString()));
                    //Log("未关闭机会数量", NoClosedChances.Count.ToString());
                    //NoClsList.ForEach(a => Log("策略：", a.StragId));
                    if (!csc.initRunningData(comSetting, NoClsList))
                    {
                        Log("为计划组分配通用参数", "配置计划组的通用参数失败,该组计划暂时不参与运算！");
                        continue;
                    }
                    RunThreads++;
                    csc.Finished = new CalcStragGroupDelegate(CheckFinished);
                    csc.GetNoClosedChances = new ReturnChances<T>(FillNoClosedChances);
                    csc.GetAllStdDevList = new ReturnStdDevList(FillAllStdDev);
                    //Program.AllServiceConfig.AllRunningPlanGrps[key] = csc;//要加吗？不知道，应该要加
                }
                //分配完未关闭的数据后，将全局内存中所有未关闭机会列表清除
                Program.AllServiceConfig.AllNoClosedChanceList = new Dictionary<string, ChanceClass<T>>() as Dictionary<string, ChanceClass<TimeSerialData>>;
                this.FinishedThreads = 0;
                foreach (string key in Program.AllServiceConfig.AllRunningPlanGrps.Keys)//再次为计划组分配资源，保证策略和计划一直在内存。
                {
                    CalcStragGroupClass<T> csc = Program.AllServiceConfig.AllRunningPlanGrps[key] as CalcStragGroupClass<T>;
                    //if (!IsTestBack &&  !csc.Running)
                    //    continue;
                    csc.IsBackTest = IsTestBack;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(csc.Run), el);
                }
            }
            catch(Exception ce)
            {
                Log(ce.Message, ce.StackTrace, true);
            }
        }
        
        void FillNoClosedChances(Dictionary<string, ChanceClass<T>> list)
        {
            lock (Program.AllServiceConfig.AllNoClosedChanceList)
            {
                list.Values.ToList<ChanceClass<T>>().ForEach(p =>
                {
                    ChanceClass<TimeSerialData> cc = p as ChanceClass<TimeSerialData>;
                    Program.AllServiceConfig.AllNoClosedChanceList.Add(p.GUID, cc);
                });
            }
        }

        void FillAllStdDev(Dictionary<string, List<double>> list)
        {
            if (list == null || list.Count == 0) return;
            lock (Program.AllServiceConfig.AllTotalStdDevList)
            {
                Program.AllServiceConfig.AllTotalStdDevList = list;
                SystemStdDevList = list;//提供给回测用
            }
        }

        bool CheckFinished()
        {
            try
            {
                string path = @"c:\inetpub\wwwroot\PK10\InstData\" + DataPoint.DataType;
                string strExpectNo = "expectNo";
                string strResult = "record";
                string strForApp = "expertNoForApp";
                string strtype = "txt";
                this.FinishedThreads++;
                //if (IsTestBack) return true; //如果是回测，不做处理
                //Log("进程结束", string.Format("目标{1}，现有{0}",this.FinishedThreads,this.RunThreads));

                if (FinishedThreads == RunThreads)
                {
                    OnFinishedCalc(DataPoint);
                    ThreadPools = new List<Thread>();
                    if (IsTestBack)
                        return true; //如果是回测，不做处理
                    Log("写入标志文件", "供web程序读取！");
                    DataReader rder = DataReaderBuild.CreateReader(DataPoint.DataType, ReadDataTableName, Codes);
                    string NewExpectNo = rder.getNextExpectNo(Program.AllServiceConfig.LastDataSector.LastData.Expect);
                    string NewNo = string.Format("{0}|{1}", NewExpectNo, Program.AllServiceConfig.LastDataSector.LastData.OpenTime);
                    rder.updateExpectInfo(DataPoint.DataType, NewExpectNo, Program.AllServiceConfig.LastDataSector.LastData.Expect);
                    new LogInfo().WriteFile(NewNo, path, strExpectNo, strtype, true, true);

                    //保存策略
                    GlobalClass.SaveStragList(BaseStragClass<TimeSerialData>.getXmlByObjectList<BaseStragClass<TimeSerialData>>(Program.AllServiceConfig.AllStrags.Values.ToList<BaseStragClass<TimeSerialData>>()));
                    Log("保存策略清单", "保存成功");
                }
            }
            catch(Exception ce)
            {

            }
            return true;
        }

        protected override void OnStart(string[] args)
        {
            
            // TODO: 在此处添加代码以启动服务。
            StartService();
        }

        public void StartService()
        {
            //暂时不会进入改过程
            if (Program.AllServiceConfig.AllStrags == null)
            {
                Log("开始服务", "策略列表异常！");
                return;
            }
            if (Program.AllServiceConfig.AllRunPlannings.Count == 0)
            {
                this.Log("开始服务", "策略为空，无法计算！");
                return;
            }
            //AllStatusStrags = new Dictionary<string,CalcStragClass>();

            AllowCalc = true;
            Log("开始服务", "成功开始服务！");
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            AllowCalc = false;
            //this.Tm.Enabled = false;
        }

        public Dictionary<string, ChanceClass<T>> CloseTheChances(bool IsTestBack,bool ForceCloseAllData=false)
        {
            List<ChanceClass<T>> cl = new List<ChanceClass<T>>();
            DateTime currTime = DateTime.Now;
            Dictionary<string, ChanceClass<T>> CloseList = new Dictionary<string, ChanceClass<T>>();
            
            if (IsTestBack)//如果回测，使用内存数据
            {
                cl = Program.AllServiceConfig.AllNoClosedChanceList.Values.Select(a => a).ToList() as List<ChanceClass<T>>;
            }
            else//非回测，使用数据库数据ReadDataTableName
            {
                //DbChanceList<T> dcl = new PK10ExpectReader().getNoCloseChances<T>(null);
                DbChanceList<T> dcl = DataReaderBuild.CreateReader(DataPoint.DataType, ReadDataTableName, Codes).getNoCloseChances<T>(null);
                //必须强制按数据库中存储的策略id,获取到策略的机会类型
                foreach (var dc in dcl.Values)
                {
                    if(!Program.AllServiceConfig.AllStrags.ContainsKey(dc.StragId))//策略未找到数据库中存储的机会所产生的策略
                    {
                        continue;
                    }
                    if (DataPoint.DataType == "PK10")
                    {
                        cl.Add(dc);
                    }
                    else
                    {
                        BaseStragClass<TimeSerialData> sc = Program.AllServiceConfig.AllStrags[dc.StragId];
                        Type ct = sc.getTheChanceType();
                        
                        object tmp = Activator.CreateInstance(ct);
                        if (DataPoint.IsXxY == 1)
                        {
                            (tmp as iXxYClass).AllNums = DataPoint.AllNums;
                            (tmp as iXxYClass).SelectNums = DataPoint.SelectNums;
                            (tmp as iXxYClass).strAllTypeBaseOdds = DataPoint.strAllTypeOdds;
                            (tmp as iXxYClass).strCombinTypeBaseOdds = DataPoint.strCombinTypeOdds;
                            (tmp as iXxYClass).strPermutTypeBaseOdds = DataPoint.strPermutTypeOdds;
                        }
                        Log("转换前机会类型", string.Format("{0}", tmp.GetType()));
                        dc.FillTo(ref tmp, true);//获取所有属性
                        ChanceClass<T> cc = tmp as ChanceClass<T>;
                        Log("转换后机会类型", string.Format("{0}:{1}:{2}", cc.GetType(), cc.StragId, cc.ChanceCode));// cc.ToDetailString()));
                        cl.Add(cc);
                    }
                    //cl = dcl.Values.ToList();
                }
            }

            Dictionary<string, ChanceClass<T>> rl = new Dictionary<string, ChanceClass<T>>();
            if(cl.Count>0)
                Log("未关闭机会列表数量为", string.Format("{0}",cl.Count));
            //2019/4/22日出现错过732497，732496两期记录错过，但是732498却收到的情况，同时，正好在732498多次重复策略正好开出结束，因错过2期导致一直未归零，
            //一直长时间追号开出近60期
            //为避免出现这种情况
            //判断是否错过了期数，如果错过期数，将所有追踪策略归零，不再追号,也不再执行选号程序，
            //是否要连续停几期？执行完后，在接收策略里面发现前10期有不连续的情况，直接跳过，只接收数据不执行选号。
            this.CurrData.UseType = this.DataPoint;
            if (this.CurrData.MissExpectCount() > 1)
            {
                if (!IsTestBack) //如果非回测，保存交易记录
                {
                    DbChanceList<T> dbsavelist = new DbChanceList<T>();
                    cl.ForEach(p => dbsavelist.Add(p.ChanceIndex, p));
                    CloseChanceInDBAndExchangeService(dbsavelist, true);//强制关闭，就算命中也不算其盈利
                }
               
                return rl;
            }


            for (int i = 0; i < cl.Count;i++)
            {
                string sGUId = cl[i].StragId;
                if (sGUId == null || !Program.AllServiceConfig.AllStrags.ContainsKey(sGUId))//如果策略已注销，立即关闭机会
                {
                    if (cl[i].Tracerable)//如果是自我跟踪机会，不理会，让它自己去跟踪
                    {
                    }
                    else
                    {
                        CloseList.Add(cl[i].GUID, cl[i]);
                        //cl.Remove(cl[i].ChanceIndex);
                        Log("强制结束机会", string.Format("该机会所属策略已经注销，并且该机会是非跟踪机会！{0}", cl[i].ChanceCode));
                        continue;
                    }
                }

                List<string> AllUsePans = Program.AllServiceConfig.AllRunningPlanGrps.SelectMany(a => a.Value.UseSPlans.Select(t => t.PlanStrag.GUID)).ToList<string>();
                if (!AllUsePans.Contains(sGUId))//不在执行的计划内
                {
                    if (cl[i].Tracerable)
                    {
                    }
                    else
                    {
                        CloseList.Add(cl[i].GUID, cl[i]);
                        Log("强制结束机会", string.Format("不存在绑定该机会所属策略的计划，并且该机会是非跟踪机会！{0}", cl[i].ChanceCode));
                        continue;
                    }
                }
                //如果策略已经超出时间
                List<string> StopedPlans = Program.AllServiceConfig.AllRunningPlanGrps.SelectMany(a => a.Value.UseSPlans.Where(t => (InitServerClass.JudgeInRunTime(currTime, t) == false))).Select(a => a.GUID).ToList<string>();
                if (!IsTestBack)
                {
                    if (StopedPlans.Contains(sGUId))//停止的计划产生的机会
                    {
                        if (cl[i].Tracerable)
                        {
                        }
                        else
                        {
                            Log("强制结束机会", string.Format("该机会所属策略超出计划运行时间！{0}", cl[i].ChanceCode));
                            CloseList.Add(cl[i].GUID, cl[i]);
                            continue;
                        }
                    }
                }
                StopedPlans = Program.AllServiceConfig.AllRunningPlanGrps.SelectMany(a => a.Value.UseSPlans.Where(t => (t.Running == false))).Select(a => a.GUID).ToList<string>();
                if (StopedPlans.Contains(sGUId))//停止的计划产生的机会
                {
                    if (cl[i].Tracerable)
                    {
                    }
                    else
                    {
                        Log("强制结束机会", string.Format("该机会所属策略的计划状态为停止！{0}", cl[i].ChanceCode));
                        CloseList.Add(cl[i].GUID, cl[i]);
                        continue;
                    }
                }
                //如果策略已经停止
                //获得策略
                BaseStragClass<T> sc = Program.AllServiceConfig.AllStrags[sGUId] as BaseStragClass<T>;
                int mcnt = 0;
                bool Matched = cl[i].Matched(CurrDataList, out mcnt);
                cl[i].MatchChips += mcnt;
                if (sc is ITraceChance)//优先关闭程序跟踪
                {
                    cl[i].Closed = (sc as ITraceChance).CheckNeedEndTheChance(cl[i], Matched);
                }
                else
                {
                    cl[i].Closed = Matched;
                }
                if (cl[i].Tracerable && Matched)//如果是策略自我跟踪，无论其策略是否是跟踪策略，先关了。
                {
                    cl[i].Closed = true;
                }
                //////if (cl[i].MaxHoldTimeCnt > 0 && cl[i].HoldTimeCnt == cl[i].MaxHoldTimeCnt)
                //////{
                //////    cl[i].Closed = true;
                //////}
                if (cl[i].Closed)//如果已经关闭
                {
                    CloseList.Add(cl[i].GUID, cl[i]);
                    continue;
                }
                rl.Add(cl[i].GUID, cl[i]);
            }
            //Log("结束机会", "所有非法，以及命中并确认需要结束的机会");
            if (!IsTestBack) //如果非回测，保存交易记录
            {
                DbChanceList<T> dbsavelist = new DbChanceList<T>();
                CloseList.Values.ToList<ChanceClass<T>>().ForEach(p =>
                {
                    if (p != null)
                    {
                        dbsavelist.Add(p.ChanceIndex, p);
                    }
                });
                CloseChanceInDBAndExchangeService(dbsavelist);
            }
            return rl;
        }

        public void CloseChanceInDBAndExchangeService(DbChanceList<T> NeedClose,bool ForceClose= false)
        {

            if (NeedClose == null || NeedClose.Count == 0)
            {
                return;
            }
            foreach (Int64 id in NeedClose.Keys)
            {
                NeedClose[id].IsEnd = 1;
                if (ForceClose)
                {
                    NeedClose[id].MatchChips = 0;
                }
                ////NeedClose[id].Gained = NeedClose[id].MatchChips * NeedClose[id].Odds * NeedClose[id].UnitCost;
                ////NeedClose[id].Profit = NeedClose[id].Gained - NeedClose[id].Cost;
                NeedClose[id].CalcProfit(NeedClose[id].MatchChips);
                NeedClose[id].UpdateTime = DateTime.Now;
                
            }
            //int ret = NeedClose.Save(null);
            if (NeedClose != null && NeedClose.Count > 0)
            {
                //int ret = new PK10ExpectReader().SaveChances(NeedClose.Values.ToList<ChanceClass<T>>(), null);
                int ret = DataReaderBuild.CreateReader(DataPoint.DataType,ReadDataTableName,Codes).SaveChances(NeedClose.Values.ToList<ChanceClass<T>>(), null);
                if (NeedClose.Count > 0)
                    Log("保存关闭的机会", string.Format("数量:{0}", ret));
            }
        }

        public bool CalcFinished
        {
            get
            {
                if (this.RunThreads > 0 && this.FinishedThreads == this.RunThreads)
                {
                    return true;
                }
                return false;
            }
        }
    }

}
