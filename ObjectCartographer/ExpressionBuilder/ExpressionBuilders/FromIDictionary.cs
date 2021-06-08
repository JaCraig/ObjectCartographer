using ObjectCartographer.ExpressionBuilder.BaseClasses;
using System;
using System.Collections.Generic;

namespace ObjectCartographer.ExpressionBuilder.ExpressionBuilders
{
    /// <summary>
    /// From IDictionary expression builder
    /// </summary>
    /// <seealso cref="ExpressionBuilderBaseClass"/>
    public class FromIDictionary : ExpressionBuilderBaseClass
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
            return DictionaryType.IsAssignableFrom(mapping.TypeInfo.Source);
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
            return (_, y) => y;
        }
    }
}