using ObjectCartographer.ExpressionBuilder.BaseClasses;
using ObjectCartographer.ExpressionBuilder.Interfaces;
using ObjectCartographer.Internal;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ObjectCartographer.ExpressionBuilder.ExpressionBuilders
{
    /// <summary>
    /// Pre mapped
    /// </summary>
    /// <seealso cref="IExpressionBuilder"/>
    public class PreMapped : ExpressionBuilderBaseClass
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public override int Order => int.MinValue;

        /// <summary>
        /// Determines whether this instance can handle the specified mapping.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="mapping">The mapping.</param>
        /// <returns>
        /// <c>true</c> if this instance can handle the specified mapping; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanHandle<TSource, TDestination>(TypeMapping<TSource, TDestination> mapping)
        {
            return mapping.Properties.Count > 0;
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

            foreach (var Property in mapping.Properties)
            {
                Expressions.Add(Expression.Assign(Property.Destination, Property.Source));
            }
            if (Expressions.Count == 0)
                return (_, y) => y;
            Expressions.Add(DestinationObjectInstance);
            var BlockExpression = Expression.Block(Expressions);
            var SourceLambda = Expression.Lambda<Func<TSource, TDestination, TDestination>>(BlockExpression, SourceObjectInstance, DestinationObjectInstance);
            return SourceLambda.Compile();
        }
    }
}