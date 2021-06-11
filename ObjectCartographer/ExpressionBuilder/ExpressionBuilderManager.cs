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
        /// <param name="expressionMappers">The expression mappers.</param>
        public ExpressionBuilderManager(IEnumerable<IExpressionMapper> expressionMappers)
        {
            ExpressionMappers = expressionMappers.OrderBy(x => x.Order).ToArray() ?? Array.Empty<IExpressionMapper>();
        }

        /// <summary>
        /// Gets the expression mappers.
        /// </summary>
        /// <value>The expression mappers.</value>
        public IExpressionMapper[] ExpressionMappers { get; }

        /// <summary>
        /// Gets the data mapper.
        /// </summary>
        /// <value>The data mapper.</value>
        internal DataMapper DataMapper { get; private set; }

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
        /// <param name="mapping">The mapping.</param>
        /// <returns>The resulting expression.</returns>
        public Func<TSource, TDestination, TDestination> Map<TSource, TDestination>(TypeMapping<TSource, TDestination> mapping)
        {
            var Mapping = new ExpressionMapping<TSource, TDestination>();
            Mapping.Expressions.Add(Array.Find(ExpressionMappers, x => x.CanHandle(mapping))?.Map(Mapping.SourceParameter, Mapping.DestinationParameter, mapping, this) ?? Expression.Empty());
            return Mapping.Build();
        }

        private Expression CreateObject(Expression destinationVariable, Expression sourceVariable, PropertyInfo[] sourceProperties, ConstructorInfo[] destinationConstructors, ExpressionBuilderManager manager)
        {
            if (destinationConstructors.Length == 0)
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
                    TempParams.Add(manager.Map(Expression.Property(sourceVariable, TempProperty), TempProperty.PropertyType, Param.ParameterType));
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
    }
}