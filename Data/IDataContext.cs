using System;
using System.Collections.Generic;
using Iridescent.Data.QueryModel;

namespace Iridescent.Data
{
    /// <summary>
    /// The main runtime interface between the application and the data access layer. This is the central
    /// API type abstracting the notion of a persistence service.
    /// </summary>
    public interface IDataContext : IDisposable
    {
        /// <summary>
        /// Reports whether this <c>IObjectSpaceServices</c> contain any changes which must be synchronized with the database
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// Reports whether this <c>IObjectSpaceServices</c> is working transactionally
        /// </summary>
        bool IsInTransaction { get; }

        /// <summary>
        /// Retrieves all the persisted instances of a given type
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve</typeparam>
        /// <returns>The list of persistent objects</returns>
        IList<T> GetAll<T>() where T : class, new();

        /// <summary>
        /// Retrieves all the persisted instances of a given type
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve</typeparam>
        /// <param name="pageIndex">The index of the page to retrieve</param>
        /// <param name="pageSize">The size of the page to retrieve</param>
        /// <returns>The list of persistent objects</returns>
        IList<T> GetAll<T>(int pageIndex, int pageSize) where T : class, new();

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <typeparam name="T">The type of the objects returned</typeparam>
        /// <param name="query">The query</param>
        /// <returns>A distinct list of instances</returns>
        IList<T> GetByCriteria<T>(Query query) where T : class, new();

        /// <summary>
        /// Executes a query using pagination facilities
        /// </summary>
        /// <typeparam name="T">The type of the objects returned</typeparam>
        /// <param name="query">The query</param>
        /// <param name="pageIndex">The index of the page to retrieve</param>
        /// <param name="pageSize">The size of the page to retrieve</param>
        /// <returns>A distinct list of instances</returns>
        IList<T> GetByCriteria<T>(Query query, int pageIndex, int pageSize) where T : class, new();

        /// <summary>
        /// Return the persistent instance of the given entity class with the given identifier, or null if there is no such persistent instance.
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="id">The identifier of the object</param>
        /// <returns>The persistent instance or null</returns>
        T GetById<T>(object id) where T : class, new();

        /// <summary>
        /// Returns the amount of objects of a given type
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <returns>The amount of objects</returns>
        int GetCount<T>();

        /// <summary>
        /// Returns the amount of objects of a given type that would be returned by a query
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="query">The query</param>
        /// <returns>The amount of objects</returns>
        int GetCount<T>(Query query);

        /// <summary>
        /// Adds an object to the repository
        /// </summary>
        /// <param name="item">The object to add</param>
        void Add(object item);

        /// <summary>
        /// Deletes an object from the repository
        /// </summary>
        /// <param name="item">The object to delete</param>
        void Delete(object item);

        /// <summary>
        /// Saves an object to the repository
        /// </summary>
        /// <param name="item">The object to save</param>
        void Save(object item);

        /// <summary>
        /// Begins a transaction
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there is an already active transaction</exception>
        void BeginTransaction();

        /// <summary>
        /// Commits the active transaction
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there isn't an active transaction</exception>
        void Commit();

        /// <summary>
        /// Rollbacks the active transaction
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there isn't an active transaction</exception>
        void Rollback();
    }
}
