﻿using Canister.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using ObjectCartographer.ExpressionBuilder;
using ObjectCartographer.ExpressionBuilder.Interfaces;

namespace ObjectCartographer.Modules
{
    /// <summary>
    /// Canister module
    /// </summary>
    /// <seealso cref="IModule"/>
    public class CanisterModule : IModule
    {
        /// <summary>
        /// Order to run this in
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// Loads the module using the bootstrapper
        /// </summary>
        /// <param name="bootstrapper">The bootstrapper.</param>
        public void Load(IBootstrapper? bootstrapper)
        {
            bootstrapper?.Register<DataMapper>(ServiceLifetime.Singleton)
                .Register<ExpressionBuilderManager>(ServiceLifetime.Singleton)
                .RegisterAll<IConverter>(ServiceLifetime.Singleton);
        }
    }
}