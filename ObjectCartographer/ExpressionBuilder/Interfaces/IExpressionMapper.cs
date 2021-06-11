using ObjectCartographer.Interfaces;
using System.Linq.Expressions;

namespace ObjectCartographer.ExpressionBuilder.Interfaces
{
    /// <summary>
    /// Expression mapper interface
    /// </summary>
    public interface IExpressionMapper
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        int Order { get; }

        /// <summary>
        /// Determines whether this instance can handle the specified types.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        /// <returns>
        /// <c>true</c> if this instance can handle the specified types; otherwise, <c>false</c>.
        /// </returns>
        bool CanHandle(IInternalTypeMapping mapping);

        /// <summary>
        /// Maps the specified source to the destination.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="manager">The manager.</param>
        /// <returns>The resulting expression.</returns>
        Expression Map(Expression source, Expression destination, IInternalTypeMapping mapping, ExpressionBuilderManager manager);
    }
}