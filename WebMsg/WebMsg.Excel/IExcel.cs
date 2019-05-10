using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace WebMsg.Excel
{
    /// <summary>
    /// 表格
    /// </summary>
    public interface IExcel
    {
        /// <summary>
        /// 读取文件(默认取第一张表)
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        DataTable Read(System.IO.MemoryStream FileMemoryStream, int TableIndex = 1);
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="FileMemoryStream"></param>
        /// <returns></returns>
        DataSet Read(System.IO.MemoryStream FileMemoryStream);
        /// <summary>
        /// 导出文件
        /// </summary>
        /// <param name="Table"></param>
        /// <returns></returns>
        MemoryStream OutFile(System.Data.DataTable Table);
    }
}
