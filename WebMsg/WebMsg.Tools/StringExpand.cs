using System;
using System.Collections.Generic;
using System.Text;

namespace WebMsg.Tools
{
    /// <summary>
    /// 字符串工具类
    /// </summary>
    public static class StringExpand
    {
        /// <summary>
        /// 在前面补0
        /// </summary>
        /// <param name="sheep">数字</param>
        /// <param name="length">补0长度</param>
        /// <returns>返回值</returns>
        public static string AddZero(this string sheep, int length)
        {
            var goat = new StringBuilder(sheep);
            for (int i = goat.Length; i < length; i++)
            {
                goat.Insert(0, "0");
            }
            return goat.ToString();
        }
    }
}
