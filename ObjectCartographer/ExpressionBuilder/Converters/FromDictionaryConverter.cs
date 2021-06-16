using ObjectCartographer.ExpressionBuilder.BaseClasses;
using ObjectCartographer.ExpressionBuilder.Interfaces;
using ObjectCartographer.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectCartographer.ExpressionBuilder.Converters
{
    /// <summary>
    /// From dictionary converter
    /// </summary>
    /// <seealso cref="ConverterBaseClass"/>
    public class FromDictionaryConverter : ConverterBaseClass
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public override int Order => 2;

        /// <summary>
        /// The dictionary type
        /// </summary>
        private static Type DictionaryType { get; } = typeof(IDictionary<string, object>);

        /// <summary>
        /// Gets the add method.
        /// </summary>
        /// <value>The add method.</value>
        private static MethodInfo TryGetValueMethod { get; } = typeof(FromDictionaryConverter).GetMethod(nameof(FromDictionaryConverter.TryGetValue));

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
            return IsDictionary(source);
        }

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool TryGetValue(IDictionary<string, object> dictionary, string key, out object? value)
        {
            if (dictionary is null || key is null)
            {
                value = null;
                return false;
            }
            var FinalKey = dictionary.Keys.FirstOrDefault(z => string.Equals(z.Replace("_", "", StringComparison.Ordinal), key, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(FinalKey))
            {
                value = dictionary[FinalKey];
                return true;
            }
            if (dictionary.TryGetValue(key, out value))
                return true;
            value = null;
            return false;
        }

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

            var SourceObjectAsIDictionary = mapping.AddVariable(DictionaryType);
            var TempHolder = mapping.AddVariable(typeof(object));

            expressions.Add(Expression.Assign(SourceObjectAsIDictionary, Expression.Convert(source, DictionaryType)));

            var SourceProperties = sourceType.ReadableProperties();
            var DestinationProperties = destinationType.WritableProperties();

            for (var x = 0; x < DestinationProperties.Length; ++x)
            {
                var DestinationProperty = DestinationProperties[x];
                var SourceProperty = SourceProperties.FindMatchingProperty(DestinationProperty.Name);

                if (SourceProperty is null)
                {
                    var PropertySet = Expression.Property(destination, DestinationProperty);
                    Expression Key = Expression.Constant(DestinationProperty.Name);
                    Expression MethodCall = Expression.Call(Expression.Constant(this), TryGetValueMethod, SourceObjectAsIDictionary, Key, TempHolder);
                    Expression Assignment = Expression.Assign(PropertySet, manager.Map(TempHolder, PropertySet, typeof(object), DestinationProperty.PropertyType, mapping));
                    Expression IfStatement = Expression.IfThen(MethodCall, Assignment);
                    expressions.Add(IfStatement);
                }
                else
                {
                    Expression PropertyGet = Expression.Property(source, SourceProperty);
                    var PropertySet = Expression.Property(destination, DestinationProperty);

                    PropertyGet = manager.Map(PropertyGet, PropertySet, SourceProperty.PropertyType, DestinationProperty.PropertyType, mapping);

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
        private static bool IsDictionary(Type type)
        {
            return DictionaryType.IsAssignableFrom(type);
        }
    }
}