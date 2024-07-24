using ObjectCartographer.ExpressionBuilder.Interfaces;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectCartographer.ExpressionBuilder.Converters
{
    /// <summary>
    /// Primitive converter
    /// </summary>
    /// <seealso cref="IConverter"/>
    public class PrimitiveConverter : IConverter
    {
        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order => OrderDefaults.Default;

        /// <summary>
        /// Gets the enum parse.
        /// </summary>
        /// <value>The enum parse.</value>
        private static Type ConvertType { get; } = typeof(Convert);

        /// <summary>
        /// Gets the create conversion method's method information.
        /// </summary>
        private static MethodInfo? CreateConversionMethodMethodInfo { get; } = typeof(PrimitiveConverter).GetMethod(nameof(CreateConversionMethod), BindingFlags.NonPublic | BindingFlags.Static);

        /// <summary>
        /// Gets the parse method.
        /// </summary>
        private static MethodInfo? TryParseMethod { get; } = typeof(PrimitiveConverter).GetMethod(nameof(TryParse), BindingFlags.NonPublic | BindingFlags.Static);

        /// <summary>
        /// Determines whether this instance can handle the specified types.
        /// </summary>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns>
        /// <c>true</c> if this instance can handle the specified types; otherwise, <c>false</c>.
        /// </returns>
        public bool CanHandle(Type sourceType, Type destinationType)
        {
            return (sourceType == typeof(string) && destinationType == typeof(DateTime))
                || (IsBuiltInType(sourceType) && IsBuiltInType(destinationType));
        }

        /// <summary>
        /// Converts the specified property get.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="manager">The manager.</param>
        /// <returns>The resulting expression.</returns>
        public Expression Map(Expression source, Expression? destination, Type sourceType, Type destinationType, IExpressionMapping mapping, ExpressionBuilderManager manager)
        {
            if (!CanHandle(sourceType, destinationType) || CreateConversionMethodMethodInfo is null)
                return Expression.Empty();
            MethodInfo GenericFuncCreation = CreateConversionMethodMethodInfo.MakeGenericMethod(sourceType, destinationType);
            var ConversionMethod = GenericFuncCreation.Invoke(null, null);

            MethodInfo GenericMethod = TryParseMethod!.MakeGenericMethod(sourceType, destinationType);
            ConstantExpression ConversionMethodVariable = Expression.Constant(ConversionMethod);

            return Expression.Call(GenericMethod, ConversionMethodVariable, source);
        }

        /// <summary>
        /// Creates a conversion method that converts the source value of type <typeparamref name="TSource"/> to the destination value of type <typeparamref name="TDestination"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the source value.</typeparam>
        /// <typeparam name="TDestination">The type of the destination value.</typeparam>
        /// <returns>A conversion method that converts the source value to the destination value.</returns>
        private static Func<TSource, TDestination?> CreateConversionMethod<TSource, TDestination>()
        {
            Type SourceType = typeof(TSource);
            Type DestinationType = typeof(TDestination);
            MethodInfo? ConvertMethod = ConvertType.GetMethod("To" + DestinationType.Name, new[] { SourceType });
            if (ConvertMethod is null)
                return x => default;
            ParameterExpression ParameterHolder = Expression.Parameter(SourceType);
            Expression SourceParameter = ParameterHolder;
            if (SourceType == typeof(string))
                SourceParameter = Expression.Coalesce(SourceParameter, Expression.Constant(""));
            MethodCallExpression ConversionMethodCall = Expression.Call(ConvertMethod, SourceParameter);
            var Lambda = Expression.Lambda<Func<TSource, TDestination?>>(ConversionMethodCall, ParameterHolder);
            return Lambda.Compile();
        }

        /// <summary>
        /// Determines whether [is built in type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is built in type] [the specified type]; otherwise, <c>false</c>.</returns>
        private static bool IsBuiltInType(Type type) => type?.IsPrimitive == true || type == typeof(string) || type == typeof(decimal);

        /// <summary>
        /// Tries to parse the source value using the specified conversion method.
        /// </summary>
        /// <typeparam name="TSource">The type of the source value.</typeparam>
        /// <typeparam name="TDestination">The type of the destination value.</typeparam>
        /// <param name="conversionMethod">The conversion method.</param>
        /// <param name="source">The source value.</param>
        /// <returns>The parsed destination value, or the default value if parsing fails.</returns>
        private static TDestination? TryParse<TSource, TDestination>(Func<TSource, TDestination> conversionMethod, TSource source)
        {
            try
            {
                return conversionMethod(source);
            }
            catch
            {
                return default;
            }
        }
    }
}