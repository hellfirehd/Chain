﻿#if !SqlDependency_Missing

using Tortuga.Chain.SqlServer.Appenders;

#if SQL_SERVER_SDS

using System.Data.SqlClient;

#elif SQL_SERVER_MDS

using Microsoft.Data.SqlClient;

#endif

#endif

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// Class SqlServerAppenders.
    /// </summary>
    public static class SqlServerAppenders
    {
#if !SqlDependency_Missing

        /// <summary>
        /// Attaches a SQL Server dependency change listener to this operation.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result type.</typeparam>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="eventHandler">The event handler to fire when the underlying data changes.</param>
        /// <returns>Tortuga.Chain.Core.ILink&lt;TResult&gt;.</returns>
        /// <remarks>This will only work for operations against non-transactional SQL Server data sources that also comform to the rules about using SQL Dependency.</remarks>
        public static ILink<TResult> WithChangeNotification<TResult>(this ILink<TResult> previousLink, OnChangeEventHandler eventHandler)
        {
            return new NotifyChangeAppender<TResult>(previousLink, eventHandler);
        }

        /// <summary>
        /// Attaches a SQL Server dependency change listener to this operation that will automatically invalidate the cache.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="previousLink">The previous link.</param>
        /// <returns></returns>
        public static ILink<TResult> AutoInvalidate<TResult>(this ICacheLink<TResult> previousLink)
        {
            return new NotifyChangeAppender<TResult>(previousLink, (s, e) => previousLink.Invalidate());
        }

#endif
    }
}
