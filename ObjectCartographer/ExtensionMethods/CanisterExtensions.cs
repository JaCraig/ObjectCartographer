using Canister.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using ObjectCartographer.ExpressionBuilder;
using ObjectCartographer.ExpressionBuilder.Interfaces;

namespace ObjectCartographer.ExtensionMethods
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
        public static ICanisterConfiguration? RegisterObjectCartographer(this ICanisterConfiguration? bootstrapper) => bootstrapper?.AddAssembly(typeof(DataMapper).Assembly);

        /// <summary>
        /// Registers the ObjectCartographer services.
        /// </summary>
        /// <param name="services">The service collection to add the services to.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection? RegisterObjectCartographer(this IServiceCollection? services)
        {
            if (services.Exists<DataMapper>())
                return services;
            Services._ServiceCollection = services;
            return services?.AddSingleton<DataMapper>()
                .AddSingleton<ExpressionBuilderManager>()
                .AddAllSingleton<IConverter>();
        }
    }
}