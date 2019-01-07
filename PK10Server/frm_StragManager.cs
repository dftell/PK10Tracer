using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Strags;
using PK10CorePress;
namespace PK10Server
{
    public partial class frm_StragManager : Form
    {

        public frm_StragManager()
        {
            InitializeComponent();
            
        }

        public StragClass SpecObject;
        public Dictionary<string, StragClass> SpecList;
        Dictionary<string, StragClass> AllList;

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            frm_NewStrag frm = new frm_NewStrag();
            frm.ShowDialog();
            if (frm.Saved)
            {
                ////MessageBox.Show(frm.RetJson);
                if (this.AllList.ContainsKey(frm.RetJson.GUID))
                {
                    MessageBox.Show(string.Format("{0}策略已存在！", frm.RetJson.GUID));
                    return;
                }
                this.AllList.Add(frm.RetJson.GUID, frm.RetJson);
                SaveList();
                RefreshGrid();
            }
        }

        void SaveList()
        {
            List<StragClass> CurrList = AllList.Values.ToList<StragClass>();
            //Program.AllGlobalSetting.gc.setStragXml(StragClass.getXmlByObjectList<StragClass>(CurrList));
            GlobalClass.SaveStragList(StragClass.getXmlByObjectList<StragClass>(CurrList));
            Program.AllGlobalSetting.AllStrags = AllList;
        }

        void RefreshGrid()
        {
            List<StragClass> CurrList = AllList.Values.ToList<StragClass>();
            if (CurrList == null)
                return;
            DataTable dt = StragClass.GetTableByStragList(CurrList);
            this.dg_Strags.DataSource = dt;
            this.dg_Strags.Refresh();
            this.dg_Strags.Tag = CurrList;
        }

        private void frm_StragManager_Load(object sender, EventArgs e)
        {
            
            AllList = Program.AllGlobalSetting.AllStrags;
            if (SpecList != null)
                AllList = SpecList;
            if (AllList.Count >0)
            {
                List<StragClass> list = AllList.Values.ToList();
                if (list == null)
                {
                    return;
                }
                RefreshGrid();
            }
            if (SpecObject != null)
            {
                propertyGrid1.SelectedObject = SpecObject;
            }
        }

        private void dg_Strags_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dg_Strags_DoubleClick(object sender, EventArgs e)
        {
            int index = dg_Strags.CurrentRow.Index;
            if (index < 0)
            {
                return;
            }
            string guid = dg_Strags.CurrentRow.Cells[0].Value.ToString();
            if (AllList.ContainsKey(guid))
            {
                this.propertyGrid1.SelectedObject = AllList[guid];
            }
            else
            {
                MessageBox.Show("选择的策略不存在！");
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (!Update(false))
            {
                return;
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (Update(true))
            {
                return;
            }
            
        }

        bool Update(bool DeleteOperate)
        {
            StragClass sc = this.propertyGrid1.SelectedObject as StragClass;
            if (sc == null)
            {
                MessageBox.Show("请先双击选择要操作的策略！");
                return false;
            }
            if (!this.AllList.ContainsKey(sc.GUID))
            {
                MessageBox.Show("选择的策略不存在！");
                return false;
            }
            this.AllList[sc.GUID] = sc;
            if (DeleteOperate)
            {
                if (!this.AllList.Remove(sc.GUID))
                {
                    MessageBox.Show("无法删除选择策略！");
                    return false;
                }
                this.propertyGrid1.SelectedObject = null;
            }
            SaveList();
            RefreshGrid();
            MessageBox.Show(string.Format("{0}成功！",DeleteOperate?"删除":"保存"));
            return true;
        }
        
    }

    
}
