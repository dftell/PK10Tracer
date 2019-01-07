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
    public partial class frm_NewStrag : Form
    {
        bool DllLoaded = false;
        public bool Saved;
        public StragClass RetJson=null;
        public frm_NewStrag()
        {
            InitializeComponent();
        }

        private void frm_NewStrag_Load(object sender, EventArgs e)
        {
            //this.propertyGrid1.SelectedObject = new StragClass();
            DataTable dt =StragClass.getAllStrags();
            DataView dv = new DataView(dt);
            this.ddl_StragObjects.DataSource = dt;
            this.ddl_StragObjects.DisplayMember = "text";
            this.ddl_StragObjects.ValueMember = "value";
            //this.ddl_StragObjects.Tag = dt;
            DllLoaded = true;
        }

        private void ddl_StragObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!DllLoaded) return;
            if (this.ddl_StragObjects.SelectedValue != null)
            {
                ////string strName = (this.ddl_StragObjects.SelectedValue as DataRowView).Row.ItemArray[1].ToString();
                ////this.propertyGrid1.SelectedObject = StragClass.getStragByName(strName);
                //this.propertyGrid1.SelectedObject = StragClass.getStragByName(strName);
                string sc = this.ddl_StragObjects.SelectedValue.ToString();
                StragClass scobj = StragClass.getStragByName(sc);
                scobj.CommSetting.SetGlobalSetting(Program.AllGlobalSetting.gc);
                this.propertyGrid1.SelectedObject = scobj;
                this.propertyGrid1.Refresh();
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            StragClass sc = this.propertyGrid1.SelectedObject as StragClass;
            if (sc == null) return;
            //sc.CommSetting.SetGlobalSetting(Program.gc);
            RetJson = sc;
            Saved = true;
            this.Close();
        }

        
    }
}
