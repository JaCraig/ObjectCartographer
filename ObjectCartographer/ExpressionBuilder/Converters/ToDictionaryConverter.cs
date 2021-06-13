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
        public override int Order => 2;

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
            var CopyConstructor = GetCopyConstructor(sourceType, destinationType);
            if (CopyConstructor is null)
            {
                return CopyToDictionary(source, destination, sourceType, destinationType, mapping, manager);
            }
            else
            {
                return Expression.Block(destinationType,
                        Expression.IfThenElse(Expression.Equal(destination, Expression.Constant(null)),
                                Expression.Assign(destination, Expression.New(CopyConstructor, source)),
                                CopyToDictionary(source, destination, sourceType, destinationType, mapping, manager)),
                        destination);
            }
        }

        /// <summary>
        /// Determines whether the specified type is dictionary.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is dictionary; otherwise, <c>false</c>.</returns>
        private static bool IsDictionary(Type type)
        {
            return DictionaryType.IsAssignableFrom(type);
            //type.GetInterfaces();
            //return Interfaces.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == DictionaryType);
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
        /// <returns></returns>
        private Expression CopyToDictionary(Expression source, Expression? destination, Type sourceType, Type destinationType, IExpressionMapping mapping, ExpressionBuilderManager manager)
        {
            if (destination is null)
                return source;

            var DestinationObjectAsIDictionary = mapping.AddVariable(DictionaryType);
            var TempHolder = mapping.AddVariable(typeof(object));

            List<Expression> Expressions = new List<Expression>()
            {
                CreateObject(destination, source, sourceType.ReadableProperties(), destinationType.PublicConstructors(), mapping, manager),
                Expression.Assign(DestinationObjectAsIDictionary, Expression.Convert(destination, DictionaryType))
            };

            var SourceProperties = sourceType.ReadableProperties();
            var DestinationProperties = destinationType.WritableProperties();

            for (var x = 0; x < SourceProperties.Length; ++x)
            {
                var SourceProperty = SourceProperties[x];
                var DestinationProperty = DestinationProperties.FindMatchingProperty(SourceProperty.Name);

                var PropertyGet = manager.Map(Expression.Property(source, SourceProperty), TempHolder, SourceProperty.PropertyType, typeof(object), mapping);

                if (DestinationProperty is null)
                {
                    Expression Key = Expression.Constant(SourceProperty.Name);
                    Expressions.Add(Expression.Call(DestinationObjectAsIDictionary, AddMethod, Key, PropertyGet));
                }
                else
                {
                    Expression PropertySet = Expression.Property(DestinationObjectAsIDictionary, DestinationProperty);
                    Expressions.Add(Expression.Assign(PropertySet, PropertyGet));
                }
            }
            Expressions.Add(destination);
            return Expression.Block(destinationType, Expressions);
        }
    }
}