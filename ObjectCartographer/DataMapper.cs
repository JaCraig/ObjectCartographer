using Microsoft.Extensions.Logging;
using ObjectCartographer.Internal;
using System;
using System.Collections.Generic;

namespace ObjectCartographer
{
    /// <summary>
    /// Data mapper
    /// </summary>
    public class DataMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataMapper"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public DataMapper(ILogger<DataMapper>? logger = null)
        {
            Logger = logger;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataMapper"/> class.
        /// </summary>
        public DataMapper()
        {
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        private ILogger<DataMapper>? Logger { get; }

        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <value>The types.</value>
        private Dictionary<TypeTuple, TypeMapping> Types { get; } = new Dictionary<TypeTuple, TypeMapping>();

        /// <summary>
        /// Automatically maps the two types.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>This.</returns>
        public DataMapper AutoMap(Type first, Type second)
        {
            Map(first, second);
            Map(second, first);
            return this;
        }

        /// <summary>
        /// Automatically maps the two types.
        /// </summary>
        /// <typeparam name="TFirst">The type of the first.</typeparam>
        /// <typeparam name="TSecond">The type of the second.</typeparam>
        /// <returns>This.</returns>
        public DataMapper AutoMap<TFirst, TSecond>()
        {
            return AutoMap(typeof(TFirst), typeof(TSecond));
        }

        /// <summary>
        /// Maps the source type to the destination type
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <returns>The type mapping.</returns>
        public TypeMapping Map<TSource, TDestination>()
        {
            return Map(typeof(TSource), typeof(TDestination));
        }

        /// <summary>
        /// Maps the specified source to the destination.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns>The type mapping.</returns>
        public TypeMapping Map(Type source, Type destination)
        {
            var Key = new TypeTuple(source, destination);
            if (Types.TryGetValue(Key, out var ReturnValue))
                return ReturnValue;
            Logger?.LogInformation($"Mapping {source.Name} to {destination.Name}");
            var NewMapping = new TypeMapping(source, destination);
            Types.Add(Key, NewMapping);
            return NewMapping;
        }
    }
}