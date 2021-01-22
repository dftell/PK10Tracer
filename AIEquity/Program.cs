using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WolfInv.com.SecurityLib;
using WolfInv.com.WDDataInit;
using WolfInv.com.WinInterComminuteLib;
using WolfInv.com.BaseObjectsLib;
using WolfInv.com.GuideLib;
namespace AIEquity
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            double[] test = new double[] { 1,2,5,4,5,-4,5,2,5,1,2,3,6,-2,6,1 };
            MaxMinElementClass<double>[] res = test.MaxMinArray(5);
            //return;
            Program<StockMongoData>.Main(new string[] { });
        }
    }
    static class Program<T> where T:TimeSerialData
    {

        static WDDataInit<T> _obj;
        public static MongoDataDictionary<T>  _AllEquitSerialDatas;
        public static MongoReturnDataList<T> getEquitSerialData(string key,string keyName,bool needSpec = false,int len=0,string begt=null,string endt=null)
        {
            if (!needSpec)
            {
                if (_AllEquitSerialDatas != null && _AllEquitSerialDatas.ContainsKey(key))
                    return _AllEquitSerialDatas[key];
                else
                {
                    WDDataInit<T>.loadEquitSerial(key, keyName, true,true);
                    _AllEquitSerialDatas = WDDataInit<T>.getAllSerialData();
                    return _AllEquitSerialDatas[key];
                }
            }
            else
            {
                WDDataInit<T>.loadEquitSerial(key, keyName, true, true, len, begt, endt);
                _AllEquitSerialDatas = WDDataInit<T>.getAllSerialData();
                if (_AllEquitSerialDatas == null || _AllEquitSerialDatas.ContainsKey(key) == false)
                    return null;
                return _AllEquitSerialDatas[key];
            }
            
        }

        public static bool loaded = false;
        public static Dictionary<string, string> AllEquits;
        public static DateTime begT;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {

            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1<T> frm = new Form1<T>(null,null,null,0);
            try
            {
                Init(DateTime.Today, frm.refreshProcess);
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

        public static void Init(DateTime begT, Action<int, int, EquitProcess<T>.EquitUpdateResult> act)
        {
           
            AllEquits = new Dictionary<string, string>();
            if(WDDataInit<T>.vipDocRoot==null)
                WDDataInit<T>.vipDocRoot = "..\\..\\WolfSecDataRecSvr\\";
            WDDataInit<T>.finishedMsg = act;
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
                WDDataInit<T>.loadAllEquitSerials(10, 5, false, true, 500, null, null, false);
                _AllEquitSerialDatas = WDDataInit<T>.getAllSerialData();
                
            });
            loaded = true;
        }
    }
}
