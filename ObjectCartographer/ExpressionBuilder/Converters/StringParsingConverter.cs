﻿using ObjectCartographer.ExpressionBuilder.Interfaces;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectCartographer.ExpressionBuilder.Converters
{
    /// <summary>
    /// String parsing converter
    /// </summary>
    /// <seealso cref="IConverter"/>
    public class StringParsingConverter : IConverter
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order => 1;

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
            return sourceType == typeof(string)
                && destinationType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, Array.Empty<ParameterModifier>()) != null;
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
            var ParseMethod = destinationType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, Array.Empty<ParameterModifier>());
            return Expression.Call(ParseMethod, propertyGet);
        }
    }
}