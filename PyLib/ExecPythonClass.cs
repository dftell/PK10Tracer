using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Scripting.Hosting;
namespace WolfInv.com.PyLib
{
    public class PythonReturnData
    {

    }
    public class ExecPythonClass
    {

        public static void RunPythonScript(string sArgName, string args = "", params string[] teps)
        {
            Process p = new Process();
            //string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + sArgName;// 获得python文件的绝对路径（将文件放在c#的debug文件夹中可以这样操作）
            p.StartInfo.FileName = @"python.exe";//没有配环境变量的话，可以像我这样写python.exe的绝对路径。如果配了，直接写"python.exe"即可
            string sArguments = sArgName;
            foreach (string sigstr in teps)
            {
                sArguments += " " + sigstr;//传递参数            
            }
            sArguments += " " + args;
            p.StartInfo.Arguments = sArguments;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.BeginOutputReadLine();
            p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
            //Console.ReadLine();
            p.WaitForExit();

        }

        static void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            PythonReturnData ret = null;
            if (!string.IsNullOrEmpty(e.Data))
            {
                //PythonReturnData = e.Data;
                //AppendText(e.Data + Environment.NewLine);
            }
        }

        public delegate void AppendTextCallback(string text);
        public static void AppendText(string text)
        {
            Console.WriteLine(text);     //此处在控制台输出.py文件print的结果         
        }
            

    }
}
