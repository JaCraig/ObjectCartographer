using ObjectCartographer.ExpressionBuilder.Interfaces;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectCartographer.ExpressionBuilder.Converters
{
    /// <summary>
    /// TypeConverter converter
    /// </summary>
    /// <seealso cref="IConverter"/>
    public class TypeConverterConverter : IConverter
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order => int.MaxValue - 1;

        /// <summary>
        /// Gets the conver from method.
        /// </summary>
        /// <value>The conver from method.</value>
        private static MethodInfo ConverFromMethod { get; } = typeof(TypeConverter).GetMethod(nameof(TypeConverter.ConvertFrom), new[] { typeof(object) });

        /// <summary>
        /// Gets the convert to method.
        /// </summary>
        /// <value>The convert to method.</value>
        private static MethodInfo ConvertToMethod { get; } = typeof(TypeConverter).GetMethod(nameof(TypeConverter.ConvertTo), new[] { typeof(object), typeof(Type) });

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
            if (sourceType is null || destinationType is null)
                return false;
            var Converter = TypeDescriptor.GetConverter(sourceType);
            if (Converter.CanConvertTo(destinationType))
                return true;
            Converter = TypeDescriptor.GetConverter(destinationType);
            return Converter.CanConvertFrom(sourceType);
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
            var Converter = TypeDescriptor.GetConverter(sourceType);
            if (Converter.CanConvertTo(destinationType))
                return Expression.Convert(Expression.Call(Expression.Constant(Converter), ConvertToMethod, source, Expression.Constant(destinationType)), destinationType);

            Converter = TypeDescriptor.GetConverter(destinationType);
            if (Converter.CanConvertFrom(sourceType))
                return Expression.Convert(Expression.Call(Expression.Constant(Converter), ConverFromMethod, source), destinationType);
            return Expression.Empty();
        }
    }
}