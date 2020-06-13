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
using System.Net;
using System.Collections.Specialized;
using System.Web;

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
            this.btn_login.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            if(this.ddl_websites.SelectedIndex<0)
            {
                MessageBox.Show("请选择平台！");
                return;
            }
            if(Program.allGc == null)
            {
                Program.allGc = Program.gc.CopyTo<GlobalClass>();
            }
            // GlobalClass.resetTypeDataPoints();//必须重新设置，每个平台投注品种不一样
            Program.gc.ClientUserName = this.txt_user.Text.Trim();
            Program.gc.ClientPassword = this.txt_password.Text.Trim();
            Program.gc.ForWeb = this.ddl_websites.SelectedValue.ToString();
            GlobalClass.SetConfig();
            GlobalClass sgc = new GlobalClass(this.ddl_websites.SelectedValue.ToString());
            if(sgc.loadSucc)
            {

                Program.gc = sgc;
                
                //Program.gc.ForWeb = this.ddl_websites.SelectedValue.ToString();
                //return;
            }
            CommunicateToServer cts =new CommunicateToServer();
            CommResult cr =  cts.Login( GlobalClass.strLoginUrlModel, this.txt_user.Text,this.txt_password.Text,this.ddl_websites.SelectedValue.ToString());
            if(cr == null)
            {
                
                this.Cursor = Cursors.Default;
                MessageBox.Show("无法返回对象！");
                this.btn_login.Enabled = true;
                return ;
            }
            if(cr.Succ == false)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(cr.Message);
                this.btn_login.Enabled = true;
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

            Program.gc.ClientAliasName =  string.IsNullOrEmpty(ret.BaseInfo.AliasName)?ret.BaseInfo.UserCode:ret.BaseInfo.AliasName;
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                string strXml = HttpUtility.UrlDecode(ret.BaseInfo.AssetConfig, Encoding.UTF8);
                xmldoc.LoadXml(strXml);
                XmlNodeList items = xmldoc.SelectNodes("config[@type='AssetUnits']/item");
                Dictionary<string, AssetInfoConfig> assetconfig = new Dictionary<string, AssetInfoConfig>();
                if(items.Count>0)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        string key = items[i].SelectSingleNode("@key").Value;
                        //int val = int.Parse(items[i].SelectSingleNode("@value").Value);
                        //if (!assetconfig.ContainsKey(key))
                        //    assetconfig.Add(key, val);
                        AssetInfoConfig aic = new AssetInfoConfig(GlobalClass.readXmlItems(items[i].OuterXml));
                        if(!assetconfig.ContainsKey(key))
                        {
                            assetconfig.Add(key, aic);
                        }
                    }
                }
                Program.gc.AssetUnits = assetconfig;
            }
            catch(Exception ce)
            {
                //return;
            }
            if (Program.gc.SvrConfigUrl != null)//获取服务器默认配置
            {
                string strConfig = AccessWebServerClass.GetData(string.Format(Program.gc.SvrConfigUrl,Program.gc.InstHost), Encoding.UTF8);
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
                    if (scc.DefaultNavHost != null)
                        Program.gc.NavHost = scc.DefaultNavHost;
                }
            }
            try
            {
                //Program.gc.LoginDefaultHost = WebRuleBuilder.Create(Program.gc).GetChanle(Program.gc.WebNavUrl, Program.gc.LoginDefaultHost); ;
                GlobalClass.SetConfig( Program.gc.ForWeb);
                this.Hide();
                this.Cursor = Cursors.Default;
                //必须重新指定登录用户
                Program.wxl = new WolfInv.com.LogLib.WXLogClass(Program.gc.ClientAliasName, Program.gc.WXLogNoticeUser, Program.gc.WXLogUrl);
                MainWindow mw = new MainWindow();
                mw.setUser(ret.BaseInfo);
                //testWeb mw = new testWeb();
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
            catch(Exception ce1)
            {
                MessageBox.Show(string.Format("{0}:{1}",ce1.Message,ce1.StackTrace));
            }
        }

        


        class SvrConfigClass:iSerialJsonClass<SvrConfigClass>
        {
            public string WXSvrHost { get; set; }
            public string DefaultExchangeHost { get; set; }
            public string DefaultNavHost { get; set; }

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
            InitWebSites();
        }

        void InitWebSites()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("key");
            dt.Columns.Add("val");
            foreach(string key in GlobalClass.WebSites.Keys)
            {
                //DataRow dr = dt.NewRow();
                dt.Rows.Add(new string[] { key, GlobalClass.WebSites[key] });
            }
            this.ddl_websites.Items.Clear();
            ddl_websites.DisplayMember = "val";
            ddl_websites.ValueMember = "key";
            ddl_websites.DataSource = dt;
            ddl_websites.Refresh();
            ddl_websites.SelectedValue = Program.gc.ForWeb;

        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
