using ObjectCartographer.ExpressionBuilder.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace ObjectCartographer.ExpressionBuilder.Converters
{
    /// <summary>
    /// From JsonDocument converter
    /// </summary>
    /// <seealso cref="IConverter" />
    public class JsonDocumentConverter : IConverter
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order => 2;

        /// <summary>
        /// Gets the convert to method.
        /// </summary>
        /// <value>The convert to method.</value>
        private static MethodInfo ConvertToMethod { get; } = typeof(JsonDocumentConverter).GetMethod(nameof(JsonDocumentConverter.ConvertTo));

        /// <summary>
        /// The JsonDocument type
        /// </summary>
        private static Type JsonDocumentType { get; } = typeof(JsonDocument);

        /// <summary>
        /// Determines whether this instance can handle the specified types.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns>
        /// <c>true</c> if this instance can handle the specified types; otherwise, <c>false</c>.
        /// </returns>
        public bool CanHandle(Type source, Type destination)
        {
            return source == JsonDocumentType
                && destination != JsonDocumentType;
        }

        /// <summary>
        /// Converts the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns>The converted value</returns>
        public object? ConvertTo(JsonDocument source, object? destination, Type destinationType)
        {
            return DataMapper.Instance?.Copy(source?.RootElement.ValueKind switch
            {
                JsonValueKind.Array => source.RootElement.EnumerateArray().ToArray(),
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                JsonValueKind.Number => source.RootElement.GetDouble(),
                JsonValueKind.Object => source.RootElement.EnumerateObject().ToDictionary(x => x.Name, x => x.Value),
                JsonValueKind.String => source.RootElement.GetString(),
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
        /// <returns>
        /// The resulting expression.
        /// </returns>
        public Expression Map(Expression source, Expression? destination, Type sourceType, Type destinationType, IExpressionMapping mapping, ExpressionBuilderManager manager)
        {
            if (!CanHandle(sourceType, destinationType))
                return Expression.Empty();

            return Expression.Convert(Expression.Call(Expression.Constant(this), ConvertToMethod, source, Expression.Convert(destination ?? Expression.Constant(null), typeof(object)), Expression.Constant(destinationType)), destinationType);
        }
    }
}