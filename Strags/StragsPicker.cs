using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Strags
{
    public partial class StragsPicker : UserControl
    {

        StragClass _SelectStrag = null;
        Dictionary<string, StragClass> AllList;
        public StragClass SelectStrag
        {
            get
            {
                return _SelectStrag;
            }
        }

        public StragsPicker()
        {
        }

        public StragsPicker(List<StragClass> strags)
        {
            InitializeComponent();
            AllList = strags.ToDictionary(t => t.GUID, t => t);
            DataTable dt = StragClass.GetTableByStragList(strags);
            this.dataGridView1.DoubleClick+=new EventHandler(dataGridView1_DoubleClick);
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
            this.Visible = false;
            
        }
    }
}
