using Fast.Activator;
using ObjectCartographer.ExpressionBuilder.Interfaces;
using ObjectCartographer.ExtensionMethods;
using ObjectCartographer.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectCartographer.ExpressionBuilder.Converters
{
    /// <summary>
    /// Default converters
    /// </summary>
    /// <seealso cref="IConverter"/>
    public class DefaultConverter : IConverter
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order => int.MaxValue;

        /// <summary>
        /// Gets the convert to method.
        /// </summary>
        /// <value>The convert to method.</value>
        private static MethodInfo ConvertToMethod { get; } = typeof(DefaultConverter).GetMethod(nameof(DefaultConverter.ConvertTo));

        /// <summary>
        /// Gets or sets the data mapper.
        /// </summary>
        /// <value>The data mapper.</value>
        private DataMapper DataMapper { get; set; }

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
            return true;
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
            if (DataMapper is null)
                DataMapper = expressionBuilderManager.DataMapper;
            if (sourceType != typeof(object))
                propertyGet = Expression.Convert(propertyGet, typeof(object));
            return Expression.Convert(Expression.Call(Expression.Constant(this), ConvertToMethod, propertyGet, Expression.Constant(destinationType)), destinationType);
        }

        /// <summary>
        /// Converts to.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns></returns>
        public object? ConvertTo(object? item, Type destinationType)
        {
            var SourceType = item?.GetType() ?? typeof(object);
            try
            {
                if (item is null || item is DBNull)
                {
                    return ReturnDefaultValue(destinationType);
                }

                var IEnumerableResultType = destinationType.GetIEnumerableElementType();
                var IEnumerableObjectType = SourceType.GetIEnumerableElementType();
                if (destinationType != IEnumerableResultType && SourceType != IEnumerableObjectType)
                {
                    var TempList = (IList)FastActivator.CreateInstance(typeof(List<>).MakeGenericType(IEnumerableResultType));
                    foreach (var Item in (IEnumerable)item)
                    {
                        TempList.Add(ConvertTo(item, IEnumerableResultType));
                    }
                    return TempList;
                }
                if (destinationType.IsClass)
                {
                    return DataMapper.Copy(item, null, destinationType);
                }

                try
                {
                    return System.Convert.ChangeType(item, destinationType, CultureInfo.InvariantCulture);
                }
                catch { }
            }
            catch
            {
            }
            return ReturnDefaultValue(destinationType);

            static object? ReturnDefaultValue(Type resultType)
            {
                if (!resultType.IsValueType)
                    return null;
                var ResultHash = resultType.GetHashCode();
                if (DefaultValueLookup.Values.TryGetValue(ResultHash, out var ReturnValue))
                    return ReturnValue;
                return FastActivator.CreateInstance(resultType);
            }
        }
    }
}