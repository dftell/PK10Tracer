using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 控制单元APP和UI
/// </summary>
namespace WebMsg.Unit.Control
{
    /// <summary>
    /// 注入mps映射配置
    /// </summary>
    public static class ImplantDeploy
    {
        /// <summary>
        /// 注入仓储\app层
        /// </summary>
        /// <param name="services"></param>
        public static void ImplantUnity(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var Namespace = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
            var BaseNamespace = Namespace.Split('.')[0];
            var BaseDirectory = System.IO.Path.GetDirectoryName(assembly.Location);
            //var IRepositoryDirectory = BaseDirectory + "//" + BaseNamespace + ".IRepository.dll";
            //var InterfaceDirectory = BaseDirectory + "//" + BaseNamespace + ".Interface.dll";
            var RepositoryDirectory = BaseDirectory + "//" + BaseNamespace + ".Repository.dll";
            var AppDirectory = BaseDirectory + "//" + BaseNamespace + ".App.dll";
            List<Type> ClassType = new List<Type>();
            //仓储
            ClassType.AddRange(Assembly.LoadFile(RepositoryDirectory).GetTypes().Where(p => p.Namespace == (BaseNamespace + ".Repository.Repositorys") && !p.IsGenericType && !p.IsNested));
            //App业务
            ClassType.AddRange(Assembly.LoadFile(AppDirectory).GetTypes().Where(p => p.Namespace == (BaseNamespace + ".App") && !p.IsGenericType && !p.IsNested));
            foreach (Type Item in ClassType)
            {
                Type[] InterfaceTypeArry = Item.GetInterfaces().Where(p => !p.IsGenericType && !p.IsNested).ToArray();
                if (InterfaceTypeArry.Length > 0)
                {
                    //注册类型
                    services.AddTransient(InterfaceTypeArry[0], Item);
                }
            }
        }
    }
}
