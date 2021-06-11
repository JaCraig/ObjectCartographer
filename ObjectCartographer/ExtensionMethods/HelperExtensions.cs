using System;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectCartographer.ExtensionMethods
{
    /// <summary>
    /// Helper extensions
    /// </summary>
    internal static class HelperExtensions
    {
        /// <summary>
        /// Finds the matching property.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="name">The name.</param>
        /// <returns>The property or null if it is not found.</returns>
        public static PropertyInfo? FindMatchingProperty(this PropertyInfo[] properties, string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            properties ??= Array.Empty<PropertyInfo>();
            return Array.Find(properties, x => x.Name == name)
                    ?? Array.Find(properties, x => string.Equals(RemoveChars(x.Name), RemoveChars(name), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the type of the element within the IEnumerable. Or the type itself if it is not an IEnumerable.
        /// </summary>
        /// <param name="type">The object type.</param>
        /// <returns>The element type if it is an IEnumerable, otherwise the type sent in.</returns>
        public static Type GetIEnumerableElementType(this Type type)
        {
            var IEnum = FindIEnumerableElementType(type);

            return IEnum is null ? type : IEnum.GetGenericArguments()[0];
        }

        /// <summary>
        /// Finds the type of the IEnumerable element.
        /// </summary>
        /// <param name="type">The original type.</param>
        /// <returns>Either null or the type of the IEnumerable</returns>
        private static Type? FindIEnumerableElementType(Type type)
        {
            if (type is null || type == typeof(string))
            {
                return null;
            }

            if (type.IsArray)
            {
                return typeof(IEnumerable<>).MakeGenericType(type.GetElementType());
            }

            var TypeInfo = type;
            if (TypeInfo.IsGenericType)
            {
                var maxLength = type.GetGenericArguments().Length;
                for (var x = 0; x < maxLength; ++x)
                {
                    var Arg = type.GetGenericArguments()[x];
                    var IEnum = typeof(IEnumerable<>).MakeGenericType(Arg);

                    if (IEnum.IsAssignableFrom(type))
                    {
                        return IEnum;
                    }
                }
            }

            var Interfaces = type.GetInterfaces();
            if (Interfaces?.Length > 0)
            {
                var InterfacesLength = Interfaces.Length;
                for (var x = 0; x < InterfacesLength; ++x)
                {
                    var InterfaceUsed = Interfaces[x];
                    var IEnum = FindIEnumerableElementType(InterfaceUsed);

                    if (!(IEnum is null))
                    {
                        return IEnum;
                    }
                }
            }

            return !(TypeInfo.BaseType is null) && TypeInfo.BaseType != typeof(object) ?
                FindIEnumerableElementType(TypeInfo.BaseType) :
                null;
        }

        /// <summary>
        /// Removes bad chars.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The resulting string.</returns>
        private static string RemoveChars(string name)
        {
            return name?.Replace("_", "") ?? "";
        }
    }
}