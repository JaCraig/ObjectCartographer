using System;

namespace ObjectCartographer.Internal
{
    /// <summary>
    /// Type tuple
    /// </summary>
    /// <seealso cref="IEquatable{TypeTuple}"/>
    public readonly struct TypeTuple : IEquatable<TypeTuple>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeTuple"/> struct.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public TypeTuple(Type source, Type destination)
        {
            Source = source;
            Destination = destination;
            _HashCode = HashCode.Combine(Source, Destination);
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

        /// <summary>
        /// The hash code
        /// </summary>
        private readonly int _HashCode;

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">Left parameter</param>
        /// <param name="right">Right parameter</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(TypeTuple left, TypeTuple right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">Left parameter</param>
        /// <param name="right">Right parameter</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(TypeTuple left, TypeTuple right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true"/> if the current object is equal to the <paramref name="other"/>
        /// parameter; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(TypeTuple other) => other.Source == Source && other.Destination == Destination;

        /// <summary>
        /// Determines whether the specified <see cref="object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj) => obj is TypeTuple TypeMappingObject && Equals(TypeMappingObject);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table.
        /// </returns>
        public override int GetHashCode() => _HashCode;
    }
}