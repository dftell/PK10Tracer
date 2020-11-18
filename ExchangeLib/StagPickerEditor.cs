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
    public class StagPickerEditor<T> : UITypeEditor where T:TimeSerialData
    {
        List<BaseStragClass<T>> AllList;
        Dictionary<string,StragRunPlanClass<T>> AllPlans;
        IWindowsFormsEditorService editorService;
        StragPicker<T> picker ;
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
                AllList = BaseStragClass<T>.getObjectListByXml<BaseStragClass<T>>(GlobalClass.ReReadStragList());
                AllPlans = StragRunPlanClass<T>.getObjectListByXml<StragRunPlanClass<T>>(GlobalClass.getStragRunningPlan(true)).ToDictionary(t => t.GUID, t => t);
                List<BaseStragClass<T>> list = AllList.Where(t => AllPlans.ContainsKey(t.GUID) == false).ToList<BaseStragClass<T>>();
                List<BaseStragClass<T>> list1 = new List<BaseStragClass<T>>();
                /*
                list.ForEach(a=> {
                    //list1.Add(ConvertionExtensions.ConvertTo<StragClass>(a as IConvertible));
                    //BaseStragClass<T> sc = a as BaseStragClass<T>;
                    list1.Add(a);
                    });*/
                picker = new StragPicker<T>(list);//支持一组合对多相同策略
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
