using ObjectCartographer.ExpressionBuilder.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace ObjectCartographer.ExpressionBuilder.Converters
{
    /// <summary>
    /// JsonElement converter
    /// </summary>
    /// <seealso cref="IConverter"/>
    public class JsonElementConverter : IConverter
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order { get; } = 0;

        /// <summary>
        /// Gets the convert to method.
        /// </summary>
        /// <value>The convert to method.</value>
        private static MethodInfo ConvertToMethod { get; } = typeof(JsonElementConverter).GetMethod(nameof(JsonElementConverter.ConvertTo));

        /// <summary>
        /// Gets the nullable convert to method
        /// </summary>
        private static MethodInfo NullableConvertToMethod { get; } = typeof(JsonElementConverter).GetMethod(nameof(JsonElementConverter.NullableConvertTo));

        /// <summary>
        /// Determines whether this instance can handle the specified types.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns>
        /// <c>true</c> if this instance can handle the specified types; otherwise, <c>false</c>.
        /// </returns>
        public bool CanHandle(Type source, Type destination) => (source == typeof(JsonElement) || source == typeof(JsonElement?)) && destination != typeof(JsonElement) && destination != typeof(JsonElement?);

        /// <summary>
        /// Converts the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns>The converted value</returns>
        public object? ConvertTo(JsonElement source, object? destination, Type destinationType)
        {
            return DataMapper.Instance?.Copy(source.ValueKind switch
            {
                JsonValueKind.Array => source.EnumerateArray().ToArray(),
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                JsonValueKind.Number => source.GetDouble(),
                JsonValueKind.Object => source.EnumerateObject().ToDictionary(x => x.Name, x => x.Value),
                JsonValueKind.String => source.GetString(),
                JsonValueKind.True => true,
                _ => null,
            }, destination, destinationType);
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
            if (!CanHandle(sourceType, destinationType))
                return Expression.Empty();
            if (sourceType == typeof(JsonElement))
                return Expression.Convert(Expression.Call(Expression.Constant(this), ConvertToMethod, source, Expression.Convert(destination ?? Expression.Constant(null), typeof(object)), Expression.Constant(destinationType)), destinationType);
            else
                return Expression.Convert(Expression.Call(Expression.Constant(this), NullableConvertToMethod, source, Expression.Convert(destination ?? Expression.Constant(null), typeof(object)), Expression.Constant(destinationType)), destinationType);
        }

        /// <summary>
        /// Converts the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns>The converted value</returns>
        public object? NullableConvertTo(JsonElement? source, object? destination, Type destinationType) => source is null ? null : ConvertTo(source.Value, destination, destinationType);
    }
}