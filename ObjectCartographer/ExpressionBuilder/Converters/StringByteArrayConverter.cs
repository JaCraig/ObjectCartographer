using ObjectCartographer.ExpressionBuilder.Interfaces;
using System;
using System.Linq.Expressions;
using System.Text;

namespace ObjectCartographer.ExpressionBuilder.Converters
{
    /// <summary>
    /// String/Byte array converter
    /// </summary>
    /// <seealso cref="IConverter"/>
    public class StringByteArrayConverter : IConverter
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order => 0;

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static byte[] GetBytes(string data)
        {
            if (string.IsNullOrEmpty(data))
                return Array.Empty<byte>();
            return Encoding.UTF8.GetBytes(data);
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string GetString(byte[] data)
        {
            if (data is null)
                return "";
            return Encoding.UTF8.GetString(data);
        }

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
            return (sourceType == typeof(string) && destinationType == typeof(byte[]))
                || (sourceType == typeof(byte[]) && destinationType == typeof(string));
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
        public Expression Map(
            Expression source,
            Expression? destination,
            Type sourceType,
            Type destinationType,
            IExpressionMapping mapping,
            ExpressionBuilderManager manager)
        {
            if (sourceType == typeof(string) && destinationType == typeof(byte[]))
                return StringToByteArray(source, destination, sourceType, destinationType, mapping, manager);
            return ByteArrayToString(source, destination, sourceType, destinationType, mapping, manager);
        }

        /// <summary>
        /// Bytes the array to string.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="manager">The manager.</param>
        /// <returns></returns>
        private Expression ByteArrayToString(Expression source, Expression? destination, Type sourceType, Type destinationType, IExpressionMapping mapping, ExpressionBuilderManager manager)
        {
            return Expression.Call(typeof(StringByteArrayConverter).GetMethod(nameof(StringByteArrayConverter.GetString)), source);
        }

        /// <summary>
        /// Strings to byte array.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="manager">The manager.</param>
        /// <returns></returns>
        private Expression StringToByteArray(Expression source, Expression? destination, Type sourceType, Type destinationType, IExpressionMapping mapping, ExpressionBuilderManager manager)
        {
            return Expression.Call(typeof(StringByteArrayConverter).GetMethod(nameof(StringByteArrayConverter.GetBytes)), source);
        }
    }
}