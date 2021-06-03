using Microsoft.Extensions.Logging;
using ObjectCartographer.Interfaces;
using ObjectCartographer.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;

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
        private Dictionary<TypeTuple, ITypeMapping> Types { get; } = new Dictionary<TypeTuple, ITypeMapping>();

        /// <summary>
        /// The generic map method.
        /// </summary>
        private static readonly MethodInfo MapGeneric = typeof(DataMapper).GetMethod("Map", Array.Empty<Type>());

        /// <summary>
        /// Automatically maps the two types.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>This.</returns>
        public DataMapper AutoMap(Type first, Type second)
        {
            Map(first, second)?.AutoMap().Build();
            Map(second, first)?.AutoMap().Build();
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
        public TypeMapping<TSource, TDestination>? Map<TSource, TDestination>()
        {
            var Source = typeof(TSource);
            var Destination = typeof(TDestination);
            var Key = new TypeTuple(Source, Destination);
            if (Types.TryGetValue(Key, out var ReturnValue))
                return ReturnValue as TypeMapping<TSource, TDestination>;
            Logger?.LogInformation($"Mapping {Source.Name} to {Destination.Name}");
            var NewMapping = new TypeMapping<TSource, TDestination>(Key);
            Types.Add(Key, NewMapping);
            return NewMapping;
        }

        /// <summary>
        /// Maps the specified source to the destination.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns>The type mapping.</returns>
        public ITypeMapping? Map(Type source, Type destination)
        {
            return MapGeneric.MakeGenericMethod(source, destination).Invoke(this, Array.Empty<object>()) as ITypeMapping;
        }
    }
}