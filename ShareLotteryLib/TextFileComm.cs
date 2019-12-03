using System;
using System.IO;
namespace WolfInv.com.ShareLotteryLib
{
    public class TextFileComm
    {
        public static string getFileText(string filename,string folder="")
        {
            string path = string.Format("{0}\\{1}\\{2}", AppDomain.CurrentDomain.BaseDirectory,folder,filename);
            if (!File.Exists(path))
            {
                return null;
            }
            try
            {
                return File.ReadAllText(path);
            }
            catch
            {
                return null;
            }
            
        }
    }
}
