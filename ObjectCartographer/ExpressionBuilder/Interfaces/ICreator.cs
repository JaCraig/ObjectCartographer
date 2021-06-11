using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectCartographer.ExpressionBuilder.Interfaces
{
    /// <summary>
    /// Creator interface
    /// </summary>
    public interface ICreator
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        int Order { get; }

        /// <summary>
        /// Determines whether this instance can handle the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if this instance can handle the specified type; otherwise, <c>false</c>.</returns>
        bool CanHandle(Type type);

        /// <summary>
        /// Creates the specified destination variable.
        /// </summary>
        /// <param name="destinationVariable">The destination variable.</param>
        /// <param name="sourceVariable">The source variable.</param>
        /// <param name="sourceProperties">The source properties.</param>
        /// <param name="destinationConstructors">The destination constructors.</param>
        /// <param name="manager">The manager.</param>
        /// <returns></returns>
        Expression Create(Expression destinationVariable, Expression sourceVariable, PropertyInfo[] sourceProperties, ConstructorInfo[] destinationConstructors, ExpressionBuilderManager manager);
    }
}