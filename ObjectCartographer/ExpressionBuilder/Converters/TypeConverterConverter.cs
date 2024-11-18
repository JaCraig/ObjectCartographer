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
        public int Order => OrderDefaults.LastMinusOne;

        /// <summary>
        /// Gets the conver from method.
        /// </summary>
        /// <value>The conver from method.</value>
        private static MethodInfo ConvertFromMethod { get; } = typeof(TypeConverter).GetMethod(nameof(TypeConverter.ConvertFrom), [typeof(object)]);

        /// <summary>
        /// Gets the convert to method.
        /// </summary>
        /// <value>The convert to method.</value>
        private static MethodInfo ConvertToMethod { get; } = typeof(TypeConverter).GetMethod(nameof(TypeConverter.ConvertTo), [typeof(object), typeof(Type)]);

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
            TypeConverter Converter = TypeDescriptor.GetConverter(sourceType);
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
            TypeConverter Converter = TypeDescriptor.GetConverter(sourceType);
            if (Converter.CanConvertTo(destinationType))
                return Expression.Convert(Expression.Call(Expression.Constant(Converter), ConvertToMethod, Expression.Convert(source, typeof(object)), Expression.Constant(destinationType)), destinationType);
            Converter = TypeDescriptor.GetConverter(destinationType);
            if (Converter.CanConvertFrom(sourceType))
                return Expression.Convert(Expression.Call(Expression.Constant(Converter), ConvertFromMethod, source), destinationType);
            return Expression.Empty();
        }
    }
}