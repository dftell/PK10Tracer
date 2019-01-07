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
using ExchangeLib;
namespace PK10Server
{
    public partial class frm_StragPlanSetting : Form
    {
        bool NewAPlan = false;
        public StragRunPlanClass SpecObject;
        public Dictionary<string, StragRunPlanClass> SpecList;
        public frm_StragPlanSetting()
        {
            InitializeComponent();
            
        }

        void refreshGrid(Dictionary<string,StragRunPlanClass> alllist)
        {
            List<StragRunPlanClass> list = alllist.Values.ToList<StragRunPlanClass>();
            DataTable dt = StragRunPlanClass.ToTable<StragRunPlanClass>(list, true,true);
            this.dataGridView1.DataSource = dt;
            this.dataGridView1.Refresh();
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            NewAPlan = true;
            StragRunPlanClass spc = new StragRunPlanClass();//
            //spc.GUID = "请选择需要配置的策略";
            spc.AutoRunning = true;
            spc.DailyStartTime = "09:07:00";
            spc.DailyEndTime = "23:59:58";
            spc.ExpiredTime = DateTime.Parse("2049/12/31");
            spc.Creator = "群主";
            spc.CreateTime = DateTime.Now;
            this.propertyGrid1.SelectedObject = spc;
        }

        private void frm_StragPlanSetting_Load(object sender, EventArgs e)
        {
            Dictionary<string, StragRunPlanClass> AllList = null;
            if (Program.AllGlobalSetting.AllRunPlannings!=null)
            {
                AllList = Program.AllGlobalSetting.AllRunPlannings;
            }
            if (SpecList != null)
                AllList = SpecList;
            refreshGrid(AllList);
            if (SpecObject != null)
                this.propertyGrid1.SelectedObject = SpecObject;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            StragRunPlanClass srp = this.propertyGrid1.SelectedObject as StragRunPlanClass;
            if (srp == null)
            {
                MessageBox.Show("请先双击列表选择需要修改的计划或点击新增按钮新建计划！");
                return;
            }
            if (Program.AllGlobalSetting.AllStrags.ContainsKey(srp.GUID))
            {
                StragClass sc = Program.AllGlobalSetting.AllStrags[srp.GUID];
                ////srp.StragName = sc.StragClassName;
                ////srp.StragDescript = sc.StragScript;
            }
            if (!Program.AllGlobalSetting.AllRunPlannings.ContainsKey(srp.GUID))
            {

                Program.AllGlobalSetting.AllRunPlannings.Add(srp.GUID, srp);
            }
            else
            {
                if (NewAPlan)
                {
                    MessageBox.Show("该计划已新建，标志错误！");
                    return;
                }
                Program.AllGlobalSetting.AllRunPlannings[srp.GUID] = srp;
            }
            if (!SaveData()) return;
            refreshGrid(Program.AllGlobalSetting.AllRunPlannings);
            this.propertyGrid1.SelectedObject = null;
            this.propertyGrid1.Refresh();
            MessageBox.Show("保存成功！");
        }

        bool SaveData()
        {
            bool suc = GlobalClass.setStragRunningPlan(StragRunPlanClass.getXmlByObjectList<StragRunPlanClass>(Program.AllGlobalSetting.AllRunPlannings.Values.ToList<StragRunPlanClass>()));
            if (!suc)
            {
                MessageBox.Show("保存数据错误，点击确定后将回复较早前的数据！");
                refreshGrid(Program.AllGlobalSetting.AllRunPlannings);
                return suc;
            }
            return true;
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (this.dataGridView1.CurrentRow.Index < 0)
            {
                return;
            }
            NewAPlan = false; //双击后新建的无效
            string strUid = this.dataGridView1.CurrentRow.Cells["GUID"].Value.ToString();
            if (!Program.AllGlobalSetting.AllRunPlannings.ContainsKey(strUid))
            {
                MessageBox.Show("计划已不存在，点击确定后将更新视图");
                refreshGrid(Program.AllGlobalSetting.AllRunPlannings);
                return;
            }
            this.propertyGrid1.SelectedObject = null;
            this.propertyGrid1.SelectedObject = Program.AllGlobalSetting.AllRunPlannings[strUid];
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            StragRunPlanClass srp = this.propertyGrid1.SelectedObject as StragRunPlanClass;
            if (NewAPlan || srp == null)
            {
                MessageBox.Show("请先双击列表选择需要删除计划！");
                return;
            }
            if (!Program.AllGlobalSetting.AllRunPlannings.ContainsKey(srp.GUID))
            {
                MessageBox.Show("计划已不存在，点击确定后将更新视图");
                refreshGrid(Program.AllGlobalSetting.AllRunPlannings);
                return;
            }
            Program.AllGlobalSetting.AllRunPlannings.Remove(srp.GUID);
            if (!SaveData())
            {
                return;
            }
            refreshGrid(Program.AllGlobalSetting.AllRunPlannings);
            MessageBox.Show("成功删除！");

        }
    }
}
