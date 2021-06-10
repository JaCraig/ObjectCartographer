﻿using ObjectCartographer.ExpressionBuilder.Interfaces;
using System;
using System.Linq.Expressions;

namespace ObjectCartographer.ExpressionBuilder.Converters
{
    /// <summary>
    /// Primitive converter
    /// </summary>
    /// <seealso cref="ObjectCartographer.ExpressionBuilder.Interfaces.IConverter"/>
    public class PrimitiveConverter : IConverter
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
        private static Type ConvertType { get; } = typeof(Convert);

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
            return (sourceType == typeof(string) && destinationType == typeof(DateTime))
                || (IsBuiltInType(sourceType) && IsBuiltInType(destinationType));
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
            var ConvertMethod = ConvertType.GetMethod("To" + destinationType.Name, new[] { sourceType });
            return Expression.Call(ConvertMethod, propertyGet);
        }

        /// <summary>
        /// Determines whether [is built in type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is built in type] [the specified type]; otherwise, <c>false</c>.</returns>
        private static bool IsBuiltInType(Type type)
        {
            return type.IsPrimitive || type == typeof(string) || type == typeof(decimal);
        }
    }
}