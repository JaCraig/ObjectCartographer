using ObjectCartographer.ExpressionBuilder.Interfaces;
using ObjectCartographer.ExtensionMethods;
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
        /// <param name="expressionMappers">The expression mappers.</param>
        public ExpressionBuilderManager(IEnumerable<IConverter> expressionMappers)
        {
            ExpressionMappers = expressionMappers.OrderBy(x => x.Order).ToArray() ?? Array.Empty<IConverter>();
        }

        /// <summary>
        /// Gets the data mapper.
        /// </summary>
        /// <value>The data mapper.</value>
        internal DataMapper? DataMapper { get; private set; }

        /// <summary>
        /// Gets the expression mappers.
        /// </summary>
        /// <value>The expression mappers.</value>
        private IConverter[] ExpressionMappers { get; }

        /// <summary>
        /// Initializes the specified data mapper.
        /// </summary>
        /// <param name="dataMapper">The data mapper.</param>
        /// <returns>This</returns>
        public ExpressionBuilderManager Initialize(DataMapper dataMapper)
        {
            DataMapper = dataMapper;
            return this;
        }

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
            AddPreMappedProperties(typeInfo, Mapping);
            GenerateMappings(typeInfo, Mapping);
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
            return Expression.Block(
                CreateObject(destination, source, sourceType.ReadableProperties(), destinationType.GetConstructors().Where(x => x.IsPublic).ToArray(), mapping) ?? Expression.Empty(),
                Array
                    .Find(ExpressionMappers, x => x.CanHandle(sourceType, destinationType))?
                    .Map(source, destination, sourceType, destinationType, mapping, this) ?? Expression.Empty()
            );
        }

        /// <summary>
        /// Adds the pre mapped properties.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="typeInfo">The type information.</param>
        /// <param name="Mapping">The mapping.</param>
        private static void AddPreMappedProperties<TSource, TDestination>(TypeMapping<TSource, TDestination> typeInfo, ExpressionMapping<TSource, TDestination> Mapping)
        {
            foreach (var Property in typeInfo.Properties)
            {
                Mapping.Expressions.Add(Expression.Assign(Property.Destination, Property.Source));
            }
        }

        /// <summary>
        /// Creates the object.
        /// </summary>
        /// <param name="destinationVariable">The destination variable.</param>
        /// <param name="sourceVariable">The source variable.</param>
        /// <param name="sourceProperties">The source properties.</param>
        /// <param name="destinationConstructors">The destination constructors.</param>
        /// <param name="mapping">The mapping.</param>
        /// <returns></returns>
        private Expression CreateObject(Expression destinationVariable, Expression sourceVariable, PropertyInfo[] sourceProperties, ConstructorInfo[] destinationConstructors, IExpressionMapping mapping)
        {
            if (destinationVariable is null || destinationConstructors.Length == 0 || !destinationVariable.Type.IsClass)
                return destinationVariable;
            var FinalConstructor = destinationConstructors[^1];
            var FinalParameters = new List<Expression>();
            for (var x = 0; x < destinationConstructors.Length; ++x)
            {
                var Parameters = destinationConstructors[x].GetParameters();

                var TempParams = new List<Expression>();
                bool Found = true;
                for (int y = 0; y < Parameters.Length; ++y)
                {
                    var Param = Parameters[y];
                    var TempProperty = sourceProperties.FindMatchingProperty(Param.Name);
                    if (TempProperty is null)
                    {
                        Found = false;
                        break;
                    }
                    TempParams.Add(Map(Expression.Property(sourceVariable, TempProperty), null, TempProperty.PropertyType, Param.ParameterType, mapping));
                }
                if (Found)
                {
                    FinalConstructor = destinationConstructors[x];
                    FinalParameters = TempParams;
                    break;
                }
            }
            return Expression.Assign(destinationVariable, Expression.Coalesce(destinationVariable, Expression.New(FinalConstructor, FinalParameters.ToArray())));
        }

        /// <summary>
        /// Generates the mappings.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="typeInfo">The type information.</param>
        /// <param name="Mapping">The mapping.</param>
        private void GenerateMappings<TSource, TDestination>(TypeMapping<TSource, TDestination> typeInfo, ExpressionMapping<TSource, TDestination> Mapping)
        {
            if (typeInfo.Properties.Count > 0)
                return;
            Mapping.Expressions.Add(Map(
                Mapping.SourceParameter,
                Mapping.DestinationParameter,
                typeInfo.TypeInfo.Source,
                typeInfo.TypeInfo.Destination,
                Mapping));
        }
    }
}