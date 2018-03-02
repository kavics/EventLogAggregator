using System;
using System.Diagnostics;

namespace SpaceBender.EventLogAggregator
{
    public interface ILogEntry
    {
        string Category { get; }
        short CategoryNumber { get; }
        byte[] Data { get; }
        EventLogEntryType EntryType { get; }
        int Index { get; }
        long InstanceId { get; }
        string MachineName { get; }
        string Message { get; }
        string[] ReplacementStrings { get; }
        string Source { get; }
        DateTime TimeGenerated { get; }
        DateTime TimeWritten { get; }
        string UserName { get; }
    }
}
