using System;

namespace ObjectCartographer
{
    /// <summary>
    /// Object extensions
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Automatically maps the object to the destination type.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="item">The item.</param>
        public static TSource AutoMap<TSource, TDestination>(this TSource item)
        {
            DataMapper.Instance?.AutoMap<TSource, TDestination>();
            return item;
        }

        /// <summary>
        /// Automatically maps the object to the destination type.
        /// </summary>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="item">The item.</param>
        public static void AutoMap<TDestination>(this object? item)
        {
            item.AutoMap(typeof(TDestination));
        }

        /// <summary>
        /// Automatically maps the object to the destination type.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="destinationType">Type of the destination.</param>
        public static void AutoMap(this object? item, Type destinationType)
        {
            if (item is null)
                return;
            DataMapper.Instance?.AutoMap(item.GetType(), destinationType);
        }

        /// <summary>
        /// Maps the specified two types together.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="item">The item.</param>
        /// <returns>The type mapping object.</returns>
        public static TypeMapping<TSource, TDestination>? Map<TSource, TDestination>(this TSource item)
        {
            return DataMapper.Instance?.Map<TSource, TDestination>();
        }

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