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
using WolfInv.com.RemoteObjectsLib;
using System.Xml;
using WolfInv.com.WebRuleLib;
using System.Web.Script.Serialization;
namespace ExchangeTermial
{
    public partial class Form1 : Form
    {

        public Form1(string username,string password,bool AutoLogin)
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
            Program.User = strName;
            this.txt_password.Text = strPwd;
            this.CancelButton = this.btn_cancel;
            this.AcceptButton = this.btn_login;
            if (AutoLogin)
                btn_login_Click(null, null);
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            
            CommunicateToServer cts =new CommunicateToServer();
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
            this.Cursor = Cursors.WaitCursor;
            this.Hide();
            UserInfoClass ret =  cr.Result[0] as UserInfoClass;
            Program.UserId = ret.BaseInfo.UserId;
            Program.gc.ClientUserName = ret.BaseInfo.UserCode;
            Program.gc.ClientPassword = ret.BaseInfo.Password;
            Program.gc.Odds = ret.BaseInfo.Odds;
            Program.gc.WXLogNoticeUser = ret.BaseInfo.WXToUser;
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                xmldoc.LoadXml(ret.BaseInfo.AssetConfig);
                XmlNodeList items = xmldoc.SelectNodes("config[@type='AssetUnits']/item");
                Dictionary<string, int> assetconfig = new Dictionary<string, int>();
                if(items.Count>0)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        string key = items[i].SelectSingleNode("@key").Value;
                        int val = int.Parse(items[i].SelectSingleNode("@value").Value);
                        if (!assetconfig.ContainsKey(key))
                            assetconfig.Add(key, val);
                    }
                }
                Program.gc.AssetUnits = assetconfig;
            }
            catch
            {

            }
            if (Program.gc.SvrConfigUrl != null)//获取服务器默认配置
            {
                string strConfig = AccessWebServerClass.GetData(Program.gc.SvrConfigUrl, Encoding.UTF8);
                if (strConfig != null && strConfig.Trim().Length > 0)
                {
                    SvrConfigClass scc = new SvrConfigClass().getObjectByJsonString(strConfig);
                    if(scc.DefaultExchangeHost != null)
                    {
                        Program.gc.LoginDefaultHost = scc.DefaultExchangeHost;
                    }
                    if(scc.WXSvrHost!= null)
                    {
                        Program.gc.WXSVRHost = scc.WXSvrHost;
                    }
                }
            }
            //Program.gc.LoginDefaultHost = WebRuleBuilder.Create(Program.gc).GetChanle(Program.gc.WebNavUrl, Program.gc.LoginDefaultHost); ;
            GlobalClass.SetConfig();
            this.Hide();
            this.Cursor = Cursors.Default;
            //必须重新指定登录用户
            Program.wxl = new WolfInv.com.LogLib.WXLogClass(Program.gc.ClientUserName, Program.gc.WXLogNoticeUser, Program.gc.WXLogUrl);
            MainWindow mw = new MainWindow();
            DialogResult res = mw.ShowDialog();

            //////while ( res == DialogResult.OK)//如果frm退出是因为要求重启
            //////{
            //////    if (mw.ForceReboot)
            //////    {
            //////        mw.ForceReboot = false;
            //////        Program.Reboot = false;
            //////        mw = new MainWindow();
            //////        res = mw.ShowDialog();
            //////    }
            //////    //GC.SuppressFinalize(frm);
            //////}
            Application.Exit();
        }

        class SvrConfigClass:iSerialJsonClass<SvrConfigClass>
        {
            public string WXSvrHost { get; set; }
            public string DefaultExchangeHost { get; set; }

            public SvrConfigClass getObjectByJsonString(string str)
            {
                SvrConfigClass ret = null;
                JavaScriptSerializer js = new JavaScriptSerializer();
                ret = js.Deserialize<SvrConfigClass>(str);
                return ret;
            }
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
