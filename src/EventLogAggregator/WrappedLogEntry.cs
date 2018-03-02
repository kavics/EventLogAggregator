using System;
using System.Diagnostics;

namespace SpaceBender.EventLogAggregator
{
    public class WrappedLogEntry : ILogEntry
    {
        EventLogEntry _source;

        public string Category { get { return _source.Category; } }
        public short CategoryNumber { get { return _source.CategoryNumber; } }
        public byte[] Data { get { return _source.Data; } }
        public EventLogEntryType EntryType { get { return _source.EntryType; } }
        public int Index { get { return _source.Index; } }
        public long InstanceId { get { return _source.InstanceId; } }
        public string MachineName { get { return _source.MachineName; } }
        public string Message { get { return _source.Message; } }
        public string[] ReplacementStrings { get { return _source.ReplacementStrings; } }
        public string Source { get { return _source.Source; } }
        public DateTime TimeGenerated { get { return _source.TimeGenerated; } }
        public DateTime TimeWritten { get { return _source.TimeWritten; } }
        public string UserName { get { return _source.UserName; } }

        public static WrappedLogEntry Create(EventLogEntry source)
        {
            return new WrappedLogEntry() { _source = source };
        }
    }
}