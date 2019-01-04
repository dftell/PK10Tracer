using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
namespace BaseObjectsLib
{
    public partial class SerialableObjectEditor<T>: Form
    {
        T obj = default(T);
        public T SelectedObject
        {
            get
            {
                return obj;
            }
        }

        public SerialableObjectEditor()
        {
            InitializeComponent();
            Type t = typeof(T);
            obj = (T)Convert.ChangeType(Activator.CreateInstance(t),typeof(T));
            string strLblName = "编辑{0}对象";
            string strName = t.Name;
            DisplayNameAttribute att = Attribute.GetCustomAttribute(t, typeof(DisplayNameAttribute)) as DisplayNameAttribute;
            if (att != null)
            {
                strName = att.DisplayName;
            }
            this.lbl_EditorName.Text = string.Format(strLblName, strName); 
            this.propertyGrid1.SelectedObject = obj;
        }

        public void FillObject(T o)
        {
            obj = o;
            this.propertyGrid1.SelectedObject = obj;
            this.propertyGrid1.Refresh();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            obj = (T)this.propertyGrid1.SelectedObject;
            this.Hide();
            this.Visible = false;
            //this.Close();
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            btn_ok_Click(null, null);
        }
    }
}
