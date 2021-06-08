using ObjectCartographer.ExpressionBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectCartographer.ExpressionBuilder
{
    /// <summary>
    /// Expression builder manager
    /// </summary>
    public class ExpressionBuilderManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionBuilderManager"/> class.
        /// </summary>
        /// <param name="builders">The builders.</param>
        /// <param name="converters">The converters.</param>
        public ExpressionBuilderManager(IEnumerable<IExpressionBuilder> builders, IEnumerable<IConverter> converters)
        {
            Builders = builders;
            Converters = converters;
        }

        /// <summary>
        /// Gets the builders.
        /// </summary>
        /// <value>The builders.</value>
        private IEnumerable<IExpressionBuilder> Builders { get; }

        /// <summary>
        /// Gets the converters.
        /// </summary>
        /// <value>The converters.</value>
        private IEnumerable<IConverter> Converters { get; }

        /// <summary>
        /// Converts the specified property get.
        /// </summary>
        /// <param name="propertyGet">The property get.</param>
        /// <param name="property">The property.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns></returns>
        public Expression Convert(Expression propertyGet, PropertyInfo property, Type destinationType)
        {
            return propertyGet;
        }

        /// <summary>
        /// Converts the specified source and destination.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="mapping">The mapping.</param>
        /// <returns>The resulting expression.</returns>
        public Func<TSource, TDestination, TDestination> Map<TSource, TDestination>(TypeMapping<TSource, TDestination> mapping)
        {
            return Builders.FirstOrDefault(x => x.CanHandle(mapping))?.Map(mapping, this) ?? ((_, y) => y);
        }
    }
}