using System.Linq.Expressions;

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
        Expression Destination { get; }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>The source.</value>
        Expression Source { get; }
    }
}