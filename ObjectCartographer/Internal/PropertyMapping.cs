using ObjectCartographer.Interfaces;
using System;
using System.Reflection;

namespace ObjectCartographer.Internal
{
    /// <summary>
    /// Property mapping
    /// </summary>
    /// <typeparam name="TLeft">The type of the left.</typeparam>
    /// <typeparam name="TRight">The type of the right.</typeparam>
    /// <typeparam name="TLeftPropertyType">The type of the right property type.</typeparam>
    /// <seealso cref="IPropertyMapping"/>
    public class PropertyMapping<TLeft, TRight, TLeftPropertyType> : IPropertyMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMapping{TLeft, TRight,
        /// TRightPropertyType}"/> class.
        /// </summary>
        /// <param name="leftProperty">The left property.</param>
        /// <param name="rightProperty">The right property.</param>
        public PropertyMapping(Func<TLeft, TLeftPropertyType> leftProperty, Action<TRight, TLeftPropertyType> rightProperty)
        {
            SourceTarget = leftProperty.Target;
            Source = leftProperty.Method;
            Destination = rightProperty.Method;
            DestinationTarget = rightProperty.Target;
        }

        /// <summary>
        /// Gets the destination.
        /// </summary>
        /// <value>The destination.</value>
        public MethodInfo Destination { get; }

        /// <summary>
        /// Gets the destination target.
        /// </summary>
        /// <value>The destination target.</value>
        public object? DestinationTarget { get; }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>The source.</value>
        public MethodInfo Source { get; }

        /// <summary>
        /// Gets the source target.
        /// </summary>
        /// <value>The source target.</value>
        public object? SourceTarget { get; }
    }
}