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
using BaseObjectsLib;
using System.Reflection;
namespace PK10Server
{
    public partial class frm_CommDBObjectsSetting<T> : Form
    {
        bool NewAPlan = false;
        public T SpecObject;
        public iDbFile UseObject;
        public Dictionary<string, T> SpecList;
        public Dictionary<string, T> OuterList { get; set; }
        string strKey = null;
        string strClassName = null;
        public frm_CommDBObjectsSetting()
        {
            InitializeComponent();
            Type UseType = typeof(T);
            T spc = (T)Convert.ChangeType(Activator.CreateInstance(UseType), UseType);
            if (spc is iDbFile)
            {
                UseObject = spc as iDbFile;
                strKey = UseObject.strKey;
                strClassName = UseObject.strObjectName;
            }
            this.Text = string.Format("{0}管理器", strClassName); 
        }

        void refreshGrid(Dictionary<string,T> alllist)
        {
            List<T> list = alllist.Values.ToList<T>();
            DataTable dt = DisplayAsTableClass.ToTable<T>(list, true,true);
            this.dataGridView1.DataSource = dt;
            this.dataGridView1.Refresh();
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            NewAPlan = true;
            Type UseType = typeof(T);
            T spc = (T)Activator.CreateInstance(UseType);
            //////spc.GUID = "请选择需要配置的策略";
            ////spc.AutoRunning = true;
            ////spc.DailyStartTime = "09:07:00";
            ////spc.DailyEndTime = "23:59:58";
            ////spc.ExpiredTime = DateTime.Parse("2049/12/31");
            ////spc.Creator = "群主";
            ////spc.CreateTime = DateTime.Now;
            this.propertyGrid1.SelectedObject = spc;
        }

        private void frm_CommDBObjectsSetting_Load(object sender, EventArgs e)
        {
            Dictionary<string, T> AllList = null;
            if (OuterList!=null)
            {
                AllList = OuterList;
            }
            if (SpecList != null)
                AllList = SpecList;
            refreshGrid(AllList);
            if (SpecObject != null)
                this.propertyGrid1.SelectedObject = SpecObject;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            iDbFile srp = null;
            srp = (iDbFile)this.propertyGrid1.SelectedObject;
            if (srp == null)
            {
                MessageBox.Show(string.Format("请先双击列表选择需要修改的{0}或点击新增按钮新建{0}！",strClassName));
                return;
            }
            ////if (Program.AllGlobalSetting.AllStrags.ContainsKey(srp))
            ////{
            ////    StragClass sc = Program.AllGlobalSetting.AllStrags[srp.GUID];
            ////    ////srp.StragName = sc.StragClassName;
            ////    ////srp.StragDescript = sc.StragScript;
            ////}
            if (!OuterList.ContainsKey(srp.strKeyValue()))
            {
                string keyval = srp.strKeyValue();
                OuterList.Add(keyval, (T)srp);
            }
            else
            {
                if (NewAPlan)
                {
                    MessageBox.Show(string.Format("该{0}已新建，标志错误！",strClassName));
                    return;
                }
                OuterList[srp.strKeyValue()] = (T)srp;
            }
            if (!SaveData()) return;
            refreshGrid(OuterList);
            this.propertyGrid1.SelectedObject = null;
            this.propertyGrid1.Refresh();
            MessageBox.Show("保存成功！");
        }

        bool SaveData()
        {

            //bool suc = GlobalClass.setStragRunningPlan(StragRunPlanClass.getXmlByObjectList<StragRunPlanClass>(Program.AllGlobalSetting.AllRunPlannings.Values.ToList<StragRunPlanClass>()));
            bool suc = UseObject.SaveDBFile(OuterList.Values.ToList<T>());
            if (!suc)
            {
                MessageBox.Show("保存数据错误，点击确定后将恢复较早前的数据！");
                refreshGrid(OuterList);
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
            string strUid = this.dataGridView1.CurrentRow.Cells[UseObject.strKey].Value.ToString();
            if (!OuterList.ContainsKey(strUid))
            {
                MessageBox.Show(string.Format("{0}已不存在，点击确定后将更新视图",UseObject.strKey));
                refreshGrid(OuterList);
                return;
            }
            this.propertyGrid1.SelectedObject = null;
            this.propertyGrid1.SelectedObject = OuterList[strUid];
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            iDbFile srp = this.propertyGrid1.SelectedObject as iDbFile;
            if (NewAPlan || srp == null)
            {
                MessageBox.Show(string.Format("请先双击列表选择需要删除{0}！",strClassName));
                return;
            }
            if (!OuterList.ContainsKey(srp.strKeyValue()))
            {
                MessageBox.Show(string.Format("{0}已不存在，点击确定后将更新视图",strClassName));
                refreshGrid(OuterList);
                return;
            }
            OuterList.Remove(srp.strKeyValue());
            if (!SaveData())
            {
                return;
            }
            refreshGrid(OuterList);
            MessageBox.Show("成功删除！");

        }
    }
}
