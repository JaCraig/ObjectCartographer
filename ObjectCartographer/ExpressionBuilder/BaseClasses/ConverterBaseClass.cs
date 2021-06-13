using ObjectCartographer.ExpressionBuilder.Interfaces;
using ObjectCartographer.ExtensionMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectCartographer.ExpressionBuilder.BaseClasses
{
    /// <summary>
    /// Converter base class
    /// </summary>
    /// <seealso cref="IConverter"/>
    public abstract class ConverterBaseClass : IConverter
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public abstract int Order { get; }

        /// <summary>
        /// Determines whether this instance can handle the specified types.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns>
        /// <c>true</c> if this instance can handle the specified types; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool CanHandle(Type source, Type destination);

        /// <summary>
        /// Maps the specified source to the destination.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="manager">The manager.</param>
        /// <returns>The resulting expression.</returns>
        public abstract Expression Map(Expression source, Expression? destination, Type sourceType, Type destinationType, IExpressionMapping mapping, ExpressionBuilderManager manager);

        /// <summary>
        /// Creates the object.
        /// </summary>
        /// <param name="destinationVariable">The destination variable.</param>
        /// <param name="sourceVariable">The source variable.</param>
        /// <param name="sourceProperties">The source properties.</param>
        /// <param name="destinationConstructors">The destination constructors.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="manager">The manager.</param>
        /// <returns></returns>
        protected Expression CreateObject(Expression destinationVariable, Expression sourceVariable, PropertyInfo[] sourceProperties, ConstructorInfo[] destinationConstructors, IExpressionMapping mapping, ExpressionBuilderManager manager)
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
                    TempParams.Add(manager.Map(Expression.Property(sourceVariable, TempProperty), null, TempProperty.PropertyType, Param.ParameterType, mapping));
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
        /// Foreach loop helper method.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="loopVar">The loop variable.</param>
        /// <param name="loopContent">Content of the loop.</param>
        /// <returns>The resulting expression</returns>
        protected Expression ForEach(Expression collection, ParameterExpression loopVar, Expression loopContent)
        {
            var elementType = loopVar.Type;
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
            var enumeratorType = typeof(IEnumerator<>).MakeGenericType(elementType);

            var enumeratorVar = Expression.Variable(enumeratorType, "enumerator");
            var getEnumeratorCall = Expression.Call(collection, enumerableType.GetMethod("GetEnumerator"));
            var enumeratorAssign = Expression.Assign(enumeratorVar, getEnumeratorCall);

            var moveNextCall = Expression.Call(enumeratorVar, typeof(IEnumerator).GetMethod("MoveNext"));

            var breakLabel = Expression.Label("LoopBreak");

            var ifThenElseExpr = Expression.IfThenElse(
                Expression.Equal(moveNextCall, Expression.Constant(true)),
                Expression.Block(new[] { loopVar },
                    Expression.Assign(loopVar, Expression.Property(enumeratorVar, "Current")),
                    loopContent
                ),
                Expression.Break(breakLabel)
            );

            var loop = Expression.Loop(ifThenElseExpr, breakLabel);

            return Expression.Block(new[] { enumeratorVar },
                enumeratorAssign,
                loop
            );
        }

        /// <summary>
        /// Gets the constructor.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns></returns>
        protected ConstructorInfo GetCopyConstructor(Type source, Type destination)
        {
            return destination.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[] { source }, null);
        }
    }
}