using ObjectCartographer.ExpressionBuilder.BaseClasses;
using ObjectCartographer.ExpressionBuilder.Interfaces;
using System;
using System.Collections;
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
                return CopyToCollection(source, destination, sourceType, destinationType, mapping, manager);
            }
            else
            {
                return Expression.Block(destinationType,
                        Expression.IfThenElse(Expression.Equal(destination, Expression.Constant(null)),
                                Expression.Assign(destination, Expression.New(CopyConstructor, source)),
                                CopyToCollection(source, destination, sourceType, destinationType, mapping, manager)),
                        destination);
            }
        }

        private Expression CopyToCollection(Expression source, Expression? destination, Type sourceType, Type destinationType, IExpressionMapping mapping, ExpressionBuilderManager manager)
        {
            var CollectionValueType = sourceType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == CollectionType);
            var AddMethod = sourceType.GetMethod("Add", new[] { CollectionValueType });
            var LoopBody = Expression.Call()
            var loopBody = Expression.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(int) }), loopVar);
            var loop = ForEach(collection, loopVar, loopBody);
            var results = Expression.Lambda<Action<List<int>>>(loop, collection).Compile();

            var SourceIEnumerable = mapping.AddVariable(typeof(IEnumerable));
            List<Expression> Expressions = new List<Expression>
            {
                Expression.Assign(SourceIEnumerable, source),
                ForEach(SourceIEnumerable, Expression.Parameter(typeof(int), "loopVar"), loopBody)
            };

            foreach (var Item in (IEnumerable)item)
            {
                TempList.Add(ConvertTo(item, IEnumerableResultType));
            }
            return TempList;

            var SourceKey = manager.Map(Expression.Property(source, "Key"), null, SourceArgs[0], DestinationArgs[0], mapping);
            var SourceValue = manager.Map(Expression.Property(source, "Value"), null, SourceArgs[1], DestinationArgs[1], mapping);
            return Expression.New(destinationType.GetConstructor(DestinationArgs), SourceKey, SourceValue);
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
    }
}