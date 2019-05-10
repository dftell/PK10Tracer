using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Data.Common;
using System.Threading.Tasks;
using System.Reflection;
using WebMsg.IRepository;
using WebMsg.MultiData.Control;
using System.Data;
using System.Data.SqlClient;

namespace WebMsg.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        /// <summary>
        /// 是否逻辑删除
        /// </summary>
        public bool IsLogicDelete { get; set; }
        /// <summary>
        /// 逻辑删除字段
        /// </summary>
        public string RemoveField { get; set; }
        /// <summary>
        /// 逻辑删除已删除的状态值
        /// </summary>
        public object RemoveTrueValue { get; set; }
        /// <summary>
        /// 逻辑删除未删除的状态值
        /// </summary>
        public object RemoveFalseValue { get; set; }
        #region 数据上下文
        /// <summary>
        /// 初始化默认（不使用逻辑删除）
        /// </summary>
        public BaseRepository()
        {
            IsLogicDelete = false;
            RemoveField = string.Empty;
        }
        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="removeField"></param>
        public BaseRepository(string removeField, object removeTrueValue, object removeFalseValue)
        {
            IsLogicDelete = true;
            RemoveField = removeField;
            RemoveTrueValue = removeTrueValue;
            RemoveFalseValue = removeFalseValue;
        }
        #endregion
        #region 新增记录
        /// <summary>
        /// 增加一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        public virtual bool Insert(T entity, bool IsCommit = true)
        {
            var _Context = this.Manufacture(HandleType.写);
            _Context.Set<T>().Add(entity);
            if (IsCommit)
                return _Context.SaveChanges() > 0;
            else
                return false;
        }
        /// <summary>
        /// 增加一条记录(异步方式)
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        public virtual async Task<bool> InsertAsync(T entity, bool IsCommit = true)
        {
            var _Context = this.Manufacture(HandleType.写);
            _Context.Set<T>().Add(entity);
            if (IsCommit)
                return await Task.Run(() => _Context.SaveChanges() > 0);
            else
                return await Task.Run(() => false);
        }
        #endregion
        #region 修改记录
        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        public virtual bool Update(T entity, bool IsCommit = true)
        {
            var _Context = this.Manufacture(HandleType.写);
            _Context.Set<T>().Attach(entity);
            _Context.Entry<T>(entity).State = EntityState.Modified;
            if (IsCommit)
                return _Context.SaveChanges() > 0;
            else
                return false;
        }
        /// <summary>
        /// 更新一条记录（异步方式）
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(T entity, bool IsCommit = true)
        {
            var _Context = this.Manufacture(HandleType.写);
            _Context.Set<T>().Attach(entity);
            _Context.Entry<T>(entity).State = EntityState.Modified;
            if (IsCommit)
                return await Task.Run(() => _Context.SaveChanges() > 0);
            else
                return await Task.Run(() => false);
        }
        /// <summary>
        /// 更新多条记录，同一模型
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        public virtual bool UpdateList(List<T> T1, bool IsCommit = true)
        {
            var _Context = this.Manufacture(HandleType.写);
            if (T1 == null || T1.Count == 0) return false;
            T1.ToList().ForEach(item =>
            {
                _Context.Set<T>().Attach(item);
                _Context.Entry<T>(item).State = EntityState.Modified;
            });

            if (IsCommit)
                return _Context.SaveChanges() > 0;
            else
                return false;
        }
        /// <summary>
        /// 更新多条记录，同一模型（异步方式）
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateListAsync(List<T> T1, bool IsCommit = true)
        {
            var _Context = this.Manufacture(HandleType.写);
            if (T1 == null || T1.Count == 0) return await Task.Run(() => false);
            T1.ToList().ForEach(item =>
            {
                _Context.Set<T>().Attach(item);
                _Context.Entry<T>(item).State = EntityState.Modified;
            });

            if (IsCommit)
                return await Task.Run(() => _Context.SaveChanges() > 0);
            else
                return await Task.Run(() => false);
        }
        /// <summary>
        /// 更新多条记录，独立模型
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        public virtual bool UpdateList<T1>(List<T1> T, bool IsCommit = true) where T1 : class
        {
            var _Context = this.Manufacture(HandleType.写);
            if (T == null || T.Count == 0) return false;

            T.ToList().ForEach(item =>
            {
                _Context.Set<T1>().Attach(item);
                _Context.Entry(item).State = EntityState.Modified;
            });
            if (IsCommit)
                return _Context.SaveChanges() > 0;
            else
                return false;
        }
        /// <summary>
        /// 更新多条记录，独立模型（异步方式）
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateListAsync<T1>(List<T1> T, bool IsCommit = true) where T1 : class
        {
            var _Context = this.Manufacture(HandleType.写);
            if (T == null || T.Count == 0) return await Task.Run(() => false);
            T.ToList().ForEach(item =>
            {
                _Context.Set<T1>().Attach(item);
                _Context.Entry(item).State = EntityState.Modified;
            });

            if (IsCommit)
                return await Task.Run(() => _Context.SaveChanges() > 0);
            else
                return await Task.Run(() => false);
        }
        #endregion
        #region 新增或修改记录
        /// <summary>
        /// 增加或更新一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <param name="IsInsert">是否增加</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        public virtual bool InsertOrUpdate(T entity, bool IsInsert, bool IsCommit = true)
        {
            return IsInsert ? Insert(entity, IsCommit) : Update(entity, IsCommit);
        }
        /// <summary>
        /// 增加或更新一条记录（异步方式）
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <param name="IsInsert">是否增加</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        public virtual async Task<bool> InsertOrUpdateAsync(T entity, bool IsInsert, bool IsCommit = true)
        {
            return IsInsert ? await InsertAsync(entity, IsCommit) : await UpdateAsync(entity, IsCommit);
        }
        /// <summary>
        /// 增加多条记录，同一模型
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        public virtual bool InsertList(List<T> T1, bool IsCommit = true)
        {
            var _Context = this.Manufacture(HandleType.写);
            if (T1 == null || T1.Count == 0) return false;
            T1.ToList().ForEach(item =>
            {
                _Context.Set<T>().Add(item);
            });
            if (IsCommit)
                return _Context.SaveChanges() > 0;
            else
                return false;
        }
        /// <summary>
        /// 增加多条记录，同一模型（异步方式）
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        public virtual async Task<bool> InsertListAsync(List<T> T1, bool IsCommit = true)
        {
            var _Context = this.Manufacture(HandleType.写);
            if (T1 == null || T1.Count == 0) return await Task.Run(() => false);
            T1.ToList().ForEach(item =>
            {
                _Context.Set<T>().Add(item);
            });
            if (IsCommit)
                return await Task.Run(() => _Context.SaveChanges() > 0);
            else
                return await Task.Run(() => false);
        }
        /// <summary>
        /// 增加多条记录，独立模型
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        public virtual bool InsertList<T1>(List<T1> T, bool IsCommit = true) where T1 : class
        {
            var _Context = this.Manufacture(HandleType.写);
            if (T == null || T.Count == 0) return false;
            var tmp = _Context.ChangeTracker.Entries<T>().ToList();
            foreach (var x in tmp)
            {
                var properties = typeof(T).GetTypeInfo().GetProperties();
                foreach (var y in properties)
                {
                    var entry = x.Property(y.Name);
                    entry.CurrentValue = entry.OriginalValue;
                    entry.IsModified = false;
                    y.SetValue(x.Entity, entry.OriginalValue);
                }
                x.State = EntityState.Unchanged;
            }
            T.ToList().ForEach(item =>
            {
                _Context.Set<T1>().Add(item);
            });
            if (IsCommit)
                return _Context.SaveChanges() > 0;
            else
                return false;
        }
        /// <summary>
        /// 增加多条记录，独立模型（异步方式）
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        public virtual async Task<bool> InsertListAsync<T1>(List<T1> T, bool IsCommit = true) where T1 : class
        {
            var _Context = this.Manufacture(HandleType.写);
            if (T == null || T.Count == 0) return await Task.Run(() => false);
            var tmp = _Context.ChangeTracker.Entries<T>().ToList();
            foreach (var x in tmp)
            {
                var properties = typeof(T).GetTypeInfo().GetProperties();
                foreach (var y in properties)
                {
                    var entry = x.Property(y.Name);
                    entry.CurrentValue = entry.OriginalValue;
                    entry.IsModified = false;
                    y.SetValue(x.Entity, entry.OriginalValue);
                }
                x.State = EntityState.Unchanged;
            }
            T.ToList().ForEach(item =>
            {
                _Context.Set<T1>().Add(item);
            });
            if (IsCommit)
                return await Task.Run(() => _Context.SaveChanges() > 0);
            else
                return await Task.Run(() => false);
        }
        #endregion
        #region 删除记录
        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        public virtual bool Delete(T entity, bool IsCommit = true)
        {
            var _Context = this.Manufacture(HandleType.写);
            if (IsLogicDelete)
            {
                _Context.Entry(entity).Property(RemoveField).CurrentValue = RemoveTrueValue;
                return Update(entity, IsCommit);
            }
            else
            {
                if (entity == null) return false;
                _Context.Set<T>().Attach(entity);
                _Context.Set<T>().Remove(entity);
                if (IsCommit)
                    return _Context.SaveChanges() > 0;
                else
                    return false;
            }
        }
        /// <summary>
        /// 删除一条记录（异步方式）
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(T entity, bool IsCommit = true)
        {
            return await Task.Run(() => Delete(entity, IsCommit));
        }
        /// <summary>
        /// 删除多条记录，同一模型
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        public virtual bool DeleteList(List<T> T1, bool IsCommit = true)
        {
            var _Context = this.Manufacture(HandleType.写);
            if (IsLogicDelete)
            {
                if (T1 == null || T1.Count == 0) return false;
                T1.ToList().ForEach(item =>
                {
                    _Context.Entry(item).Property(RemoveField).CurrentValue = RemoveTrueValue;
                });
                return UpdateList(T1, IsCommit);
            }
            else
            {
                if (T1 == null || T1.Count == 0) return false;
                T1.ToList().ForEach(item =>
                {
                    _Context.Set<T>().Attach(item);
                    _Context.Set<T>().Remove(item);
                });
                if (IsCommit)
                    return _Context.SaveChanges() > 0;
                else
                    return false;
            }
        }
        /// <summary>
        /// 删除多条记录，同一模型（异步方式）
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteListAsync(List<T> T1, bool IsCommit = true)
        {
            return await Task.Run(() => DeleteList(T1, IsCommit));
        }
        /// <summary>
        /// 删除多条记录，独立模型
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        public virtual bool DeleteList<T1>(List<T1> T, bool IsCommit = true) where T1 : class
        {
            var _Context = this.Manufacture(HandleType.写);
            if (IsLogicDelete)
            {
                if (T == null || T.Count == 0) return false;
                T.ToList().ForEach(item =>
                {
                    _Context.Entry(item).Property(RemoveField).CurrentValue = RemoveTrueValue;
                });
                return UpdateList(T, IsCommit);
            }
            else
            {
                if (T == null || T.Count == 0) return false;
                T.ToList().ForEach(item =>
                {
                    _Context.Set<T1>().Attach(item);
                    _Context.Set<T1>().Remove(item);
                });
                if (IsCommit)
                    return _Context.SaveChanges() > 0;
                else
                    return false;
            }
        }
        /// <summary>
        /// 删除多条记录，独立模型（异步方式）
        /// </summary>
        /// <param name="T1">实体模型集合</param>
        /// <param name="IsCommit">是否提交（默认提交）</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteListAsync<T1>(List<T1> T, bool IsCommit = true) where T1 : class
        {
            return await Task.Run(() => DeleteList(T, IsCommit));
        }
        /// <summary>
        /// 通过Lamda表达式，删除一条或多条记录
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="IsCommit"></param>
        /// <returns></returns>
        public virtual bool Delete(Expression<Func<T, bool>> predicate, bool IsCommit = true)
        {
            List<T> list = Find(predicate).ToList();
            return DeleteList(list, IsCommit);
        }
        /// <summary>
        /// 通过Lamda表达式，删除一条或多条记录（异步方式）
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="IsCommit"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate, bool IsCommit = true)
        {
            List<T> list = Find(predicate).ToList();
            return await DeleteListAsync(list, IsCommit);
        }
        #endregion
        #region 获取记录
        /// <summary>
        /// 通过Lamda表达式获取实体
        /// </summary>
        /// <param name="predicate">Lamda表达式（p=>p.Id==Id）</param>
        /// <returns></returns>
        public virtual T FindSingle(Expression<Func<T, bool>> predicate)
        {
            var _Context = this.Manufacture(HandleType.读);
            var result = _Context.Set<T>().AsNoTracking();
            if (IsLogicDelete)
            {
                return result.Where(string.Format("{0}={1}", RemoveField, RemoveFalseValue)).SingleOrDefault(predicate);
            }
            else
            {
                return result.SingleOrDefault(predicate);
            }
        }
        /// <summary>
        /// 通过Lamda表达式获取实体（异步方式）
        /// </summary>
        /// <param name="predicate">Lamda表达式（p=>p.Id==Id）</param>
        /// <returns></returns>
        public virtual async Task<T> FindSingleAsync(Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() =>
            {
                return FindSingle(predicate);
            });
        }
        /// <summary>
        /// Lamda返回IQueryable集合，延时加载数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            var _Context = this.Manufacture(HandleType.读);
            var result = predicate != null ? _Context.Set<T>().Where(predicate).AsNoTracking<T>() : _Context.Set<T>().AsQueryable<T>().AsNoTracking<T>();
            if (IsLogicDelete)
            {
                return result.Where(string.Format("{0}={1}", RemoveField, RemoveFalseValue));
            }
            else
            {
                return result;
            }
        }
        /// <summary>
        /// Lamda返回IQueryable集合，延时加载数据
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> Find()
        {
            return Find(null);
        }
        /// <summary>
        /// 返回IQueryable集合，延时加载数据（异步方式）
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<IQueryable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() =>
            {
                return Find(predicate);
            });
        }
        /// <summary>
        /// 返回IQueryable集合，延时加载数据（异步方式）
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IQueryable<T>> FindAsync()
        {
            return await FindAsync(null);
        }
        /// <summary>
        /// 返回List<T>集合,不采用延时加载
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual List<T> FindList(Expression<Func<T, bool>> predicate)
        {
            return Find(predicate).ToList();
        }
        /// <summary>
        /// 返回List<T>集合,不采用延时加载
        /// </summary>
        /// <returns></returns>
        public virtual List<T> FindList()
        {
            return Find(null).ToList();
        }
        /// <summary>
        /// 返回List<T>集合
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderby">排序方式CreateTime desc</param>
        /// <returns></returns>
        public virtual List<T> FindList(Expression<Func<T, bool>> predicate, string orderby)
        {
            return Find(predicate).OrderBy(orderby).ToList();
        }
        /// <summary>
        /// 返回List<T>集合
        /// </summary>
        /// <param name="orderby">排序方式CreateTime desc</param>
        /// <returns></returns>
        public virtual List<T> FindList(string orderby)
        {
            return Find(null).OrderBy(orderby).ToList();
        }
        // <summary>
        /// 返回List<T>集合,不采用延时加载（异步方式）
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> FindListAsync(Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() =>
            {
                return FindList(predicate);
            });
        }
        /// <summary>
        /// 返回List<T>集合,不采用延时加载（异步方式）
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<T>> FindListAsync()
        {
            return await Task.Run(() =>
            {
                return Find(null).ToList();
            });
        }
        /// <summary>
        /// 返回List<T>集合
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderby">排序方式CreateTime desc</param>
        /// <returns></returns>
        public virtual async Task<List<T>> FindListAsync(Expression<Func<T, bool>> predicate, string orderby)
        {
            return await Task.Run(() =>
            {
                return FindList(predicate, orderby);
            });
        }
        /// <summary>
        /// 返回List<T>集合
        /// </summary>
        /// <param name="orderby">排序方式CreateTime desc</param>
        /// <returns></returns>
        public virtual async Task<List<T>> FindListAsync(string orderby)
        {
            return await FindListAsync(null, orderby);
        }
        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <param name="sortExpression"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual List<T> FindPageing(int pageIndex, int pageSize, out int recordCount, string sortExpression, Expression<Func<T, bool>> predicate = null)
        {
            if (pageIndex < 1) pageIndex = 1;
            var result = Find(predicate);
            //分页操作
            var list = result.OrderBy(sortExpression).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            recordCount = result.Count();
            return list;
        }
        /// <summary>
        /// T-Sql方式：返回IQueryable<T>集合
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="para">Parameters参数</param>
        /// <returns></returns>
        public virtual IQueryable<T> FindBySql(string sql, params DbParameter[] para)
        {
            var _Context = this.Manufacture(HandleType.读);
            return _Context.Set<T>().FromSql(sql, para);
        }
        /// <summary>
        /// T-Sql方式：返回IQueryable<T>集合（异步方式）
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="para">Parameters参数</param>
        /// <returns></returns>
        public virtual async Task<IQueryable<T>> FindBySqlAsync(string sql, params DbParameter[] para)
        {
            var _Context = this.Manufacture(HandleType.读);
            return await Task.Run(() => _Context.Set<T>().FromSql(sql, para));
        }
        /// <summary>
        /// T-Sql方式：返回List<T>集合
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="para">Parameters参数</param>
        /// <returns></returns>
        public virtual List<T> FindListBySql(string sql, params DbParameter[] para)
        {
            var _Context = this.Manufacture(HandleType.读);
            return _Context.Set<T>().FromSql(sql, para).Cast<T>().ToList();
        }
        /// <summary>
        /// T-Sql方式：返回List<T>集合（异步方式）
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="para">Parameters参数</param>
        /// <returns></returns>
        public virtual async Task<List<T>> FindListBySqlAsync(string sql, params DbParameter[] para)
        {
            var _Context = this.Manufacture(HandleType.读);
            return await Task.Run(() => _Context.Set<T>().FromSql(sql, para).Cast<T>().ToList());
        }
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
        public virtual List<TResult> QueryEntity<TEntity, TOrderBy, TResult>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Expression<Func<TEntity, TResult>> selector, bool IsAsc)
            where TEntity : class
            where TResult : class
        {
            var _Context = this.Manufacture(HandleType.读);
            IQueryable<TEntity> query = _Context.Set<TEntity>();
            if (where != null)
            {
                query = query.Where(where);
            }
            if (IsLogicDelete)
            {
                query = query.Where(string.Format("{0}={1}", RemoveField, RemoveFalseValue));
            }
            if (orderby != null)
            {
                query = IsAsc ? query.OrderBy(orderby) : query.OrderByDescending(orderby);
            }
            if (selector == null)
            {
                return query.Cast<TResult>().AsNoTracking().ToList();
            }
            return query.Select(selector).AsNoTracking().ToList();
        }
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
        public virtual async Task<List<TResult>> QueryEntityAsync<TEntity, TOrderBy, TResult>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Expression<Func<TEntity, TResult>> selector, bool IsAsc)
            where TEntity : class
            where TResult : class
        {
            var _Context = this.Manufacture(HandleType.读);
            IQueryable<TEntity> query = _Context.Set<TEntity>();
            if (where != null)
            {
                query = query.Where(where);
            }
            if (IsLogicDelete)
            {
                query = query.Where(string.Format("{0}={1}", RemoveField, RemoveFalseValue));
            }
            if (orderby != null)
            {
                query = IsAsc ? query.OrderBy(orderby) : query.OrderByDescending(orderby);
            }
            if (selector == null)
            {
                return await Task.Run(() => query.Cast<TResult>().AsNoTracking().ToList());
            }
            return await Task.Run(() => query.Select(selector).AsNoTracking().ToList());
        }
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
        public virtual List<object> QueryObject<TEntity, TOrderBy>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Func<IQueryable<TEntity>, List<object>> selector, bool IsAsc)
            where TEntity : class
        {
            var _Context = this.Manufacture(HandleType.读);
            IQueryable<TEntity> query = _Context.Set<TEntity>();
            if (where != null)
            {
                query = query.Where(where);
            }
            if (IsLogicDelete)
            {
                query = query.Where(string.Format("{0}={1}", RemoveField, RemoveFalseValue));
            }
            if (orderby != null)
            {
                query = IsAsc ? query.OrderBy(orderby) : query.OrderByDescending(orderby);
            }
            if (selector == null)
            {
                return query.AsNoTracking().ToList<object>();
            }
            return selector(query);
        }
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
        public virtual async Task<List<object>> QueryObjectAsync<TEntity, TOrderBy>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Func<IQueryable<TEntity>, List<object>> selector, bool IsAsc)
            where TEntity : class
        {
            var _Context = this.Manufacture(HandleType.读);
            IQueryable<TEntity> query = _Context.Set<TEntity>();
            if (where != null)
            {
                query = query.Where(where);
            }
            if (IsLogicDelete)
            {
                query = query.Where(string.Format("{0}={1}", RemoveField, RemoveFalseValue));
            }
            if (orderby != null)
            {
                query = IsAsc ? query.OrderBy(orderby) : query.OrderByDescending(orderby);
            }
            if (selector == null)
            {
                return await Task.Run(() => query.AsNoTracking().ToList<object>());
            }
            return await Task.Run(() => selector(query));
        }
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
        public virtual dynamic QueryDynamic<TEntity, TOrderBy>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Func<IQueryable<TEntity>, List<object>> selector, bool IsAsc)
            where TEntity : class
        {
            List<object> list = QueryObject<TEntity, TOrderBy>
                 (where, orderby, selector, IsAsc);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Newtonsoft.Json.JsonConvert.SerializeObject(list));
        }
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
        public virtual async Task<dynamic> QueryDynamicAsync<TEntity, TOrderBy>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Func<IQueryable<TEntity>, List<object>> selector, bool IsAsc)
            where TEntity : class
        {
            List<object> list = QueryObject<TEntity, TOrderBy>
                 (where, orderby, selector, IsAsc);
            return await Task.Run(() => Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Newtonsoft.Json.JsonConvert.SerializeObject(list)));
        }
        /// <summary>
        /// 获取Table
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public DataTable FindTableBySql(string sql, params DbParameter[] para)
        {
            var _Context = this.Manufacture(HandleType.读);
            SqlConnection conn = new System.Data.SqlClient.SqlConnection();
            conn.ConnectionString = _Context.Database.GetDbConnection().ConnectionString;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            if (para != null && para.Length > 0)
            {
                foreach (var item in para)
                {
                    cmd.Parameters.Add(item);
                }
            }
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
        /// <summary>
        /// 异步获取
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public async Task<DataTable> FindTableBySqlAsync(string sql, params DbParameter[] para)
        {
            return await Task.Run(() => { return FindTableBySql(sql, para); });
        }
        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public DataSet FindDataSetBySql(string sql, params DbParameter[] para)
        {
            var _Context = this.Manufacture(HandleType.读);
            SqlConnection conn = new System.Data.SqlClient.SqlConnection();
            conn.ConnectionString = _Context.Database.GetDbConnection().ConnectionString;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            if (para != null && para.Length > 0)
            {
                foreach (var item in para)
                {
                    cmd.Parameters.Add(item);
                }
            }
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            return ds;
        }
        /// <summary>
        /// 异步获取DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public async Task<DataSet> FindDataSetBySqlAsync(string sql, params DbParameter[] para)
        {
            return await Task.Run(() => { return FindDataSetBySql(sql, para); });
        }
        #endregion
        #region 验证是否存在
        /// <summary>
        /// 验证当前条件是否存在相同项
        /// </summary>
        public virtual bool IsExist(Expression<Func<T, bool>> predicate)
        {
            var _Context = this.Manufacture(HandleType.读);
            var entry = _Context.Set<T>().Where(predicate);
            if (IsLogicDelete)
            {
                entry = entry.Where(string.Format("{0}={1}", RemoveField, RemoveFalseValue));
            }
            return (entry.Any());
        }
        /// <summary>
        /// 验证当前条件是否存在相同项（异步方式）
        /// </summary>
        public virtual async Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate)
        {
            var _Context = this.Manufacture(HandleType.读);
            var entry = _Context.Set<T>().Where(predicate);
            if (IsLogicDelete)
            {
                entry = entry.Where(string.Format("{0}={1}", RemoveField, RemoveFalseValue));
            }
            return await Task.Run(() => entry.Any());
        }
        /// <summary>
        /// 根据SQL验证实体对象是否存在
        /// </summary>
        public virtual bool IsExist(string sql, params DbParameter[] para)
        {
            var _Context = this.Manufacture(HandleType.读);
            return _Context.Database.ExecuteSqlCommand(sql, para) > 0;
        }
        /// <summary>
        /// 根据SQL验证实体对象是否存在（异步方式）
        /// </summary>
        public virtual async Task<bool> IsExistAsync(string sql, params DbParameter[] para)
        {
            var _Context = this.Manufacture(HandleType.读);
            return await Task.Run(() => _Context.Database.ExecuteSqlCommand(sql, para) > 0);
        }
        #endregion
        #region 数据条数
        /// <summary>
        /// 获取数据总条数
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int Count()
        {
            return Count(null);
        }
        /// <summary>
        /// 获取数据总条数（异步）
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<int> CountAsync()
        {
            return await CountAsync(null);
        }
        /// <summary>
        /// 获取数据总条数
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int Count(Expression<Func<T, bool>> predicate)
        {
            var _Context = this.Manufacture(HandleType.读);
            var result = predicate != null ? _Context.Set<T>().Where(predicate) : _Context.Set<T>();
            if (IsLogicDelete)
            {
                result = result.Where(string.Format("{0}={1}", RemoveField, RemoveFalseValue));
            }
            return result.Count();
        }
        /// <summary>
        /// 获取数据总条数（异步）
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            var _Context = this.Manufacture(HandleType.读);
            return await Task.Run(() =>
            {
                var result = predicate != null ? _Context.Set<T>().Where(predicate) : _Context.Set<T>();
                if (IsLogicDelete)
                {
                    result = result.Where(string.Format("{0}={1}", RemoveField, RemoveFalseValue));
                }
                return result.Count();
            });
        }
        #endregion
    }
}
