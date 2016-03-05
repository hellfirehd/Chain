﻿using System;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.Appenders
{

    /// <summary>
    /// Executes the previous link and caches the result.
    /// </summary>
    /// <typeparam name="TResultType">The type of the t result type.</typeparam>
    public class CacheResultAppender<TResultType> : Appender<TResultType>
    {
        private readonly Func<TResultType, string> m_CacheKeyFunction;
        private readonly string m_CacheKey;
        private readonly string m_RegionName;
        private readonly CacheItemPolicy m_Policy;
        private readonly Func<TResultType, string> m_RegionNameFunction;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheResultAppender{TResultType}" /> class.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKeyFunction">Function to generate cache keys.</param>
        /// <param name="regionNameFunction">Optional function to generate region names.</param>
        /// <param name="policy">Optional cache policy.</param>
        public CacheResultAppender(ILink<TResultType> previousLink, Func<TResultType, string> cacheKeyFunction, Func<TResultType, string> regionNameFunction = null, CacheItemPolicy policy = null) : base(previousLink)
        {
            if (cacheKeyFunction == null)
                throw new ArgumentNullException("cacheKeyFunction", "cacheKeyFunction is null.");
            if (previousLink == null)
                throw new ArgumentNullException("previousLink", "previousLink is null.");

            if (regionNameFunction == null)
                regionNameFunction = (x) => null;

            m_CacheKeyFunction = cacheKeyFunction;
            m_RegionNameFunction = regionNameFunction;
            m_Policy = policy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheResultAppender{TResultType}" /> class.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="regionName">Optional name of the cache region. WARNING: The default cache does not support region names.</param>
        /// <param name="policy">Optional cache policy.</param>
        public CacheResultAppender(ILink<TResultType> previousLink, string cacheKey, string regionName, CacheItemPolicy policy) : base(previousLink)
        {
            if (previousLink == null)
                throw new ArgumentNullException("previousLink", "previousLink is null.");
            if (string.IsNullOrEmpty(cacheKey))
                throw new ArgumentException("cacheKey is null or empty.", "cacheKey");

            m_Policy = policy;
            m_RegionName = regionName;
            m_CacheKey = cacheKey;
        }

        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        public override TResultType Execute(object state = null)
        {

            var result = PreviousLink.Execute(state);

            DataSource.WriteToCache(new CacheItem(m_CacheKey ?? m_CacheKeyFunction(result), result, m_RegionName ?? m_RegionNameFunction(result)), m_Policy);

            return result;
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override Task<TResultType> ExecuteAsync(object state = null)
        {
            return ExecuteAsync(CancellationToken.None, state);
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public override async Task<TResultType> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {

            var result = await PreviousLink.ExecuteAsync(state).ConfigureAwait(false);

            DataSource.WriteToCache(new CacheItem(m_CacheKey, result, m_RegionName), m_Policy);

            return result;
        }

    }

}
