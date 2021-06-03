using Canister.Interfaces;
using ObjectCartographer;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Canister extension methods
    /// </summary>
    public static class CanisterExtensions
    {
        /// <summary>
        /// Registers the big book of data types.
        /// </summary>
        /// <param name="bootstrapper">The bootstrapper.</param>
        /// <returns>The bootstrapper</returns>
        public static ICanisterConfiguration? RegisterObjectCartographer(this ICanisterConfiguration? bootstrapper)
        {
            return bootstrapper?.AddAssembly(typeof(DataMapper).Assembly);
        }
    }
}