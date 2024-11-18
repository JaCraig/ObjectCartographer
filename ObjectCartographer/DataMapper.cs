using Fast.Activator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ObjectCartographer.ExpressionBuilder;
using ObjectCartographer.ExtensionMethods;
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
            Instance = this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataMapper"/> class.
        /// </summary>
        public DataMapper()
        {
            Instance = this;
        }

        /// <summary>
        /// The copy create lock object
        /// </summary>
        private static readonly object _CopyCreateLockObject = new();

        /// <summary>
        /// The copy generic
        /// </summary>
        private static readonly MethodInfo _CopyGeneric = Array.Find(typeof(DataMapper).GetMethods(), x => string.Equals(x.Name, nameof(DataMapper.Copy), StringComparison.OrdinalIgnoreCase) && x.GetGenericArguments().Length == 2);

        /// <summary>
        /// The instance lock object
        /// </summary>
        private static readonly object _InstanceLockObject = new();

        /// <summary>
        /// The internal lock
        /// </summary>
        private static readonly object _InternalLock = new();

        /// <summary>
        /// The map create lock object
        /// </summary>
        private static readonly object _MapCreateLockObject = new();

        /// <summary>
        /// The generic map method.
        /// </summary>
        private static readonly MethodInfo _MapGeneric = typeof(DataMapper).GetMethod(nameof(DataMapper.Map), []);

        /// <summary>
        /// The instance
        /// </summary>
        private static DataMapper? _Instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        internal static DataMapper? Instance
        {
            get
            {
                if (_Instance is not null)
                    return _Instance;
                lock (_InstanceLockObject)
                {
                    if (_Instance is not null)
                        return _Instance;
                    _Instance = Services.ServiceProvider?.GetService<DataMapper>();
                }
                return _Instance;
            }
            set => _Instance = value;
        }

        /// <summary>
        /// Gets the create instance.
        /// </summary>
        /// <value>The create instance.</value>
        private static MethodInfo CreateInstance { get; } = typeof(FastActivator).GetMethod(nameof(FastActivator.CreateInstance), 1, []);

        /// <summary>
        /// Gets the copy methods.
        /// </summary>
        /// <value>The copy methods.</value>
        private Dictionary<TypeTuple, MethodWrapperDelegate> CopyMethods { get; } = [];

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
        private Dictionary<TypeTuple, MethodWrapperDelegate> MapMethods { get; } = [];

        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <value>The types.</value>
        private Dictionary<TypeTuple, ITypeMapping> Types { get; } = [];

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
        public DataMapper AutoMap<TFirst, TSecond>() => AutoMap(typeof(TFirst), typeof(TSecond));

        /// <summary>
        /// Copies the specified source to the destination object (or a new TDestination object if
        /// one is not passed in).
        /// </summary>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns>The resulting object</returns>
        public TDestination Copy<TDestination>(object? source, TDestination destination = default!) => (TDestination)Copy(source, destination, typeof(TDestination))!;

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
            Type Source = source.GetType();
            Type? Destination = destinationType ?? destination?.GetType();
            if (Destination is null)
                return destination;
            var Key = new TypeTuple(Source, Destination);
            if (CopyMethods.TryGetValue(Key, out MethodWrapperDelegate? Method))
                return Method([source, destination]);
            lock (_CopyCreateLockObject)
            {
                if (CopyMethods.TryGetValue(Key, out Method))
                    return Method([source, destination]);
                MethodInfo GenericMethod = _CopyGeneric.MakeGenericMethod(Source, Destination);
                MethodWrapperDelegate FinalMethod = CreateMethod(GenericMethod, GenericMethod.GetParameters());
                _ = CopyMethods.TryAdd(Key, FinalMethod);
                return FinalMethod([source, destination]);
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
            Type Source = source.GetType();
            Type Destination = typeof(TDestination);
            var Key = new TypeTuple(Source, Destination);
            if (!Types.TryGetValue(Key, out ITypeMapping? ReturnValue))
            {
                _ = AutoMap<TSource, TDestination>();
                if (!Types.TryGetValue(Key, out ReturnValue))
                    return destination;
            }
            var Mapping = ReturnValue as TypeMapping<TSource, TDestination>;
            Mapping?.Build();
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
            Type Source = typeof(TSource);
            Type Destination = typeof(TDestination);
            var Key = new TypeTuple(Source, Destination);
            if (Types.TryGetValue(Key, out ITypeMapping? ReturnValue))
                return ReturnValue as TypeMapping<TSource, TDestination>;
            lock (_InternalLock)
            {
                if (Types.TryGetValue(Key, out ReturnValue))
                    return ReturnValue as TypeMapping<TSource, TDestination>;
                Logger?.LogDebug("Mapping {Source} => {Destination}", Source, Destination);
                var NewMapping = new TypeMapping<TSource, TDestination>(Key, Logger, ExpressionBuilder);
                _ = Types.TryAdd(Key, NewMapping);
                return NewMapping;
            }
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
            if (MapMethods.TryGetValue(Key, out MethodWrapperDelegate? Method))
                return Method([]) as ITypeMapping;
            lock (_MapCreateLockObject)
            {
                if (MapMethods.TryGetValue(Key, out Method))
                    return Method([]) as ITypeMapping;
                MethodInfo GenericMethod = _MapGeneric.MakeGenericMethod(source, destination);
                MethodWrapperDelegate FinalMethod = CreateMethod(GenericMethod, GenericMethod.GetParameters());
                _ = MapMethods.TryAdd(Key, FinalMethod);
                return FinalMethod([]) as ITypeMapping;
            }
        }

        /// <summary>
        /// Creates the argument expression.
        /// </summary>
        /// <param name="parameterExpression">The parameter expression.</param>
        /// <param name="parameterInfo">The parameter information.</param>
        /// <param name="index">The index.</param>
        /// <returns>The argument expression</returns>
        private static UnaryExpression CreateArgumentExpression(ParameterExpression parameterExpression, ParameterInfo parameterInfo, int index)
        {
            BinaryExpression IndexValue = Expression.ArrayIndex(parameterExpression, Expression.Constant(index));
            if (DefaultValueLookup.Values.TryGetValue(parameterInfo.ParameterType.GetHashCode(), out var DefaultValue))
                IndexValue = Expression.Coalesce(IndexValue, Expression.Constant(DefaultValue));
            else if (!parameterInfo.ParameterType.IsClass)
                IndexValue = Expression.Coalesce(IndexValue, Expression.Call(CreateInstance.MakeGenericMethod(parameterInfo.ParameterType)));
            return Expression.Convert(
                IndexValue,
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
            ParameterExpression ParameterExpression = Expression.Parameter(typeof(object[]), "args");
            ConstantExpression ThisValue = Expression.Constant(this);

            Expression[] ArgsExpressions = parameters
                .Select((info, index) => CreateArgumentExpression(ParameterExpression, info, index))
                .ToArray();

            Expression CallExpression = Expression.Convert(Expression.Call(ThisValue, method, ArgsExpressions), typeof(object));

            return (Expression.Lambda(
                typeof(MethodWrapperDelegate),
                CallExpression,
                ParameterExpression).Compile() as MethodWrapperDelegate)!;
        }
    }
}