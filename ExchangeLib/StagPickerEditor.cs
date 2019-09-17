using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using WolfInv.com.BaseObjectsLib;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using WolfInv.com.Strags;
namespace WolfInv.com.ExchangeLib
{
    public class StagPickerEditor : UITypeEditor
    {
        List<BaseStragClass<TimeSerialData>> AllList;
        Dictionary<string,StragRunPlanClass<TimeSerialData>> AllPlans;
        IWindowsFormsEditorService editorService;
        StragPicker picker ;
        public StagPickerEditor()
        {
            
            //picker = new StragsPicker(list.ToList<StragClass>());
        }

       

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
	        {
		        this.editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
	        }
            if (this.editorService != null)
            {
                AllList = BaseStragClass<TimeSerialData>.getObjectListByXml<BaseStragClass<TimeSerialData>>(GlobalClass.ReReadStragList());
                AllPlans = StragRunPlanClass<TimeSerialData>.getObjectListByXml<StragRunPlanClass<TimeSerialData>>(GlobalClass.getStragRunningPlan(true)).ToDictionary(t => t.GUID, t => t);
                List<BaseStragClass<TimeSerialData>> list = AllList.Where(t => AllPlans.ContainsKey(t.GUID) == false).ToList<BaseStragClass<TimeSerialData>>();
                List<StragClass> list1 = new List<StragClass>();

                list.ForEach(a=> {
                    //list1.Add(ConvertionExtensions.ConvertTo<StragClass>(a as IConvertible));
                    StragClass sc = a as StragClass;
                    list1.Add(sc);
                    });
                picker = new StragPicker(list1);//支持一组合对多相同策略
                editorService.ShowDialog(picker);
                if (picker.SelectedStrag == null)
                {
                    value = null;
                }
                else
                {
                    value = picker.SelectedStrag;
                    //picker.Hide();
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
