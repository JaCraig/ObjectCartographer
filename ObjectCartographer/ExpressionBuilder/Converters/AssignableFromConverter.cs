using ObjectCartographer.ExpressionBuilder.Interfaces;
using System;
using System.Linq.Expressions;

namespace ObjectCartographer.ExpressionBuilder.Converters
{
    /// <summary>
    /// Assignable from converter
    /// </summary>
    /// <seealso cref="IConverter"/>
    public class AssignableFromConverter : IConverter
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order => OrderDefaults.Default;

        /// <summary>
        /// Determines whether this instance can handle the specified types.
        /// </summary>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns>
        /// <c>true</c> if this instance can handle the specified types; otherwise, <c>false</c>.
        /// </returns>
        public bool CanHandle(Type sourceType, Type destinationType) => destinationType?.IsAssignableFrom(sourceType) ?? false;

        /// <summary>
        /// Maps the specified source to the destination.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="manager">The manager.</param>
        /// <returns>The resulting expression.</returns>
        public Expression Map(Expression source, Expression? destination, Type sourceType, Type destinationType, IExpressionMapping mapping, ExpressionBuilderManager manager)
        {
            if (source is null || destinationType is null)
                return Expression.Empty();
            return Expression.Convert(source, destinationType);
        }
    }
}