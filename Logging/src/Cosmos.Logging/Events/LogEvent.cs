﻿using System;
using System.Collections.Generic;
using System.IO;
using Cosmos.Logging.Core;
using Cosmos.Logging.MessageTemplates;

namespace Cosmos.Logging.Events {
    public class LogEvent : ILogEventInfo {
        private readonly AdditionalOptContext _additionalOptContext;

        private readonly Dictionary<(string name, PropertyResolvingMode mode), MessagePropertyValue> _namedProperties;
        private readonly Dictionary<(int position, PropertyResolvingMode mode), MessagePropertyValue> _positionalProperties;
        private readonly Dictionary<string, ExtraMessageProperty> _extraMessageProperties;

        internal LogEvent() {
            _additionalOptContext = null;
            _namedProperties = null;
            _positionalProperties = null;
            _extraMessageProperties = null;
        }

        public LogEvent(
            string stateNamespace,
            DateTimeOffset timestamp,
            LogEventLevel level,
            MessageTemplate messageTemplate,
            Exception exception,
            LogEventSendMode sendMode,
            Dictionary<(string name, PropertyResolvingMode mode), MessageProperty> namedMessageProperties,
            Dictionary<(int position, PropertyResolvingMode mode), MessageProperty> positionalMessageProperties,
            AdditionalOptContext additionalOptContext) {

            if (namedMessageProperties == null) throw new ArgumentNullException(nameof(namedMessageProperties));
            if (positionalMessageProperties == null) throw new ArgumentNullException(nameof(positionalMessageProperties));

            StateNamespace = stateNamespace;
            Timestamp = timestamp;
            Level = level;
            Exception = exception;
            SendMode = sendMode;
            MessageTemplate = messageTemplate ?? throw new ArgumentNullException(nameof(messageTemplate));
            _additionalOptContext = additionalOptContext ?? new AdditionalOptContext();

            _namedProperties = new Dictionary<(string name, PropertyResolvingMode mode), MessagePropertyValue>();
            _positionalProperties = new Dictionary<(int position, PropertyResolvingMode mode), MessagePropertyValue>();
            _extraMessageProperties = new Dictionary<string, ExtraMessageProperty>();

            UpdateProperty(namedMessageProperties, positionalMessageProperties);
        }

        public string StateNamespace { get; }
        public DateTimeOffset Timestamp { get; }
        public LogEventLevel Level { get; }
        public LogEventSendMode SendMode { get; }
        public Exception Exception { get; }
        public MessageTemplate MessageTemplate { get; }

        #region Reder Message

        public void RenderMessage(TextWriter output, IFormatProvider provider = null) {
            MessageTemplate.Render(NamedProperties, PositionalProperties, output, this, provider);
        }

        public string RenderMessage(IFormatProvider provider = null) {
            return MessageTemplate.Render(NamedProperties, PositionalProperties, this, provider);
        }

        #endregion

        #region Additional Operations

        public IEnumerable<IAdditionalOperation> GetAdditionalOperations(Type flagType, AdditionalOperationTypes optType)
            => LogAdditionalOperationFilter.Filter(_additionalOptContext, flagType, optType);

        #endregion

        #region Normal Message Properties

        public IReadOnlyDictionary<(string name, PropertyResolvingMode mode), MessagePropertyValue> NamedProperties => _namedProperties;

        public IReadOnlyDictionary<(int position, PropertyResolvingMode mode), MessagePropertyValue> PositionalProperties => _positionalProperties;

        private void UpdateProperty(
            Dictionary<(string name, PropertyResolvingMode mode), MessageProperty> namedMessageProperties,
            Dictionary<(int position, PropertyResolvingMode mode), MessageProperty> positionalMessageProperties) {
            foreach (var prop in namedMessageProperties) _namedProperties.Add(prop.Key, prop.Value.Value);
            foreach (var prop in positionalMessageProperties) _positionalProperties.Add(prop.Key, prop.Value.Value);
        }

        #endregion

        #region Extra Message Properties

        public void AddExtraProperty(ExtraMessageProperty property) {
            if (property != null && !string.IsNullOrWhiteSpace(property.Name))
                _extraMessageProperties[property.Name] = property;
        }

        public void RemoveExtraProperty(string name) {
            if (_extraMessageProperties.ContainsKey(name))
                _extraMessageProperties.Remove(name);
        }

        public void RemoveAllExtraProperties() {
            _extraMessageProperties.Clear();
        }

        public IReadOnlyDictionary<string, ExtraMessageProperty> ExtraProperties => _extraMessageProperties;

        #endregion

    }
}