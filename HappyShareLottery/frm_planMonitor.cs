using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WolfInv.com.JdUnionLib;
using WolfInv.com.ShareLotteryLib;
using WolfInv.com.WXMessageLib;
using WolfInv.Com.WCS_Process;
using JdEBuy;
using WolfInv.Com.MetaDataCenter;

namespace HappyShareLottery
{

    delegate void SetDataCallback(string key, object obj);
    public partial class frm_planMonitor : Form
    {
        Dictionary<string, TabControls> AllTabControls;
        ChartPushService cps;
        JdUnion_GoodsDataLoadClass dlc;
        public frm_planMonitor()
        {
            InitializeComponent();
           
            
            AllTabControls = new Dictionary<string, TabControls>();
            
        }

        private void frm_planMonitor_Load(object sender, EventArgs e)
        {
            
            this.tabControl1.TabPages.Clear();
            this.txt_ToMeMsgs.Lines = new string[] { };
            //JdGoodsQueryClass.LoadAllcommissionGoods = loadAllData;
            //new Task(initWords).Start();//载入初始数据
            dlc = new JdUnion_GoodsDataLoadClass();
            dlc.UpdateText = UpdateMsg;
            
            

        }

        void initWords()
        {
            setControl(this.txt_ToMeMsgs,  "正在初始化字典。。。");
            loadAllData();
            setControl(this.txt_ToMeMsgs,  "初始化字典完成");
        }

        void UpdateMsg(string txt)
        {
            setControl(this.txt_ToMeMsgs, txt);
            Application.DoEvents();
        }

        delegate void SetControlCallback(string ctrlid,string val);
        void setControl(Control ctrl,string txt)
        {
            ctrl.Invoke(new SetControlCallback(SetTextControlById), new object[] {ctrl.Name, txt });
        }

        void SetTextControlById(string id, string txt)
        {
            Control[] ctrls = this.Controls.Find(id, true);
            if (ctrls.Length != 1)
            {
                return;
            }
            if (ctrls[0] is TextBox && (ctrls[0] as TextBox).Multiline==true)
            {
                TextBox tb = ctrls[0] as TextBox;
                List<string> txts = txt_ToMeMsgs.Lines.ToList();
                txts.Add(txt);
                tb.Lines = txts.ToArray();
                tb.Refresh();
            }
            else
                ctrls[0].Text  = txt;
        }
        void loadAllData()
        {
            string datasourceName = "JdUnion_Client_Goods_Coupon_NoXml";
            string msg = null;

            DataSet ds = DataSource.InitDataSource(GlobalShare.UserAppInfos.First().Value.mapDataSource[datasourceName], new List<DataCondition>(), out msg);
            if (msg != null)
            {
                //MessageBox.Show(msg);
                setControl(txt_ToMeMsgs, msg);
                return;
            }
            setControl(txt_ToMeMsgs, string.Format("共计{0}条！",ds.Tables[0].Rows.Count));
            JdGoodsQueryClass.AllcommissionGoods = new Dictionary<string, JdGoodSummayInfoItemClass>();
            JdGoodsQueryClass.AllKeys = new Dictionary<string, List<string>>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                JdGoodSummayInfoItemClass jsiic = new JdGoodSummayInfoItemClass();
                jsiic.skuId = dr["JGD02"].ToString();
                jsiic.skuName = dr["JGD03"].ToString();
                jsiic.couponLink = dr["JGD07"].ToString();
                jsiic.imgageUrl = dr["JGD08"].ToString();
                jsiic.materialUrl = dr["JGD09"].ToString();
                jsiic.price = dr["JGD11"].ToString();
                jsiic.discount = dr["JGD06"].ToString();
                if(JdGoodsQueryClass.AllcommissionGoods.ContainsKey(jsiic.skuId))
                {
                    continue;
                }
                JdGoodsQueryClass.AllcommissionGoods.Add(jsiic.skuId, jsiic);
                List<string> keys = JdGoodsQueryClass.splitTheWords(jsiic.skuName, true);
                JdGoodsQueryClass.AllKeys.Add(jsiic.skuId, keys);
                if (i>0&&(i % 5000) == 0)
                {
                    setControl(txt_ToMeMsgs, string.Format("完成第{0}条！", i + 1));
                }
            }
            JdGoodsQueryClass.Inited = true;
        }

        void SetDataGridDataTable(Control ctrl,string key, object data)
        {
            if(ctrl == this.txt_ToMeMsgs)
            {
                ctrl.Invoke(new SetDataCallback(refreshMsg), new object[] { key, data});
                return;
            }
            if(ctrl == this.tabControl1)
            {
                ctrl.Invoke(new SetDataCallback(refreshTab), new object[] { key, data });
                return;
            }
        }

        public void refreshMsg(string sender, wxMessageClass data)
        {
            SetDataGridDataTable(txt_ToMeMsgs, sender, data);
        }

        void refreshMsg(string sender,object data)
        {
            wxMessageClass a = data as wxMessageClass;
            List<string> txts = txt_ToMeMsgs.Lines.ToList();
            txts.Add(string.Format("[{0}] {1} :{2}",a.FromNikeName,a.FromMemberNikeName,a.Msg));
            txt_ToMeMsgs.Lines = txts.ToArray();
        }

        public void refreshTab(string roomName, ShareLotteryPlanClass data)
        {
            SetDataGridDataTable(tabControl1, roomName, data);
        }
        void refreshTab(string roomName,object data)
        {
            try
            {
                ShareLotteryPlanClass slpc = data as ShareLotteryPlanClass;
                TabPage page = null;

                if (tabControl1.TabPages.ContainsKey(roomName))
                {
                    tabControl1.TabPages.Add(roomName);
                }
                page = tabControl1.TabPages[roomName];
                if (!AllTabControls.ContainsKey(roomName))
                {
                    AllTabControls.Add(roomName, InitPage(page));
                }
                TabControls tabs = AllTabControls[roomName];
                tabs.propGrid.SelectedObject = slpc;
            }
            catch(Exception e)
            {

            }
        }

        TabControls InitPage(TabPage page)
        {
            if (page == null)
                return null;
            ////ListView lv = new ListView();
            ////lv.GridLines = true;
            ////lv.Dock = DockStyle.Fill;
            ////lv.Show();
            ////page.Controls.Add(lv);
            TabControls tcs = new TabControls();
            PropertyGrid pg = new PropertyGrid();
            pg.Dock = DockStyle.Fill;
            page.Controls.Add(pg);



            tcs.propGrid = pg;

            return tcs;
        }

         class TabControls
        {
            public ListView Lvi;
            public PropertyGrid propGrid;
        }

        private void startPushMsgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(Program.allContacts == null || Program.allContacts.Count == 0)
            {
                Program.Heart_Timer_Tick(null, null);
            }
            if(Program.allContacts == null)
            {
                MessageBox.Show("等待接收到心跳包！");

                return;
            }
            cps = new ChartPushService();
            cps.Start();
        }

        private void endPushMsgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cps.Stop();
        }

        private void startRecieveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JdEBuy.Form1 frm = new JdEBuy.Form1(dlc);
            frm.Show();
        }
    }
}
