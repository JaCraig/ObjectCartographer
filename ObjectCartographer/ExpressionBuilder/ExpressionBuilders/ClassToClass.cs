using ObjectCartographer.ExpressionBuilder.BaseClasses;
using ObjectCartographer.ExpressionBuilder.Interfaces;
using ObjectCartographer.Internal;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ObjectCartographer.ExpressionBuilder.ExpressionBuilders
{
    /// <summary>
    /// Simple class to class copying.
    /// </summary>
    /// <seealso cref="IExpressionBuilder"/>
    public class ClassToClass : ExpressionBuilderBaseClass
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public override int Order => int.MaxValue;

        /// <summary>
        /// Determines whether this instance can handle the specified types.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="mapping">The mapping.</param>
        /// <returns>
        /// <c>true</c> if this instance can handle the specified types; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanHandle<TSource, TDestination>(TypeMapping<TSource, TDestination> mapping)
        {
            return !mapping.TypeInfo.Source.IsValueType && !mapping.TypeInfo.Destination.IsValueType;
        }

        /// <summary>
        /// Converts the specified source and destination.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="mapping">The mapping.</param>
        /// <param name="manager">The manager.</param>
        /// <returns>The resulting expression.</returns>
        public override Func<TSource, TDestination, TDestination> Map<TSource, TDestination>(TypeMapping<TSource, TDestination> mapping, ExpressionBuilderManager manager)
        {
            var SourceType = typeof(TSource);
            var DestinationType = typeof(TDestination);

            List<Expression> Expressions = new List<Expression>();
            var SourceObjectInstance = Expression.Parameter(SourceType, "source");
            var DestinationObjectInstance = Expression.Parameter(DestinationType, "destination");

            Expressions.Add(CreateObjectIfNeeded(DestinationObjectInstance, SourceObjectInstance, TypeCache<TSource>.ReadableProperties, TypeCache<TDestination>.Constructors, manager));

            foreach (var Property in TypeCache<TSource>.ReadableProperties)
            {
                var DestinationProperty = Array.Find(TypeCache<TDestination>.WritableProperties, x => x.Name == Property.Name)
                    ?? Array.Find(TypeCache<TDestination>.WritableProperties, x => string.Equals(RemoveChars(x.Name), RemoveChars(Property.Name), StringComparison.OrdinalIgnoreCase));
                if (DestinationProperty is null)
                    continue;

                Expression PropertyGet = Expression.Property(SourceObjectInstance, Property);
                var PropertySet = Expression.Property(DestinationObjectInstance, DestinationProperty);

                PropertyGet = manager.Convert(PropertyGet, Property, DestinationProperty.PropertyType);

                Expressions.Add(Expression.Assign(PropertySet, PropertyGet));
            }
            if (Expressions.Count == 0)
                return (_, y) => y;
            Expressions.Add(DestinationObjectInstance);
            var BlockExpression = Expression.Block(DestinationType, Expressions);
            var SourceLambda = Expression.Lambda<Func<TSource, TDestination, TDestination>>(BlockExpression, SourceObjectInstance, DestinationObjectInstance);
            return SourceLambda.Compile();
        }
    }
}