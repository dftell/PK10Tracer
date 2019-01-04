using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Dictionary<string, InstsClass> HistoryInsts;
        public Form1()
        {
            InitializeComponent();
            HistoryInsts = new Dictionary<string, InstsClass>();
            this.timer_sender.Enabled = true;
            this.timer_sender.Tick += new EventHandler(timer_sender_Tick);
            this.timer_sender.Start();
            
        }

        void timer_sender_Tick(object sender, EventArgs e)
        {
            InstsClass res =  CommitClass.GetInst();
            if (res == null) return;
            if (HistoryInsts.ContainsKey(res.Expect))
            {
                return;
            }
            this.txt_ExpectNo.Text = res.Expect;
            this.txt_Insts.Text = res.Insts;
            HistoryInsts.Add(res.Expect, res);
            string strInst = CommitClass.TranslateInstByWB(res.Expect,res.Insts);
            if (strInst == null)
            {
                return;
            }
            CommitResultClass ret = CommitClass.SendInst(strInst);


        }
    }
}
