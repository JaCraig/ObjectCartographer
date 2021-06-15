using System.Reflection;

namespace ObjectCartographer.Interfaces
{
    /// <summary>
    /// Property mapping interface
    /// </summary>
    public interface IPropertyMapping
    {
        /// <summary>
        /// Gets the destination.
        /// </summary>
        /// <value>The destination.</value>
        MethodInfo Destination { get; }

        /// <summary>
        /// Gets the destination target.
        /// </summary>
        /// <value>The destination target.</value>
        object? DestinationTarget { get; }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>The source.</value>
        MethodInfo Source { get; }

        /// <summary>
        /// Gets the source target.
        /// </summary>
        /// <value>The source target.</value>
        object? SourceTarget { get; }
    }
}