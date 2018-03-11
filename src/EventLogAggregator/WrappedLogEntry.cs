using System;
using System.Diagnostics;

namespace SpaceBender.EventLogAggregator
{
    public class WrappedLogEntry : ILogEntry
    {
        EventLogEntry _source;

        public string Category => _source.Category;
        public short CategoryNumber => _source.CategoryNumber;
        public byte[] Data => _source.Data;
        public EventLogEntryType EntryType => _source.EntryType;
        public int Index => _source.Index;
        public long InstanceId => _source.InstanceId;
        public string MachineName => _source.MachineName;
        public string Message => _source.Message;
        public string[] ReplacementStrings => _source.ReplacementStrings;
        public string Source => _source.Source;
        public DateTime TimeGenerated => _source.TimeGenerated;
        public DateTime TimeWritten => _source.TimeWritten;
        public string UserName => _source.UserName;

        public WrappedLogEntry(EventLogEntry source)
        {
           _source = source;
        }
    }
}