using Microsoft.Extensions.Logging;
using ObjectCartographer.Interfaces;
using ObjectCartographer.Internal;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ObjectCartographer
{
    /// <summary>
    /// Type mapping
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TDestination">The type of the destination.</typeparam>
    /// <seealso cref="ITypeMapping"/>
    public class TypeMapping<TSource, TDestination> : ITypeMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeMapping"/> class.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <param name="logger">The logger.</param>
        public TypeMapping(TypeTuple typeInfo, ILogger? logger)
        {
            Source = typeInfo.Source;
            Destination = typeInfo.Destination;
            Logger = logger;
        }

        /// <summary>
        /// Gets or sets the converter.
        /// </summary>
        /// <value>The converter.</value>
        public Func<TSource, TDestination, TDestination>? Converter { get; set; }

        /// <summary>
        /// Gets the destination.
        /// </summary>
        /// <value>The destination.</value>
        public Type Destination { get; }

        /// <summary>
        /// Gets the mappings.
        /// </summary>
        /// <value>The mappings.</value>
        public List<IPropertyMapping> Properties { get; } = new List<IPropertyMapping>();

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>The source.</value>
        public Type Source { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        private ILogger? Logger { get; }

        /// <summary>
        /// Adds the mapping specified.
        /// </summary>
        /// <param name="leftExpression">The left expression.</param>
        /// <param name="rightExpression">The right expression.</param>
        /// <returns>This.</returns>
        public TypeMapping<TSource, TDestination> AddMapping(Expression<Func<TSource, object?>> leftExpression, Expression<Func<TDestination, object?>> rightExpression)
        {
            if (leftExpression is null || rightExpression is null)
                return this;
            Properties.Add(new PropertyMapping<TSource, TDestination>(leftExpression, rightExpression));
            return this;
        }

        /// <summary>
        /// Automatically maps the two types.
        /// </summary>
        /// <returns>This.</returns>
        public ITypeMapping AutoMap()
        {
            return this;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        public void Build()
        {
            Logger?.LogInformation($"Building {Source.Name} => {Destination.Name}");
        }

        /// <summary>
        /// Uses the method supplied instead of building out a converter.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <returns>This.</returns>
        public TypeMapping<TSource, TDestination> UseMethod(Func<TSource, TDestination, TDestination> func)
        {
            Converter = func;
            return this;
        }
    }
}