using Microsoft.Extensions.Logging;
using ObjectCartographer.ExpressionBuilder;
using ObjectCartographer.Interfaces;
using ObjectCartographer.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        /// <param name="expressionBuilder">The expression builder.</param>
        /// <param name="logger">The logger.</param>
        public DataMapper(ExpressionBuilderManager expressionBuilder, ILogger<DataMapper>? logger = null)
        {
            Logger = logger;
            ExpressionBuilder = expressionBuilder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataMapper"/> class.
        /// </summary>
        public DataMapper()
        {
        }

        /// <summary>
        /// Gets the copy methods.
        /// </summary>
        /// <value>The copy methods.</value>
        private Dictionary<TypeTuple, MethodWrapperDelegate> CopyMethods { get; } = new Dictionary<TypeTuple, MethodWrapperDelegate>();

        /// <summary>
        /// Gets the expression builder.
        /// </summary>
        /// <value>The expression builder.</value>
        private ExpressionBuilderManager? ExpressionBuilder { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        private ILogger<DataMapper>? Logger { get; }

        /// <summary>
        /// Gets the map methods.
        /// </summary>
        /// <value>The map methods.</value>
        private Dictionary<TypeTuple, MethodWrapperDelegate> MapMethods { get; } = new Dictionary<TypeTuple, MethodWrapperDelegate>();

        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <value>The types.</value>
        private Dictionary<TypeTuple, ITypeMapping> Types { get; } = new Dictionary<TypeTuple, ITypeMapping>();

        /// <summary>
        /// The copy create lock object
        /// </summary>
        private static readonly object CopyCreateLockObject = new object();

        /// <summary>
        /// The copy generic
        /// </summary>
        private static readonly MethodInfo CopyGeneric = Array.Find(typeof(DataMapper).GetMethods(), x => string.Equals(x.Name, "copy", StringComparison.OrdinalIgnoreCase) && x.GetGenericArguments().Length == 2);

        /// <summary>
        /// The map create lock object
        /// </summary>
        private static readonly object MapCreateLockObject = new object();

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
            if (first is null || second is null)
                return this;
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
        /// Copies the specified source to the destination object (or a new TDestination object if
        /// one is not passed in).
        /// </summary>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns>The resulting object</returns>
        public TDestination Copy<TDestination>(object? source, TDestination destination = default!)
        {
            return (TDestination)Copy(source, destination, typeof(TDestination))!;
        }

        /// <summary>
        /// Copies the specified source to the destination
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="destinationType">
        /// Type of the destination (if null, system uses the type of the destination object).
        /// </param>
        /// <returns>The resulting object.</returns>
        public object? Copy(object? source, object? destination, Type? destinationType = null)
        {
            if (source is null)
                return destination;
            var Source = source.GetType();
            var Destination = destinationType ?? destination?.GetType();
            if (Destination is null)
                return destination;
            var Key = new TypeTuple(Source, Destination);
            if (CopyMethods.TryGetValue(Key, out var Method))
                return Method(new object?[] { source, destination });
            lock (CopyCreateLockObject)
            {
                if (CopyMethods.TryGetValue(Key, out Method))
                    return Method(new object?[] { source, destination });
                var GenericMethod = CopyGeneric.MakeGenericMethod(Source, Destination);
                var FinalMethod = CreateMethod(GenericMethod, GenericMethod.GetParameters());
                CopyMethods.Add(Key, FinalMethod);
                return FinalMethod(new object?[] { source, destination });
            }
        }

        /// <summary>
        /// Copies the specified source to the destination object (or a new TDestination object if
        /// one is not passed in).
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns>The resulting object</returns>
        public TDestination Copy<TSource, TDestination>(TSource source, TDestination destination = default!)
        {
            if (source is null)
                return destination;
            var Source = source.GetType();
            var Destination = typeof(TDestination);
            var Key = new TypeTuple(Source, Destination);
            if (!Types.TryGetValue(Key, out var ReturnValue))
            {
                AutoMap<TSource, TDestination>();
                if (!Types.TryGetValue(Key, out ReturnValue))
                    return destination;
            }
            var Mapping = ReturnValue as TypeMapping<TSource, TDestination>;
            Mapping.Build();
            return Mapping.Converter(source, destination);
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
            Logger?.LogInformation($"Mapping {Source} => {Destination}");
            var NewMapping = new TypeMapping<TSource, TDestination>(Key, Logger, ExpressionBuilder);
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
            if (source is null || destination is null)
                return null;
            var Key = new TypeTuple(source, destination);
            if (MapMethods.TryGetValue(Key, out var Method))
                return Method(Array.Empty<object>()) as ITypeMapping;
            lock (MapCreateLockObject)
            {
                if (MapMethods.TryGetValue(Key, out Method))
                    return Method(Array.Empty<object>()) as ITypeMapping;
                var GenericMethod = MapGeneric.MakeGenericMethod(source, destination);
                var FinalMethod = CreateMethod(GenericMethod, GenericMethod.GetParameters());
                MapMethods.Add(Key, FinalMethod);
                return FinalMethod(Array.Empty<object>()) as ITypeMapping;
            }
        }

        /// <summary>
        /// Creates the argument expression.
        /// </summary>
        /// <param name="parameterExpression">The parameter expression.</param>
        /// <param name="parameterInfo">The parameter information.</param>
        /// <param name="index">The index.</param>
        /// <returns>The argument expression</returns>
        private static Expression CreateArgumentExpression(ParameterExpression parameterExpression, ParameterInfo parameterInfo, int index)
        {
            return Expression.Convert(
                Expression.ArrayIndex(parameterExpression, Expression.Constant(index)),
                parameterInfo.ParameterType);
        }

        /// <summary>
        /// Creates the method wrapper.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The resulting method.</returns>
        private MethodWrapperDelegate CreateMethod(MethodInfo method, ParameterInfo[] parameters)
        {
            var ParameterExpression = Expression.Parameter(typeof(object[]), "args");
            var ThisValue = Expression.Constant(this);

            var ArgsExpressions = parameters
                .Select((info, index) => CreateArgumentExpression(ParameterExpression, info, index))
                .ToArray();

            Expression CallExpression = Expression.Call(ThisValue, method, ArgsExpressions);

            return (Expression.Lambda(
                typeof(MethodWrapperDelegate),
                CallExpression,
                ParameterExpression).Compile() as MethodWrapperDelegate)!;
        }
    }
}