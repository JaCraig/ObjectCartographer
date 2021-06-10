using Fast.Activator;
using ObjectCartographer.ExpressionBuilder.BaseClasses;
using ObjectCartographer.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectCartographer.ExpressionBuilder.ExpressionBuilders
{
    /// <summary>
    /// Both items are IDictionaries
    /// </summary>
    /// <seealso cref="ExpressionBuilderBaseClass"/>
    public class BothIDictionary : ExpressionBuilderBaseClass
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public override int Order => -1;

        /// <summary>
        /// Gets the default method information.
        /// </summary>
        /// <value>The default method information.</value>
        private static MethodInfo DefaultMethodInfo { get; } = typeof(BothIDictionary).GetMethod("DefaultMethod", BindingFlags.NonPublic | BindingFlags.Static);

        /// <summary>
        /// The dictionary type
        /// </summary>
        private static readonly Type DictionaryType = typeof(IDictionary<,>);

        /// <summary>
        /// Determines whether this instance can handle the specified types.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="mapping">The mapping.</param>
        /// <returns>
        /// <c>true</c> if this instance can handle the specified types; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanHandle<TSource, TDestination>(TypeMapping<TSource, TDestination> mapping)
        {
            return IsDictionary(mapping.TypeInfo.Source) && IsDictionary(mapping.TypeInfo.Destination);
        }

        /// <summary>
        /// Converts the specified source and destination.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="mapping">The mapping.</param>
        /// <param name="manager">The manager.</param>
        /// <returns>The resulting expression.</returns>
        public override Func<TSource, TDestination, TDestination> Map<TSource, TDestination>(TypeMapping<TSource, TDestination> mapping, ExpressionBuilderManager manager)
        {
            var SourceType = typeof(TSource);
            var DestinationType = typeof(TDestination);
            var FinalMethod = DefaultMethodInfo.MakeGenericMethod(SourceType, DestinationType);

            List<Expression> Expressions = new List<Expression>();
            var SourceObjectInstance = Expression.Parameter(SourceType, "source");
            var DestinationObjectInstance = Expression.Parameter(DestinationType, "destination");

            Expressions.Add(CreateObjectIfNeeded(DestinationObjectInstance, SourceObjectInstance, TypeCache<TSource>.ReadableProperties, TypeCache<TDestination>.Constructors, manager));
            Expressions.Add(Expression.Call(FinalMethod, SourceObjectInstance, DestinationObjectInstance));
            var BlockExpression = Expression.Block(DestinationType, Expressions);
            var SourceLambda = Expression.Lambda<Func<TSource, TDestination, TDestination>>(BlockExpression, SourceObjectInstance, DestinationObjectInstance);
            return SourceLambda.Compile();
        }

        /// <summary>
        /// Default mapping method.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns>The resulting dictionary.</returns>
        private static TDestination DefaultMethod<TSource, TDestination>(TSource source, TDestination destination)
        {
            destination ??= FastActivator.CreateInstance<TDestination>();
            if (!(source is IDictionary<string, object> TempSource))
                return destination;
            if (!(destination is IDictionary<string, object> TempDestination))
                return destination;
            foreach (var item in TempSource)
            {
                TempDestination.Add(item.Key, item.Value);
            }
            return destination;
        }

        /// <summary>
        /// Determines whether the specified type is dictionary.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is dictionary; otherwise, <c>false</c>.</returns>
        private static bool IsDictionary(Type type)
        {
            var Interfaces = type.GetInterfaces();
            return Interfaces.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == DictionaryType);
        }
    }
}