using ObjectCartographer.ExpressionBuilder.BaseClasses;
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
    public class ClassToClassConverter : ConverterBaseClass
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public override int Order => int.MaxValue - 1;

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
            return !sourceType.IsValueType && !destinationType.IsValueType;
        }

        /// <summary>
        /// Copies the properties.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="expressions">The expressions.</param>
        /// <returns></returns>
        protected override Expression CopyObject(
            Expression source,
            Expression? destination,
            Type sourceType,
            Type destinationType,
            IExpressionMapping mapping,
            ExpressionBuilderManager manager,
            List<Expression> expressions)
        {
            foreach (var Property in sourceType.ReadableProperties())
            {
                var DestinationProperty = destinationType.WritableProperties().FindMatchingProperty(Property.Name);
                if (DestinationProperty is null)
                    continue;

                Expression PropertyGet = Expression.Property(source, Property);
                var PropertySet = Expression.Property(destination, DestinationProperty);

                PropertyGet = manager.Map(PropertyGet, PropertySet, Property.PropertyType, DestinationProperty.PropertyType, mapping);

                expressions.Add(Expression.Assign(PropertySet, PropertyGet));
            }
            expressions.Add(destination);
            return Expression.Block(destinationType, expressions);
        }
    }
}