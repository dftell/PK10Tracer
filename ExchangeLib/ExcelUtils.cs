using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.CustomProperties;
using System.Runtime.InteropServices;
using System.IO;

namespace ExchangeLib
{
    public class ExcelUtils
    {///注意:  
     ///需要提前添加DocumentFormat.OpenXml.dll  
     ///openXml只能处理后缀为xlsx的excel文件，xls格式的不支持  
        private object m_objOpt = Missing.Value;

        private DocumentFormat.OpenXml.Packaging.SpreadsheetDocument m_objSpreadsheetDocument = null;
        private WorkbookPart m_objWorkbookPart = null;
        private WorksheetPart m_objWorksheetPart = null;
        private Worksheet m_objWorksheet = null;
        private SheetData m_objSheetData = null;

        List<string> NormalSheets = new List<string>();
        const int BranchCodeRow = 2;
        const int BranchCodeStartCol = 3;
        const int DataEndRow = 7;

        #region CreateExcel Interop 2013  
        /// <summary>  
        /// 创建excel,并且把dataTable导入到excel中  
        /// </summary>         
        /// <param name="destination">保存路径</param>  
        /// <param name="dataTables">数据源</param>  
        /// <param name="sheetNames">excel中sheet的名称</param>    
        public void CreateExcel(string destination, DataTable[] dataTables, string[] sheetNames = null)
        {
            using (var workbook = SpreadsheetDocument.Create(destination, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();
                workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
                workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

                uint sheetId = 1;
                foreach (DataTable table in dataTables)
                {
                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    sheetPart.Worksheet = new Worksheet();
                    Columns headColumns = CrateColunms(table);
                    sheetPart.Worksheet.Append(headColumns);
                    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
                    string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                    if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
                    {
                        sheetId =
                            sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }
                    string sheetName = string.Empty;
                    if (sheetNames != null)
                    {
                        if (sheetNames.Length >= sheetId)
                        {
                            sheetName = sheetNames[sheetId - 1].ToString();
                        }
                    }
                    else
                    {
                        sheetName = table.TableName ?? sheetId.ToString();
                    }

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    List<String> columns = new List<string>();
                    foreach (DataColumn column in table.Columns)
                    {
                        columns.Add(column.ColumnName);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.StyleIndex = 11;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName);
                        headerRow.AppendChild(cell);
                    }
                    sheetData.AppendChild(headerRow);

                    foreach (DataRow dsrow in table.Rows)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (String col in columns)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell.StyleIndex = 10;
                            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(dsrow[col].ToString()); //  
                            newRow.AppendChild(cell);
                        }
                        sheetData.AppendChild(newRow);
                    }
                    sheetPart.Worksheet.Append(sheetData);
                }
                workbook.Close();
            }
        }

        private Columns CrateColunms(DataTable table)
        {
            int numCols = table.Columns.Count;
            var columns = new Columns();
            for (int col = 0; col < table.Columns.Count; col++)
            {
                int maxWidth = table.Columns[col].ColumnName.
                    Length;
                int valueWidth = 0;
                for (int row = 0; row < table.Rows.Count; row++)
                {
                    valueWidth = table.Rows[row][col].ToString().Trim().Length;
                    if (maxWidth < valueWidth)
                    {
                        maxWidth = valueWidth;
                    }
                }
                //Column c = new CustomColumn((UInt32)col + 1, (UInt32)numCols + 1, maxWidth + 5);
                Column c = new Column();
                c.Width = maxWidth + 5;
                columns.Append(c);
            }
            return columns;
        }
        private SheetData CreateSheetData(DataTable table)
        {
            var sheetData = new SheetData();
            Row headerRow = new Row();
            List<String> columns = new List<string>();
            foreach (DataColumn column in table.Columns)
            {
                columns.Add(column.ColumnName);

                Cell cell = new Cell();
                cell.StyleIndex = 11;
                cell.DataType = CellValues.String;
                cell.CellValue = new CellValue(column.ColumnName);
                headerRow.Append(cell);
            }

            sheetData.Append(headerRow);

            foreach (DataRow dsrow in table.Rows)
            {
                Row newRow = new Row();
                foreach (String col in columns)
                {
                    Cell cell = new Cell();
                    cell.StyleIndex = 10;
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(dsrow[col].ToString()); //  
                    newRow.Append(cell);
                }

                sheetData.Append(newRow);
            }

            return sheetData;
        }
        #endregion

        public DataTable ImportExcel(string fileName)
        {
            DataTable dt = new DataTable();
            Stream stream = null;
            try
            {
                stream = File.Open(fileName, FileMode.Open);
                List<DataTable> rets = ReadAllSheetToList(stream);
                dt = rets[0];
            }
            catch (Exception ce)
            {
                return null;
            }
            finally
            {
                stream.Close();
            }
            return dt;
        }

        public void ReadExcelFromStream(Stream stream, bool isEditable)
        {
            try
            {
                m_objSpreadsheetDocument = SpreadsheetDocument.Open(stream, isEditable);
                m_objWorkbookPart = m_objSpreadsheetDocument.WorkbookPart;
                m_objWorksheetPart = m_objWorkbookPart.WorksheetParts.First();
                m_objWorksheet = m_objWorksheetPart.Worksheet;
                m_objSheetData = m_objWorksheet.Elements<SheetData>().First();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                this.DisposeRead();
                throw;
            }
        }

        /// <summary>  
        /// 获取excel中cell的值  
        /// </summary>  
        /// <param name="columnName"></param>  
        /// <param name="rowIndex"></param>  
        /// <returns></returns>  
        public string GetCellValue(string columnName, int rowIndex)
        {
            Cell cell = GetCell(columnName, rowIndex);
            if (cell == null)
            {
                return string.Empty;
            }
            if (cell.CellValue == null)
            {
                return string.Empty;
            }
            int cellId = Convert.ToInt32(cell.CellValue.Text);
            return m_objWorkbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(cellId).InnerText;
        }

        public Cell GetCell(string columnName, int rowIndex)
        {
            Row row = GetRow(Convert.ToUInt32(rowIndex));

            if (row == null)
            {
                return null;
            }
            return row.Elements<Cell>().Where(c => string.Compare(c.CellReference.Value, columnName + rowIndex, true) == 0).First();
        }

        // Given a worksheet and a row index, return the row.  
        public Row GetRow(uint rowIndex)
        {
            if (m_objWorksheet == null)
            {
                return null;
            }
            return m_objWorksheet.GetFirstChild<SheetData>().Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
        }

        public List<Row> GetRows()
        {
            if (m_objSheetData == null)
            {
                return null;
            }
            return m_objSheetData.Elements<Row>().ToList();
        }

        public void CloseRead()
        {
            if (m_objSpreadsheetDocument != null)
            {
                m_objSpreadsheetDocument.Dispose();
            }
        }

        public void DisposeRead()
        {
            if (m_objWorksheet != null)
            {
                Marshal.FinalReleaseComObject(m_objWorksheet);
            }
            if (m_objWorksheetPart != null)
            {
                Marshal.FinalReleaseComObject(m_objWorksheetPart);
            }
            if (m_objWorkbookPart != null)
            {
                Marshal.FinalReleaseComObject(m_objWorkbookPart);
            }
            if (m_objSpreadsheetDocument != null)
            {
                Marshal.FinalReleaseComObject(m_objSpreadsheetDocument);
                int intGeneration = GC.GetGeneration(m_objSpreadsheetDocument);
                m_objSpreadsheetDocument = null;
                GC.Collect(intGeneration);
            }
            GC.Collect();
            GC.Collect();
        }


        public List<DataTable> ReadAllSheetToList(Stream stream)
        {
            List<DataTable> tables = new List<DataTable>();

            //MemoryStream ms = new MemoryStream();  
            //stream.CopyTo(ms);  
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(stream, false))
            {
                foreach (Sheet sheet in doc.WorkbookPart.Workbook.Descendants<Sheet>())
                {

                    WorksheetPart worksheet = (WorksheetPart)doc.WorkbookPart.GetPartById(sheet.Id);
                    List<string> columnsNames = new List<string>();
                    if (NormalSheets.Contains(sheet.Name.Value))
                    {
                        DataTable table = new DataTable(sheet.Name.Value);
                        int rowid = 0;
                        foreach (Row row in worksheet.Worksheet.Descendants<Row>())
                        {
                            rowid++;
                            if (rowid == 1)
                            {
                                List<string> colvalue = new List<string>();
                                int colindex = 1;
                                int tablecolindex = 0;
                                table.Columns.Add("C" + tablecolindex.ToString());
                                foreach (Cell cell in row)
                                {
                                    tablecolindex++;
                                    if (colindex >= BranchCodeStartCol && !String.IsNullOrEmpty(GetCellValue(doc, cell).ToString().Trim()))
                                    {
                                        table.Columns.Add("C" + tablecolindex.ToString());
                                        colvalue.Add(GetCellValue(doc, cell).ToString().Trim());
                                    }
                                    colindex++;
                                }
                                DataRow dr1 = table.NewRow();
                                dr1[0] = "";
                                for (int i1 = 0; i1 < colvalue.Count; i1++)
                                {
                                    dr1[i1 + 1] = colvalue[i1];
                                }
                                table.Rows.Add(dr1);
                            }
                            else if (rowid>1)
                            {
                                DataRow dr = table.NewRow();
                                int colindex = 1;
                                int readindex = 0;
                                foreach (Cell cell in row)
                                {
                                    if (colindex >= BranchCodeStartCol - 1 && readindex < table.Columns.Count)
                                    {
                                        dr[readindex] = GetCellValue(doc, cell).ToString().Trim();
                                        readindex++;
                                    }
                                    else if (readindex >= table.Columns.Count)
                                    {
                                        break;
                                    }
                                    colindex++;

                                }
                                table.Rows.Add(dr);
                            }
                            else
                            {
                                break;
                            }
                        }
                        tables.Add(table);
                    }
                    else
                    {
                        break;
                    }


                }
            }
            return tables;
        }

        /// <summary>  
        /// 将excel中第一个sheet的数据导入到DataTable  
        /// </summary>  
        /// <param name="stream"></param>  
        /// <returns></returns>  
        public DataTable ReadFirstSheetData(Stream stream)
        {
            DataTable dt = new DataTable();
            SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(stream, false);
            WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
            IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
            //string relationshipId = sheets.First().Id.Value = sheets.First(x => x.Name == "Sheet1").Id.Value;  
            string relationshipId = sheets.First().Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
            Worksheet workSheet = worksheetPart.Worksheet;
            SheetData sheetData = workSheet.GetFirstChild<SheetData>();
            Row[] rows = sheetData.Descendants<Row>().ToArray();
            // Add columns  
            foreach (Cell cell in rows.ElementAt(0))
            {
                dt.Columns.Add((string)GetCellValue(spreadSheetDocument, cell));
            }
            // Add rows  
            for (int rowIndex = 1; rowIndex < rows.Count(); rowIndex++)
            {
                DataRow tempRow = dt.NewRow();
                for (int i = 0; i < rows[rowIndex].Descendants<Cell>().Count(); i++)
                {
                    tempRow[i] = GetCellValue(spreadSheetDocument, rows[rowIndex].Descendants<Cell>().ElementAt(i));
                }
                dt.Rows.Add(tempRow);
            }
            // Release document  
            if (spreadSheetDocument != null)
            {
                int intGeneration = GC.GetGeneration(spreadSheetDocument);
                spreadSheetDocument.Dispose();
                spreadSheetDocument = null;
                GC.Collect(intGeneration);
            }
            return dt;
        }

        private string GetCellValue(SpreadsheetDocument document, Cell cell)
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
    }

}
