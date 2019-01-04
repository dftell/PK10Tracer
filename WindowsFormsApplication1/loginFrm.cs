using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class loginFrm : Form
    {
        
        public loginFrm()
        {
            InitializeComponent();
            //CommitClass.getJQueryLib();
            
            CommitClass.SelectQuickestHost();
            ////CommitClass.LoadHtml(false);
            ////CommitClass.ResetWebStatus();
            CommitClass.LoadHtml(true);
            Label lastlbl = null;
            for (int i = 0; i < CommitClass.HostList.Count; i++)
            {
                HostInfo hi = CommitClass.HostList[i];
                Control[] ctrls = this.Controls.Find(String.Format("lbl_host{0}", i + 1), true);
                if (ctrls.Length > 0)
                {
                    (ctrls[0] as Label).Text = string.Format("主机：{0};延迟:{1};Cookie:{2}", hi.name, hi.times, hi.RequestVerificationToken);
                    lastlbl = ctrls[0] as Label;
                }
                else
                {
                    Label lbl = new Label();
                    lbl.Location = new Point(lastlbl.Location.X, lastlbl.Location.Y + 20);
                    lbl.AutoSize = true;
                    lbl.Size = new System.Drawing.Size(lastlbl.Size.Width, lastlbl.Size.Height);
                    lbl.Visible = true;
                    lbl.Text = string.Format("主机：{0};延迟:{1};Cookie:{2}", hi.name, hi.times, hi.RequestVerificationToken);
                   
                    this.Controls.Add(lbl);
                    lastlbl = lbl;
                }
                this.ResumeLayout(false);
                this.PerformLayout();
                
            }
            this.lbl_defaultHost.Text = string.Format("主机：{0};延迟:{1};Cookie:{2}", CommitClass.DefaultHostName,0, CommitClass.strRequestVerificationToken);
      
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            HostUser hu = new HostUser();
            hu.userName = this.txt_username.Text;
            hu.password = this.txt_password.Text;
            hu.valicode = this.txt_valicode.Text;
            string strPost = CommitClass.TranslateByWB(hu.ToJsonString<HostUser>());
            CommitResultClass ret = CommitClass.LoginHost(strPost);
            if (ret.Suc)
            {
                Form1 frm = new Form1();
                frm.Show();
                this.Close();
                return;
            }
            MessageBox.Show(ret.Message);
            return;
            ////InstsClass ret=  CommitClass.GetInst();
            ////if (ret != null)
            ////{
            ////    MessageBox.Show(ret.ToJsonString<InstsClass>());
            ////}
        }

        void Translate()
        {
            
        }
    }
}
