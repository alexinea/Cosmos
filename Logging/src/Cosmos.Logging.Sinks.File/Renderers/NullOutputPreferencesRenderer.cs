﻿using System;
using System.Collections.Generic;
using System.Text;
using Cosmos.Logging.Core;
using Cosmos.Logging.Formattings;
using Cosmos.Logging.Renders;

namespace Cosmos.Logging.Sinks.File.Renderers {
    [NonScanRenderer]
    public class NullOutputPreferencesRenderer : IOutputPreferencesRenderer {
        private static readonly NullOutputPreferencesRenderer NullOutputPreferencesRendererCache;

        static NullOutputPreferencesRenderer() {
            NullOutputPreferencesRendererCache = new NullOutputPreferencesRenderer();
        }

        public static NullOutputPreferencesRenderer Instance => NullOutputPreferencesRendererCache;

        public string Name => "NullOutputPreferencesRender";
        public bool IsNull => true;

        public void Render(string format, string paramsText, StringBuilder stringBuilder,
            ILogEventInfo logEventInfo = null, IFormatProvider formatProvider = null) { }

        public void Render(IList<FormatEvent> formattingEvents, string paramsText, StringBuilder stringBuilder,
            ILogEventInfo logEventInfo = null, IFormatProvider formatProvider = null) { }

        public void Render(IList<Func<object, IFormatProvider, object>> formattingFuncs, string paramsText, StringBuilder stringBuilder,
            ILogEventInfo logEventInfo = null, IFormatProvider formatProvider = null) { }

        public string ToString(string format, string paramsText,
            ILogEventInfo logEventInfo = null, IFormatProvider formatProvider = null) => string.Empty;

        public string ToString(IList<FormatEvent> formattingEvents, string paramsText,
            ILogEventInfo logEventInfo = null, IFormatProvider formatProvider = null) => string.Empty;

        public string ToString(IList<Func<object, IFormatProvider, object>> formattingFuncs, string paramsText,
            ILogEventInfo logEventInfo = null, IFormatProvider formatProvider = null) => string.Empty;
    }
}