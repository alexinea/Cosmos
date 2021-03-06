﻿using System;
using Cosmos.Logging.Configurations;
using Cosmos.Logging.Core;
using Cosmos.Logging.Extensions.Microsoft;
using Cosmos.Logging.Extensions.Microsoft.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cosmos.Logging {
    /// <summary>
    /// Extensions for logging builder
    /// </summary>
    public static class LoggingBuilderExtensions {
        /// <summary>
        /// Add Cosmos Logging
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddCosmosLogging(this ILoggingBuilder builder, Action<ILogServiceCollection> config = null) {
            return builder.AddCosmosLogging(null, config);
        }

        /// <summary>
        /// Add Cosmos Logging
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="root"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ILoggingBuilder AddCosmosLogging(this ILoggingBuilder builder, IConfigurationRoot root, Action<ILogServiceCollection> config = null) {
            if (builder is null) throw new ArgumentNullException(nameof(builder));

            var servicesImpl = new StandardLogServiceCollection(builder.Services, root);

            config?.Invoke(servicesImpl);

            builder.Services.TryAdd(ServiceDescriptor.Singleton<ILoggingServiceProvider, MicrosoftLoggingServiceProvider>());
            builder.Services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(MicrosoftLoggerWrapper<>)));

            servicesImpl.BuildConfiguration();
            servicesImpl.ActiveSinkSettings();
            servicesImpl.ActiveOriginConfiguration();

            builder.Services.TryAdd(ServiceDescriptor.Singleton(Options.Create((LoggingOptions) servicesImpl.ExposeLogSettings())));
            builder.Services.TryAdd(ServiceDescriptor.Singleton(servicesImpl.ExposeLoggingConfiguration()));
            builder.Services.TryAdd(ServiceDescriptor.Singleton(typeof(StaticServiceResolveInitialization),
                provider => new StaticServiceResolveInitialization(
                    provider.GetRequiredService<ILoggingServiceProvider>(),
                    servicesImpl.ActiveLogEventEnrichers
                )));

            return builder;
        }
    }
}