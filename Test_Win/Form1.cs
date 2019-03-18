using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CFZQ_LHProcess;
using StrategyLibForWD;
using System.Threading;
using WAPIWrapperCSharp;
namespace Test_Win
{
    public partial class Form1 : Form
    {
        string tCode = "";
        public string SecCode { get { return tCode; } set { tCode = value; } } 
        public Form1()
        {
            InitializeComponent();
        }

        public Form1(string code)
        {
            InitializeComponent();
            SecCode = code;
            button1_Click(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GLClassProcess glpro = new GLClassProcess();
            DataTable dt = glpro.getRecords(tCode, DateTime.Today);
            this.dataGridView1.DataSource = dt;
            this.dataGridView1.Show();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (SecCode != null && SecCode.Length > 0) return;
            string strCode = this.dataGridView1.CurrentRow.Cells[1].Value.ToString();
            Form1 frm = new Form1(strCode);
            frm.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            WSSProcess wssp = new WSSProcess();
            WindAPI w = new WindAPI();
            w.start();
            DataTable dt = wssp.getRecods(w,textBox1.Text, textBox2.Text);
            this.dataGridView1.DataSource = dt;
            this.dataGridView1.Show();
            this.Cursor = Cursors.Default;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            string ret = null;
            try
            {
                GLClassProcess glpro = new GLClassProcess();
                ret = glpro.UpdateAllGLList(DateTime.Today);
            }
            catch (Exception ce)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(ce.Message);
                return;
            }
            this.Cursor = Cursors.Default;
            if (ret != null)
            {
                
                MessageBox.Show(ret);
                return;
            }
            MessageBox.Show("Save Success!");
            return;
        }

        private void btn_savemumber_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            string ret = null;
            try
            {
                GLClassProcess glpro = new GLClassProcess();
                ret = glpro.UpdateAllGLNumbers(DateTime.Today);
                List<GLThreadProcess> threadpool = glpro.CheckPin as List<GLThreadProcess>;
                List<string> msgs = new List<string>();
                while (true)
                {
                    Thread.Sleep(1000);
                    int endcnt = 0;
                    this.textBox3.Clear();
                    
                    bool NeedCheck = true;
                    foreach (GLThreadProcess thrd in threadpool)
                    {
                        if (NeedCheck && thrd.Trdobj.ThreadState == ThreadState.Running)
                        {
                            NeedCheck = false;
                        }
                        else
                        {
                            ++endcnt;
                        }
                        if(thrd.CheckPin != null)  msgs.Add(thrd.CheckPin.ToString());
                    }
                    if(msgs.ToArray().Length > 15)
                        this.textBox3.Text = string.Join("\r\n", msgs.ToArray(),msgs.Count-11,10);
                    
                    //this.textBox3.Show();
                    this.textBox3.Refresh();
                    if (endcnt == threadpool.Count) break;
                }
            }
            catch (Exception ce)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(ce.StackTrace);
                return;
            }
            this.Cursor = Cursors.Default;
            if (ret != null)
            {

                MessageBox.Show(ret);
                return;
            }
            MessageBox.Show("Save Success!");
            return;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            string ret = null;
            try
            {
                GLClassProcess glpro = new GLClassProcess();
                ret = glpro.UpdateAllGLHQs(DateTime.Today);
                List<GLThreadProcess> threadpool = glpro.CheckPin as List<GLThreadProcess>;
                List<string> msgs = new List<string>();
                while (true)
                {
                    Thread.Sleep(1000);
                    int endcnt = 0;
                    this.textBox3.Clear();

                    bool NeedCheck = true;
                    foreach (GLThreadProcess thrd in threadpool)
                    {
                        if (NeedCheck && thrd.Trdobj.ThreadState == ThreadState.Running)
                        {
                            NeedCheck = false;
                        }
                        else
                        {
                            ++endcnt;
                        }
                        if (thrd.CheckPin != null) msgs.Add(thrd.CheckPin.ToString());
                    }
                    if (msgs.ToArray().Length > 15)
                        this.textBox3.Text = string.Join("\r\n", msgs.ToArray(), msgs.Count - 11, 10);

                    //this.textBox3.Show();
                    this.textBox3.Refresh();
                    if (endcnt == threadpool.Count) break;
                }
            }
            catch (Exception ce)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(ce.StackTrace);
                return;
            }
            this.Cursor = Cursors.Default;
            if (ret != null)
            {

                MessageBox.Show(ret);
                return;
            }
            MessageBox.Show("Save Success!");
            return;
        }
    }
}
