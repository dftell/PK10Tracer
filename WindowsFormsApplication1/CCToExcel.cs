using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.IO;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace BackTestSys
{

    public class CExcel
    {
        /// <summary>
        /// 导出Excel文件
        /// </summary>
        /// <param name="lvwShow">数据源</param>
        /// <param name="strExcelTitle">Excel文件名</param>
        public void LvwToExcel(ListView lvwShow, string strExcelName)
        {
            int row = lvwShow.Items.Count;//listview行数
            int col = lvwShow.Items[0].SubItems.Count;//listview列数

            if (row == 0 || string.IsNullOrEmpty(strExcelName))
            {
                return;
            }
            if (row > 0)
            {
                Microsoft.Office.Interop.Excel._Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
                if (ExcelApp == null)
                {
                    MessageBox.Show("无法创建Excel对象，可能你的系统没有安装Excel！！!");
                    return;
                }
                object m_objOpt = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Excel.Workbooks ExcleBooks = (Microsoft.Office.Interop.Excel.Workbooks)ExcelApp.Workbooks;
                Microsoft.Office.Interop.Excel._Workbook ExcleBook = (Microsoft.Office.Interop.Excel._Workbook)(ExcleBooks.Add(m_objOpt));
                Microsoft.Office.Interop.Excel._Worksheet ExcelSheet = (Microsoft.Office.Interop.Excel._Worksheet)ExcleBook.ActiveSheet;
                ExcelApp.Visible = true;
                //读取listview表头做为excel列标题，listview中，行和列的下标索引都是从0开始
                for (int i = 1; i <= col; i++)
                {
                    ExcelSheet.Cells[1, i] = lvwShow.Columns[i - 1].Text;
                }
                for (int i = 0; i < row; i++)
                {
                    ExcelSheet.Cells[i + 2, 1] = lvwShow.Items[i].Text;//获取所有行第一列的值
                    for (int j = 1; j < col; j++)
                    {
                        ExcelSheet.Cells[i + 2, j + 1] = lvwShow.Items[i].SubItems[j].Text;//获取某一行某一列的值
                    }
                }
                ExcleBook.SaveAs(strExcelName);
                //ExcelApp.Quit();//退出excel
                ExcelApp = null;
                ExcelSheet = null;
                ExcleBooks = null;
                ExcleBook = null;
            }
        }
        /// <summary>
        /// 保存Excel文件
        /// </summary>
        public void ExportExcel(ListView lvwShow)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "xls";
            sfd.Filter = "Excel文件(*.xls)|*.xls";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                LvwToExcel(lvwShow, sfd.FileName);
            }
        }

        public void ExportCSV(ListView lvwShow)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "csv";
            sfd.Filter = "CSV文件(*.csv)|*.csv";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                LvwToCSV(lvwShow, sfd.FileName);
            }
        }
        /// <summary>
        /// 保存Excel文件
        /// </summary>
        public void ExportExcel(DataGridView dg)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "xls";
            sfd.Filter = "Excel文件(*.xls)|*.xls";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                LvwToExcel(dg, sfd.FileName);
            }
        }

        public void ExportExcel(DataView dg)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "xls";
            sfd.Filter = "Excel文件(*.xls)|*.xls";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ToExcel(dg, sfd.FileName);
            }
        }

        public void ExportExcel(System.Data.DataTable dg)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "xls";
            sfd.Filter = "Excel文件(*.xls)|*.xls";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ToExcel(dg, sfd.FileName);
            }
        }

        public System.Data.DataTable ImportExcel(DataGridView dgv)
        {
            OpenFileDialog sfd = new OpenFileDialog();
            sfd.DefaultExt = "xls,xlsx";
            sfd.Multiselect = false;
            sfd.Filter = "Excel文件(*.xls;*.xlsx)|*.xls;*.xlsx";
            Form frm = (dgv.TopLevelControl as Form);
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (frm != null)
                    frm.Cursor = Cursors.WaitCursor;
                dgv.DataSource = FromExcel(sfd.FileName);
                if (frm != null)
                    frm.Cursor = Cursors.Default;
                return dgv.DataSource as System.Data.DataTable;
            }
            return null;
        }

        /// <summary>
        /// 保存为csv
        /// </summary>
        /// <param name="dg"></param>
        public void ExportCSV(DataGridView dg)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "csv";
            sfd.Filter = "CSV文件(*.csv)|*.csv";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                LvwToExcel(dg, sfd.FileName);
            }
        }

        /// <summary>
        /// 导出Excel文件
        /// </summary>
        /// <param name="lvwShow">数据源</param>
        /// <param name="strExcelTitle">Excel文件名</param>
        public void LvwToExcel(DataGridView dg, string strExcelName)
        {
            int row = dg.RowCount;// .Items.Count;//listview行数
            int col = dg.ColumnCount;// lvwShow.Items[0].SubItems.Count;//listview列数

            if (row == 0 || string.IsNullOrEmpty(strExcelName))
            {
                return;
            }
            if (row > 0)
            {
                Microsoft.Office.Interop.Excel._Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
                if (ExcelApp == null)
                {
                    MessageBox.Show("无法创建Excel对象，可能你的系统没有安装Excel！！!");
                    return;
                }
                object m_objOpt = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Excel.Workbooks ExcleBooks = (Microsoft.Office.Interop.Excel.Workbooks)ExcelApp.Workbooks;
                Microsoft.Office.Interop.Excel._Workbook ExcleBook = (Microsoft.Office.Interop.Excel._Workbook)(ExcleBooks.Add(m_objOpt));
                Microsoft.Office.Interop.Excel._Worksheet ExcelSheet = (Microsoft.Office.Interop.Excel._Worksheet)ExcleBook.ActiveSheet;
                ExcelApp.Visible = true;
                //读取listview表头做为excel列标题，listview中，行和列的下标索引都是从0开始
                for (int i = 1; i <= col; i++)
                {
                    ExcelSheet.Cells[1, i] = dg.Columns[i - 1].HeaderText;// lvwShow.Columns[i - 1].Text;
                }
                for (int i = 0; i < row; i++)
                {
                    //ExcelSheet.Cells[i + 2, 1] = dg.Rows[i].Cells[0]; lvwShow.Items[i].Text;//获取所有行第一列的值
                    for (int j = 0; j < col; j++)
                    {
                        ExcelSheet.Cells[i + 2, j + 1] = dg.Rows[i].Cells[j].Value;// lvwShow.Items[i].SubItems[j].Text;//获取某一行某一列的值
                    }
                }
                ExcleBook.SaveAs(strExcelName);
                //ExcelApp.Quit();//退出excel
                ExcelApp = null;
                ExcelSheet = null;
                ExcleBooks = null;
                ExcleBook = null;
            }
        }


        public void ToExcel(DataView dv, string strExcelName)
        {
            System.Data.DataTable dg = dv.Table;
            int row = dg.Rows.Count;// .Items.Count;//listview行数
            int col = dg.Columns.Count;// lvwShow.Items[0].SubItems.Count;//listview列数

            if (row == 0 || string.IsNullOrEmpty(strExcelName))
            {
                return;
            }
            if (row > 0)
            {
                Microsoft.Office.Interop.Excel._Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
                if (ExcelApp == null)
                {
                    MessageBox.Show("无法创建Excel对象，可能你的系统没有安装Excel！！!");
                    return;
                }
                object m_objOpt = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Excel.Workbooks ExcleBooks = (Microsoft.Office.Interop.Excel.Workbooks)ExcelApp.Workbooks;
                Microsoft.Office.Interop.Excel._Workbook ExcleBook = (Microsoft.Office.Interop.Excel._Workbook)(ExcleBooks.Add(m_objOpt));
                Microsoft.Office.Interop.Excel._Worksheet ExcelSheet = (Microsoft.Office.Interop.Excel._Worksheet)ExcleBook.ActiveSheet;
                ExcelApp.Visible = true;
                //读取listview表头做为excel列标题，listview中，行和列的下标索引都是从0开始
                for (int i = 1; i <= col; i++)
                {
                    ExcelSheet.Cells[1, i] = dg.Columns[i - 1].ColumnName;// lvwShow.Columns[i - 1].Text;
                }
                for (int i = 0; i < row; i++)
                {
                    //ExcelSheet.Cells[i + 2, 1] = dg.Rows[i].Cells[0]; lvwShow.Items[i].Text;//获取所有行第一列的值
                    for (int j = 0; j < col; j++)
                    {
                        ExcelSheet.Cells[i + 2, j + 1] = dg.Rows[i][j].ToString();// lvwShow.Items[i].SubItems[j].Text;//获取某一行某一列的值
                    }
                }
                ExcleBook.SaveAs(strExcelName);
                //ExcelApp.Quit();//退出excel
                ExcelApp = null;
                ExcelSheet = null;
                ExcleBooks = null;
                ExcleBook = null;
            }
        }

        public void ToExcel(System.Data.DataTable dg, string strExcelName)
        {
            int row = dg.Rows.Count;// .Items.Count;//listview行数
            int col = dg.Columns.Count;// lvwShow.Items[0].SubItems.Count;//listview列数

            if (row == 0 || string.IsNullOrEmpty(strExcelName))
            {
                return;
            }
            if (row > 0)
            {
                Microsoft.Office.Interop.Excel._Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
                if (ExcelApp == null)
                {
                    MessageBox.Show("无法创建Excel对象，可能你的系统没有安装Excel！！!");
                    return;
                }
                object m_objOpt = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Excel.Workbooks ExcleBooks = (Microsoft.Office.Interop.Excel.Workbooks)ExcelApp.Workbooks;
                Microsoft.Office.Interop.Excel._Workbook ExcleBook = (Microsoft.Office.Interop.Excel._Workbook)(ExcleBooks.Add(m_objOpt));
                Microsoft.Office.Interop.Excel._Worksheet ExcelSheet = (Microsoft.Office.Interop.Excel._Worksheet)ExcleBook.ActiveSheet;
                ExcelApp.Visible = true;
                //读取listview表头做为excel列标题，listview中，行和列的下标索引都是从0开始
                for (int i = 1; i <= col; i++)
                {
                    ExcelSheet.Cells[1, i] = dg.Columns[i - 1].ColumnName;// lvwShow.Columns[i - 1].Text;
                }
                for (int i = 0; i < row; i++)
                {
                    //ExcelSheet.Cells[i + 2, 1] = dg.Rows[i].Cells[0]; lvwShow.Items[i].Text;//获取所有行第一列的值
                    for (int j = 0; j < col; j++)
                    {
                        ExcelSheet.Cells[i + 2, j + 1] = dg.Rows[i][j].ToString();// lvwShow.Items[i].SubItems[j].Text;//获取某一行某一列的值
                    }
                }
                ExcleBook.SaveAs(strExcelName);
                //ExcelApp.Quit();//退出excel
                ExcelApp = null;
                ExcelSheet = null;
                ExcleBooks = null;
                ExcleBook = null;
            }
        }

        public System.Data.DataTable FromExcel(string strExcelName)
        {

            System.Data.DataTable dg = new System.Data.DataTable();
            Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
            Workbooks ExcelBooks;
            _Workbook ExcelBook;
            try
            {
                ExcelApp.DisplayAlerts = false;
                ExcelApp.Visible = false;
                ExcelApp.UserControl = true;
                if (File.Exists(strExcelName) == false)
                {
                    return dg;
                }
                object m_objOpt = System.Reflection.Missing.Value;
                
                ExcelBooks = ExcelApp.Workbooks;
                ExcelBook = ExcelBooks.Open(strExcelName);
                if (ExcelBook == null)
                {
                    return dg;
                }
                _Worksheet ExcelSheet = ExcelBook.ActiveSheet;

                int row = ExcelSheet.UsedRange.Rows.Count;// rows
                int col = ExcelSheet.UsedRange.Columns.Count;// colums;
                List<Type> allCols = new List<Type>(); 
                for (int i = 1; i <= col; i++)
                {
                    string id= (ExcelSheet.Cells[1, i] as Microsoft.Office.Interop.Excel.Range).Value;
                    dg.Columns.Add(id);
                    allCols.Add(typeof(string));
                }
                if(row == 1)
                {
                    return dg;
                }
                for (int j = 0; j < col; j++)
                {
                    Object obj = (ExcelSheet.Cells[2, j + 1] as Microsoft.Office.Interop.Excel.Range).Value;
                    if (obj != null)
                    {
                        double dl = 0;
                        //DateTime dt;
                        //if (DateTime.TryParse(obj.ToString(), out dt))
                        //{
                        //    allCols[j] = typeof(DateTime);
                        //    continue;
                        //}
                        int test = 0;
                        if (int.TryParse(obj.ToString(), out test))
                        {
                            allCols[j] = typeof(int);
                            continue;
                        }
                        if (double.TryParse(obj.ToString(), out dl))
                        {
                            allCols[j] = typeof(double);
                            continue;
                        }

                        allCols[j] = typeof(string);
                    }
                }
                try
                {
                    for (int i = 0; i < allCols.Count; i++)
                    {
                        dg.Columns[i].DataType = allCols[i];
                    }
                }
                catch (Exception teste)
                {

                }
                for (int i = 1; i < row; i++)
                {
                    DataRow dr = dg.NewRow();
                    for (int j = 0; j < col; j++)
                    {
                        Object obj = (ExcelSheet.Cells[1 + i, j + 1] as Microsoft.Office.Interop.Excel.Range).Value;
                        dr[j] = obj;
                    }
                    dg.Rows.Add(dr);
                }
                ExcelBook.Close();
                return dg;
            }
            catch (Exception ce)
            {

            }
            finally
            {
                ExcelBook = null;
                ExcelApp.Quit();
                ExcelCommLib.Kill(ExcelApp);
                ExcelApp = null;
            }
            return dg;
        }


        public void LvwToCSV(DataGridView dg, string strExcelName, string defaultSplit = null)
        {
            string splitChar = "|";
            if (defaultSplit != null)
            {
                splitChar = defaultSplit;
            }
            int row = dg.RowCount;// .Items.Count;//listview行数
            int col = dg.ColumnCount;// lvwShow.Items[0].SubItems.Count;//listview列数

            if (row == 0 || string.IsNullOrEmpty(strExcelName))
            {
                return;
            }
            if (row > 0)
            {

                StringBuilder sb = new StringBuilder();
                //读取listview表头做为excel列标题，listview中，行和列的下标索引都是从0开始
                List<string> writeLine = new List<string>();
                for (int i = 1; i <= col; i++)
                {
                    writeLine.Add(dg.Columns[i - 1].HeaderText);
                    //ExcelSheet.Cells[1, i] = dg.Columns[i - 1].HeaderText;// lvwShow.Columns[i - 1].Text;
                }
                sb.AppendLine(string.Join(splitChar, writeLine));
                for (int i = 0; i < row; i++)
                {
                    writeLine = new List<string>();
                    //ExcelSheet.Cells[i + 2, 1] = dg.Rows[i].Cells[0]; lvwShow.Items[i].Text;//获取所有行第一列的值
                    for (int j = 0; j < col; j++)
                    {
                        writeLine.Add(dg.Rows[i].Cells[j].Value?.ToString());
                        //ExcelSheet.Cells[i + 2, j + 1] = dg.Rows[i].Cells[j].Value;// lvwShow.Items[i].SubItems[j].Text;//获取某一行某一列的值
                    }
                    sb.AppendLine(string.Join(splitChar, writeLine));
                }
                saveCsv(strExcelName, sb.ToString());
            }
        }

        public void LvwToCSV(System.Data.DataTable dt, string strExcelName, string defaultSplit = null)
        {
            string splitChar = "|";
            if (defaultSplit != null)
            {
                splitChar = defaultSplit;
            }
            int row = dt.Rows.Count;// .Items.Count;//listview行数
            int col = dt.Columns.Count;// lvwShow.Items[0].SubItems.Count;//listview列数
            StringBuilder sb = new StringBuilder();
            List<string> writeLine = new List<string>();
            if (string.IsNullOrEmpty(strExcelName))
            {
                return;
            }
            for (int i = 1; i <= col; i++)
            {
                writeLine.Add(dt.Columns[i].ColumnName);
            }
            sb.AppendLine(string.Join(splitChar, writeLine));
            for (int i = 0; i < row; i++)
            {
                writeLine = new List<string>();
                for (int j = 0; j < col; j++)
                {
                    writeLine.Add(dt.Rows[i][j] == null ? "" : dt.Rows[i][j].ToString());
                }
                sb.AppendLine(string.Join(splitChar, writeLine));
            }
            saveCsv(strExcelName, sb.ToString());
        }


        public void LvwToCSV(ListView lvwShow, string strExcelName, string defaultSplit = null)
        {
            string splitChar = "|";
            if (defaultSplit != null)
            {
                splitChar = defaultSplit;
            }
            int row = lvwShow.Items.Count;//listview行数
            int col = lvwShow.Items[0].SubItems.Count;//listview列数

            if (row == 0 || string.IsNullOrEmpty(strExcelName))
            {
                return;
            }
            if (row > 0)
            {
                StringBuilder sb = new StringBuilder();
                //读取listview表头做为excel列标题，listview中，行和列的下标索引都是从0开始
                List<string> writeLine = new List<string>();
                //读取listview表头做为excel列标题，listview中，行和列的下标索引都是从0开始
                for (int i = 1; i <= col; i++)
                {
                    //ExcelSheet.Cells[1, i] = lvwShow.Columns[i - 1].Text;
                    writeLine.Add(lvwShow.Columns[i - 1].Text);
                }
                sb.AppendLine(string.Join(splitChar, writeLine));
                writeLine = new List<string>();
                for (int i = 0; i < row; i++)
                {
                    //ExcelSheet.Cells[i + 2, 1] = lvwShow.Items[i].Text;//获取所有行第一列的值
                    writeLine.Add(lvwShow.Items[i].Text);
                    for (int j = 1; j < col; j++)
                    {
                        //ExcelSheet.Cells[i + 2, j + 1] = lvwShow.Items[i].SubItems[j].Text;//获取某一行某一列的值
                        writeLine.Add(lvwShow.Items[i].SubItems[j].Text);
                    }
                    sb.AppendLine(string.Join(splitChar, writeLine));
                }
                saveCsv(strExcelName, sb.ToString());
            }
        }

        void saveCsv(string strExcelName, string sb)
        {
            try
            {

                StreamWriter sr = new StreamWriter(strExcelName, false);
                sr.WriteLine(string.Format("{0}", sb));
                sr.Close();
            }
            catch (Exception ce)
            {
                MessageBox.Show(ce.StackTrace, ce.Message);
            }
        }

    }

    public class ExcelCommLib
    {

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        public static void Kill(Microsoft.Office.Interop.Excel.Application excel)
        {
            IntPtr t = new IntPtr(excel.Hwnd);//得到这个句柄，具体作用是得到这块内存入口    
            int k = 0;
            GetWindowThreadProcessId(t, out k);   //得到本进程唯一标志k   
            System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(k);   //得到对进程k的引用   
            p.Kill();     //关闭进程k  
        }


    }
}
