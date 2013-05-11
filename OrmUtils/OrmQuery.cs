using System;
using System.Collections.Generic;
using System.Data;
using Iridescent.Data.QueryModel;

namespace Iridescent.OrmExpress
{
    public abstract class OrmQuery
    {
        public string ConnectionString { get; set; }

        protected OrmQuery(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public TEntity Get<TEntity>(string sqlStatement, Query query = null) where TEntity : class 
        {
            return Get<TEntity>(SqlExpressCommand.Create(sqlStatement, query));
        }

        public TEntity Get<TEntity>(SqlExpressCommand command) where TEntity : class 
        {
            using (IDataReader reader = ExecuteReader(command))
            {
                return OrmUtils.ReadToEntity<TEntity>(reader);
            }
        }

        protected abstract IDataReader ExecuteReader(SqlExpressCommand command);

        public IList<TEntity> GetList<TEntity>(string sqlStatement, Query query = null) where TEntity:class 
        {
            return GetList<TEntity>(SqlExpressCommand.Create(sqlStatement, query));
        }

        public IList<TEntity> GetList<TEntity>(SqlExpressCommand command) where TEntity : class
        {
            using (IDataReader reader = ExecuteReader(command))
            {
                return OrmUtils.ReadToEntityList<TEntity>(reader);
            }
        }

    }
}