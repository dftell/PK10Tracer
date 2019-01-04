using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebCommunicateClass;
using PK10CorePress;
using BaseObjectsLib;
namespace ExchangeTermial
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
            this.Hide();
            MainWindow mw = new MainWindow();
            mw.Show();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
