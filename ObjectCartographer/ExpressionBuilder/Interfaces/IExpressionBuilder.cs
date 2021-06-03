using ObjectCartographer.Internal;
using System.Linq.Expressions;

namespace ObjectCartographer.ExpressionBuilder.Interfaces
{
    /// <summary>
    /// Expression builder interface
    /// </summary>
    public interface IExpressionBuilder
    {
        /// <summary>
        /// Determines whether this instance can handle the specified types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>
        /// <c>true</c> if this instance can handle the specified types; otherwise, <c>false</c>.
        /// </returns>
        bool CanHandle(TypeTuple types);

        /// <summary>
        /// Converts the specified source and destination.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns>The resulting expression.</returns>
        Expression Map(Expression source, Expression destination);
    }
}