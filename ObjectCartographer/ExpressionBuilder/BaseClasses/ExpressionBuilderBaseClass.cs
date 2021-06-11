using ObjectCartographer.ExpressionBuilder.Interfaces;
using System;

namespace ObjectCartographer.ExpressionBuilder.BaseClasses
{
    /// <summary>
    /// Expression builder base class
    /// </summary>
    /// <seealso cref="IExpressionBuilder"/>
    public abstract class ExpressionBuilderBaseClass : IExpressionBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionBuilderBaseClass"/> class.
        /// </summary>
        protected ExpressionBuilderBaseClass()
        {
        }

        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public abstract int Order { get; }

        /// <summary>
        /// Determines whether this instance can handle the specified types.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="mapping">The mapping.</param>
        /// <returns>
        /// <c>true</c> if this instance can handle the specified types; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool CanHandle<TSource, TDestination>(TypeMapping<TSource, TDestination> mapping);

        /// <summary>
        /// Converts the specified source and destination.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="mapping">The mapping.</param>
        /// <param name="manager">The manager.</param>
        /// <returns>The resulting expression.</returns>
        public abstract Func<TSource, TDestination, TDestination> Map<TSource, TDestination>(TypeMapping<TSource, TDestination> mapping, ExpressionBuilderManager manager);
    }
}