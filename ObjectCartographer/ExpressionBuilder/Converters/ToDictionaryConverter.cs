using ObjectCartographer.ExpressionBuilder.BaseClasses;
using ObjectCartographer.ExpressionBuilder.Interfaces;
using ObjectCartographer.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ObjectCartographer.ExpressionBuilder.Converters
{
    /// <summary>
    /// To dictionary converter
    /// </summary>
    /// <seealso cref="IConverter"/>
    public class ToDictionaryConverter : ConverterBaseClass
    {
        /// <summary>
        /// The dictionary type
        /// </summary>
        private static readonly Type DictionaryType = typeof(IDictionary<,>);

        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public override int Order => 0;

        /// <summary>
        /// Determines whether this instance can handle the specified types.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns>
        /// <c>true</c> if this instance can handle the specified types; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanHandle(Type source, Type destination)
        {
            return IsDictionary(destination);
        }

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
        public override Expression Map(Expression source, Expression? destination, Type sourceType, Type destinationType, IExpressionMapping mapping, ExpressionBuilderManager manager)
        {
            if (destination is null)
                return source;
            var AddMethod = destinationType.GetMethod("Add");

            var IDictionaryType = Array.Find(destinationType.GetInterfaces(), x => string.Equals(x.Name, "IDictionary`2", StringComparison.OrdinalIgnoreCase));
            var IDictionaryArgs = IDictionaryType.GetGenericArguments();

            var IDictionaryKeyType = IDictionaryArgs[0];
            var IDictionaryValueType = IDictionaryArgs[1];

            var DestinationObjectAsIDictionary = mapping.AddVariable(IDictionaryType);

            List<Expression> Expressions = new List<Expression>()
            {
                CreateObject(destination, source, sourceType.ReadableProperties(), destinationType.PublicConstructors(), mapping, manager),
                Expression.Assign(DestinationObjectAsIDictionary, Expression.Convert(destination, IDictionaryType))
            };

            var SourceProperties = sourceType.ReadableProperties();

            for (var x = 0; x < sourceType.ReadableProperties().Length; ++x)
            {
                //var SourceProperty = TypeCache<TSource>.ReadableProperties[x];
                //var DestinationProperty = TypeCache<TDestination>.WritableProperties.FindMatchingProperty(SourceProperty.Name);

                //var PropertyGet = manager.Convert(Expression.Property(SourceObjectInstance, SourceProperty), SourceProperty.PropertyType, IDictionaryValueType);

                //if (DestinationProperty is null)
                //{
                //    Expression Key = Expression.Constant(SourceProperty.Name);
                //    Expressions.Add(Expression.Call(DestinationObjectInstance, SetValueMethod, Key, PropertyGet));
                //}
                //else
                //{
                //    Expression PropertySet = Expression.Property(DestinationObjectInstance, DestinationProperty);
                //    Expressions.Add(Expression.Assign(PropertySet, PropertyGet));
                //}
            }
            Expressions.Add(destination);
            return Expression.Block(destinationType, Expressions);
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