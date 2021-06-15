using ObjectCartographer.ExpressionBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ObjectCartographer.ExpressionBuilder.Converters
{
    /// <summary>
    /// IEnumerable to array converter
    /// </summary>
    /// <seealso cref="IConverter"/>
    public class ArrayIEnumerableConverter : IConverter
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order => 1;

        /// <summary>
        /// Gets the type of the i enumerable.
        /// </summary>
        /// <value>The type of the i enumerable.</value>
        private static Type IEnumerableType { get; } = typeof(IEnumerable<>);

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
            return IsIEnumerable(sourceType) && destinationType.IsArray;
        }

        /// <summary>
        /// Conversions the specified source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns></returns>
        public TDestination[] Conversion<TSource, TDestination>(IEnumerable<TSource> source, TDestination[] destination)
        {
            if (source is null)
                return destination ?? Array.Empty<TDestination>();
            if (destination is null)
                return source.Select(x => DataMapper.Instance.Copy<TDestination>(x) ?? default).ToArray();
            var Index = 0;
            foreach (var Item in source)
            {
                if (destination.Length <= Index)
                    return destination;
                destination[Index] = DataMapper.Instance.Copy<TDestination>(Item);
                ++Index;
            }
            return destination;
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
            if (sourceType is null || destinationType is null)
                return Expression.Empty();
            var SourceCollectionValueType = Array.Find(sourceType.GetInterfaces(), x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)).GenericTypeArguments[0];
            var DestionationCollectionValueType = Array.Find(destinationType.GetInterfaces(), x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)).GenericTypeArguments[0];

            var ConversionMethod = typeof(ArrayIEnumerableConverter).GetMethod(nameof(ArrayIEnumerableConverter.Conversion)).MakeGenericMethod(SourceCollectionValueType, DestionationCollectionValueType);

            return Expression.Call(Expression.Constant(this), ConversionMethod, source, destination);
        }

        /// <summary>
        /// Determines whether [is i enumerable] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is i enumerable] [the specified type]; otherwise, <c>false</c>.</returns>
        private bool IsIEnumerable(Type type)
        {
            return type?.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == IEnumerableType) ?? false;
        }
    }
}