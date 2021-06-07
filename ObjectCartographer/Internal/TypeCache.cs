using System.Linq;
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
        /// The readable properties
        /// </summary>
        public static readonly PropertyInfo[] ReadableProperties = typeof(TObject).GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanRead).ToArray();

        /// <summary>
        /// The writable properties
        /// </summary>
        public static readonly PropertyInfo[] WritableProperties = typeof(TObject).GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanWrite).ToArray();
    }
}