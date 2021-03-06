﻿using System;
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
using System.Threading.Tasks;
using WolfInv.com.Strags.Security;
using System.Collections.Concurrent;

namespace DataRecSvr
{
    public delegate void EventFinishedCalc(DataTypePoint dtp);
    public partial class CalcService<T> : SelfDefBaseService<T> where T :TimeSerialData
    {
        public string runErrorMsg = null;
        public string ErrorStackTrace = null;
        public KLineData<T>.getSingleDataFunc getSingleData;
        public DataTypePoint DataPoint { get; set; }
        public string[] Codes { get; set; }
        public string benchMark { get; set; }
        public string ReadDataTableName { get; set; }
        public bool AllowCalc = false;
        int FinishedThreads = 0;
        int RunThreads = 0;
        public ConcurrentDictionary<string,MongoReturnDataList<T>> allSecurityDic;
        List<Thread> ThreadPools;
        public EventFinishedCalc OnFinishedCalc;

        public Action<string, string,string> StragAfterProcessEvent;
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
                Program<T>.AllServiceConfig = TestSetting as ServiceSetting<T>;
                //Program<T>.AllServiceConfig.AllNoClosedChanceList = new Dictionary<string, ChanceClass<T>>();
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
        
        public bool Calc()
        {
            try
            {
                
                ////if (!AllowCalc) return;
                DateTime currTime = DateTime.Now;
                ////int MaxViewCnt = Program<T>.AllServiceConfig.AllRunningPlanGrps.Max(t=>t.Value.UseSPlans.ForEach(a=>Program<T>.AllServiceConfig.AllStrags[a.GUID]))));
                //ExpectList<T> el = CurrData;// Program<T>.AllServiceConfig.LastDataSector;
                //Log("数据源", string.Format("数据条数:{0};最后数据:{1}", el.Count, el.LastData.ToString()));
                //if (CurrDataList.Count < MaxViewCnt)
                //{
                //    el = new ExpectReader().ReadNewestData(MaxViewCnt);
                //}
                //Log("计算准备", "获取未关闭的机会");
                if (DataPoint.IsSecurityData == 1)
                {
                    if(allSecurityDic == null)
                        allSecurityDic = this.CurrData.MongoData;
                }
                string currExpect = CurrData.LastData.Expect;
                Dictionary<string, ChanceClass<T>> NoClosedChances = new Dictionary<string, ChanceClass<T>>();
                Dictionary<string, ChanceClass<T>> tCloseChances = null;
                NoClosedChances =  CloseTheChances(this.IsTestBack,ref tCloseChances);//关闭机会
                if(NoClosedChances == null)
                {
                    runErrorMsg = "出现获取到机会数据的异常，计算中止！";
                    ErrorStackTrace = "在读取机会数据时无法链接到数据库！";
                    return false;
                }
                if(DataPoint.IsSecurityData == 1)//单独处理ed,更新资产变动的金额
                {
                    
                    foreach(string key in NoClosedChances.Keys)
                    {

                        SecurityChance<T> cc = NoClosedChances[key] as SecurityChance<T>;
                        ////if(allData.ContainsKey(key))
                        ////{
                        ////    MongoReturnDataList<T> chanceData = allData[key];
                        ////    StockMongoData smd = (chanceData.Last() as StockMongoData);
                        ////    cc.currUnitPrice = smd.close;
                        ////    cc.preUnitPrice = smd.preclose;
                        ////}
                        BaseStragClass<T> sc = Program<T>.AllServiceConfig.AllStrags[cc.StragId];
                        StragRunPlanClass<T> splan = sc.RunningPlan as StragRunPlanClass<T>;//暂时未有任何调用。 2020-12-13
                        if (sc.AssetUnitId == null)
                        {
                            continue; //所属计划未指定资产单元，不记录交易信息
                        }
                        if (!Program<T>.AllServiceConfig.AllAssetUnits.ContainsKey(sc.AssetUnitId))
                        {
                            continue;//所属分类无记录的资产单元，不记录信息
                        }
                        if (cc.UnitCost == 0)//无交易金额，不处理
                            continue;
                        if(!allSecurityDic.ContainsKey(cc.ChanceCode))
                        {
                            continue;
                        }
                        //增加是否停牌标志
                        KLineData<T> klines = new KLineData<T>(currExpect, allSecurityDic[cc.ChanceCode]);//前复权
                        string status = klines.IsStoped()? "停牌":null;
                        if(status == null)
                        {
                            if(klines.IsDownStop(null))
                            {
                                status = "跌停";
                            }
                            else if(klines.IsUpStop(null))
                            {
                                status = "涨停";
                            }
                        }
                        AssetUnitClass<T> uu = Program<T>.AllServiceConfig.AllAssetUnits[sc.AssetUnitId];
                        //当前在买入日后第三天应该都是正确的，唯一的差距是第一天，在刷新，建立chance时把控
                        uu.getCurrExchangeServer().UpdateSecurity(new ExchangeChance<T>(uu.getCurrExchangeServer(), sc, CurrData.LastData.Expect, CurrData.LastData.Expect, cc),true,status,cc.ChipCount*(cc.currUnitPrice-cc.preUnitPrice));
                        
                    }
                    
                }
                this.FinishedThreads = 0;//完成数量置0
                RunThreads = 0;
                Dictionary<string, CalcStragGroupClass<T>> testGrps = Program<T>.AllServiceConfig.AllRunningPlanGrps as Dictionary<string, CalcStragGroupClass<T>>;
                if (!IsTestBack)
                {
                    foreach (string key in testGrps.Keys)
                    {
                        ///修改：分组不用备份，每次策略都要获取前次的状态
                        ///先剔除状态异常的计划，然后再增加新的计划
                        ///CalcStragGroupClass<T> csc = Program<T>.AllServiceConfig.AllRunningPlanGrps[key].Copy();//只用备份，允许窗体启动曾经停止的计划
                        CalcStragGroupClass<T> csc = Program<T>.AllServiceConfig.AllRunningPlanGrps[key] as CalcStragGroupClass<T>;
                        for (int i = csc.UseSPlans.Count - 1; i >= 0; i--)
                        {
                            //修改为检查计划的状态，前台程序修改的是计划表的状态
                            StragRunPlanClass<T> UsePlan = csc.UseSPlans[i];
                            if (!Program<T>.AllServiceConfig.AllRunPlannings.ContainsKey(UsePlan.GUID))
                            {
                                Log("计划表中已注销", UsePlan.Plan_Name);
                                csc.UseSPlans.RemoveAt(i);
                                csc.UseStrags.Remove(UsePlan.PlanStrag.GUID);
                                continue;
                            }
                            StragRunPlanClass<T> spc = Program<T>.AllServiceConfig.AllRunPlannings[UsePlan.GUID] as StragRunPlanClass<T>;
                            if (!spc.Running)
                            {
                                Log("计划未启动", csc.UseSPlans[i].Plan_Name);
                                csc.UseSPlans.RemoveAt(i);
                                csc.UseStrags.Remove(UsePlan.PlanStrag.GUID);
                                continue;
                            }
                            if (!InitServerClass<T>.JudgeInRunTime(currTime, spc))
                            {
                                Log("计划超时", csc.UseSPlans[i].Plan_Name);
                                csc.UseSPlans.RemoveAt(i);
                                csc.UseStrags.Remove(UsePlan.PlanStrag.GUID); //只清除计划，不清除策略？？？？？
                                continue;
                            }
                        }
                    }
                    Dictionary<string, CalcStragGroupClass<T>> allGrps = Program<T>.AllServiceConfig.AllRunningPlanGrps as Dictionary<string, CalcStragGroupClass<T>>;
                    //加入后续启动的计划
                    Program<T>.AllServiceConfig.AllRunningPlanGrps = InitServerClass<T>.InitCalcStrags(DataPoint, ref allGrps, Program<T>.AllServiceConfig.AllStrags as Dictionary<string, BaseStragClass<T>>, Program<T>.AllServiceConfig.AllRunPlannings, Program<T>.AllServiceConfig.AllAssetUnits, false, this.IsTestBack);
                    //从系统数据中取得对应的索引数据
                    allGrps.Values.ToList().ForEach(
                        grp =>
                        {
                            lock (grp)
                            {
                                if (grp.grpIndexs != null)
                                {
                                    foreach (var kv in grp.grpIndexs)
                                    {
                                        if (Program<T>.AllServiceConfig.AllStragIndexs.ContainsKey(kv.Key))
                                        {
                                            grp.grpIndexs[kv.Key] = Program<T>.AllServiceConfig.AllStragIndexs[kv.Key];
                                        }
                                    }
                                }
                            }
                        }
                        );
                }
                foreach (string key in Program<T>.AllServiceConfig.AllRunningPlanGrps.Keys)//再次为计划组分配资源，保证策略和计划一直在内存。
                {
                    CalcStragGroupClass<T> csc = Program<T>.AllServiceConfig.AllRunningPlanGrps[key] as CalcStragGroupClass<T>;
                    //增加新的计划
                    if (DataPoint.IsSecurityData == 1)
                        csc.allSecurityDic = this.allSecurityDic;//准备好股票数据
                    SettingClass comSetting = new SettingClass();
                    comSetting.SetGlobalSetting(Program<T>.AllServiceConfig.gc);
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
                    csc.afterStragProcessed = StragAfterProcessEvent;
                    csc.getSingleData = getSingleData;//增加内部访问外部数据的功能
                    csc.Finished = new CalcStragGroupDelegate(CheckFinished);
                    csc.GetNoClosedChances = new ReturnChances<T>(FillNoClosedChances);
                    csc.GetAllStdDevList = new ReturnStdDevList(FillAllStdDev);
                    //Program<T>.AllServiceConfig.AllRunningPlanGrps[key] = csc;//要加吗？不知道，应该要加
                }
                if (allSecurityDic.ContainsKey(benchMark))
                {
                    foreach (var unit in Program<T>.AllServiceConfig.AllAssetUnits.Values)
                    {
                        unit.getCurrExchangeServer().updateBenchVal((allSecurityDic[benchMark].Last() as StockMongoData).close);
                    }
                }
                //分配完未关闭的数据后，将全局内存中所有未关闭机会列表清除
                Program<T>.AllServiceConfig.AllNoClosedChanceList = new Dictionary<string, ChanceClass<T>>() as Dictionary<string, ChanceClass<T>>;
                this.FinishedThreads = 0;
                List<Task> tasks = new List<Task>(); 
                foreach (string key in Program<T>.AllServiceConfig.AllRunningPlanGrps.Keys)//再次为计划组分配资源，保证策略和计划一直在内存。
                {
                    //Log("计算组执行处理", key, Program<T>.gc.NormalNoticeFlag);
                    CalcStragGroupClass<T> csc = Program<T>.AllServiceConfig.AllRunningPlanGrps[key] as CalcStragGroupClass<T>;
                    //if (!IsTestBack &&  !csc.Running)
                    //    continue;
                    csc.IsBackTest = IsTestBack;
                    
                    ThreadPool.QueueUserWorkItem(new WaitCallback(csc.Run), CurrData);
                    //Task task = Task.Factory.StartNew(csc.Run, el);
                    //tasks.Add(task);
                }
                
                //Task.WaitAll(tasks.ToArray());
            }
            catch(Exception ce)
            {
                //强制设置完成线程等于线程数，再检查是否完成，保证最后更新文件标志
                FinishedThreads = RunThreads;
                CheckFinished();
                Log(ce.Message, ce.StackTrace, !IsTestBack);
                runErrorMsg = ce.Message;
                ErrorStackTrace = ce.StackTrace;
                return false;
            }
            finally
            {
                allSecurityDic = null;
                CurrData = null;
            }
            return true;
        }
        
