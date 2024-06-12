using ObjectCartographer.ExpressionBuilder.BaseClasses;
using ObjectCartographer.ExpressionBuilder.Interfaces;
using ObjectCartographer.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectCartographer.ExpressionBuilder.Converters
{
    /// <summary>
    /// To dictionary converter
    /// </summary>
    /// <seealso cref="IConverter"/>
    public class ToDictionaryConverter : ConverterBaseClass
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public override int Order => OrderDefaults.DefaultPlusTwo;

        /// <summary>
        /// Gets the add method.
        /// </summary>
        /// <value>The add method.</value>
        private static MethodInfo AddMethod { get; } = typeof(IDictionary<string, object>).GetMethod(nameof(IDictionary<string, object>.Add));

        /// <summary>
        /// The dictionary type
        /// </summary>
        private static Type DictionaryType { get; } = typeof(IDictionary<string, object>);

        /// <summary>
        /// Determines whether this instance can handle the specified types.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns>
        /// <c>true</c> if this instance can handle the specified types; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanHandle(Type source, Type destination) => IsDictionary(destination);

        /// <summary>
        /// Copies to dictionary.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="expressions">The expressions.</param>
        /// <returns>The resulting expression.</returns>
        protected override Expression CopyObject(Expression source, Expression? destination, Type sourceType, Type destinationType, IExpressionMapping mapping, ExpressionBuilderManager manager, List<Expression> expressions)
        {
            if (destination is null)
                return source;

            ParameterExpression? DestinationObjectAsIDictionary = mapping.AddVariable(DictionaryType);
            ParameterExpression? TempHolder = mapping.AddVariable(typeof(object));

            expressions.Add(Expression.Assign(DestinationObjectAsIDictionary!, Expression.Convert(destination, DictionaryType)));

            PropertyInfo[] SourceProperties = sourceType.ReadableProperties();
            PropertyInfo[] DestinationProperties = destinationType.WritableProperties();

            for (var x = 0; x < SourceProperties.Length; ++x)
            {
                PropertyInfo SourceProperty = SourceProperties[x];
                PropertyInfo? DestinationProperty = DestinationProperties.FindMatchingProperty(SourceProperty.Name);

                if (DestinationProperty is null)
                {
                    Expression PropertyGet = manager.Map(Expression.Property(source, SourceProperty), TempHolder, SourceProperty.PropertyType, typeof(object), mapping);
                    Expression Key = Expression.Constant(SourceProperty.Name);
                    expressions.Add(Expression.Call(DestinationObjectAsIDictionary, AddMethod, Key, PropertyGet));
                }
                else
                {
                    Expression PropertyGet = manager.Map(Expression.Property(source, SourceProperty), TempHolder, SourceProperty.PropertyType, DestinationProperty.PropertyType, mapping);
                    Expression PropertySet = Expression.Property(destination, DestinationProperty);
                    expressions.Add(Expression.Assign(PropertySet, PropertyGet));
                }
            }
            expressions.Add(destination);
            return Expression.Block(destinationType, expressions);
        }

        /// <summary>
        /// Determines whether the specified type is dictionary.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is dictionary; otherwise, <c>false</c>.</returns>
        private static bool IsDictionary(Type type) => DictionaryType.IsAssignableFrom(type);//type.GetInterfaces();//return Interfaces.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == DictionaryType);
    }
}