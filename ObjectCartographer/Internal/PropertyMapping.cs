using ObjectCartographer.Interfaces;
using System;
using System.Linq.Expressions;

namespace ObjectCartographer.Internal
{
    /// <summary>
    /// Property mapping
    /// </summary>
    /// <typeparam name="TLeft">The type of the left.</typeparam>
    /// <typeparam name="TRight">The type of the right.</typeparam>
    public class PropertyMapping<TLeft, TRight> : IPropertyMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMapping{TLeft, TRight}"/> class.
        /// </summary>
        /// <param name="leftProperty">The left property.</param>
        /// <param name="rightProperty">The right property.</param>
        public PropertyMapping(Expression<Func<TLeft, object?>> leftProperty, Expression<Func<TRight, object?>> rightProperty)
        {
            RightProperty = rightProperty;
            LeftProperty = leftProperty;
        }

        /// <summary>
        /// Gets the destination.
        /// </summary>
        /// <value>The destination.</value>
        public Expression Destination => RightProperty;

        /// <summary>
        /// Gets the left property.
        /// </summary>
        /// <value>The left property.</value>
        public Expression<Func<TLeft, object?>> LeftProperty { get; }

        /// <summary>
        /// Gets the right property.
        /// </summary>
        /// <value>The right property.</value>
        public Expression<Func<TRight, object?>> RightProperty { get; }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>The source.</value>
        public Expression Source => LeftProperty;
    }
}