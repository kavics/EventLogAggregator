using SenseNet.Tools.CommandLineArguments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBender.EventLogAggregator
{
    public enum SourceType { LocalComputer, RemoteComputer, XmlFile, EvtxFile }
    public class Configuration
    {
        public static readonly string DefaultLogName = "SenseNet";

        [NoNameOption(order: 1, nameInHelp: "computername", helpText: "Name of the remote computer (default: local)")]
        private string ComputerNameArg { get; set; }
        public string ComputerName
        {
            get { return string.IsNullOrEmpty(ComputerNameArg) ? Environment.MachineName : ComputerNameArg; }
        }
        [CommandLineArgument(aliases: "l, log", helpText: "Name of the log on the specified file.")]
        private string LogNameArg { get; set; }
        public string LogName
        {
            get { return string.IsNullOrEmpty(LogNameArg) ? DefaultLogName : LogNameArg; }
        }
        [CommandLineArgument(aliases: "f, file", helpText: "Saved log file. Supported types: evtx or xml. This argument ignores the <computername> and <logname> arguments.")]
        public string FileName { get; set; }

        public SourceType SourceType
        {
            get
            {
                if (!string.IsNullOrEmpty(FileName))
                {
                    var ext = Path.GetExtension(FileName).ToLowerInvariant();
                    if (".xml" == ext)
                        return SourceType.XmlFile;
                    if (".evtx" == ext)
                        return SourceType.EvtxFile;
                    throw new ArgumentException($"Xml or evtx files are not supported.");
                }
                var name = ComputerName;
                return string.IsNullOrEmpty(name)
                    ? SourceType.LocalComputer
                    : name == Environment.MachineName ? SourceType.LocalComputer : SourceType.RemoteComputer;
            }
        }

        public string SourceName
        {
            get
            {
                switch (SourceType)
                {
                    case SourceType.LocalComputer:
                    case SourceType.RemoteComputer:
                        return $"{ComputerName}_{LogName}";
                    case SourceType.XmlFile:
                    case SourceType.EvtxFile:
                        return FileName;
                    default:
                        throw new NotSupportedException();
                }
            }
        }
        public string OutputDirectory
        {
            get { return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); }
        }
        public string EventsFileName
        {
            get { return Path.Combine(OutputDirectory, SourceName + "-events.txt"); }
        }
        public string ErrorsFileName
        {
            get { return Path.Combine(OutputDirectory, SourceName + "-errors.txt"); }
        }
    }
}
