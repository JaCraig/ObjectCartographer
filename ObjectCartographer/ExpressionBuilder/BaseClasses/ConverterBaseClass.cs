﻿using ObjectCartographer.ExpressionBuilder.Interfaces;
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
        /// Initializes a new instance of the <see cref="ConverterBaseClass"/> class.
        /// </summary>
        protected ConverterBaseClass()
        { }

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
        public Expression Map(Expression source, Expression? destination, Type sourceType, Type destinationType, IExpressionMapping mapping, ExpressionBuilderManager manager)
        {
            if (!CanHandle(sourceType, destinationType))
                return Expression.Empty();
            ConstructorInfo? CopyConstructor = GetCopyConstructor(sourceType, destinationType);
            if (CopyConstructor is null)
            {
                return Expression.Block(destinationType,
                    Expression.IfThenElse(Expression.Equal(source, Expression.Constant(null)),
                        Expression.Empty(),
                        Expression.Block(destinationType,
                            CreateObject(destination!, source, sourceType.ReadableProperties(), destinationType.PublicConstructors(), mapping, manager),
                            CopyObject(source, destination!, sourceType, destinationType, mapping, manager, []),
                            destination!)
                        ),
                    destination!);
            }
            else
            {
                return Expression.Block(destinationType,
                    Expression.IfThenElse(Expression.Equal(source, Expression.Constant(null)),
                        Expression.Empty(),
                        Expression.IfThenElse(Expression.Equal(destination!, Expression.Constant(null)),
                                Expression.Assign(destination!, Expression.New(CopyConstructor, source)),
                                CopyObject(source, destination!, sourceType, destinationType, mapping, manager, []))),
                    destination!);
            }
        }

        /// <summary>
        /// Copies the object after object is created.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="expressions">The expressions.</param>
        /// <returns>The resulting expression.</returns>
        protected abstract Expression CopyObject(Expression source, Expression destination, Type sourceType, Type destinationType, IExpressionMapping mapping, ExpressionBuilderManager manager, List<Expression> expressions);

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
                return destinationVariable ?? Expression.Empty();
            ConstructorInfo FinalConstructor = destinationConstructors[^1];
            var FinalParameters = new List<Expression>();
            for (var x = 0; x < destinationConstructors.Length; ++x)
            {
                ParameterInfo[] Parameters = destinationConstructors[x].GetParameters();

                var TempParams = new List<Expression>();
                var Found = true;
                for (var y = 0; y < Parameters.Length; ++y)
                {
                    ParameterInfo Param = Parameters[y];
                    if (Param is null || string.IsNullOrEmpty(Param.Name))
                    {
                        Found = false;
                        break;
                    }
                    PropertyInfo? TempProperty = sourceProperties.FindMatchingProperty(Param.Name);
                    if (TempProperty is null || TempProperty.GetIndexParameters().Length > 0)
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
            if (FinalConstructor.GetParameters().Length != FinalParameters.Count)
                return Expression.Empty();
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
            Type elementType = loopVar.Type;
            Type enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
            Type enumeratorType = typeof(IEnumerator<>).MakeGenericType(elementType);

            ParameterExpression enumeratorVar = Expression.Variable(enumeratorType, "enumerator");
            MethodInfo? GetEnumeratorMethod = enumerableType.GetMethod("GetEnumerator");
            if (GetEnumeratorMethod is null)
                return Expression.Empty();
            MethodCallExpression getEnumeratorCall = Expression.Call(collection, GetEnumeratorMethod);
            BinaryExpression enumeratorAssign = Expression.Assign(enumeratorVar, getEnumeratorCall);
            MethodInfo? MoveNextMethod = typeof(IEnumerator).GetMethod("MoveNext");
            if (MoveNextMethod is null)
                return Expression.Empty();
            MethodCallExpression moveNextCall = Expression.Call(enumeratorVar, MoveNextMethod);

            LabelTarget breakLabel = Expression.Label("LoopBreak");

            ConditionalExpression ifThenElseExpr = Expression.IfThenElse(
                Expression.Equal(moveNextCall, Expression.Constant(true)),
                Expression.Block(new[] { loopVar },
                    Expression.Assign(loopVar, Expression.Property(enumeratorVar, "Current")),
                    loopContent
                ),
                Expression.Break(breakLabel)
            );

            LoopExpression loop = Expression.Loop(ifThenElseExpr, breakLabel);

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
        protected ConstructorInfo? GetCopyConstructor(Type source, Type destination) => destination?.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, [source], null);
    }
}