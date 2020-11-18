using System;
using System.IO;
namespace WolfInv.com.ShareLotteryLib
{
    public class TextFileComm
    {
        public static string getFileText(string filename,string folder="")
        {
            string path = string.Format("{0}\\{1}\\{2}", AppDomain.CurrentDomain.BaseDirectory,folder,filename);
            LogLib.LogableClass.ToLog("文件", "尝试读取path", path);
            if (!File.Exists(path))
            {
                string strTest = string.Format("{0}\\{1}\\{2}", AppDomain.CurrentDomain.RelativeSearchPath, folder, filename);
                LogLib.LogableClass.ToLog("文件", "尝试读取path", strTest);
                if (!File.Exists(strTest))
                {
                    LogLib.LogableClass.ToLog("文件", "找不到文件",string.Format("{0},{1}", path, strTest));
                    return null;
                }
                path = strTest;
            }
            try
            {
                return File.ReadAllText(path);
            }
            catch
            {
                LogLib.LogableClass.ToLog("文件", "读取文件错误", string.Format("{0}", path));
                return null;
            }
            
        }
    }
}
