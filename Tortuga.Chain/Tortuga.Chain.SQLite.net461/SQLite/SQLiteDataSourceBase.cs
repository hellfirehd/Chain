﻿using System;
using System.Data.SQLite;
using System.Threading;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SQLite.SQLite.CommandBuilders;

namespace Tortuga.Chain.SQLite
{
    /// <summary>
    /// Base class that represents a SQLite Datasource.
    /// </summary>
    public abstract class SQLiteDataSourceBase : DataSource<SQLiteCommand, SQLiteParameter>, IClass1DataSource
    {
        private readonly ReaderWriterLockSlim m_SyncLock = new ReaderWriterLockSlim(); //Sqlite is single-threaded for writes. It says otherwise, but it spams the trace window with exceptions.


        /// <summary>
        /// Gets the database metadata.
        /// </summary>
        /// <value>The database metadata.</value>
        public abstract SQLiteMetadataCache DatabaseMetadata { get; }

        /// <summary>
        /// Creates a operation based on a raw SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="lockType">Type of the lock.</param>
        /// <returns>SQLiteSqlCall.</returns>
        public SQLiteSqlCall Sql(string sqlStatement, LockType lockType)
        {
            return new SQLiteSqlCall(this, sqlStatement, null, lockType);
        }

        /// <summary>
        /// Creates a operation based on a raw SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="lockType">Type of the lock.</param>
        /// <returns>SQLiteSqlCall.</returns>
        public SQLiteSqlCall Sql(string sqlStatement, object argumentValue, LockType lockType)
        {
            return new SQLiteSqlCall(this, sqlStatement, argumentValue, lockType);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <returns></returns>
        public SQLiteTableOrView From(string tableOrViewName)
        {
            return new SQLiteTableOrView(this, tableOrViewName, null, null);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public SQLiteTableOrView From(string tableOrViewName, string whereClause)
        {
            return new SQLiteTableOrView(this, tableOrViewName, whereClause, null);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <param name="whereClause"></param>
        /// <param name="argumentValue"></param>
        /// <returns></returns>
        public SQLiteTableOrView From(string tableOrViewName, string whereClause, object argumentValue)
        {
            return new SQLiteTableOrView(this, tableOrViewName, whereClause, argumentValue);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public SQLiteTableOrView From(string tableOrViewName, object filterValue)
        {
            return new SQLiteTableOrView(this, tableOrViewName, filterValue);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteInsertObject" /> used to perform an insert operation.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <returns></returns>
        public SQLiteInsertObject Insert(string tableName, object argumentValue)
        {
            return new SQLiteInsertObject(this, tableName, argumentValue);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteUpdateObject" /> used to perform an update operation.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public SQLiteUpdateObject Update(string tableName, object argumentValue, UpdateOptions options = UpdateOptions.None)
        {
            return new SQLiteUpdateObject(this, tableName, argumentValue, options);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteDeleteObject" /> used to perform a delete operation.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="argumentValue"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public SQLiteDeleteObject Delete(string tableName, object argumentValue, DeleteOptions options = DeleteOptions.None)
        {
            return new SQLiteDeleteObject(this, tableName, argumentValue, options);
        }

        ISingleRowDbCommandBuilder IClass1DataSource.Insert(string tableName, object argumentValue)
        {
            return Insert(tableName, argumentValue);
        }

        ISingleRowDbCommandBuilder IClass1DataSource.Update(string tableName, object argumentValue, UpdateOptions options)
        {
            return Update(tableName, argumentValue, options);
        }

        IDbCommandBuilder IClass1DataSource.Delete(string tableName, object argumentValue, DeleteOptions options)
        {
            return Delete(tableName, argumentValue, options);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.From(string tableOrViewName)
        {
            return From(tableOrViewName);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.From(string tableOrViewName, string whereClause)
        {
            return From(tableOrViewName, whereClause);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.From(string tableOrViewName, string whereClause, object argumentValue)
        {
            return From(tableOrViewName, whereClause, argumentValue);
        }

        IMultipleRowDbCommandBuilder IClass1DataSource.From(string tableOrViewName, object filterValue)
        {
            return From(tableOrViewName, filterValue);
        }

        ISingleRowDbCommandBuilder IClass1DataSource.InsertOrUpdate(string tableName, object argumentValue, InsertOrUpdateOptions options)
        {
            throw new NotImplementedException();
        }

        IDatabaseMetadataCache IClass1DataSource.DatabaseMetadata
        {
            get { return DatabaseMetadata; }
        }


        /// <summary>
        /// Normally we use a reader/writer lock to avoid simutaneous writes to a SQlite database. If you disable this locking, you may see extra noise in your tracing output or unexcepted exceptions.
        /// </summary>
        public bool DisableLocks { get; set; }

        /// <summary>
        /// Gets the synchronize lock used during exection of database operations.
        /// </summary>
        /// <value>The synchronize lock.</value>
        protected ReaderWriterLockSlim SyncLock
        {
            get { return m_SyncLock; }
        }
    }
}
