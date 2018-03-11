# Windows EventLog aggregator for [Sense/Net ECM](https://github.com/SenseNet "Sense/Net on github")

Very simple **console application** for reading and grouping the event logs written by the Sense/Net ECM. The logs can be on the Local or remote machines. The application writes two output file:

- **errors.txt:** text file containing aggregated events.
- **events.txt:** a simple text file that contains all events in the time order.

## Usage

Open a console (cmd.exe) or a powershell console and execute the application without any argument. In this case default event log (SenseNet) will be processed on the local machine:

_**EventLogAggregator.exe**_

Change the log still on the local machine:

_EventLogAggregator.exe_ **-log:mylog**

Process the default sensenet log on a remote computer (e.g. PC02):

_EventLogAggregator.exe_ **PC02**

Change the log on a remote computer:

_EventLogAggregator.exe_ PC02 **-log:mylog**

The application can procass saved log files. The accepted formats are **XML** and **EVTX**:

_EventLogAggregator.exe_ **-file:c:\mylog.xml**

_EventLogAggregator.exe_ **-file:c:\mylog.evtx**

If the file argument is specified, the conputer name and log specifications are ignored.

### The output file locations:

The files are written to the application's directory if the computer and/or log are specified. The files will be prefixed with the computer and log name:




 In case of processing a saved file the output files are placed next to the processed file with **"-errors.txt"** and **"-events.txt"** suffixes.
