﻿using System;
using Cosmos.Logging.Core;
using Cosmos.Logging.Core.Components;
using Cosmos.Logging.Extensions.Exceptions.Configurations;
using Cosmos.Logging.Extensions.Exceptions.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Cosmos.Logging {
    /// <summary>
    /// Extensions for exceptions integration
    /// </summary>
    public static class ExceptionsIntegrationExtensions {
        /// <summary>
        /// Add exception integration for Cosmos Logging
        /// </summary>
        /// <param name="service"></param>
        /// <param name="exceptionOptionAct"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ILogServiceCollection AddExceptionsIntegration(
            this ILogServiceCollection service,
            Action<ExceptionOptions> exceptionOptionAct = null,
            Action<IConfiguration, ExceptionConfiguration> configAction = null) {
            if (service is null)
                throw new ArgumentNullException(nameof(service));

            var settings = new ExceptionOptions();
            exceptionOptionAct?.Invoke(settings);

            //string GetExpectLevelName() => settings.MinimumLevel.HasValue ? settings.MinimumLevel.Value.GetName() : LogEventLevel.Verbose.GetName();

            return UseExceptionExtensionsCore(service, Options.Create(settings), (conf, sink, _) => configAction?.Invoke(conf, sink));
        }

        private static ILogServiceCollection UseExceptionExtensionsCore(ILogServiceCollection service, IOptions<ExceptionOptions> settings,
            Action<IConfiguration, ExceptionConfiguration, LoggingConfiguration> config = null) {
            var exceptionDestructuringAccessor = new ExceptionDestructuringAccessor();

            service.AddExtraSinkSettings(settings.Value, config);
            service.AddDependency(s => s.TryAdd(ServiceDescriptor.Singleton(settings)));
            service.AddDependency(s => s.TryAdd(ServiceDescriptor.Singleton<IExceptionDestructuringAccessor>(exceptionDestructuringAccessor)));
            service.AddEnricher(() => new ExceptionEnricher(exceptionDestructuringAccessor.Get()));

            RegisterCoreComponentsTypes();

            return service;
        }

        private static void RegisterCoreComponentsTypes() {
            SinkComponentsTypes.SafeAddAppendType(new ComponentsRegistration(typeof(IOptions<ExceptionOptions>), false, ServiceLifetime.Singleton));
        }
    }
}