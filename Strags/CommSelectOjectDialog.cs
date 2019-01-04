using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseObjectsLib;
namespace Strags
{
    public partial class CommSelectOjectDialog<T> : Form
    {
        T[] _selectedObjects;
        bool _AllowMutliSelect;
        string keyname = "";
        public bool AllowMutliSelect { get { return _AllowMutliSelect; } }
        string strClassName;
        public CommSelectOjectDialog()
        {
            InitializeComponent();
        }

        Dictionary<string,T> AllList;

        public T[] SelectObjects
        {
            get
            {
                return _selectedObjects;
            }
        }

        public CommSelectOjectDialog(Dictionary<string,T> list,string KeyName,bool AllowMultiObjects,string colname)
        {
            InitializeComponent();
            FillGrid(list);
            AllList = list;
            keyname = KeyName;
            _AllowMutliSelect = AllowMultiObjects;
            this.dg_Datas.MultiSelect = AllowMultiObjects;
            this.lbl_Title.Text = string.Format("选择{1}{0}", colname,_AllowMutliSelect?"1个":"1个或多个");
            this.Text = string.Format("{0}选择器",colname);
        }

        public void FillGrid(Dictionary<string, T> list)
        {
            DataTable dt = DisplayAsTableClass.ToTable<T>(list.Values.ToList<T>());
            dg_Datas.DataSource = dt;
            dg_Datas.Refresh();
        }

        private void btn_Selected_Click(object sender, EventArgs e)
        {
            if (this.dg_Datas.CurrentRow.Index < 0)
                return;
            if (_AllowMutliSelect)
            {
                List<T> ret = new List<T>();
                for (int i = 0; i < this.dg_Datas.SelectedRows.Count; i++)
                {
                    string KeyVal = this.dg_Datas.SelectedRows[i].Cells[keyname].Value.ToString();
                    if (AllList.ContainsKey(KeyVal))
                    {
                        ret.Add(AllList[KeyVal]);
                    }
                }
                _selectedObjects = ret.ToArray();
                this.Close();
                return;
            }
            string guid = this.dg_Datas.CurrentRow.Cells[keyname].Value.ToString();
            if (!AllList.ContainsKey(guid))
            {
                MessageBox.Show(string.Format("{0}中不存在值为的{1}项。",keyname,guid));
                return;
            }
            this._selectedObjects = new T[]{AllList[guid]};
            this.Close();
        }
    }
}
