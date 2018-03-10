using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Text;
using System.Xml;

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

        internal static List<ILogEntry> LoadFrom(string fileName)
        {
            var ext = Path.GetExtension(fileName).Trim('.').ToLowerInvariant();
            if (ext == "xml")
                return LoadFromXml(fileName);
            if (ext == "evtx")
                return LoadFromEvtx(fileName);
            throw new NotSupportedException("File type is not supported: " + ext);
        }
        private static List<ILogEntry> LoadFromEvtx(string fileName)
        {
            var result = new List<ILogEntry>();
            EventRecord record;
            using (var reader = new EventLogReader(fileName, PathType.FilePath))
                while ((record = reader.ReadEvent()) != null)
                    using (record)
                        result.Add(new WrappedEventRecord(record));
            return result;
        }
        private static List<ILogEntry> LoadFromXml(string fileName)
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
/*

<?xml version="1.0" encoding="utf-8" standalone="yes"?>
<Events>
  <Event xmlns='http://schemas.microsoft.com/win/2004/08/events/event'>
    <System>
      <Provider Name='SenseNetInstrumentation'/>
      <EventID Qualifiers='0'>60002</EventID>
.      <Level>3</Level>
      <Task>0</Task>
      <Keywords>0x80000000000000</Keywords>
.      <TimeCreated SystemTime='2016-04-13T15:26:20.000000000Z'/>
      <EventRecordID>5958</EventRecordID>
.      <Channel>SenseNet</Channel>
.      <Computer>tpma-tc1web1.tcndext.dev.local</Computer>
      <Security/>
    </System>
    <EventData>
      <Data>
        Timestamp: 4/13/2016 3:26:20 PM
        Message: Access denied.
        Category: General
        Priority: -1
        EventId: 60002
        Severity: Warning
        Title:
        Machine: TPMA-TC1WEB1
        Application Domain: /LM/W3SVC/2/ROOT-2-131050268086334543
        Process Id: 9852
        Process Name: c:\windows\system32\inetsrv\w3wp.exe
        Win32 Thread Id: 6428
        Thread Name:
        Extended Properties: Messages - ODataException: Access denied.

        ---- Inner Exception:
        SenseNetSecurityException: Access denied.
        at SenseNet.ContentRepository.Storage.BinaryData.AssertChunk(Int32 contentId, String fieldName, Node&amp; node, PropertyType&amp; propertyType) in d:\temp\sn-evaluation-src-6.5.3.8741\Source\SenseNet\Storage\BinaryData.cs:line 550
        at SenseNet.ContentRepository.Storage.BinaryData.WriteChunk(Int32 contentId, String token, Int64 fullStreamSize, Byte[] buffer, Int64 offset, String fieldName) in d:\temp\sn-evaluation-src-6.5.3.8741\Source\SenseNet\Storage\BinaryData.cs:line 513
        at SenseNet.ApplicationModel.UploadAction.SaveFileToRepository(Content uploadedContent, Content parent, String token, Boolean mustFinalize, Boolean mustCheckIn, HttpPostedFile file) in d:\temp\sn-evaluation-src-6.5.3.8741\Source\SenseNet\Portal\ApplicationModel\UploadAction.cs:line 278
        at SenseNet.ApplicationModel.UploadAction.Execute(Content content, Object[] parameters) in d:\temp\sn-evaluation-src-6.5.3.8741\Source\SenseNet\Portal\ApplicationModel\UploadAction.cs:line 500
        at SenseNet.Portal.OData.ODataFormatter.WriteOperationResult(Stream inputStream, PortalContext portalContext, ODataRequest odataReq) in d:\temp\sn-evaluation-src-6.5.3.8741\Source\SenseNet\Portal\OData\ODataFormatter.cs:line 467
        at SenseNet.Portal.OData.ODataHandler.ProcessRequest(HttpContext context, String httpMethod, Stream inputStream) in d:\temp\sn-evaluation-src-6.5.3.8741\Source\SenseNet\Portal\OData\ODataHandler.cs:line 180
        =====================

        ODataException/SenseNetSecurityException/FormattedMessage - It is only allowed to upload a binary chunk if the content is locked by the current user. NodeId: 8276
        ODataException/SenseNetSecurityException/EventId - 60002
        ODataException/SenseNetSecurityException/Message - It is only allowed to upload a binary chunk if the content is locked by the current user.
        ODataException/SenseNetSecurityException/NodeId - 8276
        UserName - BuiltIn\Admin
        WorkingMode -
        IsHttpContext - yes
        Url - https://sn1-staging-in.tpinteractive.com/OData.svc/Root/Sites/Default_Site/workspaces/Document/ChennaiQA Load Test/Document_Library('ProjectPearl')/Upload
        Referrer - https://sn1-staging-in.tpinteractive.com/workspaces/Document/ChennaiQA Load Test/Document_Library/ProjectPearl?action=Upload&amp;back=%2Fworkspaces%2FDocument%2FChennaiQA Load Test%2FDocument_Library%2FProjectPearl
      </Data>
    </EventData>
  </Event>

*/
