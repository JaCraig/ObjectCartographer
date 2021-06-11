using ObjectCartographer.Internal;
using System.Collections.Generic;

namespace ObjectCartographer.Interfaces
{
    /// <summary>
    /// Type mapping interface
    /// </summary>
    public interface IInternalTypeMapping
    {
        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        List<IPropertyMapping> Properties { get; }

        /// <summary>
        /// Gets the type information.
        /// </summary>
        /// <value>The type information.</value>
        TypeTuple TypeInfo { get; }
    }

    /// <summary>
    /// Type mapping interface
    /// </summary>
    public interface ITypeMapping
    {
        /// <summary>
        /// Automatically maps the two types.
        /// </summary>
        /// <returns>This.</returns>
        ITypeMapping AutoMap();

        /// <summary>
        /// Builds this instance.
        /// </summary>
        void Build();
    }
}