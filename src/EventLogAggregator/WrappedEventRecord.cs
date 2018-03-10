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
/*
Timestamp: 2018. 03. 01. 8:00:36
Message: Object reference not set to an instance of an object.
Category: General
Priority: -1
EventId: 1
Severity: Error
Title:
Machine: SNPC007
Application Domain: UnitTestAdapter: Running test
Process Id: 5480
Process Name: C:\PROGRAM FILES (X86)\MICROSOFT VISUAL STUDIO\2017\ENTERPRISE\COMMON7\IDE\EXTENSIONS\TESTPLATFORM\testhost.x86.exe
Win32 Thread Id: 15672
Thread Name: 
Extended Properties: Messages - NullReferenceException: Object reference not set to an instance of an object.
   at SenseNet.Communication.Messaging.MsmqChannelProvider.SendToAllQueues(Message message, Boolean debugMessage) in D:\dev\tfs\SenseNet\Releases\enterprise\v6.5\servicepack\Source\SenseNet\Storage\DistributedApplication\Messaging\MsmqChannel.cs:line 157
   at SenseNet.Communication.Messaging.MsmqChannelProvider.InternalSend(Stream messageBody, Boolean debugMessage) in D:\dev\tfs\SenseNet\Releases\enterprise\v6.5\servicepack\Source\SenseNet\Storage\DistributedApplication\Messaging\MsmqChannel.cs:line 229
   at SenseNet.Communication.Messaging.ClusterChannel.Send(ClusterMessage message) in D:\dev\tfs\SenseNet\Releases\enterprise\v6.5\servicepack\Source\SenseNet\Storage\DistributedApplication\Messaging\ClusterChannel.cs:line 107
=====================

UserName - \
WorkingMode - 
IsHttpContext - no
*/
