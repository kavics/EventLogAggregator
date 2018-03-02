using System;
using System.Diagnostics;

namespace SpaceBender.EventLogAggregator
{
    class DetailedLogEntry
    {
        public EventLogEntryType EntryType { get; set; }
        public DateTime Timestamp { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }

        public override int GetHashCode()
        {
            return (EntryType.ToString() + Name).GetHashCode();
        }

        internal static DetailedLogEntry Create(ILogEntry entry)
        {
            var message = ParseItem(entry, "Message:", "\r\n");
            return new DetailedLogEntry
            {
                EntryType = entry.EntryType,
                Timestamp = entry.TimeGenerated,
                Message = message,
                Name = ParseName(message),
                StackTrace = ParseItem(entry, "Extended Properties:", "====================="),
            };
        }

        private static string ParseItem(ILogEntry entry, string from, string to)
        {
            if (entry.EntryType != EventLogEntryType.Error
                && entry.EntryType != EventLogEntryType.Warning
                && entry.EntryType != EventLogEntryType.Information)
                return null;

            var p0 = entry.Message.IndexOf(from);
            if (p0 < 0)
                return null;

            var p1 = entry.Message.IndexOf(to, p0);
            if (p1 <= p0)
                return null;

            return entry.Message.Substring(p0 + from.Length, p1 - p0 - from.Length).Trim();
        }


        static string[] names = new[] {
            "The server failed to resume the transaction.",
            "Cannot perform the operation because another process is making changes on this path:",
            "Version of a content was not found.",
            "cluster channel stopped:",
            "Error saving node.",
            "Node is out of date Id:",
            "Rollback called on it and it is executed successfully.",
            "An error occured during extracting text.",
            "IFilter error: Exception from HRESULT:",
            "Content actions serialization error: invalid xml element.",
            "System.Exception: An error occurred when receiving from the queue"
        };
        private static string ParseName(string message)
        {
            if (message == null)
                return string.Empty;

            foreach (var name in names)
                if (message.Contains(name))
                    return name;
            return message;
        }
    }
}
