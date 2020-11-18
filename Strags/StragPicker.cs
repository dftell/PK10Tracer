using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WolfInv.com.BaseObjectsLib;
namespace WolfInv.com.Strags
{
    public partial class StragPicker<T> : Form where T:TimeSerialData
    {
        BaseStragClass<T> _SelectStrag = null;
        Dictionary<string, BaseStragClass<T>> AllList;
        public BaseStragClass<T> SelectedStrag
        {
            get
            {
                return _SelectStrag;
            }
        }
        public StragPicker(List<BaseStragClass<T>> strags)
        {
            InitializeComponent();
            AllList = strags.ToDictionary(t => t.GUID, t=>t);
            DataTable dt = BaseStragClass<T>.GetTableByStragList(strags);
            this.dataGridView1.DataSource = dt;
            this.dataGridView1.Refresh();
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (this.dataGridView1.CurrentRow.Index < 0)
                return;
            string guid = this.dataGridView1.CurrentRow.Cells[0].Value.ToString();
            if (!AllList.ContainsKey(guid))
            {
                return;
            }
            this._SelectStrag = AllList[guid];
            this.Close();
        }
    }
}
