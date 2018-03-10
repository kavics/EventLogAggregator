using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBender.EventLogAggregator
{
    class Program
    {
        const string SnLogName = "SenseNet";

        static void Main(string[] args)
        {
            Run(args);

            if (Debugger.IsAttached)
            {
                Console.Write("Press <enter> to exit...");
                Console.ReadLine();
            }
        }
        static void Run(string[] args)
        {
            //args = new[] { @"D:\Desktop\TPI_test\transformed\tc1web1 sensenet.xml" };
            //args = new[] { @"D:\Desktop\SenseNet.xml" };
            args = new[] { @"D:\Desktop\SenseNet.evtx" };

            var computerName = args.Length == 0 ? Environment.MachineName : args[0];
            var outputDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            var eventsFileName = Path.Combine(outputDirectory, computerName + "-events.txt");
            var errorsFileName = Path.Combine(outputDirectory, computerName + "-errors.txt");

            var entries = new List<ILogEntry>();

            if (!File.Exists(computerName))
            {
                var logs = computerName == null ? EventLog.GetEventLogs() : EventLog.GetEventLogs(computerName);
                var snLog = logs.FirstOrDefault(l => l.Log == SnLogName);
                if (snLog == null)
                {
                    Console.WriteLine("EventLog '{0}' was not found.", SnLogName);
                    return;
                }
                foreach (var item in snLog.Entries)
                    entries.Add(new WrappedLogEntry((EventLogEntry)item));
            }
            else
            {
                entries = LoadedLogEntry.LoadFrom(computerName);
            }

            Console.WriteLine("Source: {0}", computerName ?? "<local>");
            //Console.WriteLine("  Log name = \t\t {0}", snLog.Log);
            Console.WriteLine("  Number of events = \t {0}", entries.Count.ToString());
            Console.WriteLine("-----------------------------------------------------------------");

            var firstTime = entries.First().TimeGenerated;

            var lastTime = DateTime.MinValue;
            var detailedEntries = new List<DetailedLogEntry>();
            var entryCount = 0;
            var errorCount = 0;
            var warningCount = 0;
            foreach (var entry in entries)
            {
                entryCount++;
                lastTime = entry.TimeGenerated;
                if (entry.EntryType == EventLogEntryType.Error
                    || entry.EntryType == EventLogEntryType.Warning
                    || entry.EntryType == EventLogEntryType.Information)
                {
                    if (entry.EntryType == EventLogEntryType.Error)
                        errorCount++;
                    if (entry.EntryType == EventLogEntryType.Warning)
                        warningCount++;
                    detailedEntries.Add(DetailedLogEntry.Create(entry));
                }
            }

            if (lastTime < firstTime)
            {
                var d = lastTime;
                lastTime = firstTime;
                firstTime = d;
            }

            var grouped = new Dictionary<int, List<DetailedLogEntry>>();
            foreach (var entry in detailedEntries)
            {
                List<DetailedLogEntry> list;
                var key = entry.GetHashCode();
                if (!grouped.TryGetValue(key, out list))
                {
                    list = new List<DetailedLogEntry>();
                    grouped.Add(key, list);
                }
                list.Add(entry);
            }

            var ordered = grouped.Values
                .OrderBy(i => i.First().EntryType)
                .ThenByDescending(i => i.Count)
                .ThenByDescending(i => i.First().Message)
                .ToArray();

            using (var writer = new StreamWriter(errorsFileName))
            {
                Console.WriteLine("Writing aggregated errors to {0}", errorsFileName);
                writer.WriteLine("Source:        {0}", computerName);
                writer.WriteLine("First event:   {0}", firstTime.ToString("yyyy-MM-dd HH:mm:ss.fffff"));
                writer.WriteLine("Last event :   {0}", lastTime.ToString("yyyy-MM-dd HH:mm:ss.fffff"));
                writer.WriteLine("Event count:   {0}", entryCount);
                writer.WriteLine("Error count:   {0}", errorCount);
                writer.WriteLine("Warning count: {0}", warningCount);
                writer.WriteLine("Group count:   {0}", grouped.Count);
                writer.WriteLine("==========================================================================");
                writer.WriteLine(" Error message summary:");
                writer.WriteLine("Count DiffMsg DiffStack        Type  Message");
                writer.WriteLine("----- ------- --------- -----------  -------");
                //                    3       3         3       Error  Invalid manifest: missing "ReleaseDate" element.
                //                   35       1         1 Information  ContentTypeManager.Reset called.

                foreach (var list in ordered)
                {
                    var first = list[0];

                    var differentMessageCount = list.Select(i => i.StackTrace).Distinct().Count();
                    var differentStackTraceCount = list.Select(i => GetStackLines(i.StackTrace)).Distinct().Count();

                    writer.WriteLine("{0,5}{1,8}{2,10}{3,12}  {4}", list.Count, differentMessageCount, differentStackTraceCount, first.EntryType, first.Name);
                }

                writer.WriteLine();
                writer.WriteLine("##########################################################################");
                writer.WriteLine("##   Group details                                                      ##");
                writer.WriteLine("##########################################################################");
                writer.WriteLine();
                foreach (var list in ordered)
                {
                    var first = list[0];

                    if (first.EntryType != EventLogEntryType.Error)
                        continue;

                    writer.WriteLine("Name: {0}", first.Name);
                    writer.WriteLine("Type: {0}", first.EntryType);
                    writer.WriteLine("Count: {0}", list.Count);

                    var subGroups = new Dictionary<string, Dictionary<string, int>>(); // Dictionary<stacktrace, Dictionary<message, count>>
                    foreach (var item in list)
                    {
                        var messageLines = GetMessageLines(item.StackTrace) ?? item.Message;
                        var stackLines = GetStackLines(item.StackTrace) ?? string.Empty;

                        Dictionary<string, int> messages;
                        if (!subGroups.TryGetValue(stackLines, out messages))
                        {
                            messages = new Dictionary<string, int>();
                            subGroups.Add(stackLines, messages);
                        }

                        if (!messages.ContainsKey(messageLines))
                            messages.Add(messageLines, 1);
                        else
                            messages[messageLines]++;
                    }

                    foreach (var entry in subGroups)
                    {
                        writer.WriteLine(
                            "----------------------------------------------------------------------------------------------------------------------------------------------------");
                        foreach (var subEntry in entry.Value)
                            writer.WriteLine("Count: {0}: {1}", subEntry.Value, subEntry.Key);

                        writer.WriteLine();
                        writer.WriteLine("Stack:");
                        writer.WriteLine(entry.Key); //stacktrace
                    }

                    writer.WriteLine();
                    writer.WriteLine();
                    writer.WriteLine("====================================================================================================================================================");
                }
            }

            using (var writer = new StreamWriter(eventsFileName))
            {
                Console.WriteLine("Writing all events to        {0}", eventsFileName);
                writer.WriteLine("First event: {0}", firstTime.ToString("yyyy-MM-dd HH:mm:ss.fffff"));
                writer.WriteLine("Last event : {0}", lastTime.ToString("yyyy-MM-dd HH:mm:ss.fffff"));
                writer.WriteLine("Entry count: {0}", entryCount);
                writer.WriteLine("##########################################################################");

                foreach (var entry in entries.OrderBy(e => e.TimeGenerated).ThenBy(e=>e.Message))
                {
                    WriteEntry(entry, writer);
                    writer.WriteLine("##########################################################################");
                }
            }
        }

        private static string GetMessageLines(string stackTrace)
        {
            if (stackTrace == null)
                return null;

            var filtered = String.Join("\r\n",
                stackTrace.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(l => !l.StartsWith("   at ")));
            return filtered;
        }
        private static string GetStackLines(string stackTrace)
        {
            if (stackTrace == null)
                return null;

            var filtered = String.Join("\r\n",
                stackTrace.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(l => l.StartsWith("   at ") || l.StartsWith("----")));
            return filtered;
        }

        private static void WriteEntry(ILogEntry entry, TextWriter writer)
        {
            writer.WriteLine("Category:       {0}", entry.Category);
            writer.WriteLine("CategoryNumber: {0}", entry.CategoryNumber);
            writer.WriteLine("EntryType:      {0}", entry.EntryType);
            writer.WriteLine("Index:          {0}", entry.Index);
            writer.WriteLine("InstanceId:     {0}", entry.InstanceId);
            writer.WriteLine("MachineName:    {0}", entry.MachineName);
            //writer.WriteLine("ReplacementStrings: {0}", string.Join("|", entry.ReplacementStrings));
            writer.WriteLine("Source:         {0}", entry.Source);
            writer.WriteLine("TimeGenerated:  {0}", entry.TimeGenerated.ToString("yyyy-MM-dd HH:mm:ss.fffff"));
            writer.WriteLine("TimeWritten:    {0}", entry.TimeWritten.ToString("yyyy-MM-dd HH:mm:ss.fffff"));
            writer.WriteLine("UserName:       {0}", entry.UserName);

            writer.WriteLine("Message:");
            writer.WriteLine(entry.Message);
            writer.WriteLine();
        }



    }
}
