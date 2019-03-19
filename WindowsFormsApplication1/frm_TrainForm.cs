using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.MachineLearnLib;
using WolfInv.com.PK10CorePress;
using WolfInv.com.Strags;
using System.IO;
using System.Threading;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.SecurityLib;
namespace BackTestSystem
{
    delegate void SetDataGridCallback(string id, DataTable dt,int currRow);
    public partial class frm_TrainForm : Form
    {
        
        public frm_TrainForm()
        {
            InitializeComponent();
            DataTable dt = ClassOperateTool.getAllSubClass(typeof(MachineLearnClass<int, int>),"text","value");
            this.ddl_MLFunc.DataSource = dt;
            this.ddl_MLFunc.DisplayMember = "text";
            this.ddl_MLFunc.ValueMember = "value";
            
        }
        
        Type MLType;
        Thread RunningThread = null;
        List<MachineLearnClass<int, int>> SelectFuncs = new List<MachineLearnClass<int, int>>();
        int ThreadCnt = 0;
        int FinishedCnt = 0;
        private void btn_Train_Click(object sender, EventArgs e)
        {
           
            long len = long.Parse(this.txt_DataLength.Text);
            int deep = int.Parse(this.txt_LearnDeep.Text);
            ExpectList el = new ExpectReader().ReadHistory(long.Parse(this.txt_BegExpect.Text), len+deep+1);
            MLDataFactory mldf = new MLDataFactory(el);
            for (int i = 0; i < 10; i++)
            {
                MLInstances<int, int> TrainSet = mldf.getAllSpecColRoundLabelAndFeatures(i,deep, chkb_AllUseShift.Checked ? 1 : 0);
                MachineLearnClass<int, int> SelectFunc;
                MLType = (Type)this.ddl_MLFunc.SelectedValue;
                SelectFunc = (MachineLearnClass<int, int>)ClassOperateTool.getInstanceByType(MLType);

                SelectFunc.OnTrainFinished += OnTrainFinished;
                SelectFunc.OnPeriodEvent += OnPeriodEvent;

                SelectFunc.OnSaveEvent += SaveData;
                SelectFunc.GroupId = i;
                SelectFunc.FillTrainData(TrainSet);
                SelectFunc.InitTrain();
                
                SelectFunc.TrainIterorCnt = int.Parse(txt_IteratCnt.Text);
                SelectFuncs.Add(SelectFunc);
                this.txt_begT.Text = DateTime.Now.ToLongTimeString();
                this.Cursor = Cursors.WaitCursor;
                RunningThread = new Thread(SelectFunc.Train);
                RunningThread.Start();
                ThreadCnt++;
            }
            
        }
        

        void OnPeriodEvent(params object[] objs)
        {
            int test = 1;
            if(test > 0)
                return;
            lock (dataGridView1.Tag)
            {
                try
                {
                    string strModel = "{0};GrpId:{1};大:{2};小:{3};";
                    this.txt_endT.Text = string.Format(strModel, DateTime.Now.ToLongTimeString(), objs[0], objs[1], objs[2]);
                    string[] keynames = (string[])objs[3];
                    double[] keydata = (double[])objs[4];
                    DataTable dt = (DataTable)this.dataGridView1.Tag;
                    if (dt == null) return;
                    int col = (int)objs[0];
                    string strCol = string.Format("{0}", (col + 1) % 10);
                    string colname = string.Format("列{0}", strCol);
                    string colval = string.Format("值{0}", strCol);
                    for (int i = 0; i < keynames.Length; i++)
                    {
                        DataRow dr = null;
                        if (i >= dt.Rows.Count)
                        {
                            dr = dt.NewRow();
                            dt.Rows.Add(dr);
                        }
                        dr = dt.Rows[i];
                        dr["Id"] = i + 1;
                        //dr[colname] = keynames[i];
                        dr[colval] = keydata[i];
                        //dt.Rows.Add(dr);
                    }
               
                    int row = (int)objs[1];
                    this.SetDataGridDataTable(this.dataGridView1, dt, row);
                    ////Monitor.Enter(this.dataGridView1);
                    ////this.dataGridView1.DataSource = dt;
                    ////this.dataGridView1.Refresh();
                    ////this.dataGridView1.ClearSelection();
                    ////DataGridViewRow rows = this.dataGridView1.Rows[row];
                    ////rows.Selected = true;
                    ////this.dataGridView1.CurrentCell = rows.Cells[1];

                }
                catch (Exception ce)
                {
                    string msg = ce.Message;
                }
            }

        }

        void initDataTable(ref DataTable dt)
        {
            dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            for(int i=0;i<10;i++)
            {
                string strcol = string.Format("{0}", (i + 1) % 10);
                //string colName = string.Format("列{0}", strcol);
                string colVal = string.Format("值{0}", strcol);
                //dt.Columns.Add(colName);
                dt.Columns.Add(colVal);
            }
        }

