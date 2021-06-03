using ObjectCartographer.ExpressionBuilder.Interfaces;
using ObjectCartographer.Internal;
using System;
using System.Linq.Expressions;

namespace ObjectCartographer.ExpressionBuilder.BaseClasses
{
    /// <summary>
    /// Expression builder base class
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TDestination">The type of the destination.</typeparam>
    public abstract class ExpressionBuilderBaseClass<TSource, TDestination> : IExpressionBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionBuilderBaseClass{TSource,
        /// TDestination}"/> class.
        /// </summary>
        protected ExpressionBuilderBaseClass()
        {
            SourceType = typeof(TSource);
            DestinationType = typeof(TDestination);
        }

        /// <summary>
        /// Gets the type of the destination.
        /// </summary>
        /// <value>The type of the destination.</value>
        private Type DestinationType { get; }

        /// <summary>
        /// Gets the type of the source.
        /// </summary>
        /// <value>The type of the source.</value>
        private Type SourceType { get; }

        /// <summary>
        /// Determines whether this instance can handle the specified types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>
        /// <c>true</c> if this instance can handle the specified types; otherwise, <c>false</c>.
        /// </returns>
        public bool CanHandle(TypeTuple types) => SourceType.IsAssignableFrom(types.Source) && DestinationType.IsAssignableFrom(types.Destination);

        /// <summary>
        /// Converts the specified source and destination.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns>The resulting expression.</returns>
        public abstract Expression Map(Expression source, Expression destination);
    }
}