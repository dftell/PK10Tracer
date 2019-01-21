using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseObjectsLib;
using MachineLearnLib;
using PK10CorePress;
using Strags;
using System.IO;
using System.Threading;
namespace BackTestSystem
{
    delegate void SetDataGridCallback(string id, DataTable dt,int currRow);
    public partial class frm_TrainForm : Form
    {
        
        public frm_TrainForm()
        {
            InitializeComponent();
            DataTable dt = ClassOperateTool.getAllSubClass(typeof(MachineLearnLib.MachineLearnClass<int, int>),"text","value");
            this.ddl_MLFunc.DataSource = dt;
            this.ddl_MLFunc.DisplayMember = "text";
            this.ddl_MLFunc.ValueMember = "value";
        }
        MachineLearnClass<int, int> SelectFunc;
        Type MLType;
        private void btn_Train_Click(object sender, EventArgs e)
        {
            MLType = (Type)this.ddl_MLFunc.SelectedValue;
            SelectFunc = (MachineLearnClass<int, int>)ClassOperateTool.getInstanceByType(MLType);
            SelectFunc.OnTrainFinished += OnTrainFinished;
            SelectFunc.OnPeriodEvent += OnPeriodEvent;
            long len = long.Parse(this.txt_DataLength.Text);
            int deep = int.Parse(this.txt_LearnDeep.Text);
            ExpectList el = new ExpectReader().ReadHistory(long.Parse(this.txt_BegExpect.Text), len+deep+1);
            MLInstances<int, int> TrainSet = new MLDataFactory(el).getAllSpecColRoundLabelAndFeatures(0,deep);
            SelectFunc.FillTrainData(TrainSet);
            SelectFunc.InitTrain();
            this.txt_begT.Text = DateTime.Now.ToLongTimeString();
            this.Cursor = Cursors.WaitCursor;
            new Thread(new ThreadStart(StartTrain)).Start();
            
        }

        void StartTrain()
        {
            SelectFunc.Train();
        }

        void OnPeriodEvent(params object[] objs)
        {
            try
            {
                string strModel = "{0};大:{1};小:{2};";
                this.txt_endT.Text = string.Format(strModel,DateTime.Now.ToLongTimeString(), objs[0], objs[1]);
                string[] keynames = (string[])objs[2];
                double[] keydata = (double[])objs[3];
                DataTable dt = new DataTable();
                dt.Columns.Add("key");
                dt.Columns.Add("val");
                for(int i=0;i<keynames.Length;i++)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = keynames[i];
                    dr[1] = keydata[i];
                    dt.Rows.Add(dr);
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
            catch(Exception ce)
            {

            }

        }

        void SetDgTableById(string id, DataTable dt,int CurrRow)
        {
            Control[] ctrls = this.Controls.Find(id, true);
            if (ctrls.Length != 1)
            {
                return;
            }
            (ctrls[0] as DataGridView).DataSource = dt;
            (ctrls[0] as DataGridView).Tag = dt;
        }
        void SetDataGridDataTable(DataGridView dg, DataTable dt,int CurrRow)
        {
            dg.Invoke(new SetDataGridCallback(SetDgTableById), new object[] { dg.Name, dt, CurrRow });
        }
        void OnTrainFinished()
        {
            this.Cursor = Cursors.Default;
            this.txt_endT.Text = DateTime.Now.ToLongTimeString();
            double[] keydata = SelectFunc.GetKeyResult();
            string msg = SaveData(keydata);
            if (msg != null)
                MessageBox.Show(msg);
        }

        string SaveData(double[] data)
        {
            try
            {
                string path = string.Format("{0}\\{1}.data", Path.GetFullPath(Application.ExecutablePath), MLType.Name);
                FileStream fs = File.Open(path, FileMode.CreateNew);
                StreamWriter sw = new StreamWriter(fs);
                for (int i = 0; i < data.Length; i++)
                    sw.WriteLine();
                sw.Close();
                fs.Close();
            }
            catch(Exception ce)
            {
                return ce.Message;
            }
            finally
            {
                
            }
            return null;
        }
    }
}
