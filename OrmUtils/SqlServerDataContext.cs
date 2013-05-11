using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using System.Transactions;
using Iridescent.Data;
using Iridescent.Data.QueryModel;
using System.Linq;

namespace Iridescent.OrmExpress
{
    public class SqlServerDataContext : IDataContext, IDisposable
    {
        private List<SqlExpressCommand> _commands = new List<SqlExpressCommand>();
        private ISqlExpressCommandGenerator _commandGenerator = SqlExpressCommandGeneratorFactory.Create();
        private OrmQuery _ormQuery;

        public SqlServerDataContext() : this("ConnectionString") { }

        public SqlServerDataContext(string configNode)
        {
            IsDirty = false;
            IsInTransaction = false;
            ConnectionString = ConfigurationManager.ConnectionStrings[configNode].ConnectionString;
            _ormQuery = OrmQueryFactory.Create(ConnectionString);
        }

        #region Members

        public string ConnectionString { get; private set; }

        /// <summary>
        /// 未实现 始终返回false
        /// </summary>
        public bool IsDirty { get; private set; }

        public bool IsInTransaction { get; private set; }


        public T GetById<T>(object key) where T : class, new()
        {
            string primaryKey = PrimaryKeyFinder.GetPrimaryKey<T>();
            Query query = new Query();
            query.Criteria.Add(new Criterion(primaryKey, CriteriaOperator.Equal, key));
            SqlExpressCommand command = _commandGenerator.GenerateSelectCommand<T>(query);
            return _ormQuery.Get<T>(command);
        }

        public IList<T> GetAll<T>() where T : class, new()
        {
            return GetByCriteria<T>(null);
        }

        public IList<T> GetAll<T>(int pageIndex, int pageSize) where T : class, new()
        {
            return GetByCriteria<T>(null, pageIndex, pageSize);
        }

        public IList<T> GetByCriteria<T>(Query query) where T : class, new()
        {
            return _ormQuery.GetList<T>(_commandGenerator.GenerateSelectCommand<T>(query));
        }

        public IList<T> GetByCriteria<T>(Query query, int pageIndex, int pageSize) where T : class, new()
        {
            return _ormQuery.GetList<T>(_commandGenerator.GenerateGetPagingListCommand<T>(pageIndex, pageSize, query));
        }

        public virtual int GetCount<T>()
        {
            return GetCount<T>(null);
        }

        public virtual int GetCount<T>(Query query)
        {
            SqlExpressCommand command = _commandGenerator.GenerateSelectCommand<T>(query, true);
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, command.CommandType, command.StatementString,
                                                        command.GetDbParameterArray().Cast<SqlParameter>().ToArray()));
        }

        private void ManageCommand(SqlExpressCommand action)
        {
            if (this.IsInTransaction)
            {
                _commands.Add(action);
            }
            else
            {
                this.ExecuteScheduledCommand(action);
            }
        }

        protected void ExecuteScheduledCommand(SqlExpressCommand command)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, command.CommandType, command.StatementString,
                                      command.GetDbParameterArray().Cast<SqlParameter>().ToArray());
        }

        public void Add(object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            SqlExpressCommand command = _commandGenerator.GenerateInsertCommand(item);
            if (this.IsInTransaction)
            {
                ManageCommand(command);
            }
            else
            {
                object indentity = SqlHelper.ExecuteScalar(ConnectionString, command.CommandType,
                                                           command.StatementString,
                                                           command.GetDbParameterArray().Cast<SqlParameter>().ToArray());
                PropertyInfo primaryKeyInfo = PrimaryKeyFinder.GetPrimaryKeyPropertyInfo(item.GetType());
                primaryKeyInfo.SetValue(item,Convert.ToInt32(indentity),null);
            }
        }

        /// <summary>
        /// item存在时更新 不存在时添加
        /// </summary>
        /// <param name="item"></param>
        public void Save(object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            PropertyInfo primaryKeyInfo = PrimaryKeyFinder.GetPrimaryKeyPropertyInfo(item.GetType());
            object key = primaryKeyInfo.GetValue(item, null);
            if (key == null || (key is int && (int)key == 0))//主键没有值时 添加一条
            {
                Add(item);
            }
            else
            {
                ManageCommand(_commandGenerator.GenerateUpdateCommand(item));
            }
        }

        public void Delete(object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            ManageCommand(_commandGenerator.GenerateDeleteCommand(item));
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <exception cref="InvalidOperationException">如果已经打开事务 则抛出异常</exception>
        public void BeginTransaction()
        {
            if (this.IsInTransaction)
            {
                throw new InvalidOperationException("事务已经打开");
            }

            this.IsInTransaction = true;
        }

        /// <summary>
        /// 提交活动事务
        /// </summary>
        /// <exception cref="InvalidOperationException">如果没打开了事务 则抛出异常</exception>
        public void Commit()
        {
            if (!this.IsInTransaction)
            {
                throw new InvalidOperationException("执行该操作需要打开事务");
            }

            using (TransactionScope tx = new TransactionScope())
            {
                _commands.ForEach(ExecuteScheduledCommand);
                tx.Complete();
            }
            _commands.Clear();
            this.IsInTransaction = false;
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <exception cref="InvalidOperationException">如果没打开了事务 则抛出异常</exception>
        public void Rollback()
        {
            if (!this.IsInTransaction)
            {
                throw new InvalidOperationException("执行该操作需要打开事务");
            }

            this.IsInTransaction = false;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (this.IsInTransaction)
            {
                this.Rollback();
            }
        }

        #endregion
    }
}