using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace WebMsg.ScanCode
{
    /// <summary>
    /// 二维码
    /// </summary>
    public interface IQRCode
    {
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="value">需要生成的值</param>
        /// <param name="pixelsPerModule">像素</param>
        /// <param name="level">级别</param>
        /// <param name="drawQuietZones">是否生成白色边框</param>
        /// <param name="ico">ico logo</param>
        /// <param name="iconSizePercent">log大小比例</param>
        /// <param name="iconBorderWidth">log的边框</param>
        /// <returns></returns>
        Bitmap GetQRCode(string value, int pixelsPerModule = 8, QRCodeLevel level = QRCodeLevel.M, bool drawQuietZones = true, Bitmap ico = null, int iconSizePercent = 15, int iconBorderWidth = 6);
    }
}
