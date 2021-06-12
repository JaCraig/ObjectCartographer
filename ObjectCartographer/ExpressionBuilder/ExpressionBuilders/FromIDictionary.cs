namespace ObjectCartographer.ExpressionBuilder.ExpressionBuilders
{
    /// <summary>
    /// From IDictionary expression builder
    /// </summary>
    /// <seealso cref="ExpressionBuilderBaseClass"/>
    /*public class FromIDictionary : ExpressionBuilderBaseClass
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public override int Order => 0;

        /// <summary>
        /// The dictionary type
        /// </summary>
        private static readonly Type DictionaryType = typeof(IDictionary<string, object>);

        /// <summary>
        /// The try get value method
        /// </summary>
        private static readonly MethodInfo TryGetValueMethod = typeof(IDictionary<string, object>).GetMethod("TryGetValue");

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
            return DictionaryType.IsAssignableFrom(mapping.TypeInfo.Source);
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
            var IDictionaryType = Array.Find(SourceType.GetInterfaces(), x => string.Equals(x.Name, "IDictionary`2", StringComparison.OrdinalIgnoreCase));
            var IDictionaryKeyType = IDictionaryType.GetGenericArguments()[0];
            var IDictionaryValueType = IDictionaryType.GetGenericArguments()[1];
            var DestinationType = typeof(TDestination);

            List<Expression> Expressions = new List<Expression>();
            var SourceObjectInstance = Expression.Parameter(SourceType, "source");
            var SourceObjectAsIDictionary = Expression.Variable(IDictionaryType);
            var DestinationObjectInstance = Expression.Parameter(DestinationType, "destination");
            var Value = Expression.Variable(IDictionaryValueType);

            Expressions.Add(manager.Create(DestinationObjectInstance, SourceObjectInstance, TypeCache<TSource>.ReadableProperties, TypeCache<TDestination>.Constructors));
            Expressions.Add(Expression.Assign(SourceObjectAsIDictionary, Expression.Convert(SourceObjectInstance, IDictionaryType)));

            for (var x = 0; x < TypeCache<TDestination>.WritableProperties.Length; ++x)
            {
                var DestinationProperty = TypeCache<TDestination>.WritableProperties[x];
                var SourceProperty = TypeCache<TSource>.ReadableProperties.FindMatchingProperty(DestinationProperty.Name);

                var PropertySet = Expression.Property(DestinationObjectInstance, DestinationProperty);

                if (SourceProperty is null)
                {
                    Expression Key = Expression.Constant(DestinationProperty.Name);
                    Expression MethodCall = Expression.Call(SourceObjectInstance, TryGetValueMethod, Key, Value);
                    Expression Assignment = Expression.Assign(PropertySet, manager.Convert(Value, IDictionaryValueType, DestinationProperty.PropertyType));
                    Expression IfStatement = Expression.IfThen(MethodCall, Assignment);
                    Expressions.Add(IfStatement);
                }
                else
                {
                    Expression PropertyGet = manager.Convert(Expression.Property(SourceObjectInstance, SourceProperty), SourceProperty.PropertyType, DestinationProperty.PropertyType);

                    Expressions.Add(Expression.Assign(PropertySet, PropertyGet));
                }
            }
            if (Expressions.Count == 0)
                return (_, y) => y;
            Expressions.Add(DestinationObjectInstance);
            var BlockExpression = Expression.Block(DestinationType, new[] { SourceObjectAsIDictionary, Value }, Expressions);
            var SourceLambda = Expression.Lambda<Func<TSource, TDestination, TDestination>>(BlockExpression, SourceObjectInstance, DestinationObjectInstance);
            return SourceLambda.Compile();
        }
    }*/
}