using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebMsg.MultiData.Control
{
    public enum HandleType
    {
        写 = 0,
        读 = 1
    }
    public class ManufactureContext
    {
        /// <summary>
        /// 执行记录
        /// </summary>
        private static List<ManufactureCache> CacheList { get; set; }
        /// <summary>
        /// 数据库链接的配置
        /// </summary>
        private static List<ManufactureDeploy> ManufactureDeployList { get; set; }
        /// <summary>
        /// 多数据库数据同步延迟
        /// </summary>
        private int SyncDelayTime { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        private ManufactureContext()
        {
            SyncDelayTime = 1;//1秒钟
            //初始化数据库的所有链接
            CacheList = new List<ManufactureCache>();
            //加载配置文件
            ManufactureDeployList = LoadConfig();
            //连接检查（每5分钟检查一次）
            BuildExamineConnect();
        }
        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <returns></returns>
        private List<ManufactureDeploy> LoadConfig()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile(Directory.GetCurrentDirectory() + "\\appsettings.json").Build();
            var ConnectionStrings = config.GetSection("ConnectionStrings");
            var DataBaseSyncDelayTime = config.GetSection("DataBaseSyncDelayTime");
            if (DataBaseSyncDelayTime != null)
            {
                SyncDelayTime = DataBaseSyncDelayTime.Get<int>();
            }
            if (ConnectionStrings == null)
            {
                throw new Exception("请在appsettings.json中配置数据库连接字符串：ConnectionStrings:[{ \"ServiceType\": 1, \"Weight\": 1, \"ConnectionString\": \"\" }]");
            }
            return ConnectionStrings.Get<List<ManufactureDeploy>>();
        }
        /// <summary>
        /// 抽取一个数据库配置
        /// </summary>
        /// <returns></returns>
        private ManufactureDeploy Extract(ServiceType serviceType)
        {
            ManufactureDeploy MDModel = RandomExtraction(serviceType);
            if (serviceType == ServiceType.只读 && MDModel == null)
            {
                //如果没有只读就使用读写
                MDModel = RandomExtraction(ServiceType.读写);
            }
            return MDModel;
        }
        #region 随机抽取
        /// <summary>
        /// 随机抽取
        /// </summary>
        /// <param name="ListModel"></param>
        /// <returns></returns>
        private ManufactureDeploy RandomExtraction(ServiceType serviceType)
        {
            List<ManufactureDeploy> ListModel = ManufactureDeployList;
            if (ListModel == null || ListModel.Count < 1)
            {
                throw new Exception("请在appsettings.json中配置数据库连接字符串：ConnectionStrings:[{ \"ServiceType\": 1, \"Weight\": 1, \"ConnectionString\": \"\" }]");
            }
            else if (ListModel.Count == 1)
            {
                //只有一个数据库的时候不进行抽取，提高性能
                return ListModel[0];
            }
            else
            {
                ListModel = ListModel.Where(p => p.ServiceTypeEnum == serviceType).ToList();
            }
            //权重总和
            int TotalWeights = ListModel.Sum(p =>
            {
                int Weight = p.Weight;
                if (Weight <= 0)
                {
                    return 1;
                }
                else
                {
                    return (int)Weight;
                }
            });
            //随机赋值权重
            Random ran = new Random(GetRandomSeed());  //GetRandomSeed()随机种子，防止快速频繁调用导致随机一样的问题 
            List<KeyValuePair<int, int>> wlist = new List<KeyValuePair<int, int>>();    //第一个int为list下标索引、第一个int为权重排序值
            for (int i = 0; i < ListModel.Count; i++)
            {
                int MWeight = 0;
                int Weight = ListModel[i].Weight;
                if (Weight <= 0)
                {
                    MWeight = 1;
                }
                else
                {
                    MWeight = (int)Weight;
                }
                int w = MWeight + ran.Next(0, TotalWeights);   // （权重+1） + 从0到（总权重-1）的随机数
                wlist.Add(new KeyValuePair<int, int>(i, w));
            }
            //排序
            wlist.Sort(
              delegate (KeyValuePair<int, int> kvp1, KeyValuePair<int, int> kvp2)
              {
                  return kvp2.Value - kvp1.Value;
              });

            //根据实际情况取排在最前面的几个
            return ListModel[wlist[0].Key];
        }
        /// <summary>
        /// 获取随机种子
        /// </summary>
        /// <returns></returns>
        private int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
        #endregion
        /// <summary>
        /// 单例
        /// </summary>
        internal static ManufactureContext Instance { get; } = new ManufactureContext();
        /// <summary>
        /// 生产一个数据库上下文
        /// </summary>
        /// <param name="handleType"></param>
        /// <returns></returns>
        internal CommonContext Manufacture(HandleType handleType, Type handleClass)
        {
            ManufactureCache MCModel = CacheList.FindLast(p => p.HandleClass == handleClass);
            if (handleType == HandleType.读 && MCModel != null && MCModel.UseWriteTime != null && (DateTime.Now - ((DateTime)MCModel.UseWriteTime)).TotalSeconds <= SyncDelayTime)
            {
                //读取的时候检测如果距离上一次对该表写入的时间间隔不超过设置的延迟（秒）则继续用上一次写的数据库上下文进行读取数据
                return new CommonContext(MCModel.Context.ConnectionString, MCModel.Context.Service);
            }
            else
            {
                ManufactureDeploy MDModel;
                if (handleType == HandleType.读)
                {
                    //读数据库
                    MDModel = Extract(ServiceType.只读);
                }
                else
                {
                    //写数据库
                    MDModel = Extract(ServiceType.读写);
                }
                //抽取数据库实例
                var context = new CommonContext(MDModel.ConnectionString, MDModel.ServiceTypeEnum, handleClass);
                context.SaveChangesEvent += Context_SaveChangesEvent;
                return context;
            }
        }
        /// <summary>
        /// 保存后发生
        /// </summary>
        /// <param name="context"></param>
        /// <param name="handleClass"></param>
        private void Context_SaveChangesEvent(CommonContext context, Type handleClass)
        {
            ManufactureCache MCModel = CacheList.FindLast(p => p.HandleClass == handleClass);
            if (MCModel == null)
            {
                MCModel = new ManufactureCache()
                {
                    Context = context,
                    HandleClass = handleClass,
                    UseWriteTime = DateTime.Now
                };
                CacheList.Add(MCModel);
            }
            else
            {
                MCModel.Context = context;
                MCModel.HandleClass = handleClass;
                MCModel.UseWriteTime = DateTime.Now;
            }
        }
        /// <summary>
        /// 连接状态检查(只有在多数据库的时候才会进行检查)
        /// </summary>
        private void BuildExamineConnect()
        {
            if (ManufactureDeployList != null && ManufactureDeployList.Count > 1)
            {
                System.Threading.Thread t = new System.Threading.Thread(() =>
                {
                    while (true)
                    {
                        List<ManufactureDeploy> Tem = new List<ManufactureDeploy>();
                        foreach (ManufactureDeploy Item in ManufactureDeployList)
                        {
                            //检查数据库连接是否可以使用
                            try
                            {
                                using (var CTool = new CommonContext(Item.ConnectionString))
                                {
                                    if (!CTool.Database.CanConnect())
                                    {
                                        //连接不可用
                                        Tem.Add(Item);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                //连接不可用
                                Tem.Add(Item);
                            }
                        }
                        ManufactureDeployList = ManufactureDeployList.Except(Tem).ToList();
                        //每5分钟检查一次
                        System.Threading.Thread.Sleep(5 * 60 * 1000);
                        //重新加载配置文件
                        ManufactureDeployList = LoadConfig();
                    }
                });
                t.Start();
            }
        }
    }
}
