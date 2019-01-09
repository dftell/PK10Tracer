﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using BaseObjectsLib;
using Strags;
using PK10CorePress;
using System.Data;
using System.IO;

namespace ExchangeLib
{
    [DescriptionAttribute("资产单元类"),
        DisplayName("资产单元类")]
    [Serializable]
    public class AssetUnitClass:DisplayAsTableClass,iDbFile
    {
        
        string Xslxpath;
        string uid;

        #region 静态数据
        [DescriptionAttribute("UnitId"),
        DisplayName("UnitId"),
        CategoryAttribute("基本信息")]
        public string UnitId
        {
            get
            {
                Xslxpath = string.Format("{0}\\{1}.xlsx", Path.GetDirectoryName(typeof(GlobalClass).Assembly.Location), this.uid);
                return uid;
            }
            set
            {
                uid = value;
                Xslxpath = string.Format("{0}\\{1}.xlsx", Path.GetDirectoryName(typeof(GlobalClass).Assembly.Location), this.uid);
            }
        }

        [DescriptionAttribute("资产单元名"),
        DisplayName("资产单元名"),
        CategoryAttribute("基本信息")]
        public string UnitName { get; set; }

        [DescriptionAttribute("资产单元总资金"),
        DisplayName("资产单元总资金"),
        CategoryAttribute("资金信息")]
        public double TotalAsset { get; set; }

        [DescriptionAttribute("赔率"),
        DisplayName("赔率"),
        CategoryAttribute("通用设置")]
        public double Odds { get; set; }
        #endregion

        #region 运行时数据
        /// <summary>
        /// 交易服务
        /// </summary>
        public ExchangeService ExchangeServer { get; set; }
        /// <summary>
        /// 运行计划清单 不能有计划，循环了
        /// </summary>
        ///Dictionary<string, StragRunPlanClass> RunningPlans;
        #endregion

        public AssetUnitClass()
        {
        }

        public bool Running;

        DataTable LoadDataFromFile()
        {
            DataTable dt = new DataTable();
            dt = OpenXmlHelper.ImportExcel(Xslxpath);
            return dt;
        }

        public void SaveDataToFile()
        {
            OpenXmlHelper.ExportExcel(Xslxpath, SummaryLine());
        }
        public void Run()
        {
            Run(true);
        }
        public void Run(bool LoadTheData) //add by zhouys 2019/1/5 
        {
            ExchangeServer = new ExchangeService(TotalAsset,Odds);
            if(LoadTheData)
                ExchangeServer.LoadTheLastRecords(LoadDataFromFile());
            Running = true;
        }

        public DataTable SummaryLine()
        {
            return ExchangeServer.MoneyIncreamLine;
        }

        public double Summary()
        {
            return ExchangeServer.summary;
        } 

       

       
        #region  支持propertygrid默认信息
        static List<StragClass> DBfiles;
        
        static AssetUnitClass()
        {
            DBfiles = new List<StragClass>();
            _strDbXml = GlobalClass.ReReadAssetUnitList();
        }
        public List<T> getAllListFromDb<T>()
        {
            List<T> ret = new List<T>();
            if (strDbXml == null)
            {
                return ret;
            }
            return getObjectListByXml<T>(strDbXml);
        }

        static string _strDbXml;
        public string strDbXml
        {
            get 
            {
                return _strDbXml; 
            }
        }
       

        public bool AllowMutliSelect
        {
            get { return false; }
        }

        public string strKey
        {
            get { return "UnitId"; }
        }

        public bool SaveDBFile<T>(List<T> list)
        {
            return GlobalClass.SaveAssetUnits(getXmlByObjectList<T>(list));
        }

        public string strKeyValue()
        {
            return this.UnitId;
        }

        public string strObjectName
        {
            get { return "资产单元"; }
        }
        #endregion


    }
}
