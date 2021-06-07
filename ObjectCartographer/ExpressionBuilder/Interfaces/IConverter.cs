namespace ObjectCartographer.ExpressionBuilder.Interfaces
{
    /// <summary>
    /// Converter interface
    /// </summary>
    public interface IConverter
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
    }
}