        void FillNoClosedChances(Dictionary<string, ChanceClass<T>> list)
        {
            lock (Program<T>.AllServiceConfig.AllNoClosedChanceList)
            {
                list.Values.ToList<ChanceClass<T>>().ForEach(p =>
                {
                    ChanceClass<T> cc = p as ChanceClass<T>;
                    if(!Program<T>.AllServiceConfig.AllNoClosedChanceList.ContainsKey(p.GUID))
                        Program<T>.AllServiceConfig.AllNoClosedChanceList.Add(p.GUID, cc);
                });
            }
        }

        void FillAllStdDev(Dictionary<string, List<double>> list)
        {
            try
            {
                if (list == null || list.Count == 0) return;
                lock (Program<T>.AllServiceConfig.AllTotalStdDevList)
                {
                    Program<T>.AllServiceConfig.AllTotalStdDevList = list;
                    SystemStdDevList = list;//提供给回测用
                }
            }
            catch
            {

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
                    Task.Factory.StartNew(()=> { OnFinishedCalc(DataPoint); });
                    ThreadPools = new List<Thread>();
                    if (IsTestBack)
                        return true; //如果是回测，不做处理
                    Log("写入标志文件", "供web程序读取！");
                    string expectCode = CurrData.LastData.Expect;
                    DataReader<T> rder = DataReaderBuild.CreateReader<T>(DataPoint.DataType, ReadDataTableName, Codes);
                    string NewExpectNo = DataReader<T>.getNextExpectNo(expectCode,DataPoint);
                    string NewNo = string.Format("{0}|{1}|{2}|{3}", NewExpectNo, CurrData.LastData.OpenTime, CurrData.LastData.OpenCode,expectCode);
                    rder.updateExpectInfo(DataPoint.DataType, NewExpectNo, expectCode,CurrData.LastData.OpenCode,CurrData.LastData.OpenTime.ToString());
                    new LogInfo().WriteFile(NewNo, path, strExpectNo, strtype, true, true);

                    //保存策略
                    GlobalClass.SaveStragList(BaseStragClass<TimeSerialData>.getXmlByObjectList<BaseStragClass<T>>(Program<T>.AllServiceConfig.AllStrags.Values.ToList<BaseStragClass<T>>()));
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
            if (Program<T>.AllServiceConfig.AllStrags == null)
            {
                Log("开始服务", "策略列表异常！");
                return;
            }
            if (Program<T>.AllServiceConfig.AllRunPlannings.Count == 0)
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

        public Dictionary<string, ChanceClass<T>> CloseTheChances(bool IsTestBack,ref Dictionary<string, ChanceClass<T>> CloseList, bool ForceCloseAllData = false)
        {
            List<ChanceClass<T>> cl = new List<ChanceClass<T>>();
            DateTime currTime = DateTime.Now;
            CloseList = new Dictionary<string, ChanceClass<T>>();
            
            if (IsTestBack)//如果回测，使用内存数据
            {
                cl = Program<T>.AllServiceConfig.AllNoClosedChanceList.Values.Select(a => a).ToList() as List<ChanceClass<T>>;
            }
            else//非回测，使用数据库数据ReadDataTableName
            {
                //DbChanceList<T> dcl = new PK10ExpectReader().getNoCloseChances<T>(null);
                DbChanceList<T> dcl = DataReaderBuild.CreateReader<T>(DataPoint.DataType, ReadDataTableName, Codes).getNoCloseChances(null);
                int MaxRepeateCnt = 5;
                int Rcnt = 0;
                while (dcl == null)
                {
                    if(Rcnt>MaxRepeateCnt)
                    {
                        Log("无法访问到机会数据", string.Format("连续{0}次未能读取到机会数据", MaxRepeateCnt),true);
                        return null;
                        //break;
                        //throw new Exception("无法获取到机会数据！");
                    }
                    Rcnt++;
                    Log("无法访问到机会数据", string.Format("继续尝试访问第{0}次",Rcnt),true);
                    Thread.Sleep(1000);
                    dcl = DataReaderBuild.CreateReader<T>(DataPoint.DataType, ReadDataTableName, Codes).getNoCloseChances(null);
                    
                }
                //必须强制按数据库中存储的策略id,获取到策略的机会类型
                foreach (var dc in dcl.Values)
                {
                    if(!Program<T>.AllServiceConfig.AllStrags.ContainsKey(dc.StragId))//策略未找到数据库中存储的机会所产生的策略
                    {
                        continue;
                    }
                    if (DataPoint.DataType == "PK10")
                    {
                        cl.Add(dc);
                    }
                    else
                    {
                        BaseStragClass<T> sc = Program<T>.AllServiceConfig.AllStrags[dc.StragId];
                        sc.setDataTypePoint(DataPoint);
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
                        //Log("转换前机会类型", string.Format("{0}", tmp.GetType()));
                        dc.FillTo(ref tmp, true);//获取所有属性
                        ChanceClass<T> cc = tmp as ChanceClass<T>;
                        //Log("转换后机会类型", string.Format("{0}:{1}:{2}", cc.GetType(), cc.StragId, cc.ChanceCode));// cc.ToDetailString()));
                        cl.Add(cc);
                    }
                    //cl = dcl.Values.ToList();
                }
            }

            Dictionary<string, ChanceClass<T>> rl = new Dictionary<string, ChanceClass<T>>();
            if(cl.Count>0 && !IsTestBack)
                Log("未关闭机会列表数量为", string.Format("{0}",cl.Count));
            //2019/4/22日出现错过732497，732496两期记录错过，但是732498却收到的情况，同时，正好在732498多次重复策略正好开出结束，因错过2期导致一直未归零，
            //一直长时间追号开出近60期
            //为避免出现这种情况
            //判断是否错过了期数，如果错过期数，将所有追踪策略归零，不再追号,也不再执行选号程序，
            //是否要连续停几期？执行完后，在接收策略里面发现前10期有不连续的情况，直接跳过，只接收数据不执行选号。
            this.CurrData.UseType = this.DataPoint;
            if (this.DataPoint.DataType == "PK10" &&  this.CurrData.MissExpectCount() > 1)
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
                if (sGUId == null || !Program<T>.AllServiceConfig.AllStrags.ContainsKey(sGUId))//如果策略已注销，立即关闭机会
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

                List<string> AllUsePans = Program<T>.AllServiceConfig.AllRunningPlanGrps.SelectMany(a => a.Value.UseSPlans.Select(t => t.PlanStrag.GUID)).ToList<string>();
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
                List<string> StopedPlans = Program<T>.AllServiceConfig.AllRunningPlanGrps.SelectMany(a => a.Value.UseSPlans.Where(t => (InitServerClass<T>.JudgeInRunTime(currTime, t) == false))).Select(a => a.GUID).ToList<string>();
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
                StopedPlans = Program<T>.AllServiceConfig.AllRunningPlanGrps.SelectMany(a => a.Value.UseSPlans.Where(t => (t.Running == false))).Select(a => a.GUID).ToList<string>();
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
                BaseStragClass<T> sc = Program<T>.AllServiceConfig.AllStrags[sGUId] as BaseStragClass<T>;
                if (DataPoint.IsSecurityData == 1)
                {
                    (sc as BaseSecurityStragClass<T>).setSingleDataFunc(getSingleData);//为策略增加穿透访问外部数据的功能
                }
                sc.SetLastUserData(CurrData);//为检查是否需要关闭机会，必须填充入数据才能进行。
                int mcnt = 0;
                bool Matched = cl[i].Matched(CurrData, out mcnt);
                cl[i].MatchChips += mcnt;
                if (sc is ITraceChance<T>)//优先关闭程序跟踪
                {
                    if (DataPoint.IsSecurityData == 1)
                    {
                        (sc as BaseSecurityStragClass<T>).FillSecDic(allSecurityDic);
                    }
                    sc.SetLastUserData(CurrData);
                    cl[i].Closed = (sc as ITraceChance<T>).CheckNeedEndTheChance(cl[i], Matched,DataPoint.IsSecurityData==1);
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
                if(cl[i].needEndChance)//如果是需要关闭并且是跌停状态,其后任何时刻都需要关闭，
                {
                    cl[i].Closed = true;
                }
                if (cl[i].Closed)//如果已经关闭
                {
                    cl[i].needEndChance = true;//此机会需要关闭了，此条件一直保存直至关闭
                    if (!cl[i].currCantEndChance) //如果不是跌停，关闭
                    {
                        CloseList.Add(cl[i].GUID, cl[i]);
                        continue;
                    }
                    else
                    {

                    }
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
            else
            {
                if(DataPoint.IsSecurityData==1)
                {
                    CloseList.Values.ToList().ForEach(cc=> {
                        
                        BaseStragClass<T> sc = Program<T>.AllServiceConfig.AllStrags[cc.StragId] as BaseStragClass<T>;
                        CalcStragGroupClass<T> plan = Program<T>.AllServiceConfig.AllRunningPlanGrps.Where(a=>a.Value.UseStrags.ContainsKey(cc.StragId)).First().Value;
                        string strExpectNo = CurrData.LastData.Expect;
                        cc.EndExpectNo = strExpectNo;
                        AssetUnitClass<T> ass = plan.UseAssetUnits.First().Value;
                        ExchangeService<T> es = ass.getCurrExchangeServer();
                        if (es != null)
                        {
                            ExchangeChance<T> ec = new ExchangeChance<T>(es, sc, strExpectNo, strExpectNo, cc);
                            es.UpdateSecurity(ec,false,"清仓");
                        }
                    });
                }
            }
            return rl;
        }

        public void CloseChanceInDBAndExchangeService(DbChanceList<T> NeedClose,bool ForceClose= false)
        {
            MongoDataDictionary<T> allDatas = null;
            if(DataPoint.IsSecurityData == 1)
            {
                allDatas = new MongoDataDictionary<T>(CurrData,DataPoint.IsSecurityData==1);
            }
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
                if (DataPoint.IsSecurityData == 1)
                {
                    SecurityChance<T> cc = NeedClose[id] as SecurityChance<T>;
                    if(allDatas == null || !allDatas.ContainsKey(cc.ChanceCode))
                    {
                        continue;
                    }
                    double lastPrice = (allDatas[cc.ChanceCode].Last() as StockMongoData).close;
                    cc.CalcProfit(lastPrice);
                }
                else
                {
                    NeedClose[id].CalcProfit(NeedClose[id].MatchChips);
                }
                NeedClose[id].UpdateTime = DateTime.Now;
                
            }
            //int ret = NeedClose.Save(null);
            if (NeedClose != null && NeedClose.Count > 0)
            {
                //int ret = new PK10ExpectReader().SaveChances(NeedClose.Values.ToList<ChanceClass<T>>(), null);
                int ret = DataReaderBuild.CreateReader<T>(DataPoint.DataType,ReadDataTableName,Codes).SaveChances(NeedClose.Values.ToList<ChanceClass<T>>(), null);
                if (NeedClose.Count > 0)
                    Log("保存关闭的机会", string.Format("数量:{0}", ret));
            }
        }

        public bool CalcFinished
        {
            get
            {
                if (this.RunThreads > 0 && this.FinishedThreads >= this.RunThreads)
                {
                    return true;
                }
                return false;
            }
        }
    }

}
