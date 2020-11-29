using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WolfInv.com.SecurityLib;
using WolfInv.com.WDDataInit;
using WolfInv.com.WinInterComminuteLib;
using WolfInv.com.BaseObjectsLib;
namespace AIEquity
{
    static class Program
    {
        static void Main()
        {
            Program<StockMongoData>.Main(new string[] { });
        }
    }
    static class Program<T> where T:TimeSerialData
    {

        static WDDataInit<T> _obj;
        public static MongoDataDictionary<T>  _AllEquitSerialDatas;
        public static MongoReturnDataList<T> getEquitSerialData(string key,string keyName)
        {
            
            if(_AllEquitSerialDatas!= null && _AllEquitSerialDatas.ContainsKey(key))
                return _AllEquitSerialDatas[key];
            else
            {
                WDDataInit<T>.loadEquitSerial(key,keyName, true, true);
                _AllEquitSerialDatas = WDDataInit<T>.getAllSerialData();
                return _AllEquitSerialDatas[key];
            }
            
        }
        public static Dictionary<string, string> AllEquits;
        public static DateTime begT;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {

            begT = DateTime.Now;
            WDDataInit< T>.vipDocRoot = "..\\..\\WolfSecDataRecSvr\\";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1<T> frm = new Form1<T>();
            try
            {
                WDDataInit<T>.finishedMsg = frm.refreshProcess;
                Task.Factory.StartNew(() =>
                {
                    WDDataInit<T>.Init();
                    //WDDataInit<T>.Debug = true;
                    AllEquits = WDDataInit<T>.AllSecurities;
                    
                    
                    if (AllEquits == null || AllEquits.Count == 0)
                    {

                        MessageBox.Show("没有数据！");
                        return;
                    }
                    WDDataInit<T>.loadAllEquitSerials(10,20, false, true,500,null,null,true);
                    _AllEquitSerialDatas = WDDataInit<T>.getAllSerialData();
                });
                /*
                foreach(string key in AllEquits.Keys)
                {
                    _AllEquitSerialDatas.Add(key,SecDataObj.getEquitSerialData(key));
                }*/
                
            }
            catch(Exception ce)
            {
                MessageBox.Show(ce.Message);
                return;
            }
            
            Application.Run(frm);
        }
    }
}
