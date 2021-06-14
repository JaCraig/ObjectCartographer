using System;

namespace ObjectCartographer
{
    /// <summary>
    /// Object extensions
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Attempts to convert the object to another type and returns the value
        /// </summary>
        /// <typeparam name="TReturn">Return type</typeparam>
        /// <param name="item">Object to convert</param>
        /// <param name="defaultValue">
        /// Default value to return if there is an issue or it can't be converted
        /// </param>
        /// <returns>
        /// The object converted to the other type or the default value if there is an error or
        /// can't be converted
        /// </returns>
        public static TReturn To<TReturn>(this object? item, TReturn defaultValue = default!)
        {
            if (item is TReturn ReturnValue)
                return ReturnValue;
            return (TReturn)item.To(typeof(TReturn), defaultValue)!;
        }

        /// <summary>
        /// Attempts to convert the object to another type and returns the value
        /// </summary>
        /// <param name="item">Object to convert</param>
        /// <param name="resultType">Result type</param>
        /// <param name="defaultValue">
        /// Default value to return if there is an issue or it can't be converted
        /// </param>
        /// <returns>
        /// The object converted to the other type or the default value if there is an error or
        /// can't be converted
        /// </returns>
        public static object? To(this object? item, Type resultType, object? defaultValue)
        {
            return DataMapper.Instance?.Copy(item, defaultValue, resultType) ?? defaultValue;
        }
    }
}