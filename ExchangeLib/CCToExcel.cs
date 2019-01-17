using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace ExchangeLib
{

    using System;
    using System.Data;
    using System.IO;
    using System.Web;
    using DocumentFormat.OpenXml;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Spreadsheet;

    public static class OpenXmlHelper
    {
        /// <summary>
        /// 导出Excel文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="dataSet">DataSet中每个DataTable生成一个Sheet</param>
        public static void ExportExcel(string fileName, DataSet dataSet)
        {
            if (dataSet.Tables.Count == 0)
            {
                return;
            }

            using (MemoryStream stream = DataTable2ExcelStream(dataSet))
            {
                FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate);
                stream.WriteTo(fs);
                fs.Flush();
                fs.Close();
            }
        }

        public static void ExportExcel(string fileName, DataTable dataTable)
        {
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(dataTable);
            ExportExcel(fileName, dataSet);
        }

        /// <summary>
        /// Web导出Excel文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="dataSet">DataSet中每个DataTable生成一个Sheet</param>
        public static void ResponseExcel(string fileName, DataSet dataSet)
        {
            if (dataSet.Tables.Count == 0)
            {
                return;
            }

            using (MemoryStream stream = DataTable2ExcelStream(dataSet))
            {
                ExportExcel(fileName, stream);
            }
        }

        public static void ResponseExcel(string fileName, DataTable dataTable)
        {
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(dataTable.Copy());
            ResponseExcel(fileName, dataSet);
        }

        private static void ExportExcel(string fileName, MemoryStream stream)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Charset = "UTF-8";
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename= " + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        private static MemoryStream DataTable2ExcelStream(DataSet dataSet)
        {
            MemoryStream stream = new MemoryStream();
            SpreadsheetDocument document = SpreadsheetDocument.Create(stream,
                SpreadsheetDocumentType.Workbook);

            WorkbookPart workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

            for (int i = 0; i < dataSet.Tables.Count; i++)
            {
                DataTable dataTable = dataSet.Tables[i];
                WorksheetPart worksheetPart = document.WorkbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                Sheet sheet = new Sheet
                {
                    Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = (UInt32)(i + 1),
                    Name = dataTable.TableName
                };
                sheets.Append(sheet);

                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                Row headerRow = CreateHeaderRow(dataTable.Columns);
                sheetData.Append(headerRow);

                for (int j = 0; j < dataTable.Rows.Count; j++)
                {
                    sheetData.Append(CreateRow(dataTable.Rows[j], j + 2));
                }
            }

            document.Close();

            return stream;
        }

        private static Row CreateHeaderRow(DataColumnCollection columns)
        {
            Row header = new Row();
            for (int i = 0; i < columns.Count; i++)
            {
                Cell cell = CreateCell(i + 1, 1, columns[i].ColumnName, CellValues.String);
                header.Append(cell);
            }
            return header;
        }

        private static Row CreateRow(DataRow dataRow, int rowIndex)
        {
            Row row = new Row();
            for (int i = 0; i < dataRow.Table.Columns.Count; i++)
            {
                Cell cell = CreateCell(i + 1, rowIndex, dataRow[i], GetType(dataRow.Table.Columns[i].DataType));
                row.Append(cell);
            }
            return row;
        }

        private static CellValues GetType(Type type)
        {
            if (type == typeof(decimal))
            {
                return CellValues.Number;
            }
            //if ((type == typeof(DateTime)))
            //{
            //    return CellValues.Date;
            //}
            return CellValues.SharedString;
        }

        private static Cell CreateCell(int columnIndex, int rowIndex, object cellValue, CellValues cellValues)
        {
            Cell cell = new Cell
            {
                CellReference = GetCellReference(columnIndex) + rowIndex,
                CellValue = new CellValue { Text = cellValue.ToString() },
                DataType = new EnumValue<CellValues>(cellValues),
                StyleIndex = 0
            };
            return cell;
        }

        private static string GetCellReference(int colIndex)
        {
            int dividend = colIndex;
            string columnName = String.Empty;

            while (dividend > 0)
            {
                int modifier = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modifier) + columnName;
                dividend = (dividend - modifier) / 26;
            }

            return columnName;
        }

        public static DataTable ImportExcel(string fileName)
        {
            DataTable dt = new DataTable();
            Stream stream = null;
            try
            {
                stream = File.Open(fileName, FileMode.Open);
                dt = ReadAnExcel(stream);
            }
            catch (Exception ce)
            {
                return dt;
            }
            finally
            {
                stream.Close();
            }
            return dt;
        }

     

        /// <summary>
        /// Excel流组织成Datatable
        /// </summary>
        /// <param name="stream">Excel文件流</param>
        /// <returns>DataTable</returns>
        public static DataTable ReadExcel(Stream stream)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(stream, false))     //若导入.xls格式的Excel则会出现【文件包含损坏的数据】的错误！
            {
                //打开Stream
                IEnumerable<Sheet> sheets = document.WorkbookPart.Workbook.Descendants<Sheet>();
                if (sheets.Count() == 0)
                {//找出符合条件的sheet，没有则返回
                    return null;
                }

                WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheets.First().Id);
                //获取Excel中共享数据
                SharedStringTable stringTable = document.WorkbookPart.SharedStringTablePart.SharedStringTable;
                IEnumerable<Row> rows = worksheetPart.Worksheet.Descendants<Row>();//得到Excel中得数据行

                DataTable dt = new DataTable("Excel");
                //因为需要将数据导入到DataTable中，所以我们假定Excel的第一行是列名，从第二行开始是行数据
                foreach (Row row in rows)
                {
                    if (row.RowIndex == 1)
                    {
                        //Excel第一行为列名
                        GetDataColumn(row, stringTable, ref dt);
                    }
                    GetDataRow(row, stringTable, ref dt);//Excel第二行同时为DataTable的第一行数据
                }
                return dt;
            }
        }

        static DataTable ReadAnExcel(Stream stream)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(stream, false))     //若导入.xls格式的Excel则会出现【文件包含损坏的数据】的错误！
            {
                //打开Stream
                IEnumerable<Sheet> sheets = document.WorkbookPart.Workbook.Descendants<Sheet>();
                if (sheets.Count() == 0)
                {//找出符合条件的sheet，没有则返回
                    return null;
                }

                WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheets.First().Id);
                //获取Excel中共享数据
                //SharedStringTable stringTable = document.WorkbookPart.SharedStringTablePart.SharedStringTable;
                IEnumerable<Row> rows = worksheetPart.Worksheet.Descendants<Row>();//得到Excel中得数据行

                DataTable dt = new DataTable("Excel");
                //因为需要将数据导入到DataTable中，所以我们假定Excel的第一行是列名，从第二行开始是行数据
                long rid = 0;
                foreach (Row row in rows)
                {
                    rid++;
                    if (rid == 1)
                    {
                        //Excel第一行为列名
                        //GetDataColumn(row, stringTable, ref dt);
                        foreach (Cell cell in row)
                        {
                            dt.Columns.Add(cell.CellValue.InnerText);
                        }
                        continue;
                    }
                    DataRow dr = dt.NewRow();
                    int coli = 0;
                    foreach(Cell cell in row)
                    {
                        dr[coli] = cell.CellValue.Text;
                        coli++;
                    }
                    dt.Rows.Add(dr);
                    //GetDataRow(row, stringTable, ref dt);//Excel第二行同时为DataTable的第一行数据
                }
                return dt;
            }
        }

        static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            if (cell == null)
            {
                return string.Empty;
            }
            if (cell.CellValue == null)
            {
                return string.Empty;
            }
            return cell.CellValue.InnerXml;
        }


        /// <summary>
        /// 根据给定的Excel流组织成Datatable
        /// </summary>
        /// <param name="stream">Excel文件流</param>
        /// <param name="sheetName">需要读取的Sheet</param>
        /// <returns>组织好的DataTable</returns>
        public static  DataTable ReadExcelBySheetName(string sheetName, Stream stream)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(stream, false))
            {//打开Stream
                IEnumerable<Sheet> sheets = document.WorkbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName);
                if (sheets.Count() == 0)
                {//找出符合条件的sheet，没有则返回
                    return null;
                }

                WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheets.First().Id);

                //获取Excel中共享数据
                SharedStringTable stringTable = document.WorkbookPart.SharedStringTablePart.SharedStringTable;
                IEnumerable<Row> rows = worksheetPart.Worksheet.Descendants<Row>();//得到Excel中得数据行

                DataTable dt = new DataTable("Excel");
                //因为需要将数据导入到DataTable中，所以我们假定Excel的第一行是列名，从第二行开始是行数据
                foreach (Row row in rows)
                {
                    if (row.RowIndex == 1)
                    {
                        //Excel第一行为列名
                        GetDataColumn(row, stringTable, ref dt);
                    }
                    GetDataRow(row, stringTable, ref dt);//Excel第二行同时为DataTable的第一行数据
                }
                return dt;
            }
        }

        /// <summary>
        /// 构建DataTable的列
        /// </summary>
        /// <param name="row">OpenXML定义的Row对象</param>
        /// <param name="stringTablePart"></param>
        /// <param name="dt">需要返回的DataTable对象</param>
        /// <returns></returns>
        public static void GetDataColumn(Row row, SharedStringTable stringTable, ref DataTable dt)
        {
            DataColumn col = new DataColumn();
            foreach (Cell cell in row)
            {
                string cellVal = GetValue(cell, stringTable);
                col = new DataColumn(cellVal);
                dt.Columns.Add(col);
            }
        }

        /// <summary>
        /// 构建DataTable的每一行数据，并返回该Datatable
        /// </summary>
        /// <param name="row">OpenXML的行</param>
        /// <param name="stringTablePart"></param>
        /// <param name="dt">DataTable</param>
        private static void GetDataRow(Row row, SharedStringTable stringTable, ref DataTable dt)
        {
            // 读取算法：按行逐一读取单元格，如果整行均是空数据
            // 则忽略改行（因为本人的工作内容不需要空行）-_-
            DataRow dr = dt.NewRow();
            int i = 0;
            int nullRowCount = i;
            foreach (Cell cell in row)
            {
                string cellVal = GetValue(cell, stringTable);
                if (cellVal == string.Empty)
                {
                    nullRowCount++;
                }
                dr[i] = cellVal;
                i++;
            }
            if (nullRowCount != i)
            {
                dt.Rows.Add(dr);
            }
        }


        /// <summary>
        /// 获取单元格的值
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="stringTablePart"></param>
        /// <returns></returns>
        private static string GetValue(Cell cell, SharedStringTable stringTable)
        {
            //由于Excel的数据存储在SharedStringTable中，需要获取数据在SharedStringTable 中的索引
            string value = string.Empty;
            try
            {
                if (cell.ChildElements.Count == 0)
                    return value;

                value = double.Parse(cell.CellValue.InnerText).ToString();

                if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
                {
                    value = stringTable.ChildElements[Int32.Parse(value)].InnerText;
                }
            }
            catch (Exception)
            {
                value = "N/A";
            }
            return value;
        }

        
    }
}
