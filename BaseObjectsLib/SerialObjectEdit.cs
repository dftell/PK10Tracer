using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.ComponentModel;
namespace BaseObjectsLib
{
    public class SerialObjectEdit<T> :  UITypeEditor
    {
 
        IWindowsFormsEditorService editorService;
        //SerialableObjectEditor<T> picker;
        public SerialObjectEdit()
        {
            
        }

   
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                this.editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            }
            if (this.editorService != null)
            {
                SerialableObjectEditor<T> picker = new SerialableObjectEditor<T>();
                picker.FillObject((T)value);
                editorService.ShowDialog(picker);
                picker.Show();
                
                if (picker.SelectedObject == null)
                {
                    value = default(T);
                }
                else
                {
                    value = picker.SelectedObject;
                    picker.Hide();
                }
                ////if (value == null)
                ////{
                ////    picker.Show();
                ////    picker.Visible = true;
                ////}
                ////else
                ////{
                ////}
            }
            return value;
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return false;
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;//显示下拉按钮
        }
    }


}
