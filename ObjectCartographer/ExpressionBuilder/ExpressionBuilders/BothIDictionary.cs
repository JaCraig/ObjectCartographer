namespace ObjectCartographer.ExpressionBuilder.ExpressionBuilders
{
    /// <summary>
    /// Both items are IDictionaries
    /// </summary>
    /// <seealso cref="ExpressionBuilderBaseClass"/>
    /*public class BothIDictionary : ExpressionBuilderBaseClass
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public override int Order => -1;

        /// <summary>
        /// Gets the default method information.
        /// </summary>
        /// <value>The default method information.</value>
        private static MethodInfo DefaultMethodInfo { get; } = typeof(BothIDictionary).GetMethod("DefaultMethod", BindingFlags.NonPublic | BindingFlags.Static);

        /// <summary>
        /// The dictionary type
        /// </summary>
        private static readonly Type DictionaryType = typeof(IDictionary<,>);

        /// <summary>
        /// The set value method
        /// </summary>
        private static readonly MethodInfo SetValueMethod = typeof(IDictionary<,>).GetMethod("Add");

        /// <summary>
        /// The try get value method
        /// </summary>
        private static readonly MethodInfo TryGetValueMethod = typeof(IDictionary<,>).GetMethod("TryGetValue");

        /// <summary>
        /// Determines whether this instance can handle the specified types.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="mapping">The mapping.</param>
        /// <returns>
        /// <c>true</c> if this instance can handle the specified types; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanHandle<TSource, TDestination>(TypeMapping<TSource, TDestination> mapping)
        {
            return IsDictionary(mapping.TypeInfo.Source) && IsDictionary(mapping.TypeInfo.Destination);
        }

        /// <summary>
        /// Converts the specified source and destination.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="mapping">The mapping.</param>
        /// <param name="manager">The manager.</param>
        /// <returns>The resulting expression.</returns>
        public override Func<TSource, TDestination, TDestination> Map<TSource, TDestination>(TypeMapping<TSource, TDestination> mapping, ExpressionBuilderManager manager)
        {
            var SourceType = typeof(TSource);
            var ISourceDictionaryType = Array.Find(SourceType.GetInterfaces(), x => string.Equals(x.Name, "IDictionary`2", StringComparison.OrdinalIgnoreCase));
            var ISourceDictionaryKeyType = ISourceDictionaryType.GetGenericArguments()[0];
            var ISourceDictionaryValueType = ISourceDictionaryType.GetGenericArguments()[1];

            var DestinationType = typeof(TDestination);
            var IDestinationDictionaryType = Array.Find(DestinationType.GetInterfaces(), x => string.Equals(x.Name, "IDictionary`2", StringComparison.OrdinalIgnoreCase));
            var IDestinationDictionaryKeyType = IDestinationDictionaryType.GetGenericArguments()[0];
            var IDestinationDictionaryValueType = IDestinationDictionaryType.GetGenericArguments()[1];

            List<Expression> Expressions = new List<Expression>();

            var SourceObjectInstance = Expression.Parameter(SourceType, "source");
            var SourceObjectAsIDictionary = Expression.Variable(ISourceDictionaryType);

            var DestinationObjectInstance = Expression.Parameter(DestinationType, "destination");
            var DestinationObjectAsIDictionary = Expression.Variable(IDestinationDictionaryType);

            var SourceValue = Expression.Variable(ISourceDictionaryValueType);
            var DestinationValue = Expression.Variable(IDestinationDictionaryValueType);

            Expressions.Add(manager.Create(DestinationObjectInstance, SourceObjectInstance, TypeCache<TSource>.ReadableProperties, TypeCache<TDestination>.Constructors));
            Expressions.Add(Expression.Assign(SourceObjectAsIDictionary, Expression.Convert(SourceObjectInstance, ISourceDictionaryType)));
            Expressions.Add(Expression.Assign(DestinationObjectAsIDictionary, Expression.Convert(DestinationObjectInstance, IDestinationDictionaryType)));

            for (var x = 0; x < TypeCache<TDestination>.WritableProperties.Length; ++x)
            {
                var DestinationProperty = TypeCache<TDestination>.WritableProperties[x];
                var SourceProperty = TypeCache<TSource>.ReadableProperties.FindMatchingProperty(DestinationProperty.Name);

                if (SourceProperty is null)
                {
                    Expression Key = manager.Convert(Expression.Constant(DestinationProperty.Name), typeof(string), ISourceDictionaryKeyType);
                    Expression MethodCall = Expression.Call(SourceObjectInstance, TryGetValueMethod, Key, SourceValue);
                    Expression PropertySet = Expression.Property(DestinationObjectInstance, DestinationProperty);
                    Expression Assignment = Expression.Assign(PropertySet, manager.Convert(SourceValue, ISourceDictionaryValueType, DestinationProperty.PropertyType));
                    Expression IfStatement = Expression.IfThen(MethodCall, Assignment);
                    Expressions.Add(IfStatement);
                }
                else
                {
                    Expression PropertyGet = manager.Convert(Expression.Property(SourceObjectInstance, SourceProperty), SourceProperty.PropertyType, DestinationProperty.PropertyType);
                    Expression PropertySet = Expression.Property(DestinationObjectInstance, DestinationProperty);
                    Expressions.Add(Expression.Assign(PropertySet, PropertyGet));
                }
            }
            Expressions.Add(Expression.Call(DefaultMethodInfo.MakeGenericMethod(ISourceDictionaryKeyType, ISourceDictionaryValueType, IDestinationDictionaryKeyType, IDestinationDictionaryValueType), SourceValue, DestinationValue));
            if (Expressions.Count == 0)
                return (_, y) => y;
            Expressions.Add(DestinationObjectInstance);
            var BlockExpression = Expression.Block(DestinationType, new[] { SourceObjectAsIDictionary, DestinationObjectAsIDictionary, SourceValue, DestinationValue }, Expressions);
            var SourceLambda = Expression.Lambda<Func<TSource, TDestination, TDestination>>(BlockExpression, SourceObjectInstance, DestinationObjectInstance);
            return SourceLambda.Compile();
        }

        /// <summary>
        /// Default mapping method.
        /// </summary>
        /// <typeparam name="TSourceKey">The type of the source key.</typeparam>
        /// <typeparam name="TSourceValue">The type of the source value.</typeparam>
        /// <typeparam name="TDestinationKey">The type of the destination key.</typeparam>
        /// <typeparam name="TDestinationValue">The type of the destination value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="manager">The manager.</param>
        /// <returns>The resulting dictionary.</returns>
        private static void DefaultMethod<TSourceKey, TSourceValue, TDestinationKey, TDestinationValue>(
            IDictionary<TSourceKey, TSourceValue> source,
            IDictionary<TDestinationKey, TDestinationValue> destination)
        {
            foreach (var item in source)
            {
                destination.Add(item.Key, item.Value);
            }
            return destination;
        }

        /// <summary>
        /// Determines whether the specified type is dictionary.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is dictionary; otherwise, <c>false</c>.</returns>
        private static bool IsDictionary(Type type)
        {
            var Interfaces = type.GetInterfaces();
            return Interfaces.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == DictionaryType);
        }
    }*/
}