﻿using Cosmos.Logging.Events;
using Cosmos.Logging.Simple;

namespace Cosmos.Logging.Filters.Internals {
    internal static class PreliminaryEventPercolator {
        public static bool Percolate(LogEventLevel level, ILogger logger) {
            if (logger == null) return false;
            if (logger.MinimumLevel == LogEventLevel.Off) return false;
            if (level == LogEventLevel.Off) return false;

            if ((int) logger.MinimumLevel > (int) level) return false;

            return true;
        }

        public static bool Percolate(LogEventLevel level, ISimpleLogger logger) {
            if (logger == null) return false;
            if (logger.MinimumLevel == LogEventLevel.Off) return false;
            if (level == LogEventLevel.Off) return false;

            if ((int) logger.MinimumLevel > (int) level) return false;

            return true;
        }
    }
}