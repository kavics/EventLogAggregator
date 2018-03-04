using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBender.EventLogAggregator
{
    public class WrappedEventRecord : ILogEntry
    {
        EventRecord _source;

        public string Category => throw new NotImplementedException();
        public short CategoryNumber => throw new NotImplementedException();
        public byte[] Data => throw new NotImplementedException();
        public EventLogEntryType EntryType => throw new NotImplementedException();
        public int Index => throw new NotImplementedException();
        public long InstanceId => throw new NotImplementedException();
        public string MachineName => throw new NotImplementedException();
        public string Message => throw new NotImplementedException();
        public string[] ReplacementStrings => throw new NotImplementedException();
        public string Source => throw new NotImplementedException();
        public DateTime TimeGenerated => throw new NotImplementedException();
        public DateTime TimeWritten => throw new NotImplementedException();
        public string UserName => throw new NotImplementedException();

        public WrappedEventRecord(EventRecord source)
        {
            _source = source;
        }
    }
}
