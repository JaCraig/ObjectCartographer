using Fast.Activator;
using ObjectCartographer.ExpressionBuilder.BaseClasses;
using System;
using System.Collections.Generic;

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
        /// The dictionary type
        /// </summary>
        private static readonly Type DictionaryType = typeof(IDictionary<string, object>);

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
            return DictionaryType.IsAssignableFrom(mapping.TypeInfo.Source) && DictionaryType.IsAssignableFrom(mapping.TypeInfo.Destination);
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
            return DefaultMethod;
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
    }
}