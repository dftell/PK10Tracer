using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WolfInv.com.WebCommunicateClass;
//using WolfInv.com.PK10CorePress;
using WolfInv.com.BaseObjectsLib;
namespace ExchangeTermial
{
    public partial class Form1 : Form
    {

        public Form1(string username,string password)
        {
            InitializeComponent();
            
            string strName = Program.gc.ClientUserName;
            strName = Program.gc.ClientUserName; //不能省略
            string strPwd = Program.gc.ClientPassword;
            if(username != null)
            {
                strName = username;
                strPwd = password;
            }
            this.txt_user.Text = strName;
            this.txt_password.Text = strPwd;
            this.CancelButton = this.btn_cancel;
            this.AcceptButton = this.btn_login;
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            
            CommunicateToServer cts=new CommunicateToServer();
            CommResult cr =  cts.Login( GlobalClass.strLoginUrlModel, this.txt_user.Text,this.txt_password.Text);
            if(cr == null)
            {
                MessageBox.Show("无法返回对象！");
                return ;
            }
            if(cr.Succ == false)
            {
                MessageBox.Show(cr.Message);
                return ;
            }
            UserInfoClass ret =  cr.Result[0] as UserInfoClass;
            Program.gc.ClientUserName = ret.BaseInfo.UserCode;
            Program.gc.ClientPassword = ret.BaseInfo.Password;
            Program.gc.Odds = ret.BaseInfo.Odds;
            GlobalClass.SetConfig();
            this.Hide();
            MainWindow mw = new MainWindow();
            mw.Show();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
