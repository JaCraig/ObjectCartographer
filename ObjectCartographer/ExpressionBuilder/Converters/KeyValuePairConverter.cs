using ObjectCartographer.ExpressionBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ObjectCartographer.ExpressionBuilder.Converters
{
    /// <summary>
    /// Key value pair converter
    /// </summary>
    /// <seealso cref="IConverter"/>
    public class KeyValuePairConverter : IConverter
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order => 0;

        /// <summary>
        /// Gets the type of the key value pair.
        /// </summary>
        /// <value>The type of the key value pair.</value>
        private static Type KeyValuePairType { get; } = typeof(KeyValuePair<,>);

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
            return IsKeyValuePair(sourceType) && IsKeyValuePair(destinationType);
        }

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
            var SourceArgs = sourceType.GenericTypeArguments;
            var DestinationArgs = destinationType.GenericTypeArguments;

            var SourceKey = manager.Map(Expression.Property(source, "Key"), null, SourceArgs[0], DestinationArgs[0], mapping);
            var SourceValue = manager.Map(Expression.Property(source, "Value"), null, SourceArgs[1], DestinationArgs[1], mapping);
            return Expression.New(destinationType.GetConstructor(DestinationArgs), SourceKey, SourceValue);
        }

        /// <summary>
        /// Determines whether [is key value pair] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is key value pair] [the specified type]; otherwise, <c>false</c>.</returns>
        private bool IsKeyValuePair(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == KeyValuePairType;
        }
    }
}