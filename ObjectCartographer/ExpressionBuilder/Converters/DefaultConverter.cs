using Fast.Activator;
using ObjectCartographer.ExpressionBuilder.Interfaces;
using ObjectCartographer.Internal;
using System;
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
        /// Converts to.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns></returns>
        public object? ConvertTo(object? item, object? destination, Type destinationType)
        {
            var SourceType = item?.GetType() ?? typeof(object);
            try
            {
                if (destinationType.IsAssignableFrom(SourceType))
                    return item;

                if (item is null || item is DBNull)
                {
                    return ReturnDefaultValue(destinationType);
                }

                if (destinationType.IsClass)
                {
                    return DataMapper.Instance?.Copy(item, destination, destinationType);
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
            if (sourceType != typeof(object))
                source = Expression.Convert(source, typeof(object));
            return Expression.Convert(Expression.Call(Expression.Constant(this), ConvertToMethod, source, Expression.Convert(destination ?? Expression.Constant(null), typeof(object)), Expression.Constant(destinationType)), destinationType);
        }
    }
}