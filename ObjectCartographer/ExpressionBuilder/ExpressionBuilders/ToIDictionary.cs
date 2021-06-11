using ObjectCartographer.ExpressionBuilder.BaseClasses;
using ObjectCartographer.ExtensionMethods;
using ObjectCartographer.Internal;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectCartographer.ExpressionBuilder.ExpressionBuilders
{
    /// <summary>
    /// To IDictionary
    /// </summary>
    /// <seealso cref="ExpressionBuilderBaseClass"/>
    public class ToIDictionary : ExpressionBuilderBaseClass
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public override int Order => 0;

        /// <summary>
        /// The dictionary type
        /// </summary>
        private static readonly Type DictionaryType = typeof(IDictionary<string, object>);

        /// <summary>
        /// The set value method
        /// </summary>
        private static readonly MethodInfo SetValueMethod = typeof(IDictionary<string, object>).GetMethod("Add");

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
            return DictionaryType.IsAssignableFrom(mapping.TypeInfo.Destination);
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
            var IDictionaryType = Array.Find(DestinationType.GetInterfaces(), x => string.Equals(x.Name, "IDictionary`2", StringComparison.OrdinalIgnoreCase));
            var IDictionaryValueType = IDictionaryType.GetGenericArguments()[1];

            List<Expression> Expressions = new List<Expression>();
            var SourceObjectInstance = Expression.Parameter(SourceType, "source");
            var DestinationObjectInstance = Expression.Parameter(DestinationType, "destination");

            var DestinationObjectAsIDictionary = Expression.Variable(IDictionaryType);

            Expressions.Add(manager.Create(DestinationObjectInstance, SourceObjectInstance, TypeCache<TSource>.ReadableProperties, TypeCache<TDestination>.Constructors));
            Expressions.Add(Expression.Assign(DestinationObjectAsIDictionary, Expression.Convert(DestinationObjectInstance, IDictionaryType)));

            for (var x = 0; x < TypeCache<TSource>.ReadableProperties.Length; ++x)
            {
                var SourceProperty = TypeCache<TSource>.ReadableProperties[x];
                var DestinationProperty = TypeCache<TDestination>.WritableProperties.FindMatchingProperty(SourceProperty.Name);

                var PropertyGet = manager.Convert(Expression.Property(SourceObjectInstance, SourceProperty), SourceProperty.PropertyType, IDictionaryValueType);

                if (DestinationProperty is null)
                {
                    Expression Key = Expression.Constant(SourceProperty.Name);
                    Expressions.Add(Expression.Call(DestinationObjectInstance, SetValueMethod, Key, PropertyGet));
                }
                else
                {
                    Expression PropertySet = Expression.Property(DestinationObjectInstance, DestinationProperty);
                    Expressions.Add(Expression.Assign(PropertySet, PropertyGet));
                }
            }
            if (Expressions.Count == 0)
                return (_, y) => y;
            Expressions.Add(DestinationObjectInstance);
            var BlockExpression = Expression.Block(DestinationType, new[] { DestinationObjectAsIDictionary }, Expressions);
            var SourceLambda = Expression.Lambda<Func<TSource, TDestination, TDestination>>(BlockExpression, SourceObjectInstance, DestinationObjectInstance);
            return SourceLambda.Compile();
        }
    }
}