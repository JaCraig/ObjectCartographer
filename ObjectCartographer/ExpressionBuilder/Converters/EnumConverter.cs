using ObjectCartographer.ExpressionBuilder.Interfaces;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectCartographer.ExpressionBuilder.Converters
{
    /// <summary>
    /// Enum converter
    /// </summary>
    /// <seealso cref="IConverter"/>
    public class EnumConverter : IConverter
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order => OrderDefaults.Default;

        /// <summary>
        /// Gets the enum parse.
        /// </summary>
        /// <value>The enum parse.</value>
        private static MethodInfo EnumParse { get; } = typeof(Enum).GetMethod(nameof(Enum.Parse), BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(Type), typeof(string), typeof(bool) }, Array.Empty<ParameterModifier>());

        /// <summary>
        /// Gets the enum to object.
        /// </summary>
        /// <value>The enum to object.</value>
        private static MethodInfo EnumToObject { get; } = typeof(Enum).GetMethod(nameof(Enum.ToObject), BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(Type), typeof(object) }, Array.Empty<ParameterModifier>());

        /// <summary>
        /// Determines whether this instance can handle the specified types.
        /// </summary>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns>
        /// <c>true</c> if this instance can handle the specified types; otherwise, <c>false</c>.
        /// </returns>
        public bool CanHandle(Type sourceType, Type destinationType) => sourceType != typeof(object) && (destinationType?.IsEnum ?? false);

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
            if (sourceType is null || !CanHandle(sourceType, destinationType))
                return Expression.Empty();
            if (sourceType == typeof(string))
                return Expression.Convert(Expression.Call(EnumParse, Expression.Constant(destinationType), source, Expression.Constant(true)), destinationType);
            Type UnderlyingType = destinationType.GetEnumUnderlyingType();
            if (sourceType == UnderlyingType)
                return Expression.Convert(source, destinationType);
            Expression ConversionExpression = manager.Map(source, null, sourceType, UnderlyingType, mapping);
            return Expression.Convert(Expression.Call(EnumToObject, Expression.Constant(destinationType), Expression.Convert(ConversionExpression, typeof(object))), destinationType);
        }
    }
}