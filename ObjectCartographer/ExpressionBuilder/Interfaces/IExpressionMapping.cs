using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ObjectCartographer.ExpressionBuilder.Interfaces
{
    /// <summary>
    /// Expression mapping interface
    /// </summary>
    public interface IExpressionMapping
    {
        /// <summary>
        /// Gets the destination parameter.
        /// </summary>
        /// <value>The destination parameter.</value>
        ParameterExpression DestinationParameter { get; }

        /// <summary>
        /// Gets the type of the destination.
        /// </summary>
        /// <value>The type of the destination.</value>
        Type DestinationType { get; }

        /// <summary>
        /// Gets the expressions.
        /// </summary>
        /// <value>The expressions.</value>
        Expression? FinalExpression { get; set; }

        /// <summary>
        /// Gets the source parameter.
        /// </summary>
        /// <value>The source parameter.</value>
        ParameterExpression SourceParameter { get; }

        /// <summary>
        /// Gets the type of the source.
        /// </summary>
        /// <value>The type of the source.</value>
        Type SourceType { get; }

        /// <summary>
        /// Gets the variables.
        /// </summary>
        /// <value>The variables.</value>
        List<ParameterExpression> Variables { get; }

        /// <summary>
        /// Adds the variable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The new variable</returns>
        ParameterExpression? AddVariable(Type type);
    }
}