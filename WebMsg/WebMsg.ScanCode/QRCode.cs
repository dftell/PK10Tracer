using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace WebMsg.ScanCode
{
    public enum QRCodeLevel
    {
        L = 0,
        M = 1,
        Q = 2,
        H = 3
    }
    /// <summary>
    /// 二维码工具
    /// </summary>
    public class QRCode : IQRCode
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
        public Bitmap GetQRCode(string value, int pixelsPerModule = 8, QRCodeLevel level = QRCodeLevel.M, bool drawQuietZones = true, Bitmap ico = null, int iconSizePercent = 15, int iconBorderWidth = 6)
        {
            QRCoder.QRCodeGenerator Tool = new QRCoder.QRCodeGenerator();
            QRCoder.QRCodeData data = Tool.CreateQrCode(value, (QRCoder.QRCodeGenerator.ECCLevel)((int)level), true);
            QRCoder.QRCode qrcode = new QRCoder.QRCode(data);
            return qrcode.GetGraphic(pixelsPerModule, Color.Black, Color.White, ico, iconSizePercent, iconBorderWidth, drawQuietZones);
        }
    }
}
