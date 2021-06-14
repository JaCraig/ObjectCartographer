using ObjectCartographer.ExpressionBuilder.Interfaces;
using ObjectCartographer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
        /// <param name="expressionMappers">The expression mappers.</param>
        public ExpressionBuilderManager(IEnumerable<IConverter> expressionMappers)
        {
            ExpressionMappers = expressionMappers.OrderBy(x => x.Order).ToArray() ?? Array.Empty<IConverter>();
        }

        /// <summary>
        /// Gets the expression mappers.
        /// </summary>
        /// <value>The expression mappers.</value>
        private IConverter[] ExpressionMappers { get; }

        /// <summary>
        /// Converts the specified source and destination.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="typeInfo">The mapping.</param>
        /// <returns>The resulting expression.</returns>
        public Func<TSource, TDestination, TDestination> Map<TSource, TDestination>(TypeMapping<TSource, TDestination> typeInfo)
        {
            var Mapping = new ExpressionMapping<TSource, TDestination>();
            Mapping.FinalExpression = GenerateMappings(typeInfo, Mapping);
            return Mapping.Build();
        }

        /// <summary>
        /// Maps the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <param name="mapping">The mapping.</param>
        /// <returns></returns>
        public Expression Map(Expression source, Expression? destination, Type sourceType, Type destinationType, IExpressionMapping mapping)
        {
            return Array
                    .Find(ExpressionMappers, x => x.CanHandle(sourceType, destinationType))?
                    .Map(source, destination, sourceType, destinationType, mapping, this) ?? Expression.Empty();
        }

        /// <summary>
        /// Adds the pre mapped properties.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <returns></returns>
        private static Expression AddPreMappedProperties(IInternalTypeMapping typeInfo)
        {
            List<Expression> Expressions = new List<Expression>();
            foreach (var Property in typeInfo.Properties)
            {
                Expressions.Add(Expression.Assign(Property.Destination, Property.Source));
            }
            return Expression.Block(Expressions);
        }

        /// <summary>
        /// Generates the mappings.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <param name="mapping">The mapping.</param>
        private Expression GenerateMappings(IInternalTypeMapping typeInfo, IExpressionMapping mapping)
        {
            return Expression.Block(
                typeInfo.TypeInfo.Destination,
                Expression.Assign(mapping.DestinationParameter, Map(
                    mapping.SourceParameter,
                    mapping.DestinationParameter,
                    typeInfo.TypeInfo.Source,
                    typeInfo.TypeInfo.Destination,
                    mapping
                )),
                AddPreMappedProperties(typeInfo),
                mapping.DestinationParameter
            );
        }
    }
}