        string GetLocalFile()
        {
            try
            {
                DetailStringClass dsc = new DetailStringClass();
                string pathSummaryPath = string.Format("{0}\\{1}_summary.data", Path.GetDirectoryName(Application.ExecutablePath), MLType.Name);
                string strText = File.ReadAllText(pathSummaryPath);
                return strText;
            }
            catch(Exception e)
            {
                string msg = e.Message;
                return null;
            }
        }

        void SetDgTableById(string id, DataTable dt,int CurrRow)
        {
            Control[] ctrls = this.Controls.Find(id, true);
            if (ctrls.Length != 1)
            {
                return;
            }
            DataGridView dg = (ctrls[0] as DataGridView);
            dg.DataSource = dt;
            dg.Tag = dt;
            dg.ClearSelection();
            if (CurrRow < 0) return;
            DataGridViewRow rows = this.dataGridView1.Rows[CurrRow];
            if (rows == null) return;
            rows.Selected = true;
            dg.CurrentCell = rows.Cells[1];
        }
        void SetDataGridDataTable(DataGridView dg, DataTable dt,int CurrRow)
        {
            dg.Invoke(new SetDataGridCallback(SetDgTableById), new object[] { dg.Name, dt, CurrRow });
        }
        void OnTrainFinished()
        {
            FinishedCnt++;
            if (FinishedCnt < ThreadCnt)
                return;
            SelectFuncs[0].SaveSummary();//随便一个保存就够了，反正是静态的summary
            this.Cursor = Cursors.Default;
            this.txt_endT.Text = DateTime.Now.ToLongTimeString();


            ////double[] keydata = SelectFunc.FeatureSummary.Keys;
            ////string msg = SaveData(keydata);
            ////if (msg != null)
            ////    MessageBox.Show(msg);
        }

        void SaveData(string txt)
        {
            try
            {
                string pathSummaryPath = string.Format("{0}\\{1}_summary.data", Path.GetDirectoryName(Application.ExecutablePath), MLType.Name);
                FileStream fs = File.Open(pathSummaryPath, FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(txt);
                sw.Close();
                fs.Close();
            }
            catch(Exception ce)
            {
                string msg = ce.Message;
                return;
            }
            finally
            {
                
            }
            return;
        }
        
        private void btn_stopTrain_Click(object sender, EventArgs e)
        {
            if(RunningThread!=null)
            {
                try
                {
                    RunningThread.Abort();
                }
                catch(Exception ce)
                {
                    MessageBox.Show(ce.Message);
                    
                }
                finally
                {
                    RunningThread = null;
                }
            }
            this.Cursor = Cursors.Default;
        }
        MLInstances<int, int> TestSet;
        private void btn_CheckResult_Click(object sender, EventArgs e)
        {
            long len = long.Parse(this.txt_DataLength.Text);
            int deep = int.Parse(this.txt_LearnDeep.Text);
            ExpectList el = new ExpectReader().ReadHistory(long.Parse(this.txt_BegExpect.Text), len + deep + 1);
            
            for (int i = 0; i < 10; i++)
            {
                MachineLearnClass<int, int> SelectFunc = null;
                MLType = (Type)this.ddl_MLFunc.SelectedValue;
                SelectFunc = (MachineLearnClass<int, int>)ClassOperateTool.getInstanceByType(MLType);
                TestSet = new MLDataFactory(el).getAllSpecColRoundLabelAndFeatures(i,deep, chkb_AllUseShift.Checked ? 1 : 0);
                SelectFunc.OnLoadLocalFile += GetLocalFile;
                SelectFunc.LoadSummary();
                SelectFunc.FillStructBySummary(i);
                SelectFunc.SetTestInstances(TestSet);
                this.Cursor = Cursors.WaitCursor;
                ExecClass ec = new ExecClass();
                ec.GroupId = (i + 1) % 10;
                ec.MaxEnt = SelectFunc;
                ec.TestData = TestSet;
                new Thread(new ThreadStart(ec.Exec)).Start();
            }
            
            
        }

        class ExecClass
        {
            public MLInstances<int, int> TestData;
            public MachineLearnClass<int, int> MaxEnt;
            public double outValue;
            public int GroupId;
            public void Exec()
            {
                string res = MaxEnt.CheckInstances(TestData).ToString();
                MessageBox.Show(string.Format("{0}:{1}",GroupId,res));
            }
        }

        private void frm_TrainForm_Load(object sender, EventArgs e)
        {
            DataTable dtview = null;
            initDataTable(ref dtview);
            this.SetDataGridDataTable(this.dataGridView1, dtview, -1);
        }
    }
}
