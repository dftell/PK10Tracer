using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace WebMsg.IApp
{
    public interface IAuthCode
    {
        bool AddModel(Models.AuthCode Model);
        bool AddModelList(List<Models.AuthCode> Model);
        bool UpdateModel(Models.AuthCode Model);
        bool UpdateModelList(List<Models.AuthCode> Model);
        //删除
        bool Delete(params string[] PK_ACID);

        Models.AuthCode GetModel(string PK_ACID);

        List<Models.AuthCode> GetList(string[] PK_ACID);
        int GetCount();
        bool IsCheck(string Code);
        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="sort"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        List<Models.AuthCode> GetList(string name, int PageIndex, int PageSize, string sort, out int t);

        bool AddItem();

        DataTable GetTable(string sqls);
    }
}
