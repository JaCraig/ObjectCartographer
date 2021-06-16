using ObjectCartographer.ExpressionBuilder.Interfaces;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectCartographer.ExpressionBuilder.Converters
{
    /// <summary>
    /// ToString converter
    /// </summary>
    /// <seealso cref="IConverter"/>
    public class ToStringConverter : IConverter
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order => 1;

        /// <summary>
        /// Converts to stringmethod.
        /// </summary>
        /// <value>To string method.</value>
        private static MethodInfo ToStringMethod { get; } = typeof(object).GetMethod(nameof(object.ToString));

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
            return sourceType != typeof(DBNull) && destinationType == typeof(string);
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
        /// <returns></returns>
        public Expression Map(Expression source, Expression? destination, Type sourceType, Type destinationType, IExpressionMapping mapping, ExpressionBuilderManager manager)
        {
            if (!CanHandle(sourceType, destinationType))
                return Expression.Empty();
            if (IsNullable(sourceType))
            {
                var EqualityComparison = Expression.Equal(source, Expression.Constant(null));
                if (sourceType == typeof(object) || sourceType == typeof(DBNull))
                    EqualityComparison = Expression.Or(EqualityComparison, Expression.Equal(source, Expression.Constant(DBNull.Value)));
                return Expression.Condition(EqualityComparison,
                    destination,
                    Expression.Call(source, ToStringMethod));
            }
            return Expression.Call(source, ToStringMethod);
        }

        /// <summary>
        /// Determines whether the specified type is nullable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is nullable; otherwise, <c>false</c>.</returns>
        private static bool IsNullable(Type type)
        {
            return !(type is null)
                && (!type.IsValueType || Nullable.GetUnderlyingType(type) != null);
        }
    }
}