using ObjectCartographer.ExpressionBuilder.BaseClasses;
using ObjectCartographer.ExpressionBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ObjectCartographer.ExpressionBuilder.Converters
{
    /// <summary>
    /// Collection converter
    /// </summary>
    /// <seealso cref="IConverter"/>
    public class ICollectionConverter : ConverterBaseClass
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public override int Order => OrderDefaults.DefaultPlusOne;

        /// <summary>
        /// Gets the type of the key value pair.
        /// </summary>
        /// <value>The type of the key value pair.</value>
        private static Type CollectionType { get; } = typeof(ICollection<>);

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
        public override bool CanHandle(Type sourceType, Type destinationType) => IsIEnumerable(sourceType) && IsCollection(destinationType);

        /// <summary>
        /// Copies to collection.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="expressions">The expressions.</param>
        /// <returns></returns>
        protected override Expression CopyObject(Expression source, Expression? destination, Type sourceType, Type destinationType, IExpressionMapping mapping, ExpressionBuilderManager manager, List<Expression> expressions)
        {
            Type SourceCollectionValueType = Array.Find(sourceType.GetInterfaces(), x => x.IsGenericType && x.GetGenericTypeDefinition() == IEnumerableType).GenericTypeArguments[0];
            Type? DestinationCollectionType = Array.Find(destinationType.GetInterfaces(), x => x.IsGenericType && x.GetGenericTypeDefinition() == CollectionType);
            Type DestionationCollectionValueType = DestinationCollectionType.GenericTypeArguments[0];
            System.Reflection.MethodInfo? AddMethod = DestinationCollectionType.GetMethod("Add", [DestionationCollectionValueType]);

            ParameterExpression ForEachItem = Expression.Variable(SourceCollectionValueType);
            ParameterExpression DestinationForEachItem = Expression.Variable(DestionationCollectionValueType);

            MethodCallExpression LoopBody = Expression.Call(destination, AddMethod!, manager.Map(ForEachItem, DestinationForEachItem, SourceCollectionValueType, DestionationCollectionValueType, mapping));

            ParameterExpression? SourceIEnumerable = mapping.AddVariable(typeof(IEnumerable<>).MakeGenericType(SourceCollectionValueType));
            expressions.Add(Expression.Assign(SourceIEnumerable!, source));
            expressions.Add(ForEach(SourceIEnumerable!, ForEachItem, LoopBody));
            expressions.Add(destination!);

            return Expression.Block(destinationType, new[] { DestinationForEachItem }, expressions);
        }

        /// <summary>
        /// Determines whether [is key value pair] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is key value pair] [the specified type]; otherwise, <c>false</c>.</returns>
        private bool IsCollection(Type type) => type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == CollectionType);

        /// <summary>
        /// Determines whether [is i enumerable] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is i enumerable] [the specified type]; otherwise, <c>false</c>.</returns>
        private bool IsIEnumerable(Type type) => type?.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == IEnumerableType) ?? false;
    }
}