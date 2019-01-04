using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.ComponentModel;
using BaseObjectsLib;
using System.Reflection;
using LogLib;
namespace Strags
{

    public class CommPickerEditor<T> : UITypeEditor
    {
        protected Dictionary<string,T> AllList;
        protected string key = "";
        protected bool AllowMutliSelect;
        string strClassName;
        //protected string dbFile = "";
        IWindowsFormsEditorService editorService;
        CommSelectOjectDialog<T> picker;
        public CommPickerEditor()
        {
        }
        void Init()
        {
            //picker = new StragsPicker(list.ToList<StragClass>());
            Type CurrType = typeof(T);
            T Currobj = (T)Activator.CreateInstance(CurrType);
            if (Currobj is iDbFile)
            {
                iDbFile obj = Currobj as iDbFile;
                List<T> list = obj.getAllListFromDb<T>();
                key = obj.strKey;
                AllowMutliSelect = obj.AllowMutliSelect;
                strClassName = obj.strObjectName;
                AllList = new Dictionary<string, T>();
                for (int i = 0; i < list.Count; i++)
                {
                    T item = list[i];
                    MemberInfo[] info = CurrType.GetMember(key);
                    if (info == null || info.Length != 1)
                    {
                        throw new Exception(string.Format("定义的db存储类没有关键字变量{0}",key));
                    }
                    object val = null;
                    if (info[0].MemberType == MemberTypes.Property)
                    {
                        val = (info[0] as PropertyInfo).GetValue(item, null);
                    }
                    else
                    {
                        val = (info[0] as FieldInfo).GetValue(item);
                    }
                    if(val == null)
                        throw new Exception(string.Format("定义的db存储类关键字变量{0}值为空", key));
                    string keyval = val.ToString();
                    if (AllList.ContainsKey(keyval))
                    {
                        throw new Exception(string.Format("定义的db存储类没有关键字变量{0}的值{1}重复", key,keyval));
                    }
                    else
                        AllList.Add(keyval, item);
                }
            }
        }



        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                this.editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            }
            if (this.editorService != null)
            {
                if (AllList == null) Init();
                picker = new CommSelectOjectDialog<T>(AllList, key, AllowMutliSelect,strClassName);
                editorService.ShowDialog(picker);
                if (picker.SelectObjects == null || picker.SelectObjects.Length == 0)
                {
                    value = null;
                }
                else
                {
                    if (AllowMutliSelect)
                    {
                        value = picker.SelectObjects;
                    }
                    else
                    {
                        T[] ret = picker.SelectObjects;
                        if (ret == null || ret.Length == 0)
                            return null;
                        value = ret[0];
                    }
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
