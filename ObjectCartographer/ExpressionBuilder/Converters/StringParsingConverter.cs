using ObjectCartographer.ExpressionBuilder.Interfaces;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectCartographer.ExpressionBuilder.Converters
{
    /// <summary>
    /// String parsing converter
    /// </summary>
    /// <seealso cref="IConverter"/>
    public class StringParsingConverter : IConverter
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order => OrderDefaults.DefaultPlusOne;

        /// <summary>
        /// Determines whether this instance can handle the specified types.
        /// </summary>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns>
        /// <c>true</c> if this instance can handle the specified types; otherwise, <c>false</c>.
        /// </returns>
        public bool CanHandle(Type sourceType, Type destinationType)
        {
            return sourceType == typeof(string)
                && destinationType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, [typeof(string)], []) != null;
        }

        /// <summary>
        /// Converts the specified property get.
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
            if (!CanHandle(sourceType, destinationType))
                return Expression.Empty();
            MethodInfo? ParseMethod = destinationType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, [typeof(string)], []);
            return Expression.Condition(Expression.Equal(source, Expression.Constant(null)),
                    destination!,
                    Expression.Call(ParseMethod!, source));
        }
    }
}