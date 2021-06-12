using ObjectCartographer.ExpressionBuilder.Interfaces;
using ObjectCartographer.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ObjectCartographer.ExpressionBuilder.Converters
{
    /// <summary>
    /// Class to class converter
    /// </summary>
    /// <seealso cref="IConverter"/>
    public class ClassToClassConverter : IConverter
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order => int.MaxValue - 1;

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
            return !sourceType.IsValueType && !destinationType.IsValueType;
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
        public Expression Map(Expression source, Expression? destination, Type sourceType, Type destinationType, IExpressionMapping mapping, ExpressionBuilderManager manager)
        {
            var Expressions = new List<Expression>();
            foreach (var Property in sourceType.ReadableProperties())
            {
                var DestinationProperty = destinationType.WritableProperties().FindMatchingProperty(Property.Name);
                if (DestinationProperty is null)
                    continue;

                Expression PropertyGet = Expression.Property(mapping.SourceParameter, Property);
                var PropertySet = Expression.Property(mapping.DestinationParameter, DestinationProperty);

                PropertyGet = manager.Map(PropertyGet, PropertySet, Property.PropertyType, DestinationProperty.PropertyType, mapping);

                Expressions.Add(Expression.Assign(PropertySet, PropertyGet));
            }
            Expressions.Add(destination);
            return Expression.Block(destinationType, Expressions);
        }
    }
}