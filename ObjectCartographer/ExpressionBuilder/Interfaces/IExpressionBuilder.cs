﻿using System;

namespace ObjectCartographer.ExpressionBuilder.Interfaces
{
    /// <summary>
    /// Expression builder interface
    /// </summary>
    public interface IExpressionBuilder
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        int Order { get; }

        /// <summary>
        /// Determines whether this instance can handle the specified types.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="mapping">The mapping.</param>
        /// <returns>
        /// <c>true</c> if this instance can handle the specified types; otherwise, <c>false</c>.
        /// </returns>
        bool CanHandle<TSource, TDestination>(TypeMapping<TSource, TDestination> mapping);

        /// <summary>
        /// Converts the specified source and destination.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="mapping">The mapping.</param>
        /// <param name="manager">The manager.</param>
        /// <returns>The resulting expression.</returns>
        Func<TSource, TDestination, TDestination> Map<TSource, TDestination>(TypeMapping<TSource, TDestination> mapping, ExpressionBuilderManager manager);
    }
}