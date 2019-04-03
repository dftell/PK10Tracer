using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WolfInv.com.Strags;
using System.Drawing.Design;
using WolfInv.com.ExchangeLib;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.ExchangeLib
{
    public partial class RunPlanPicker : UserControl
    {
        StragRunPlanClass<TimeSerialData>[] _Plans;
        [Description("策略运行计划")]
        [Editor(typeof(CommPickerEditor<StragRunPlanClass<TimeSerialData>>), typeof(UITypeEditor))] 
        public StragRunPlanClass<TimeSerialData>[] Plans
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
            StragRunPlanClass<TimeSerialData> srp = new StragRunPlanClass<TimeSerialData>();
            Dictionary<string,StragRunPlanClass<TimeSerialData>> list = srp.getAllListFromDb<StragRunPlanClass<TimeSerialData>>().ToDictionary(p=>p.GUID,p=>p);
            CommSelectOjectDialog<StragRunPlanClass<TimeSerialData>> frm = new CommSelectOjectDialog<StragRunPlanClass<TimeSerialData>>(list,"GUID",true,"策略运行计划");
            frm.ShowDialog(this.Parent);
            _Plans = frm.SelectObjects;
            if (_Plans == null) return;
            this.textBox1.Text = string.Join("/", _Plans.Select(p => p.Plan_Name).ToArray());

        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            this._Plans = new StragRunPlanClass<TimeSerialData>[]{};
            this.textBox1.Text = "";
        }
    }
}
