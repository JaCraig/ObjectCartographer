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
            var Converter = TypeDescriptor.GetConverter(sourceType);
            if (Converter.CanConvertTo(destinationType))
                return true;
            Converter = TypeDescriptor.GetConverter(destinationType);
            return Converter.CanConvertFrom(sourceType);
        }

        /// <summary>
        /// Converts the specified property get.
        /// </summary>
        /// <param name="propertyGet">The property get.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <param name="expressionBuilderManager">The expression builder manager.</param>
        /// <returns></returns>
        public Expression Convert(Expression propertyGet, Type sourceType, Type destinationType, ExpressionBuilderManager expressionBuilderManager)
        {
            var Converter = TypeDescriptor.GetConverter(sourceType);
            if (Converter.CanConvertTo(destinationType))
                return Expression.Convert(Expression.Call(Expression.Constant(Converter), ConvertToMethod, propertyGet, Expression.Constant(destinationType)), destinationType);

            Converter = TypeDescriptor.GetConverter(destinationType);
            if (Converter.CanConvertFrom(sourceType))
                return Expression.Convert(Expression.Call(Expression.Constant(Converter), ConverFromMethod, propertyGet), destinationType);
            return Expression.Empty();
        }
    }
}