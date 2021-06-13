using ObjectCartographer.ExpressionBuilder.BaseClasses;
using ObjectCartographer.ExpressionBuilder.Interfaces;
using ObjectCartographer.ExtensionMethods;
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
        public override int Order => 0;

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
        public override bool CanHandle(Type sourceType, Type destinationType)
        {
            return IsCollection(sourceType) && IsCollection(destinationType);
        }

        /// <summary>
        /// Maps the specified source to the destination.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="manager">The manager.</param>
        /// <returns>The resulting expression.</returns>
        public override Expression Map(Expression source, Expression? destination, Type sourceType, Type destinationType, IExpressionMapping mapping, ExpressionBuilderManager manager)
        {
            var CopyConstructor = GetCopyConstructor(sourceType, destinationType);
            if (CopyConstructor is null)
            {
                var Expressions = new List<Expression>
                {
                    CreateObject(destination, source, sourceType.ReadableProperties(), destinationType.PublicConstructors(), mapping, manager)
                };
                return CopyToCollection(source, destination, sourceType, destinationType, mapping, manager, Expressions);
            }
            else
            {
                return Expression.Block(destinationType,
                        Expression.IfThenElse(Expression.Equal(destination, Expression.Constant(null)),
                                Expression.Assign(destination, Expression.New(CopyConstructor, source)),
                                CopyToCollection(source, destination, sourceType, destinationType, mapping, manager, new List<Expression>())),
                        destination);
            }
        }

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
        private Expression CopyToCollection(Expression source, Expression? destination, Type sourceType, Type destinationType, IExpressionMapping mapping, ExpressionBuilderManager manager, List<Expression> expressions)
        {
            var SourceCollectionValueType = Array.Find(sourceType.GetInterfaces(), x => x.IsGenericType && x.GetGenericTypeDefinition() == IEnumerableType).GenericTypeArguments[0];
            var DestinationCollectionType = Array.Find(destinationType.GetInterfaces(), x => x.IsGenericType && x.GetGenericTypeDefinition() == CollectionType);
            var DestionationCollectionValueType = DestinationCollectionType.GenericTypeArguments[0];
            var AddMethod = DestinationCollectionType.GetMethod("Add", new[] { DestionationCollectionValueType });

            var ForEachItem = Expression.Variable(SourceCollectionValueType);
            var DestinationForEachItem = Expression.Variable(DestionationCollectionValueType);

            var LoopBody = Expression.Call(destination, AddMethod, manager.Map(ForEachItem, DestinationForEachItem, SourceCollectionValueType, DestionationCollectionValueType, mapping));

            var SourceIEnumerable = mapping.AddVariable(typeof(IEnumerable<>).MakeGenericType(SourceCollectionValueType));
            expressions.Add(Expression.Assign(SourceIEnumerable, source));
            expressions.Add(ForEach(SourceIEnumerable, ForEachItem, LoopBody));
            expressions.Add(destination);

            return Expression.Block(destinationType, new[] { DestinationForEachItem }, expressions);
        }

        /// <summary>
        /// Determines whether [is key value pair] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is key value pair] [the specified type]; otherwise, <c>false</c>.</returns>
        private bool IsCollection(Type type)
        {
            return type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == CollectionType);
        }

        /// <summary>
        /// Determines whether [is i enumerable] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is i enumerable] [the specified type]; otherwise, <c>false</c>.</returns>
        private bool IsIEnumerable(Type type)
        {
            return type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == IEnumerableType);
        }
    }
}