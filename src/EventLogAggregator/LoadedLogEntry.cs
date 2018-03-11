using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Text;
using System.Xml;
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable PossibleNullReferenceException

namespace SpaceBender.EventLogAggregator
{
    public class LoadedLogEntry : ILogEntry
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

        public static List<ILogEntry> LoadFromEvtx(string fileName)
        {
            var result = new List<ILogEntry>();
            EventRecord record;
            using (var reader = new EventLogReader(fileName, PathType.FilePath))
                while ((record = reader.ReadEvent()) != null)
                    using (record)
                        result.Add(new WrappedEventRecord(record));
            return result;
        }
        public static List<ILogEntry> LoadFromXml(string fileName)
        {
            var xml = new XmlDocument();
            var nsmgr = new XmlNamespaceManager(xml.NameTable);
            nsmgr.AddNamespace("x", "http://schemas.microsoft.com/win/2004/08/events/event");

            xml.Load(fileName);

            var result = new List<ILogEntry>();

            foreach (XmlElement item in xml.SelectNodes("/Events/x:Event", nsmgr))
            {
                var entry = new LoadedLogEntry();

                switch (item.SelectSingleNode("x:System/x:Level", nsmgr).InnerText)
                {
                    default: throw new NotSupportedException();
                    case "0": entry.EntryType = EventLogEntryType.Information; break;
                    case "2": entry.EntryType = EventLogEntryType.Error; break;
                    case "3": entry.EntryType = EventLogEntryType.Warning; break;
                    case "4": entry.EntryType = EventLogEntryType.Information; break;
                }

                var timeSource = item.SelectSingleNode("x:System/x:TimeCreated/@SystemTime", nsmgr).InnerText;
                var datetime = XmlConvert.ToDateTime(timeSource, XmlDateTimeSerializationMode.Unspecified);
                entry.TimeGenerated = datetime;
                entry.TimeWritten = datetime;

                entry.Category = item.SelectSingleNode("x:System/x:Channel", nsmgr).InnerText;
                entry.MachineName = item.SelectSingleNode("x:System/x:Computer", nsmgr).InnerText;

                var data = new StringBuilder();
                foreach (XmlElement dataElement in item.SelectNodes("x:EventData/x:Data", nsmgr))
                    data.Append(dataElement.InnerText).Append(Environment.NewLine);
                entry.Message = data.ToString().Trim();

                result.Add(entry);
            }

            return result;
        }
    }
}
