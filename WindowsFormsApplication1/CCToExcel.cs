using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.IO;

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
                    ExcelSheet.Cells[1, i] = dg.Columns[i-1].HeaderText;// lvwShow.Columns[i - 1].Text;
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
            DataTable dg = dv.Table;
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


        public void LvwToCSV(DataGridView dg, string strExcelName,string defaultSplit=null)
        {
            string splitChar = "|";
            if(defaultSplit!= null)
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

        public void LvwToCSV(DataTable dt, string strExcelName, string defaultSplit = null)
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

        void saveCsv(string strExcelName,string sb)
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
}
