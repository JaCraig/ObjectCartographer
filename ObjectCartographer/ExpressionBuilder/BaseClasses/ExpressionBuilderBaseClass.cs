using ObjectCartographer.ExpressionBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectCartographer.ExpressionBuilder.BaseClasses
{
    /// <summary>
    /// Expression builder base class
    /// </summary>
    /// <seealso cref="IExpressionBuilder"/>
    public abstract class ExpressionBuilderBaseClass : IExpressionBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionBuilderBaseClass"/> class.
        /// </summary>
        protected ExpressionBuilderBaseClass()
        {
        }

        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public abstract int Order { get; }

        /// <summary>
        /// Determines whether this instance can handle the specified types.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="mapping">The mapping.</param>
        /// <returns>
        /// <c>true</c> if this instance can handle the specified types; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool CanHandle<TSource, TDestination>(TypeMapping<TSource, TDestination> mapping);

        /// <summary>
        /// Converts the specified source and destination.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="mapping">The mapping.</param>
        /// <param name="manager">The manager.</param>
        /// <returns>The resulting expression.</returns>
        public abstract Func<TSource, TDestination, TDestination> Map<TSource, TDestination>(TypeMapping<TSource, TDestination> mapping, ExpressionBuilderManager manager);

        /// <summary>
        /// Creates the object if needed.
        /// </summary>
        /// <param name="destinationVariable">The destination variable.</param>
        /// <param name="sourceVariable">The source variable.</param>
        /// <param name="sourceProperties">The source properties.</param>
        /// <param name="destinationConstructors">The destination constructors.</param>
        /// <param name="manager">The manager.</param>
        /// <returns></returns>
        protected Expression CreateObjectIfNeeded(Expression destinationVariable, Expression sourceVariable, PropertyInfo[] sourceProperties, ConstructorInfo[] destinationConstructors, ExpressionBuilderManager manager)
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
                    var TempProperty = FindMatchingProperty(sourceProperties, Param.Name);
                    if (TempProperty is null)
                    {
                        Found = false;
                        break;
                    }
                    TempParams.Add(manager.Convert(Expression.Property(sourceVariable, TempProperty), TempProperty, Param.ParameterType));
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

        protected PropertyInfo? FindMatchingProperty(PropertyInfo[] properties, string name)
        {
            return Array.Find(properties, x => x.Name == name)
                    ?? Array.Find(properties, x => string.Equals(RemoveChars(x.Name), RemoveChars(name), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Removes chars to find a match.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The resulting name</returns>
        protected string RemoveChars(string name)
        {
            return name.Replace("_", "");
        }
    }
}