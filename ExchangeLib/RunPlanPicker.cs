using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Strags;
using System.Drawing.Design;
namespace ExchangeLib
{
    public partial class RunPlanPicker : UserControl
    {
        StragRunPlanClass[] _Plans;
        [Description("策略运行计划")]
        [Editor(typeof(CommPickerEditor<StragRunPlanClass>), typeof(UITypeEditor))] 
        public StragRunPlanClass[] Plans
        {
            get
            {
                return _Plans;
            }
            set
            {
                _Plans = value;
            }
        }


        public RunPlanPicker()
        {
            InitializeComponent();
            this.textBox1.ReadOnly = true;
        }

        private void btn_select_Click(object sender, EventArgs e)
        {
            StragRunPlanClass srp = new StragRunPlanClass();
            Dictionary<string,StragRunPlanClass> list = srp.getAllListFromDb<StragRunPlanClass>().ToDictionary(p=>p.GUID,p=>p);
            CommSelectOjectDialog<StragRunPlanClass> frm = new CommSelectOjectDialog<StragRunPlanClass>(list,"GUID",true,"策略运行计划");
            frm.ShowDialog(this.Parent);
            _Plans = frm.SelectObjects;
            this.textBox1.Text = string.Join("/", _Plans.Select(p => p.Plan_Name).ToArray());

        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            this._Plans = new StragRunPlanClass[]{};
            this.textBox1.Text = "";
        }
    }
}
