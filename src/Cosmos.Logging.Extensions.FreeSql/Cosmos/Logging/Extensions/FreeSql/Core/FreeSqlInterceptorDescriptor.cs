using System;
using Cosmos.Logging.Configurations;
using Cosmos.Logging.Events;
using FreeSql.Aop;
using Microsoft.Extensions.Options;

namespace Cosmos.Logging.Extensions.FreeSql.Core {
    /// <summary>
    /// FreeSql interceptor descriptor
    /// </summary>
    public class FreeSqlInterceptorDescriptor {
        private readonly FreeSqlEnricherOptions _settings;
        private readonly ILoggingServiceProvider _loggingServiceProvider;

        /// <inheritdoc />
        public FreeSqlInterceptorDescriptor(ILoggingServiceProvider loggingServiceProvider, IOptions<FreeSqlEnricherOptions> settings) {
            _loggingServiceProvider = loggingServiceProvider;
            _settings = settings?.Value;
        }

        internal Action<Exception, CurdAfterEventArgs> ExposeErrorInterceptor => _settings?.ErrorInterceptorAction;

        internal Action<CurdBeforeEventArgs> ExposeExecutingInterceptor => _settings?.ExecutingInterceptorAction;

        internal Action<CurdAfterEventArgs> ExposeExecutedInterceptor => _settings?.ExecutedInterceptorAction;

        internal Func<string, LogEventLevel, bool> ExposeGlobalFilter => _settings?.Filter;

        internal ILoggingServiceProvider ExposeLoggingServiceProvider => _loggingServiceProvider;

        internal RenderingConfiguration RenderingOptions => _settings?.GetRenderingOptions() ?? new RenderingConfiguration();
    }
}