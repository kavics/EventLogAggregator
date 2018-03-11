using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace SpaceBender.EventLogAggregator
{
    public class WrappedEventRecord : ILogEntry
    {
        public string Category { get; private set; }
        public short CategoryNumber { get; private set; }
        public byte[] Data { get; private set; }
        public EventLogEntryType EntryType { get; private set; }
        public int Index { get; private set; }
        public long InstanceId { get; private set; }
        public string MachineName { get; private set; }
        public string Message { get; private set; }
        public string[] ReplacementStrings { get; private set; }
        public string Source { get; private set; }
        public DateTime TimeGenerated { get; private set; }
        public DateTime TimeWritten { get; private set; }
        public string UserName { get; private set; }

        public WrappedEventRecord(EventRecord source)
        {

            Category = source.LogName;
            //CategoryNumber = 
            //Data = 

            // EntryType
            switch (source.Level ?? 0)
            {
                default: throw new NotSupportedException();
                case 0: EntryType = EventLogEntryType.Information; break;
                case 2: EntryType = EventLogEntryType.Error; break;
                case 3: EntryType = EventLogEntryType.Warning; break;
                case 4: EntryType = EventLogEntryType.Information; break;
            }

            //Index = 
            //InstanceId = 
            MachineName = source.MachineName;
            Message = source.Properties.First().Value.ToString().Trim();
            //ReplacementStrings = 
            //Source = source.ProviderName?
            TimeGenerated = (source.TimeCreated ?? DateTime.MinValue).ToUniversalTime();
            TimeWritten = TimeGenerated;
            //UserName = 
        }
    }
}
