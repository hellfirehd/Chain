using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Formatters
{

    /// <summary>
    /// This is the base class for formatters that return a value. Most operation are not executed without first attaching a formatter subclass.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the t command type.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    /// <typeparam name="TResultType">The type of the t result type.</typeparam>
    /// <seealso cref="Formatters.Formatter{TCommandType, TParameterType}" />
    public abstract class Formatter<TCommandType, TParameterType, TResultType> : Formatter<TCommandType, TParameterType>
        where TCommandType : DbCommand
        where TParameterType : DbParameter
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Formatter{TCommandType, TParameterType, TResultType}"/> class.
        /// </summary>
        /// <param name="commandBuilder">The associated operation.</param>
        protected Formatter(DbCommandBuilder<TCommandType, TParameterType> commandBuilder) : base(commandBuilder) { }


        /// <summary>
        /// Execute the operation synchronously.
        /// </summary>
        /// <returns></returns>
        public abstract TResultType Execute(object state = null);

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public async Task<TResultType> ExecuteAsync(object state = null)
        {
            return await ExecuteAsync(CancellationToken.None, state);
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User defined state, usually used for logging.</param>
        /// <returns></returns>
        public abstract Task<TResultType> ExecuteAsync(CancellationToken cancellationToken, object state = null);

    }
}

