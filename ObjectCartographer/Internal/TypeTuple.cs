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
            _ToString = $"({Source.Name}, {Destination.Name})";
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
        /// To string value
        /// </summary>
        private readonly string _ToString;

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
        /// Determines whether the specified <see cref="System.Object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is TypeTuple TypeMappingObject && Equals(TypeMappingObject);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table.
        /// </returns>
        public override int GetHashCode() => _HashCode;

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString() => _ToString;
    }
}