using Microsoft.Extensions.DependencyInjection;
using System;

namespace ObjectCartographer.ExtensionMethods
{
    /// <summary>
    /// Services
    /// </summary>
    internal static class Services
    {
        /// <summary>
        /// Gets the service provider.
        /// </summary>
        /// <value>The service provider.</value>
        public static IServiceProvider? ServiceProvider
        {
            get
            {
                if (_ServiceProvider is not null)
                    return _ServiceProvider;
                lock (_LockObj)
                {
                    if (_ServiceProvider is not null)
                        return _ServiceProvider;
                    _ServiceProvider = (_ServiceCollection ?? new ServiceCollection().AddCanisterModules())?.BuildServiceProvider();
                }
                return _ServiceProvider;
            }
        }

        /// <summary>
        /// The service collection
        /// </summary>
        internal static IServiceCollection? _ServiceCollection;

        /// <summary>
        /// The lock object
        /// </summary>
        private static readonly object _LockObj = new();

        /// <summary>
        /// The service provider
        /// </summary>
        private static IServiceProvider? _ServiceProvider;
    }
}