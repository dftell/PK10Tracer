using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebMsg.IRepository
{
    /// <summary>
    /// 仓储基础方法
    /// </summary>
    public interface IBaseRepository<T> where T : class
    {
        #region 相关配置
        /// <summary>
        /// 是否逻辑删除
        /// </summary>
        bool IsLogicDelete { get; set; }
        /// <summary>
        /// 逻辑删除的字段
        /// </summary>
        string RemoveField { get; set; }
        /// <summary>
        /// 逻辑删除已删除的状态值
        /// </summary>
        object RemoveTrueValue { get; set; }
        /// <summary>
        /// 逻辑删除未删除的状态值
        /// </summary>
        object RemoveFalseValue { get; set; }
        #endregion
        #region 新增记录
        /// <summary>
        /// 增加一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        bool Insert(T entity, bool IsCommit = true);
        /// <summary>
        /// 增加一条记录（异步方式）
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        Task<bool> InsertAsync(T entity, bool IsCommit = true);
        /// <summary>
        /// 增加多条记录，同一模型
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        bool InsertList(List<T> T1, bool IsCommit = true);
        /// <summary>
        /// 增加多条记录，同一模型（异步方式）
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        Task<bool> InsertListAsync(List<T> T1, bool IsCommit = true);
        /// <summary>
        /// 增加多条记录，独立模型
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        bool InsertList<T1>(List<T1> T, bool IsCommit = true) where T1 : class;
        /// <summary>
        /// 增加多条记录，独立模型（异步方式）
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        Task<bool> InsertListAsync<T1>(List<T1> T, bool IsCommit = true) where T1 : class;
        #endregion
        #region 修改记录
        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        bool Update(T entity, bool IsCommit = true);
        /// <summary>
        /// 更新一条记录（异步方式）
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T entity, bool IsCommit = true);
        /// <summary>
        /// 更新多条记录，同一模型
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        bool UpdateList(List<T> T1, bool IsCommit = true);
        /// <summary>
        /// 更新多条记录，同一模型（异步方式）
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        Task<bool> UpdateListAsync(List<T> T1, bool IsCommit = true);
        /// <summary>
        /// 更新多条记录，独立模型
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        bool UpdateList<T1>(List<T1> T, bool IsCommit = true) where T1 : class;
        /// <summary>
        /// 更新多条记录，独立模型（异步方式）
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        Task<bool> UpdateListAsync<T1>(List<T1> T, bool IsCommit = true) where T1 : class;
        #endregion
        #region 新增或修改记录
        /// <summary>
        /// 增加或更新一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <param name="IsInsert">是否增加</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        bool InsertOrUpdate(T entity, bool IsInsert, bool IsCommit = true);
        /// <summary>
        /// 增加或更新一条记录（异步方式）
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <param name="IsInsert">是否增加</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        Task<bool> InsertOrUpdateAsync(T entity, bool IsInsert, bool IsCommit = true);
        #endregion
        #region 删除记录
        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        bool Delete(T entity, bool IsCommit = true);
        /// <summary>
        /// 删除一条记录（异步方式）
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(T entity, bool IsCommit = true);
        /// <summary>
        /// 删除多条记录，同一模型
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        bool DeleteList(List<T> T1, bool IsCommit = true);
        /// <summary>
        /// 删除多条记录，同一模型（异步方式）
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        Task<bool> DeleteListAsync(List<T> T1, bool IsCommit = true);
        /// <summary>
        /// 删除多条记录，独立模型
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        bool DeleteList<T1>(List<T1> T, bool IsCommit = true) where T1 : class;
        /// <summary>
        /// 删除多条记录，独立模型（异步方式）
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        Task<bool> DeleteListAsync<T1>(List<T1> T, bool IsCommit = true) where T1 : class;
        /// <summary>
        /// 通过Lamda表达式，删除一条或多条记录
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="IsCommit"></param>
        /// <returns></returns>
        bool Delete(Expression<Func<T, bool>> predicate, bool IsCommit = true);
        /// <summary>
        /// 通过Lamda表达式，删除一条或多条记录（异步方式）
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="IsCommit"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate, bool IsCommit = true);
        #endregion
        #region 获取记录
        /// <summary>
        /// 通过Lamda表达式获取实体
        /// </summary>
        /// <param name="predicate">Lamda表达式（p=>p.Id==Id）</param>
        /// <returns></returns>
        T FindSingle(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 通过Lamda表达式获取实体（异步方式）
        /// </summary>
        /// <param name="predicate">Lamda表达式（p=>p.Id==Id）</param>
        /// <returns></returns>
        Task<T> FindSingleAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 返回IQueryable集合，延时加载数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 返回IQueryable集合，延时加载数据
        /// </summary>
        /// <returns></returns>
        IQueryable<T> Find();
        /// <summary>
        /// 返回IQueryable集合，延时加载数据（异步方式）
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<IQueryable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 返回IQueryable集合，延时加载数据（异步方式）
        /// </summary>
        /// <returns></returns>
        Task<IQueryable<T>> FindAsync();
        // <summary>
        /// 返回List<T>集合,不采用延时加载
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        List<T> FindList(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 返回List<T>集合,不采用延时加载
        /// </summary>
        /// <returns></returns>
        List<T> FindList();
        /// <summary>
        /// 返回List<T>集合，并指定排序方式
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderby">排序方式CreateTime desc</param>
        /// <returns></returns>
        List<T> FindList(Expression<Func<T, bool>> predicate, string orderby);
        /// <summary>
        /// 返回List<T>集合，并指定排序方式
        /// </summary>
        /// <param name="orderby"></param>
        /// <returns></returns>
        List<T> FindList(string orderby);
        // <summary>
        /// 返回List<T>集合,不采用延时加载（异步方式）
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<List<T>> FindListAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 返回List<T>集合,不采用延时加载（异步方式）
        /// </summary>
        /// <returns></returns>
        Task<List<T>> FindListAsync();
        /// <summary>
        /// 返回List<T>集合，并指定排序方式
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderby">排序方式CreateTime desc</param>
        /// <returns></returns>
        Task<List<T>> FindListAsync(Expression<Func<T, bool>> predicate, string orderby);
        /// <summary>
        /// 返回List<T>集合，并指定排序方式
        /// </summary>
        /// <param name="orderby"></param>
        /// <returns></returns>
        Task<List<T>> FindListAsync(string orderby);
        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <param name="sortExpression"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        List<T> FindPageing(int pageIndex, int pageSize, out int recordCount, string sortExpression, Expression<Func<T, bool>> predicate = null);
        /// <summary>
        /// T-Sql方式：返回IQueryable<T>集合
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="para">Parameters参数</param>
        /// <returns></returns>
        IQueryable<T> FindBySql(string sql, params DbParameter[] para);
        /// <summary>
        /// T-Sql方式：返回IQueryable<T>集合（异步方式）
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="para">Parameters参数</param>
        /// <returns></returns>
        Task<IQueryable<T>> FindBySqlAsync(string sql, params DbParameter[] para);
        /// <summary>
        /// T-Sql方式：返回List<T>集合
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="para">Parameters参数</param>
        /// <returns></returns>
        List<T> FindListBySql(string sql, params DbParameter[] para);
        /// <summary>
        /// T-Sql方式：返回List<T>集合（异步方式）
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="para">Parameters参数</param>
        /// <returns></returns>
        Task<List<T>> FindListBySqlAsync(string sql, params DbParameter[] para);
        /// <summary>
        /// 可指定返回结果、排序、查询条件的通用查询方法，返回实体对象集合
        /// </summary>
        /// <typeparam name="TEntity">实体对象</typeparam>
        /// <typeparam name="TOrderBy">排序字段类型</typeparam>
        /// <typeparam name="TResult">数据结果，与TEntity一致</typeparam>
        /// <param name="where">过滤条件，需要用到类型转换的需要提前处理与数据表一致的</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="selector">返回结果（必须是模型中存在的字段）</param>
        /// <param name="IsAsc">排序方向，true为正序false为倒序</param>
        /// <returns>实体集合</returns>
        List<TResult> QueryEntity<TEntity, TOrderBy, TResult>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Expression<Func<TEntity, TResult>> selector, bool IsAsc)
            where TEntity : class
            where TResult : class;
        /// <summary>
        /// 可指定返回结果、排序、查询条件的通用查询方法，返回实体对象集合（异步方式）
        /// </summary>
        /// <typeparam name="TEntity">实体对象</typeparam>
        /// <typeparam name="TOrderBy">排序字段类型</typeparam>
        /// <typeparam name="TResult">数据结果，与TEntity一致</typeparam>
        /// <param name="where">过滤条件，需要用到类型转换的需要提前处理与数据表一致的</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="selector">返回结果（必须是模型中存在的字段）</param>
        /// <param name="IsAsc">排序方向，true为正序false为倒序</param>
        /// <returns>实体集合</returns>
        Task<List<TResult>> QueryEntityAsync<TEntity, TOrderBy, TResult>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Expression<Func<TEntity, TResult>> selector, bool IsAsc)
            where TEntity : class
            where TResult : class;
        /// <summary>
        /// 可指定返回结果、排序、查询条件的通用查询方法，返回Object对象集合
        /// </summary>
        /// <typeparam name="TEntity">实体对象</typeparam>
        /// <typeparam name="TOrderBy">排序字段类型</typeparam>
        /// <param name="where">过滤条件，需要用到类型转换的需要提前处理与数据表一致的</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="selector">返回结果（必须是模型中存在的字段）</param>
        /// <param name="IsAsc">排序方向，true为正序false为倒序</param>
        /// <returns>自定义实体集合</returns>
        List<object> QueryObject<TEntity, TOrderBy>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Func<IQueryable<TEntity>, List<object>> selector, bool IsAsc)
            where TEntity : class;
        /// <summary>
        /// 可指定返回结果、排序、查询条件的通用查询方法，返回Object对象集合（异步方式）
        /// </summary>
        /// <typeparam name="TEntity">实体对象</typeparam>
        /// <typeparam name="TOrderBy">排序字段类型</typeparam>
        /// <param name="where">过滤条件，需要用到类型转换的需要提前处理与数据表一致的</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="selector">返回结果（必须是模型中存在的字段）</param>
        /// <param name="IsAsc">排序方向，true为正序false为倒序</param>
        /// <returns>自定义实体集合</returns>
        Task<List<object>> QueryObjectAsync<TEntity, TOrderBy>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Func<IQueryable<TEntity>, List<object>> selector, bool IsAsc)
            where TEntity : class;
        /// <summary>
        /// 可指定返回结果、排序、查询条件的通用查询方法，返回动态类对象集合
        /// </summary>
        /// <typeparam name="TEntity">实体对象</typeparam>
        /// <typeparam name="TOrderBy">排序字段类型</typeparam>
        /// <param name="where">过滤条件，需要用到类型转换的需要提前处理与数据表一致的</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="selector">返回结果（必须是模型中存在的字段）</param>
        /// <param name="IsAsc">排序方向，true为正序false为倒序</param>
        /// <returns>动态类</returns>
        dynamic QueryDynamic<TEntity, TOrderBy>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Func<IQueryable<TEntity>, List<object>> selector, bool IsAsc)
            where TEntity : class;
        /// <summary>
        /// 可指定返回结果、排序、查询条件的通用查询方法，返回动态类对象集合（异步方式）
        /// </summary>
        /// <typeparam name="TEntity">实体对象</typeparam>
        /// <typeparam name="TOrderBy">排序字段类型</typeparam>
        /// <param name="where">过滤条件，需要用到类型转换的需要提前处理与数据表一致的</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="selector">返回结果（必须是模型中存在的字段）</param>
        /// <param name="IsAsc">排序方向，true为正序false为倒序</param>
        /// <returns>动态类</returns>
        Task<dynamic> QueryDynamicAsync<TEntity, TOrderBy>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Func<IQueryable<TEntity>, List<object>> selector, bool IsAsc)
            where TEntity : class;
        /// <summary>
        /// 获取Table
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        DataTable FindTableBySql(string sql, params DbParameter[] para);
        /// <summary>
        /// 异步获取Table
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        Task<DataTable> FindTableBySqlAsync(string sql, params DbParameter[] para);
        /// <summary>
        /// 获取Table
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        DataSet FindDataSetBySql(string sql, params DbParameter[] para);
        /// <summary>
        /// 异步获取Table
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        Task<DataSet> FindDataSetBySqlAsync(string sql, params DbParameter[] para);
        #endregion
        #region 验证是否存在
        /// <summary>
        /// 验证当前条件是否存在相同项
        /// </summary>
        bool IsExist(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 验证当前条件是否存在相同项（异步方式）
        /// </summary>
        Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 根据SQL验证实体对象是否存在
        /// </summary>
        bool IsExist(string sql, params DbParameter[] para);
        /// <summary>
        /// 根据SQL验证实体对象是否存在（异步方式）
        /// </summary>
        Task<bool> IsExistAsync(string sql, params DbParameter[] para);
        #endregion
        #region 数据条数
        /// <summary>
        /// 获取数据总条数
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int Count();
        /// <summary>
        /// 获取数据总条数（异步）
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<int> CountAsync();
        /// <summary>
        /// 获取数据总条数
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int Count(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 获取数据总条数（异步）
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        #endregion
    }
}
