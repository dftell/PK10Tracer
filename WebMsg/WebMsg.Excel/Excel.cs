using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMsg.Excel
{
    /// <summary>
    /// Excel表格帮助类
    /// </summary>
    public class Excel : IExcel
    {
        /// <summary>
        /// 读取文件(默认取第一张表)
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public System.Data.DataTable Read(System.IO.MemoryStream FileMemoryStream, int TableIndex = 1)
        {
            System.Data.DataTable DTItem = new System.Data.DataTable();
            FileMemoryStream.Position = 0;
            using (ExcelPackage package = new ExcelPackage())
            {
                package.Load(FileMemoryStream);
                //只读取第一张表
                ExcelWorksheet EWModel = package.Workbook.Worksheets[TableIndex];
                DTItem.TableName = EWModel.Name;
                //所有列
                for (int i = 1; i <= EWModel.Dimension.Columns; i++)
                {
                    try
                    {
                        DTItem.Columns.Add(EWModel.Cells[1, i].Value.ToString());
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                //获取所有的内容 去掉列头
                for (int r = 1; r <= EWModel.Dimension.Rows; r++)
                {
                    System.Data.DataRow Dr = DTItem.NewRow();
                    for (int c = 1; c <= EWModel.Dimension.Columns; c++)
                    {
                        try
                        {
                            Dr[c - 1] = EWModel.Cells[r, c].Value;
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                    if (r > 1)
                    {
                        DTItem.Rows.Add(Dr);
                    }
                }
            }
            return DTItem;
        }
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="FileMemoryStream"></param>
        /// <returns></returns>
        public System.Data.DataSet Read(System.IO.MemoryStream FileMemoryStream)
        {
            System.Data.DataSet DSItem = new System.Data.DataSet();
            FileMemoryStream.Position = 0;
            using (ExcelPackage package = new ExcelPackage())
            {
                package.Load(FileMemoryStream);
                for (int s = 0; s < package.Workbook.Worksheets.Count; s++)
                {
                    //读取
                    System.Data.DataTable DTItem = new System.Data.DataTable();
                    ExcelWorksheet EWModel = package.Workbook.Worksheets[s + 1];
                    DTItem.TableName = EWModel.Name;
                    //所有列
                    for (int i = 1; i <= EWModel.Dimension.Columns; i++)
                    {
                        try
                        {
                            DTItem.Columns.Add(EWModel.Cells[1, i].Value.ToString());
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                    //获取所有的内容 去掉列头
                    for (int r = 1; r <= EWModel.Dimension.Rows; r++)
                    {
                        System.Data.DataRow Dr = DTItem.NewRow();
                        for (int c = 1; c <= EWModel.Dimension.Columns; c++)
                        {
                            try
                            {
                                Dr[c - 1] = EWModel.Cells[r, c].Value;
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                        if (r > 1)
                        {
                            DTItem.Rows.Add(Dr);
                        }
                    }
                    DSItem.Tables.Add(DTItem);
                }

            }
            return DSItem;
        }
        /// <summary>
        /// 导出文件
        /// </summary>
        /// <param name="Table"></param>
        /// <returns></returns>
        public System.IO.MemoryStream OutFile(System.Data.DataTable Table)
        {
            System.IO.MemoryStream FileByte = new System.IO.MemoryStream();
            using (ExcelPackage package = new ExcelPackage())
            {
                //表单薄
                ExcelWorksheet w = package.Workbook.Worksheets.Add(Table.TableName);
                for (int i = 0; i < Table.Columns.Count; i++)
                {
                    w.Cells[1, (i + 1)].Value = Table.Columns[i].ColumnName;
                }
                //写数据
                for (int r = 0; r < Table.Rows.Count; r++)
                {
                    for (int c = 0; c < Table.Columns.Count; c++)
                    {
                        w.Cells[(r + 2), (c + 1)].Value = Table.Rows[r][c].ToString();
                    }
                }
                package.SaveAs(FileByte);
            }
            FileByte.Position = 0;
            return FileByte;
        }
    }
}
