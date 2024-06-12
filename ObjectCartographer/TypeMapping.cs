using Microsoft.Extensions.Logging;
using ObjectCartographer.ExpressionBuilder;
using ObjectCartographer.Interfaces;
using ObjectCartographer.Internal;
using System;
using System.Collections.Generic;

namespace ObjectCartographer
{
    /// <summary>
    /// Type mapping
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TDestination">The type of the destination.</typeparam>
    /// <seealso cref="ITypeMapping"/>
    public class TypeMapping<TSource, TDestination> : ITypeMapping, IInternalTypeMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeMapping{TSource, TDestination}"/> class.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="expressionBuilder">The expression builder.</param>
        public TypeMapping(TypeTuple typeInfo, ILogger? logger, ExpressionBuilderManager? expressionBuilder)
        {
            Logger = logger;
            ExpressionBuilder = expressionBuilder;
            TypeInfo = typeInfo;
        }

        /// <summary>
        /// Gets or sets the converter.
        /// </summary>
        /// <value>The converter.</value>
        public Func<TSource, TDestination, TDestination>? Converter { get; private set; }

        /// <summary>
        /// Gets the mappings.
        /// </summary>
        /// <value>The mappings.</value>
        public List<IPropertyMapping> Properties { get; } = new List<IPropertyMapping>();

        /// <summary>
        /// Gets the type information.
        /// </summary>
        /// <value>The type information.</value>
        public TypeTuple TypeInfo { get; }

        /// <summary>
        /// Gets the expression builder.
        /// </summary>
        /// <value>The expression builder.</value>
        private ExpressionBuilderManager? ExpressionBuilder { get; }

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
        public TypeMapping<TSource, TDestination> AddMapping<TSourceProperty>(Func<TSource, TSourceProperty> leftExpression, Action<TDestination, TSourceProperty> rightExpression)
        {
            if (leftExpression is null || rightExpression is null)
                return this;
            Properties.Add(new PropertyMapping<TSource, TDestination, TSourceProperty>(leftExpression, rightExpression));
            return this;
        }

        /// <summary>
        /// Automatically maps the two types.
        /// </summary>
        /// <returns>This.</returns>
        public ITypeMapping AutoMap()
        {
            if (Properties.Count > 0 || Converter is not null)
                return this;
            Logger?.LogDebug("Automapping {TypeInfoSource} => {TypeInfoDestination}", TypeInfo.Source, TypeInfo.Destination);
            Converter = ExpressionBuilder?.Map(this) ?? ((_, y) => y);
            return this;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        public void Build()
        {
            if (Converter is not null)
                return;
            Logger?.LogDebug("Building {TypeInfoSource} => {TypeInfoDestination}", TypeInfo.Source, TypeInfo.Destination);
            Converter = ExpressionBuilder?.Map(this) ?? ((_, y) => y);
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