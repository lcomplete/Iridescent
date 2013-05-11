using System;
using System.Collections.Generic;
using Iridescent.Data.QueryModel;
using NHibernate;
using NHibernate.Criterion;

namespace Iridescent.Data.Hibernate
{
    public class NHibernateDataContext : IDataContext, IDisposable
    {
        private ISession session;
        private ITransaction tx;

        public NHibernateDataContext()
        {
            session = SessionHelper.GetNewSession();
        }

        #region IDataContext Members

        #region Persistence functions

        /// <summary>
        /// Adds an object to the repository
        /// </summary>
        /// <param name="item">The object to add</param>
        public void Add(object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            session.Save(item);
            if (!this.IsInTransaction)
            {
                session.Flush();
            }
        }

        /// <summary>
        /// Deletes an object from the repository
        /// </summary>
        /// <param name="item">The object to delete</param>
        public void Delete(object item) 
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            session.Delete(item);
            if (!this.IsInTransaction)
            {
                session.Flush();
            }
        }

        /// <summary>
        /// Saves an object to the repository
        /// </summary>
        /// <param name="item">The object to save</param>
        public void Save(object item) 
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            session.Update(item);
            if (!this.IsInTransaction)
            {
                session.Flush();
            }
        }

        #endregion

        #region Transaction management

        /// <summary>
        /// Reports whether this <c>ObjectSpaceServices</c> contain any changes which must be synchronized with the database
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return session.IsDirty();
            }
        }

        /// <summary>
        /// Reports whether this <c>ObjectSpaceServices</c> is working transactionally
        /// </summary>
        public bool IsInTransaction
        {
            get
            {
                return session.Transaction != null && session.Transaction.IsActive;
            }
        }

        /// <summary>
        /// Begins a transaction
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there is an already active transaction</exception>
        public void BeginTransaction()
        {
            if (this.IsInTransaction)
            {
                throw new InvalidOperationException("A transaction is already opened");
            }
            else
            {
                try
                {
                    tx = session.BeginTransaction();
                }
                catch
                {
                    throw new TransactionException();
                }
            }

        }

        /// <summary>
        /// Commits the active transaction
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there isn't an active transaction</exception>
        public void Commit() 
        {
            if (!this.IsInTransaction)
            {
                throw new InvalidOperationException("Operation requires an active transaction");
            }
            else
            {
                try
                {
                    tx.Commit();
                    tx.Dispose();
                }
                catch
                {
                    throw new TransactionException();
                }
            }
        }

        /// <summary>
        /// Rollbacks the active transaction
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there isn't an active transaction</exception>        
        public void Rollback() 
        {
            if (!this.IsInTransaction)
            {
                throw new InvalidOperationException("Operation requires an active transaction");
            }
            else
            {
                try
                {
                    tx.Rollback();
                    tx.Dispose();
                }
                catch
                {
                    throw new TransactionException();
                }
            }
        }
        
        #endregion

        /// <summary>
        /// Retrieves all the persisted instances of a given type
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve</typeparam>
        /// <returns>The list of persistent objects</returns>
        public IList<T> GetAll<T>() where T : class, new()
        {
            return GetAll<T>(0, 0);
        }

        /// <summary>
        /// Retrieves all the persisted instances of a given type
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve</typeparam>
        /// <param name="pageIndex">The index of the page to retrieve</param>
        /// <param name="pageSize">The size of the page to retrieve</param>
        /// <returns>The list of persistent objects</returns>
        public IList<T> GetAll<T>(int pageIndex, int pageSize) where T : class, new()
        {
            ICriteria criteria = session.CreateCriteria(typeof(T));
            criteria.SetFirstResult(pageIndex * pageSize);
            if (pageSize > 0)
            {
                criteria.SetMaxResults(pageSize);
            }
            return criteria.List<T>();
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <typeparam name="T">The type of the objects returned</typeparam>
        /// <param name="query">The query</param>
        /// <returns>A distinct list of instances</returns>
        public IList<T> GetByCriteria<T>(Query query) where T : class, new()
        {
            return GetByCriteria<T>(query, 0, 0);
        }

        /// <summary>
        /// Executes a query using pagination facilities
        /// </summary>
        /// <typeparam name="T">The type of the objects returned</typeparam>
        /// <param name="query">The query</param>
        /// <param name="pageIndex">The index of the page to retrieve</param>
        /// <param name="pageSize">The size of the page to retrieve</param>
        /// <returns>A distinct list of instances</returns>
        public IList<T> GetByCriteria<T>(Query query, int pageIndex, int pageSize) where T : class, new()
        {
            ICriteria criteria = session.CreateCriteria(typeof(T));
            QueryTranslator queryTranslator = new QueryTranslator(criteria, query);
            queryTranslator.Execute();
            criteria.SetFirstResult(pageIndex * pageSize);
            if (pageSize > 0)
            {
                criteria.SetMaxResults(pageSize);
            }
            return criteria.List<T>();
        }

        /// <summary>
        /// Return the persistent instance of the given entity class with the given identifier, or null if there is no such persistent instance.
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="id">The identifier of the object</param>
        /// <returns>The persistent instance or null</returns>
        public T GetById<T>(object key) where T : class, new()
        {
            return session.Load<T>(key);
        }

        /// <summary>
        /// Returns the amount of objects of a given type
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <returns>The amount of objects</returns>
        public int GetCount<T>()
        {
            ICriteria criteria = session.CreateCriteria(typeof(T));
            criteria.SetProjection(Projections.RowCount());
            return (int)criteria.List()[0];
        }

        /// <summary>
        /// Returns the amount of objects of a given type that would be returned by a query
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="query">The query</param>
        /// <returns>The amount of objects</returns>
        public int GetCount<T>(Query query)
        {
            ICriteria criteria = session.CreateCriteria(typeof(T));
            criteria.SetProjection(Projections.RowCount());
            QueryTranslator queryTranslator = new QueryTranslator(criteria, query);
            queryTranslator.Execute();
            return (int)criteria.List()[0];
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                session.Dispose();
            }
            finally
            {
            }
        }

        #endregion
    }
}
