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
        /// <param name="dataMapper">The data mapper.</param>
        public TypeMapping(TypeTuple typeInfo)
        {
            Source = typeInfo.Source;
            Destination = typeInfo.Destination;
        }

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
        }
    }
}