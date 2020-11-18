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
using WolfInv.com.BaseObjectsLib;
namespace PK10Server
{
    public partial class frm_NewStrag<T>: Form where T:TimeSerialData
    {
        bool DllLoaded = false;
        public bool Saved;
        public BaseStragClass<T> RetJson=null;
        public frm_NewStrag()
        {
            InitializeComponent();
        }
        Dictionary<string, Type> allTypes = null;
        private void frm_NewStrag_Load(object sender, EventArgs e)
        {
            //this.propertyGrid1.SelectedObject = new StragClass();
            string key = null;
            DataTypePoint dtp = GlobalClass.TypeDataPoints.First().Value;//.First().Key;
            if(dtp.IsSecurityData == 1)
            {
                key = "Security";
            }
            
            DataTable dt =BaseStragClass<T>.getAllStrags(key,ref allTypes);
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
                //StragClass scobj = StragClass.getStragByName(sc);
                Type sctype = allTypes[sc];

                var t = sctype.MakeGenericType(typeof(T));
                BaseStragClass<T>  scobj =   Activator.CreateInstance(t) as BaseStragClass<T>;
                /*
                BaseStragClass<T> scobj = Activator.CreateInstance(sctype) as BaseStragClass<T>;
                */
                if (scobj.CommSetting == null)
                    scobj.CommSetting = new SettingClass();
                scobj.CommSetting.SetGlobalSetting(Program<T>.AllGlobalSetting.gc);
                this.propertyGrid1.SelectedObject = scobj;
                this.propertyGrid1.Refresh();
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            BaseStragClass<T> sc = this.propertyGrid1.SelectedObject as BaseStragClass<T>;
            if (sc == null) return;
            //sc.CommSetting.SetGlobalSetting(Program<T>.gc);
            RetJson = sc;
            Saved = true;
            this.Close();
        }

        
    }
}
