using System;

namespace ObjectCartographer.Converter.Interfaces
{
    /// <summary>
    /// Converter interface
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified source to the destination type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified source; otherwise, <c>false</c>.
        /// </returns>
        bool CanConvert(Type source, Type destination);

        /// <summary>
        /// Converts the specified source to the destination type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns>The converted data type.</returns>
        object? Convert(object? source, Type destination);
    }
}