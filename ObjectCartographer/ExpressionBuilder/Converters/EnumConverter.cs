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
        public int Order => 0;

        /// <summary>
        /// Gets the enum parse.
        /// </summary>
        /// <value>The enum parse.</value>
        private static MethodInfo EnumParse { get; } = typeof(Enum).GetMethod(nameof(Enum.Parse), BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(Type), typeof(string) }, Array.Empty<ParameterModifier>());

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
        public bool CanHandle(Type sourceType, Type destinationType)
        {
            return destinationType.IsEnum;
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
            if (sourceType == typeof(string))
                return Expression.Convert(Expression.Call(EnumParse, Expression.Constant(destinationType), propertyGet, Expression.Constant(true)), destinationType);
            return Expression.Convert(Expression.Call(EnumToObject, Expression.Constant(destinationType), Expression.Convert(propertyGet, typeof(object))), destinationType);
        }
    }
}