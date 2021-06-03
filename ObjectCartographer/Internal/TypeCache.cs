using System.Reflection;

namespace ObjectCartographer.Internal
{
    /// <summary>
    /// Caching of type data.
    /// </summary>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    internal static class TypeCache<TObject>
    {
        /// <summary>
        /// The constructors
        /// </summary>
        public static readonly ConstructorInfo[] Constructors = typeof(TObject).GetConstructors();

        /// <summary>
        /// The properties
        /// </summary>
        public static readonly PropertyInfo[] Properties = typeof(TObject).GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
    }
}