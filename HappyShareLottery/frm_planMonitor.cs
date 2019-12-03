using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WolfInv.com.ShareLotteryLib;
using WolfInv.com.WXMessageLib;
namespace HappyShareLottery
{

    delegate void SetDataCallback(string key, object obj);
    public partial class frm_planMonitor : Form
    {
        Dictionary<string, TabControls> AllTabControls;
        
        public frm_planMonitor()
        {
            InitializeComponent();
           
            
            AllTabControls = new Dictionary<string, TabControls>();
        }

        private void frm_planMonitor_Load(object sender, EventArgs e)
        {
            this.tabControl1.TabPages.Clear();
            this.txt_ToMeMsgs.Lines = new string[] { };
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
    }
}
