using System;

namespace ObjectCartographer
{
    /// <summary>
    /// Type mapping
    /// </summary>
    public class TypeMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeMapping"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public TypeMapping(Type source, Type destination)
        {
            Source = source;
            Destination = destination;
        }

        /// <summary>
        /// Gets the destination.
        /// </summary>
        /// <value>The destination.</value>
        public Type Destination { get; }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>The source.</value>
        public Type Source { get; }
    }
}