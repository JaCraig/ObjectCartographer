using ObjectCartographer.ExpressionBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ObjectCartographer.ExpressionBuilder
{
    /// <summary>
    /// Expression mapping
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TDestination">The type of the destination.</typeparam>
    public class ExpressionMapping<TSource, TDestination> : IExpressionMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionMapping"/> class.
        /// </summary>
        public ExpressionMapping()
        {
            SourceParameter = Expression.Parameter(SourceType, "source");
            DestinationParameter = Expression.Parameter(DestinationType, "destination");
        }

        /// <summary>
        /// Gets the destination parameter.
        /// </summary>
        /// <value>The destination parameter.</value>
        public ParameterExpression DestinationParameter { get; }

        /// <summary>
        /// Gets the type of the destination.
        /// </summary>
        /// <value>The type of the destination.</value>
        public Type DestinationType { get; } = typeof(TDestination);

        /// <summary>
        /// Gets or sets the final expression.
        /// </summary>
        /// <value>The final expression.</value>
        public Expression FinalExpression { get; set; }

        /// <summary>
        /// Gets the source parameter.
        /// </summary>
        /// <value>The source parameter.</value>
        public ParameterExpression SourceParameter { get; }

        /// <summary>
        /// Gets the type of the source.
        /// </summary>
        /// <value>The type of the source.</value>
        public Type SourceType { get; } = typeof(TSource);

        /// <summary>
        /// Gets the variables.
        /// </summary>
        /// <value>The variables.</value>
        public List<ParameterExpression> Variables { get; } = new List<ParameterExpression>();

        /// <summary>
        /// Adds the variable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The new variable</returns>
        public ParameterExpression AddVariable(Type type)
        {
            var NewVariable = Expression.Variable(type);
            Variables.Add(NewVariable);
            return NewVariable;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>The final function.</returns>
        public Func<TSource, TDestination, TDestination> Build()
        {
            if (FinalExpression is null)
                return (_, y) => y;
            var BlockExpression = Expression.Block(
                DestinationType,
                Variables.ToArray(),
                Expression.Assign(DestinationParameter, FinalExpression),
                DestinationParameter);
            var SourceLambda = Expression.Lambda<Func<TSource, TDestination, TDestination>>(BlockExpression, SourceParameter, DestinationParameter);
            return SourceLambda.Compile();
        }
    }
}