using ObjectCartographer.ExpressionBuilder.Interfaces;
using ObjectCartographer.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectCartographer.ExpressionBuilder.ObjectCreators
{
    /// <summary>
    /// Default creator
    /// </summary>
    /// <seealso cref="ICreator"/>
    public class DefaultCreator : ICreator
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order => int.MaxValue;

        /// <summary>
        /// Determines whether this instance can handle the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if this instance can handle the specified type; otherwise, <c>false</c>.</returns>
        public bool CanHandle(Type type)
        {
            return true;
        }

        /// <summary>
        /// Creates the specified destination variable.
        /// </summary>
        /// <param name="destinationVariable">The destination variable.</param>
        /// <param name="sourceVariable">The source variable.</param>
        /// <param name="sourceProperties">The source properties.</param>
        /// <param name="destinationConstructors">The destination constructors.</param>
        /// <param name="manager">The manager.</param>
        /// <returns></returns>
        public Expression Create(Expression destinationVariable, Expression sourceVariable, PropertyInfo[] sourceProperties, ConstructorInfo[] destinationConstructors, ExpressionBuilderManager manager)
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