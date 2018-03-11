using SenseNet.Tools.CommandLineArguments;
using System;
using System.IO;
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable ArrangeAccessorOwnerBody

namespace SpaceBender.EventLogAggregator
{
    public enum SourceType { LocalComputer, RemoteComputer, XmlFile, EvtxFile }
    public class Configuration
    {
        public static readonly string DefaultLogName = "SenseNet";

        [NoNameOption(order: 1, nameInHelp: "computername", helpText: "Name of the remote computer (default: local)")]
        private string ComputerNameArg { get; set; }
        private string _computerName;
        public string ComputerName
        {
            get
            {
                return _computerName
                       ?? (_computerName = string.IsNullOrEmpty(ComputerNameArg) ? Environment.MachineName : ComputerNameArg);
            }
        }

        [CommandLineArgument(name: "log", aliases: "l, logname", helpText: "Name of the log on the specified file.")]
        private string LogNameArg { get; set; }
        private string _logName;
        public string LogName
        {
            get
            {
                return _logName
                       ?? (_logName = string.IsNullOrEmpty(LogNameArg) ? DefaultLogName : LogNameArg);
            }
        }

        private string _fileName;
        [CommandLineArgument(name: "file", aliases: "f, filename", helpText: "Saved log file. Supported types: evtx or xml. This argument ignores the <computername> and <logname> arguments.")]
        private string FileNameArg
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                var ext = Path.GetExtension(value ?? "").ToLowerInvariant();
                if (ext != ".xml" && ext != ".evtx")
                    throw new ArgumentException($"File type {ext} is notsupported. Select .xml or .evtx file");
            }
        }
        public string FileName
        {
            get { return FileNameArg; }
        }

        private SourceType? _sourceType;
        public SourceType SourceType
        {
            get
            {
                if (_sourceType == null)
                    _sourceType = GetSourceType();
                return _sourceType.Value;
            }
        }
        private SourceType GetSourceType()
        {
            if (!string.IsNullOrEmpty(FileName))
            {
                var ext = Path.GetExtension(FileName).ToLowerInvariant();
                if (".xml" == ext)
                    return SourceType.XmlFile;
                if (".evtx" == ext)
                    return SourceType.EvtxFile;
                throw new ArgumentException("Xml or evtx files are not supported.");
            }
            var name = ComputerName;
            return name == Environment.MachineName ? SourceType.LocalComputer : SourceType.RemoteComputer;
        }

        public bool SourceIsFile
        {
            get { return SourceType == SourceType.EvtxFile || SourceType == SourceType.XmlFile; }
        }

        private string _sourceName;
        public string SourceName
        {
            get
            {
                if(_sourceName == null)
                {
                    switch (SourceType)
                    {
                        case SourceType.LocalComputer:
                        case SourceType.RemoteComputer:
                            _sourceName = $"{ComputerName}_{LogName}";
                            break;
                        case SourceType.XmlFile:
                        case SourceType.EvtxFile:
                            _sourceName = FileName;
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }
                return _sourceName;
            }
        }

        private string _eventsFileName;
        public string EventsFileName
        {
            get
            {
                return _eventsFileName
                       ?? (_eventsFileName = Path.Combine(OutputDirectory, SourceName + "-events.txt"));
            }
        }

        private string _errorsFileName;
        public string ErrorsFileName
        {
            get
            {
                return _errorsFileName
                       ?? (_errorsFileName = Path.Combine(OutputDirectory, SourceName + "-errors.txt"));
            }
        }

        private string _outputDirectory;
        public string OutputDirectory
        {
            get
            {
                return _outputDirectory
                       ?? (_outputDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            }
        }
    }
}
