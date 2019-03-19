using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Timers;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.PK10CorePress;
namespace DataRecSvr
{
    public partial class SubscriptData : SelfDefBaseService
    {
        Timer SaveTimer = new Timer();
        Dictionary<string, DataTypePoint> DataTypes;
        List<Timer_Ex> Tm_ForSubScripts;
        Dictionary<string, bool> NeedSubscriptEquitList;
        public Dictionary<string, ExpectList> AllHy;

        public SubscriptData()
        {
            InitializeComponent();
            this.ServiceName = "接收订阅数据服务";
            DataTypes = new Dictionary<string, DataTypePoint>();
            int index = 0;
            AllHy = new Dictionary<string, ExpectList>();
            if (GlobalClass.TypeDataPoints.Where(p =>  p.Value.SubScriptModel==1).Count() == 0) return;//如果订阅模式为1的数量为0，啥都不干
            foreach (string key in GlobalClass.TypeDataPoints.Keys)
            {
                DataTypePoint dtp = GlobalClass.TypeDataPoints[key];
                if(dtp.SubScriptModel == 1)//订阅模式
                {
                    DataTypes.Add(key, dtp);
                    Timer_Ex timer = new Timer_Ex(index,key);
                    timer.AutoReset = true;
                    timer.Interval = dtp.ReceiveSeconds * 1000;
                    timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
                    timer.Enabled = false;
                    if (Tm_ForSubScripts == null) Tm_ForSubScripts = new List<Timer_Ex>();
                    Tm_ForSubScripts.Add(timer);
                    index++;
                }
            }
            try
            {
                SaveTimer.Interval = DataTypes.Where(p => p.Value.SaveInterVal > 0).First().Value.SaveInterVal;//第一个设置了SaveInterVal的对象的值，没有会报错
            }
            catch
            {
                SaveTimer.Interval = 5 * 60 * 1000;//默认5分钟保存一次
            }
            SaveTimer.Enabled = false;
            SaveTimer.AutoReset = true;
            SaveTimer.Elapsed += SaveTimer_Elapsed;
            //.First<DataTypePoint>().SaveInterVal;
            

        }

        private void SaveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //ToAdd:save
        }

        void FinishedSubscript(DataTypePoint dtp,Dictionary<string,ExpectList> els)
        {
            Log(dtp.DataType, "成功完成订阅");
            foreach(string key in els.Keys)
            {
                if(AllHy.ContainsKey(key))
                {
                    AllHy[key] = els[key];
                }
                else
                {
                    AllHy.Add(key, els[key]);
                }
            }
        }

        void OnUpdateData(DataTypePoint dtp, Dictionary<string, ExpectList> els)
        {
            Log(dtp.DataType, "成功完成订阅");
            foreach (string key in els.Keys)
            {
                if (AllHy.ContainsKey(key))
                {
                    AllHy[key] = els[key];
                }
                else
                {
                    AllHy.Add(key, els[key]);
                }
            }
        }

        void CancelSubscript(DataTypePoint dtp, Dictionary<string, ExpectList> els)
        {
            Log(dtp.DataType, "成功取消订阅");
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Timer_Ex tm = sender as Timer_Ex;
            DataTypePoint dtp = DataTypes[tm.Name];
            DateTime CurrTime = DateTime.Now;//当前时间
            ExecSubscriptClass esc = new ExecSubscriptClass(DataTypes[tm.Name]);
            esc.OnCancelSubScript += FinishedSubscript;
            esc.OnCancelSubScript += CancelSubscript;
            esc.OnUpdateData += OnUpdateData;
            if (!tm.SubsciptStatus)//未订阅状态，开始订阅，设置下次启动时间到停止时间
            {
                DateTime EndT = CurrTime.Date.Add(dtp.ReceiveEndTime.TimeOfDay);//当日的结束时间
                if (CurrTime > EndT)//当前时间已经过了结束时间，不订阅,唤醒时间设置到下个周期的开始时间
                {
                    tm.Interval = CurrTime.AddDays(1).Add(dtp.ReceiveStartTime.TimeOfDay).Subtract(CurrTime).TotalMilliseconds;
                    Log("错过取消订阅时间，不订阅", string.Format("下次唤醒订阅时间{0}", CurrTime.AddDays(1).Add(dtp.ReceiveStartTime.TimeOfDay).ToLongTimeString()));
                    return;
                }
                esc.Subscript(CurrTime > CurrTime.Date.Add(dtp.ReceiveStartTime.TimeOfDay));
                tm.SubsciptStatus = true;
                tm.Interval = EndT.Subtract(CurrTime).TotalMilliseconds;//当日结束时唤醒
                Log("已订阅，下次唤醒取消订阅时间", EndT.ToLongTimeString());
            }
            else//订阅状态，停止订阅，设置下次启动时间到停止时间
            {
                DateTime BegT = DateTime.Now.Date.AddDays(1).Add(dtp.ReceiveStartTime.TimeOfDay);//当日的结束时间
                esc.CancelSubscript();
                tm.SubsciptStatus = false;
                tm.Interval = BegT.Subtract(CurrTime).TotalMilliseconds;
                Log("下次唤醒订阅时间", BegT.ToLongTimeString());
            }
            
        }

        public void Start()
        {
            // TODO: 在此处添加代码以启动服务。
            for (int i = 0; i < Tm_ForSubScripts.Count; i++)
            {
                Tm_ForSubScripts[i].Enabled = true;
                Timer_Elapsed(Tm_ForSubScripts[i], null);
            }
            SaveTimer.Enabled = true;
            SaveTimer_Elapsed(null, null);
        }
        protected override void OnStart(string[] args)
        {
            Start();
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
        }
    }

    public class Timer_Ex:System.Timers.Timer
    {
        public int Index;
        public string Name;
        public bool SubsciptStatus;
        public Timer_Ex(int i,string _Name):base()
        {
            Index = i;
            Name = _Name;
        }
    }

    public delegate void FinishedOpt(DataTypePoint dtp,Dictionary<string,ExpectList> el);

    public class ExecSubscriptClass
    {
        DataTypePoint dtp;
        public FinishedOpt OnFinishedSubScript;
        public FinishedOpt OnCancelSubScript;
        public FinishedOpt OnUpdateData;
        SubscriptClass SubObj;
        Dictionary<string,ExpectList> Data;
        public ExecSubscriptClass(DataTypePoint _dtp)
        {
            dtp = _dtp;
        }

        void UpdateData(Dictionary<string,ExpectList> els)
        {
            Data = els;
            OnUpdateData(dtp,els);//上传给上一级
        }
        public void GetAllPastData()
        {

        }

        public void Subscript(bool NeedReqData)
        {
            SubObj = new WD_SubscriptTimeDataClass();
            SubObj.UpdateAll = dtp.SubScriptUpdateAll==1;
            SubObj.AfterUpdate += UpdateData;
            bool bAllsec = SubObj.GetAllEquits(dtp.SubScriptSector);//获得指定板块下的所有品种清单
            if (!bAllsec)
                return;
            SubObj.getData(dtp.SubScriptFields,dtp.SubScriptOptions, dtp.SubScriptGrpCnt, NeedReqData);
            OnFinishedSubScript(dtp,Data);
        }

        public void CancelSubscript()
        {
            OnCancelSubScript(dtp,Data);
        }
    }
}
