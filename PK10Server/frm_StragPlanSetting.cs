using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WolfInv.com.Strags;
using WolfInv.com.PK10CorePress;
using WolfInv.com.ExchangeLib;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.SecurityLib;
namespace PK10Server
{
    public partial class frm_StragPlanSetting<T> : Form where T:TimeSerialData
    {
        bool NewAPlan = false;
        public StragRunPlanClass<T> SpecObject;
        public Dictionary<string, StragRunPlanClass<T>> SpecList;
        public frm_StragPlanSetting()
        {
            InitializeComponent();
            
        }

        void refreshGrid(Dictionary<string,StragRunPlanClass<T>> alllist)
        {
            List<StragRunPlanClass<T>> list = alllist.Values.ToList<StragRunPlanClass<T>>();
            DataTable dt = StragRunPlanClass<T>.ToTable<StragRunPlanClass<T>>(list, true,true);
            this.dataGridView1.DataSource = dt;
            this.dataGridView1.Refresh();
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            NewAPlan = true;
            StragRunPlanClass<T> spc = new StragRunPlanClass<T>();//
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
            Dictionary<string, StragRunPlanClass<T>> AllList = null;
            if (Program<T>.AllGlobalSetting.AllRunPlannings!=null)
            {
                AllList = Program<T>.AllGlobalSetting.AllRunPlannings;
            }
            if (SpecList != null)
                AllList = SpecList;
            refreshGrid(AllList);
            if (SpecObject != null)
                this.propertyGrid1.SelectedObject = SpecObject;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            StragRunPlanClass<T> srp = this.propertyGrid1.SelectedObject as StragRunPlanClass<T>;
            if (srp == null)
            {
                MessageBox.Show("请先双击列表选择需要修改的计划或点击新增按钮新建计划！");
                return;
            }
            if (Program<T>.AllGlobalSetting.AllStrags.ContainsKey(srp.GUID))
            {
                BaseStragClass<T> sc = Program<T>.AllGlobalSetting.AllStrags[srp.GUID];
                ////srp.StragName = sc.StragClassName;
                ////srp.StragDescript = sc.StragScript;
            }
            if (!Program<T>.AllGlobalSetting.AllRunPlannings.ContainsKey(srp.GUID))
            {

                Program<T>.AllGlobalSetting.AllRunPlannings.Add(srp.GUID, srp);
            }
            else
            {
                if (NewAPlan)
                {
                    MessageBox.Show("该计划已新建，标志错误！");
                    return;
                }
                Program<T>.AllGlobalSetting.AllRunPlannings[srp.GUID] = srp;
            }
            if (!SaveData()) return;
            refreshGrid(Program<T>.AllGlobalSetting.AllRunPlannings);
            this.propertyGrid1.SelectedObject = null;
            this.propertyGrid1.Refresh();
            MessageBox.Show("保存成功！");
        }

        bool SaveData()
        {
            bool suc = GlobalClass.setStragRunningPlan(StragRunPlanClass<T>.getXmlByObjectList<StragRunPlanClass<T>>(Program<T>.AllGlobalSetting.AllRunPlannings.Values.ToList<StragRunPlanClass<T>>()));
            if (!suc)
            {
                MessageBox.Show("保存数据错误，点击确定后将回复较早前的数据！");
                refreshGrid(Program<T>.AllGlobalSetting.AllRunPlannings);
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
            if (!Program<T>.AllGlobalSetting.AllRunPlannings.ContainsKey(strUid))
            {
                MessageBox.Show("计划已不存在，点击确定后将更新视图");
                refreshGrid(Program<T>.AllGlobalSetting.AllRunPlannings);
                return;
            }
            this.propertyGrid1.SelectedObject = null;
            this.propertyGrid1.SelectedObject = Program<T>.AllGlobalSetting.AllRunPlannings[strUid];
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            StragRunPlanClass<T> srp = this.propertyGrid1.SelectedObject as StragRunPlanClass<T>;
            if (NewAPlan || srp == null)
            {
                MessageBox.Show("请先双击列表选择需要删除计划！");
                return;
            }
            if (!Program<T>.AllGlobalSetting.AllRunPlannings.ContainsKey(srp.GUID))
            {
                MessageBox.Show("计划已不存在，点击确定后将更新视图");
                refreshGrid(Program<T>.AllGlobalSetting.AllRunPlannings);
                return;
            }
            Program<T>.AllGlobalSetting.AllRunPlannings.Remove(srp.GUID);
            if (!SaveData())
            {
                return;
            }
            refreshGrid(Program<T>.AllGlobalSetting.AllRunPlannings);
            MessageBox.Show("成功删除！");

        }
    }
}
