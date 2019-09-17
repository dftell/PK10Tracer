using System;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;
namespace WolfInv.com.ExchangeLib
{
    public class TimePickerEditor : UITypeEditor
    {
	    IWindowsFormsEditorService editorService;
	    DateTimePicker picker = new DateTimePicker();

	    public TimePickerEditor()
	    {
	        picker.Format = DateTimePickerFormat.Custom;
	        picker.CustomFormat = "HH:mm";
	        picker.ShowUpDown = true;
	    }

	    public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
	    {
	        return UITypeEditorEditStyle.DropDown;//显示下拉按钮
	    }

	    public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
	    {
	        string time;
	        if (provider != null)
	        {
		    this.editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
	        }
	        if (this.editorService != null)
	        {
		        if (value == null)
		        {
		            picker.Value = DateTime.Parse("00:00" as string);
		        }
		        else
		        {
		            time = value.ToString();
		            //picker.Value = DateTime.Parse(time);
		        }
		        this.editorService.DropDownControl(picker);
		        value = picker.Value.ToShortTimeString();
	        }
	        return value;
	    }
    }

  
}
