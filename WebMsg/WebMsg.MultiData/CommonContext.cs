using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebMsg.MultiData
{
    internal enum ServiceType
    {
        只读 = 0,
        读写 = 1
    }
    internal delegate void SaveChanges(CommonContext context, Type handleClass);
    public class CommonContext : DbContext
    {
        /// <summary>
        /// 保存的时候发生
        /// </summary>
        internal event SaveChanges SaveChangesEvent;
        /// <summary>
        /// 操作的类型
        /// </summary>
        private Type HandleClass { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        internal ServiceType Service { get; set; }
        /// <summary>
        /// 链接字符串
        /// </summary>
        internal string ConnectionString { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="syncDelayTime"></param>
        internal CommonContext(string connectionString, ServiceType serviceType = ServiceType.读写, Type handleClass = null)
        {
            ConnectionString = connectionString;
            HandleClass = handleClass;
            Service = serviceType;
        }
        /// <summary>
        /// 创建链接字符串
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString,
                //需要兼容分页的语法
                p => p.UseRowNumberForPaging());
        }
        /// <summary>
        /// 初始化实体映射
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //将指定命名空间下的类全部注册
            var assembly = Assembly.GetExecutingAssembly();
            var Namespace = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
            var BaseNamespace = Namespace.Split('.')[0];
            var BaseDirectory = System.IO.Path.GetDirectoryName(assembly.Location);
            var RepositoryDirectory = BaseDirectory + "//" + BaseNamespace + ".Repository.dll";
            var Mapespace = BaseNamespace + ".Repository.Mapping";
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.LoadFile(RepositoryDirectory), p => p.Namespace == Mapespace && !p.IsGenericType && !p.IsNested);
        }
        /// <summary>
        /// 保存的方法
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            SaveChangesEvent?.Invoke(this, HandleClass);
            return base.SaveChanges();
        }
    }
}
