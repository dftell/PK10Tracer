using System;
using System.Collections.Generic;
using System.Text;

namespace WebMsg.Tools
{
    /// <summary>
    /// 订单号，数据主键，唯一ID
    /// </summary>
    public class BillNumber
    {
        private static object locker = new object();
        private static long sn = 0;
        /// <summary>
        /// 叠加种子（短订单号生成）
        /// </summary>
        private static int OrderIndex = 1;
        private static object OrderLocker = new object();
        // 防止创建类的实例
        private BillNumber() { }
        /// <summary>
        /// 本地服务器生成
        /// </summary>
        /// <returns></returns>
        public static string NextBillNumberForLocal()
        {
            lock (locker)
            {
                if (sn == Int64.MaxValue)
                    sn = 0;
                else
                    sn++;
                return DateTime.Now.ToString("yyyyMMddHHmmssfff") + sn.ToString().PadLeft(19, '0');
            }
        }
        /// <summary>
        /// 生成短订单号
        /// </summary>
        /// <returns></returns>
        public static string CreateShortOrderNumber()
        {
            lock (OrderLocker)
            {
                if (OrderIndex == 999)
                {
                    OrderIndex = 0;
                }
                else
                {
                    OrderIndex++;
                }
                return DateTime.Now.ToString("MMddHHmmyyssfff") + OrderIndex.ToString().AddZero(3);
            }
        }
    }
}
