using System;
using System.Collections.Generic;
using System.Text;
using WebMsg.IApp;
using WebMsg.Models;
using System.Linq;
using System.Data;

namespace WebMsg.App
{
    public class AuthCodeApp : IAuthCode
    {
        #region 操作声明
        IRepository.Repositorys.IAuthCodeRep _AuthCodeRep;
        public AuthCodeApp(IRepository.Repositorys.IAuthCodeRep __AuthCodeRep)
        {
            _AuthCodeRep = __AuthCodeRep;
        }
        /// <summary>
        /// 使用事务创建？
        /// </summary>
        /// <returns></returns>
        [WebMsg.Aop.Transaction]
        public bool AddItem()
        {
            return _AuthCodeRep.Insert(new AuthCode());
        }
        #endregion
        public bool AddModel(AuthCode Model)
        {
            return _AuthCodeRep.Insert(Model);
        }

        public bool AddModelList(List<AuthCode> Model)
        {
            return _AuthCodeRep.InsertList(Model);
        }

        public bool Delete(params string[] PK_ACID)
        {
            return _AuthCodeRep.Delete(p => PK_ACID.Contains(p.PK_ACID));
        }

        public int GetCount()
        {
            return _AuthCodeRep.Count();
        }

        public List<AuthCode> GetList(string[] PK_ACID)
        {
            return _AuthCodeRep.FindList(p => PK_ACID.Contains(p.PK_ACID));
        }
        [WebMsg.Aop.Caching(CachingTime = 5)]
        public List<AuthCode> GetList(string name, int PageIndex, int PageSize, string sort, out int t)
        {
            return _AuthCodeRep.FindPageing(PageIndex, PageSize, out t, sort, p => p.Code.Contains(name));
        }

        public AuthCode GetModel(string PK_ACID)
        {
            return _AuthCodeRep.FindSingle(p => p.PK_ACID == PK_ACID);
        }

        public DataTable GetTable(string sqls)
        {
            return _AuthCodeRep.FindTableBySql("select * from AuthCode");
        }

        public bool IsCheck(string Code)
        {
            return _AuthCodeRep.IsExist(p => p.Code == Code);
        }

        public bool UpdateModel(AuthCode Model)
        {
            return _AuthCodeRep.Update(Model);
        }

        public bool UpdateModelList(List<AuthCode> Model)
        {
            return _AuthCodeRep.UpdateList(Model);
        }
    }
}
